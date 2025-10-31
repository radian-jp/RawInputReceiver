using System.Collections.Concurrent;
using System.Runtime.InteropServices;
using RadianTools.Interop.Windows;

namespace RadianTools.Hardware.Input.Windows;

public ref struct RawMouseEventArgs
{
    public RawMouseEventArgs(ref HidDeviceDetail detail, ref RAWINPUT rawInput)
    {
        Detail = ref detail;
        Mouse = ref rawInput.data.mouse;
    }

    public readonly ref HidDeviceDetail Detail;
    public readonly ref RAWMOUSE Mouse;
    public override string ToString() => $"{Detail.ToString()}, {Mouse.ToString()}";
}

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


public class RawInputReceiver : IDisposable
{
    private bool _disposed;

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

    private RawInputReceiverWindow _receiverWindow = null!;

    public RawInputReceiver()
    {
        _receiverWindow = new RawInputReceiverWindow(this);
        _receiverWindow.Run();
    }

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

    private class RawInputReceiverWindow : HiddenWindow
    {
        private RawInputReceiver _receiver;

        public RawInputReceiverWindow(RawInputReceiver receiver)
        {
            _receiver = receiver;
        }

        public void RegisterMouse()
        {
            InvokeAsync(() =>
            {
                var device = new RAWINPUTDEVICE(RIM_TYPE.RIM_TYPEMOUSE, this.Handle, false);
                User32.RegisterRawInputDevices(in device, 1, Marshal.SizeOf<RAWINPUTDEVICE>());
            });
        }

        public void UnregisterMouse()
        {
            InvokeAsync(() =>
            {
                var device = new RAWINPUTDEVICE(RIM_TYPE.RIM_TYPEMOUSE, HWND.Null, true);
                User32.RegisterRawInputDevices(in device, 1, Marshal.SizeOf<RAWINPUTDEVICE>());
            });
        }

        public void RegisterKeyboard()
        {
            InvokeAsync(() =>
            {
                var device = new RAWINPUTDEVICE(RIM_TYPE.RIM_TYPEKEYBOARD, this.Handle, false);
                User32.RegisterRawInputDevices(in device, 1, Marshal.SizeOf<RAWINPUTDEVICE>());
            });
        }

        public void UnregisterKeyboard()
        {
            InvokeAsync(() =>
            {
                var device = new RAWINPUTDEVICE(RIM_TYPE.RIM_TYPEKEYBOARD, HWND.Null, true);
                User32.RegisterRawInputDevices(in device, 1, Marshal.SizeOf<RAWINPUTDEVICE>());
            });
        }

        protected override nint WndProc(HWND hwnd, WindowMessage msg, nint wParam, nint lParam)
        {
            switch (msg)
            {
                case WindowMessage.WM_INPUT:
                    OnWmInput(hwnd, wParam, lParam);
                    break;
            }

            return base.WndProc(hwnd, msg, wParam, lParam);
        }

        private readonly ConcurrentDictionary<nint, HidDeviceDetail> _dicDeviceDetail = [];

        private void OnWmInput(HWND hwnd, nint wParam, nint lParam)
        {
            const int RID_INPUT = 0x10000003;

            var hRawInput = (HRAWINPUT)lParam;
            int headerSize = Marshal.SizeOf(typeof(RAWINPUTHEADER));
            int size = Marshal.SizeOf(typeof(RAWINPUT));
            int result = User32.GetRawInputData(lParam, RID_INPUT, out var input, ref size, headerSize);
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
                        var keyboard = new RawKeyboardEventArgs(ref detail, ref input);
                        _receiver._keyboardReceived(keyboard);
                    }
                    break;
            }
        }

    }
}