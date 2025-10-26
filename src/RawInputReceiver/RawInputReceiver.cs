using System.Collections.Concurrent;
using System.Runtime.InteropServices;
using RadianTools.Interop.Windows;

namespace RadianTools.Hardware.Input.Windows;

public enum DeviceType : int
{
    Mouse,
    Keyboard
}

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
    public Action<RawMouseEventArgs>? MouseReceived;
    public Action<RawKeyboardEventArgs>? KeyboardReceived;

    private RawInputReceiverWindow _receiverWindow = null!;
    private DeviceType DeviceType { get; }

    public RawInputReceiver(DeviceType deviceType)
    {
        DeviceType = deviceType;

        _receiverWindow = new RawInputReceiverWindow(this);
        _receiverWindow.Run();
    }

    public void Dispose()
    {
        _receiverWindow.Dispose();
    }

    private class RawInputReceiverWindow : HiddenWindow
    {
        private RawInputReceiver _receiver;

        public RawInputReceiverWindow(RawInputReceiver receiver)
        {
            _receiver = receiver;
        }

        protected override nint WndProc(HWND hwnd, WindowMessage msg, nint wParam, nint lParam)
        {
            switch (msg)
            {
                case WindowMessage.WM_CREATE:
                    //WM_INPUT を有効にする
                    var device = new RAWINPUTDEVICE((int)_receiver.DeviceType, hwnd);
                    User32.RegisterRawInputDevices(in device, 1, Marshal.SizeOf(typeof(RAWINPUTDEVICE)));
                    break;

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
            if (result < 0 )
                return;

            if (!_dicDeviceDetail.TryGetValue(input.header.DeviceHandle, out var detail))
            {
                detail = HidHelper.GetDeviceDetail(input.header.DeviceHandle);
                _dicDeviceDetail[input.header.DeviceHandle] = detail;
            }

            switch(_receiver.DeviceType)
            {
                case DeviceType.Mouse:
                    if(_receiver.MouseReceived != null)
                    {
                        var mouse = new RawMouseEventArgs(ref detail, ref input);
                        _receiver.MouseReceived(mouse);
                    }
                    break;

                case DeviceType.Keyboard:
                    if (_receiver.KeyboardReceived != null)
                    {
                        var keyboard = new RawKeyboardEventArgs(ref detail, ref input);
                        _receiver.KeyboardReceived(keyboard);
                    }
                    break;
            }
        }

    }
}