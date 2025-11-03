using System.Collections.Concurrent;
using System.Runtime.InteropServices;
using RadianTools.Interop.Windows;
using RadianTools.Interop.Windows.Utility;

namespace RadianTools.Hardware.Input.Windows;

/// <summary>
/// RawInputマウス情報
/// </summary>
public ref struct RawMouseEventArgs
{
    public RawMouseEventArgs(ref HidDeviceDetail detail, ref RAWINPUT rawInput)
    {
        Detail = ref detail;
        Mouse = ref rawInput.data.mouse;
    }

    /// <summary>
    /// HIDデバイス詳細
    /// </summary>
    public readonly ref HidDeviceDetail Detail;

    /// <summary>
    /// マウス情報情報
    /// </summary>
    public readonly ref RAWMOUSE Mouse;

    /// <inheritdoc />
    public override string ToString() => $"{Detail.ToString()}, {Mouse.ToString()}";
}

/// <summary>
/// RawInputkキーボード情報
/// </summary>
public ref struct RawKeyboardEventArgs
{
    public RawKeyboardEventArgs(ref HidDeviceDetail detail, ref RAWINPUT rawInput)
    {
        Detail = ref detail;
        Keyboard = ref rawInput.data.keyboard;
    }

    public readonly ref HidDeviceDetail Detail;
    public readonly ref RAWKEYBOARD Keyboard;
    public override string ToString() => $"{Detail.ToString()}, {Keyboard.ToString()}";
}

/// <summary>
/// RawInput受信クラス
/// </summary>
public class RawInputReceiver : IDisposable
{
    private bool _disposed;

    /// <summary>
    /// マウス情報受信時イベント
    /// </summary>
    private Action<RawMouseEventArgs>? _mouseReceived;
    public event Action<RawMouseEventArgs> MouseReceived
    {
        add
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(RawInputReceiver));

            Action<RawMouseEventArgs>? original, updated;
            do
            {
                original = _mouseReceived;
                updated = (Action<RawMouseEventArgs>?)Delegate.Combine(original, value);
            }
            while (Interlocked.CompareExchange(ref _mouseReceived, updated, original) != original);

