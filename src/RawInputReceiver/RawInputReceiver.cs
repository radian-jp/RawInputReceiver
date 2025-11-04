using RadianTools.Interop.Windows;
using System.Collections.Concurrent;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;

namespace RadianTools.Hardware.Input.Windows;

/// <summary>
/// マウス操作
/// </summary>
public enum MouseOp
{
    Move = 0,
    ButtonLeft,
    ButtonRight,
    ButtonMiddle,
    Button4,
    Button5,
    VWheel,
    HWheel,
}

/// <summary>
/// RawInputマウス情報
/// </summary>
public ref struct RawMouseEventArgs
{
    public RawMouseEventArgs(ref HidDeviceDetail detail, ref RAWINPUT rawInput)
    {
        Handle = ref rawInput.header.hDevice;
        Detail = ref detail;
        Mouse = ref rawInput.data.mouse;
        WheelDelta = ref Mouse.ButtonData;

        MoveAbsolute = (Mouse.Flags & MOUSE_MOVE.MOUSE_MOVE_ABSOLUTE) != 0;
        MappingVirtualDesktop = (Mouse.Flags & MOUSE_MOVE.MOUSE_VIRTUAL_DESKTOP) != 0;
        if (MoveAbsolute)
        {
            RECT rect;
            if (MappingVirtualDesktop)
            {
                rect.Left = User32.GetSystemMetrics(SM_INDEX.SM_CXVIRTUALSCREEN);
                rect.Top = User32.GetSystemMetrics(SM_INDEX.SM_CYVIRTUALSCREEN);
                rect.Right = User32.GetSystemMetrics(SM_INDEX.SM_CXVIRTUALSCREEN);
                rect.Bottom = User32.GetSystemMetrics(SM_INDEX.SM_CYVIRTUALSCREEN);
            }
            else
            {
                rect.Left = User32.GetSystemMetrics(SM_INDEX.SM_CXVIRTUALSCREEN);
                rect.Top = User32.GetSystemMetrics(SM_INDEX.SM_CYVIRTUALSCREEN);
                rect.Right = User32.GetSystemMetrics(SM_INDEX.SM_CXVIRTUALSCREEN);
                rect.Bottom = User32.GetSystemMetrics(SM_INDEX.SM_CYVIRTUALSCREEN);
            }

            X = User32.MulDiv(X, rect.Right, 65535) + rect.Left;
            Y = User32.MulDiv(Y, rect.Bottom, 65535) + rect.Top;
        } 
        else
        {
            X = Mouse.LastX;
            Y = Mouse.LastY;
        }

        if ((Mouse.ButtonFlags & RI_MOUSE.RI_MOUSE_LEFT_BUTTON_DOWN) != 0)
        {
            MouseOp = MouseOp.ButtonLeft;
            PushState = true;
        }
        else if ((Mouse.ButtonFlags & RI_MOUSE.RI_MOUSE_LEFT_BUTTON_UP) != 0)
        {
            MouseOp = MouseOp.ButtonLeft;
        }
        else if ((Mouse.ButtonFlags & RI_MOUSE.RI_MOUSE_RIGHT_BUTTON_DOWN) != 0)
        {
            MouseOp = MouseOp.ButtonRight;
            PushState = true;
        }
        else if ((Mouse.ButtonFlags & RI_MOUSE.RI_MOUSE_RIGHT_BUTTON_UP) != 0)
        {
            MouseOp = MouseOp.ButtonRight;
        }
        else if ((Mouse.ButtonFlags & RI_MOUSE.RI_MOUSE_MIDDLE_BUTTON_DOWN) != 0)
        {
            MouseOp = MouseOp.ButtonRight;
            PushState = true;
        }
        else if ((Mouse.ButtonFlags & RI_MOUSE.RI_MOUSE_MIDDLE_BUTTON_UP) != 0)
        {
            MouseOp = MouseOp.ButtonMiddle;
        }
        else if ((Mouse.ButtonFlags & RI_MOUSE.RI_MOUSE_BUTTON_4_DOWN) != 0)
        {
            MouseOp = MouseOp.Button4;
            PushState = true;
        }
        else if ((Mouse.ButtonFlags & RI_MOUSE.RI_MOUSE_BUTTON_4_UP) != 0)
        {
            MouseOp = MouseOp.Button4;
        }
        else if ((Mouse.ButtonFlags & RI_MOUSE.RI_MOUSE_BUTTON_5_DOWN) != 0)
        {
            MouseOp = MouseOp.Button5;
            PushState = true;
        }
        else if ((Mouse.ButtonFlags & RI_MOUSE.RI_MOUSE_BUTTON_5_UP) != 0)
        {
            MouseOp = MouseOp.Button5;
        }
        else if ((Mouse.ButtonFlags & RI_MOUSE.RI_MOUSE_WHEEL) != 0)
        {
            MouseOp = MouseOp.VWheel;
        }
        else if ((Mouse.ButtonFlags & RI_MOUSE.RI_MOUSE_HWHEEL) != 0)
        {
            MouseOp = MouseOp.HWheel;
        }
    }

    /// <summary>
    /// HIDデバイス詳細
    /// </summary>
    public readonly ref HidDeviceDetail Detail;

    /// <summary>
    /// マウス情報情報
    /// </summary>
    public readonly ref RAWMOUSE Mouse;

    /// <summary>
    /// デバイスハンドル
    /// </summary>
    public readonly ref HDEVICE Handle;

    /// <summary>
    /// X移動量（MoveAbsolute=trueの場合、絶対座標）
    /// </summary>
    public readonly int X;

    /// <summary>
    /// Y移動量（MoveAbsolute=trueの場合、絶対座標）
    /// </summary>
    public readonly int Y;

    /// <summary>
    /// ホイール移動量
    /// </summary>
    public readonly ref short WheelDelta;

    /// <summary>
    /// マウス操作
    /// </summary>
    public readonly MouseOp MouseOp;

    /// <summary>
    /// On/Off状態
    /// </summary>
    public readonly bool PushState;

    /// <summary>
    /// LastX, LastY が絶対座標かどうか
    /// </summary>
    public readonly bool MoveAbsolute;

    /// <summary>
    /// マウス座標が仮想デスクトップにマッピングされているかどうか
    /// </summary>
    public readonly bool MappingVirtualDesktop;

    /// <inheritdoc />
    public override string ToString() =>
        $"{nameof(Handle)} = 0x{Handle.Value:X16}"
        + $", {nameof(MouseOp)} = {MouseOp}"
        + $", {nameof(PushState)} = {PushState} "
        + $", {nameof(MoveAbsolute)} = {MoveAbsolute}"
        + $", {nameof(X)} = {X}"
        + $", {nameof(Y)} = {Y}"
        + $", {nameof(WheelDelta)} = {WheelDelta}"
        + $", {nameof(MappingVirtualDesktop)} = {MappingVirtualDesktop} "
        + $", {nameof(Detail.ProductName)} = {Detail.ProductName}"
        + $", {nameof(Detail.Manufacturer)} = {Detail.Manufacturer}"
        ;
}

