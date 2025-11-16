using RadianTools.Interop.Windows;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace RadianTools.Hardware.Input.Windows;

/// <summary>
/// RawInput受信用ウィンドウ
/// </summary>
internal class RawInputReceiverWindow : HiddenWindow
{
    /// <summary>
    /// 通知先RawInputReceiver
    /// </summary>
    private RawInputReceiver _receiver;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="receiver">通知先RawInputReceiver</param>
    public RawInputReceiverWindow(RawInputReceiver receiver)
    {
        // デバイス詳細キャッシュ作成
        var devHandles = HidHelper.GetRawInputDeviceHandles();
        foreach (var hDevice in devHandles)
        {
            var detail = HidHelper.GetDeviceDetail((HDEVICE)hDevice);
            _dicDeviceDetail[hDevice] = detail;
        }

        _receiver = receiver;
    }

    /// <summary>
    /// マウスのRawInput受信を登録する。
    /// </summary>
    public void RegisterMouse()
    {
        InvokeAsync(() => HidHelper.RegisterRawInputDeviceSlim(HidUsagePage.GenericDesktop, HidUsage.Mouse, this.Handle, false));
    }

    /// <summary>
    /// マウスのRawInput受信を解除する。
    /// </summary>
    public void UnregisterMouse()
    {
        InvokeAsync(() => HidHelper.RegisterRawInputDeviceSlim(HidUsagePage.GenericDesktop, HidUsage.Mouse, Null<HWND>.Value, true));
    }

    /// <summary>
    /// キーボードのRawInput受信を登録する。
    /// </summary>
    public void RegisterKeyboard()
    {
        InvokeAsync(() => HidHelper.RegisterRawInputDeviceSlim(HidUsagePage.GenericDesktop, HidUsage.Keyboard, this.Handle, false));
    }

    /// <summary>
    /// キーボードのRawInput受信を解除する。
    /// </summary>
    public void UnregisterKeyboard()
    {
        InvokeAsync(() => HidHelper.RegisterRawInputDeviceSlim(HidUsagePage.GenericDesktop, HidUsage.Keyboard, Null<HWND>.Value, true));
    }

    /// <summary>
    /// ウィンドウプロシージャ
    /// </summary>
    /// <param name="hwnd">ウィンドウハンドル</param>
    /// <param name="msg">ウィンドウメッセージ</param>
    /// <param name="wParam">メッセージ追加情報1</param>
    /// <param name="lParam">メッセージ追加情報2</param>
    /// <returns>メッセージ処理結果</returns>
    protected override IntPtr WndProc(HWND hwnd, WindowMessage msg, IntPtr wParam, IntPtr lParam)
    {
        switch (msg)
        {
            case WindowMessage.WM_DEVICECHANGE:
                if (wParam == (IntPtr)DBT_WPARAM.DBT_DEVNODES_CHANGED)
                    OnDevNodesChanged(hwnd);
                break;

            case WindowMessage.WM_INPUT:
                OnWmInput(hwnd, wParam, (HRAWINPUT)lParam);
                break;
        }

        return base.WndProc(hwnd, msg, wParam, lParam);
    }

    /// <summary>
    /// デバイス詳細Dictionary(Key:デバイスハンドル Value:デバイス詳細)
    /// </summary>
    private readonly ConcurrentDictionary<HDEVICE, HidDeviceDetail> _dicDeviceDetail = [];

    /// <summary>
    /// WM_INPUTイベントハンドラ
    /// </summary>
    /// <param name="hwnd">ウィンドウハンドル</param>
    /// <param name="wParam">メッセージ追加情報1</param>
    /// <param name="lParam">メッセージ追加情報2</param>
    private void OnWmInput(HWND hwnd, IntPtr wParam, HRAWINPUT hRawInput)
    {
        try
        {
            const int RID_INPUT = 0x10000003;

            int headerSize = Marshal.SizeOf(typeof(RAWINPUTHEADER));
            int size = 0;
            int result = User32.GetRawInputData(hRawInput, RID_INPUT, IntPtr.Zero, ref size, headerSize);
            if (result < 0)
                return;

            size = Math.Max(Marshal.SizeOf<RAWINPUT>(), size);
            Span<byte> buf = stackalloc byte[size];
            result = User32.GetRawInputData(hRawInput, RID_INPUT, ref buf[0], ref size, headerSize);
            if (result < 0)
                return;

            // キャッシュからデバイス詳細を取り出す。無ければ取得する。
            Span<RAWINPUT> input = MemoryMarshal.Cast<byte, RAWINPUT>(buf);
            if (!_dicDeviceDetail.TryGetValue(input[0].header.hDevice, out var detail))
            {
                detail = HidHelper.GetDeviceDetail(input[0].header.hDevice);
                _dicDeviceDetail[input[0].header.hDevice] = detail;
            }

            switch (input[0].header.Type)
            {
                case RIM_TYPE.RIM_TYPEMOUSE:
                    if (_receiver._mouseReceived != null)
                    {
                        var mouse = new RawMouseEventArgs(ref detail, ref input[0]);
                        _receiver._mouseReceived(mouse);
                    }
                    break;

                case RIM_TYPE.RIM_TYPEKEYBOARD:
                    if (_receiver._keyboardReceived != null)
                    {
                        // KEYBOARD_OVERRUN_MAKE_CODEは無視する
                        if (input[0].data.keyboard.VKey == VKey.KEYBOARD_OVERRUN_MAKE_CODE)
                            break;

                        // 一部のキーは正しい VKey にならないのでMakeCode,Flagsから再解釈が必要
                        //（例えば、そのままだと左右CTRLは一律 VK_CONTROL になってしまう）
                        input[0].data.keyboard.VKey = HidHelper.GetTrueVKey(
                            input[0].data.keyboard.VKey,
                            input[0].data.keyboard.MakeCode,
                            input[0].data.keyboard.Flags);

                        var keyboard = new RawKeyboardEventArgs(ref detail, ref input[0]);
                        _receiver._keyboardReceived(keyboard);
                    }
                    break;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.ToString());
        }
    }

    /// <summary>
    /// デバイスツリー変化時
    /// </summary>
    /// <param name="hwnd">ウィンドウハンドル</param>
    private void OnDevNodesChanged(HWND hwnd)
    {
        uint numDevices = 0;
        uint cbSize = (uint)Marshal.SizeOf<RAWINPUTDEVICELIST>();
        if (User32.GetRawInputDeviceList(IntPtr.Zero, ref numDevices, cbSize) == -1)
            return;

        Span<RAWINPUTDEVICELIST> pDevList = stackalloc RAWINPUTDEVICELIST[(int)numDevices];
        if (User32.GetRawInputDeviceList(ref pDevList[0], ref numDevices, cbSize) == -1)
            return;

        //キャッシュ中のデバイス詳細から存在しないハンドルの要素を削除
        var existHandles = HidHelper.GetRawInputDeviceHandles();
        var dicHandles = _dicDeviceDetail.Keys.ToArray();
        for (var i = dicHandles.Length - 1; i >= 0; i--)
        {
            var handle = dicHandles[i];
            if (!existHandles.Contains(handle))
                _dicDeviceDetail.TryRemove(handle, out var _);
        }
    }
}