            if (original == null)
                _receiverWindow.RegisterMouse();
        }
        remove
        {
            Action<RawMouseEventArgs>? original, updated;
            do
            {
                original = _mouseReceived;
                updated = (Action<RawMouseEventArgs>?)Delegate.Remove(original, value);
            }
            while (Interlocked.CompareExchange(ref _mouseReceived, updated, original) != original);

            if (updated == null)
                _receiverWindow.UnregisterMouse();
        }
    }

    /// <summary>
    /// キーボード情報受信時イベント
    /// </summary>
    private Action<RawKeyboardEventArgs>? _keyboardReceived;
    public event Action<RawKeyboardEventArgs> KeyboardReceived
    {
        add
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(RawInputReceiver));

            Action<RawKeyboardEventArgs>? original, updated;
            do
            {
                original = _keyboardReceived;
                updated = (Action<RawKeyboardEventArgs>?)Delegate.Combine(original, value);
            }
            while (Interlocked.CompareExchange(ref _keyboardReceived, updated, original) != original);

            if (original == null)
                _receiverWindow.RegisterKeyboard();
        }
        remove
        {
            Action<RawKeyboardEventArgs>? original, updated;
            do
            {
                original = _keyboardReceived;
                updated = (Action<RawKeyboardEventArgs>?)Delegate.Remove(original, value);
            }
            while (Interlocked.CompareExchange(ref _keyboardReceived, updated, original) != original);

            if (updated == null)
                _receiverWindow.UnregisterKeyboard();
        }
    }

    /// <summary>
    /// RawInput情報受信ウィンドウ
    /// </summary>
    private RawInputReceiverWindow _receiverWindow;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public RawInputReceiver()
    {
        // RawInput受信ウィンドウを作成し、メッセージループを起動
        _receiverWindow = new RawInputReceiverWindow(this);
        _receiverWindow.Run();
    }

    /// <summary>
    /// 解放処理
    /// </summary>
    public void Dispose()
    {
        if (_disposed) 
            return;

        _disposed = true;
        _mouseReceived = null;
        _keyboardReceived = null;
        _receiverWindow.UnregisterMouse();
        _receiverWindow.UnregisterKeyboard();
        _receiverWindow.Dispose();
    }

    /// <summary>
    /// RawInput受信用ウィンドウ
    /// </summary>
    private class RawInputReceiverWindow : HiddenWindow
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
            _receiver = receiver;
        }

        /// <summary>
        /// マウスのRawInput受信を登録する。
        /// </summary>
        public void RegisterMouse()
        {
            InvokeAsync(() =>
            {
                var device = new RAWINPUTDEVICE(RIM_TYPE.RIM_TYPEMOUSE, this.Handle, false);
                User32.RegisterRawInputDevices(in device, 1, Marshal.SizeOf<RAWINPUTDEVICE>());
            });
        }

        /// <summary>
        /// マウスのRawInput受信を解除する。
        /// </summary>
        public void UnregisterMouse()
        {
            InvokeAsync(() =>
            {
                var device = new RAWINPUTDEVICE(RIM_TYPE.RIM_TYPEMOUSE, Null<HWND>.Value, true);
                User32.RegisterRawInputDevices(in device, 1, Marshal.SizeOf<RAWINPUTDEVICE>());
            });
        }

        /// <summary>
        /// キーボードのRawInput受信を登録する。
        /// </summary>
        public void RegisterKeyboard()
        {
            InvokeAsync(() =>
            {
                var device = new RAWINPUTDEVICE(RIM_TYPE.RIM_TYPEKEYBOARD, this.Handle, false);
                User32.RegisterRawInputDevices(in device, 1, Marshal.SizeOf<RAWINPUTDEVICE>());
            });
        }

        /// <summary>
        /// キーボードのRawInput受信を解除する。
        /// </summary>
        public void UnregisterKeyboard()
        {
            InvokeAsync(() =>
            {
                var device = new RAWINPUTDEVICE(RIM_TYPE.RIM_TYPEKEYBOARD, Null<HWND>.Value, true);
                User32.RegisterRawInputDevices(in device, 1, Marshal.SizeOf<RAWINPUTDEVICE>());
            });
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
                case WindowMessage.WM_INPUT:
                    OnWmInput(hwnd, wParam, (HRAWINPUT)lParam);
                    break;
            }

            return base.WndProc(hwnd, msg, wParam, lParam);
        }

        /// <summary>
        /// デバイス詳細Dictionary(Key:デバイスハンドル Value:デバイス詳細)
        /// </summary>
        private readonly ConcurrentDictionary<IntPtr, HidDeviceDetail> _dicDeviceDetail = [];

        /// <summary>
        /// WM_INPUTイベントハンドラ
        /// </summary>
        /// <param name="hwnd">ウィンドウハンドル</param>
        /// <param name="wParam">メッセージ追加情報1</param>
        /// <param name="lParam">メッセージ追加情報2</param>
        private void OnWmInput(HWND hwnd, IntPtr wParam, HRAWINPUT hRawInput)
        {
            const int RID_INPUT = 0x10000003;

            int headerSize = Marshal.SizeOf(typeof(RAWINPUTHEADER));
            int size = Marshal.SizeOf(typeof(RAWINPUT));
            int result = User32.GetRawInputData(hRawInput, RID_INPUT, out var input, ref size, headerSize);
            if (result < 0)
                return;

            if (!_dicDeviceDetail.TryGetValue(input.header.DeviceHandle, out var detail))
            {
                detail = HidHelper.GetDeviceDetail(input.header.DeviceHandle);
                _dicDeviceDetail[input.header.DeviceHandle] = detail;
            }

            switch (input.header.Type)
            {
                case RIM_TYPE.RIM_TYPEMOUSE:
                    if (_receiver._mouseReceived != null)
                    {
                        var mouse = new RawMouseEventArgs(ref detail, ref input);
                        _receiver._mouseReceived(mouse);
                    }
                    break;

                case RIM_TYPE.RIM_TYPEKEYBOARD:
                    if (_receiver._keyboardReceived != null)
                    {
                        //不明なキーは無視する
                        if (input.data.keyboard.VKey == VKey.Unknown)
                            break;

                        // 一部のキーは正しい VKey にならないのでMakeCode,Flagsから再解釈が必要
                        //（例えば、そのままだと左右CTRLは一律 VK_CONTROL になってしまう）
                        input.data.keyboard.VKey = HidHelper.GetTrueVKey(
                            input.data.keyboard.VKey,
                            input.data.keyboard.MakeCode,
                            input.data.keyboard.Flags);

                        var keyboard = new RawKeyboardEventArgs(ref detail, ref input);
                        _receiver._keyboardReceived(keyboard);
                    }
                    break;
            }
        }

    }
}