/// <summary>
/// RawInputkキーボード情報
/// </summary>
public ref struct RawKeyboardEventArgs
{
    public RawKeyboardEventArgs(ref HidDeviceDetail detail, ref RAWINPUT rawInput)
    {
        Handle = ref rawInput.header.hDevice;
        Detail = ref detail;
        Keyboard = ref rawInput.data.keyboard;
        VKey = ref rawInput.data.keyboard.VKey;

        PushState = (Keyboard.Flags & RI_KEY.RI_KEY_BREAK) == 0;
    }

    /// <summary>
    /// HIDデバイス詳細
    /// </summary>
    public readonly ref HidDeviceDetail Detail;

    /// <summary>
    /// キーボード状態情報
    /// </summary>
    public readonly ref RAWKEYBOARD Keyboard;

    /// <summary>
    /// デバイスハンドル
    /// </summary>
    public readonly ref HDEVICE Handle;

    /// <summary>
    /// 仮想キーコード
    /// </summary>
    public readonly ref VKey VKey;

    /// <summary>
    /// On/Off状態
    /// </summary>
    public readonly bool PushState;

    /// <inheritdoc />
    public override string ToString() =>
        $"{nameof(Handle)} = 0x{Handle.Value:X16}"
        + $", {nameof(VKey)} = {VKey}"
        + $", {nameof(PushState)} = {PushState}"
        + $", {nameof(Detail.ProductName)} = {Detail.ProductName}"
        + $", {nameof(Detail.Manufacturer)} = {Detail.Manufacturer}"
        ;
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
            const int RID_INPUT = 0x10000003;

            int headerSize = Marshal.SizeOf(typeof(RAWINPUTHEADER));
            int size = Marshal.SizeOf(typeof(RAWINPUT));
            int result = User32.GetRawInputData(hRawInput, RID_INPUT, out var input, ref size, headerSize);
            if (result < 0)
                return;

            // キャッシュからデバイス詳細を取り出す。無ければ取得する。
            if (!_dicDeviceDetail.TryGetValue(input.header.hDevice, out var detail))
            {
                detail = HidHelper.GetDeviceDetail(input.header.hDevice);
                _dicDeviceDetail[input.header.hDevice] = detail;
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
                        // KEYBOARD_OVERRUN_MAKE_CODEは無視する
                        if (input.data.keyboard.VKey == VKey.KEYBOARD_OVERRUN_MAKE_CODE)
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
}