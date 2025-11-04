using System.Runtime.InteropServices;

namespace RadianTools.Interop.Windows;

[StructLayout(LayoutKind.Sequential)]
public struct CREATESTRUCT
{
    public IntPtr lpCreateParams;
    public IntPtr hInstance;
    public IntPtr hMenu;
    public IntPtr hwndParent;
    public int cy;
    public int cx;
    public int y;
    public int x;
    public int style;
    public IntPtr lpszName;
    public IntPtr lpszClass;
    public int dwExStyle;
}

public record struct DEVINST(uint Value)
{
    public static explicit operator uint(DEVINST value) => value.Value;
    public static explicit operator DEVINST(uint value) => new DEVINST(value);
}

[StructLayout(LayoutKind.Sequential)]
public struct DEV_BROADCAST_HANDLE
{
    public int dbch_size;
    public int dbch_devicetype;
    public int dbch_reserved;
    public IntPtr dbch_handle;
    public IntPtr dbch_hdevnotify;
    public Guid dbch_eventguid;
    public int dbch_nameoffset;
    public byte dbch_data;
}

[StructLayout(LayoutKind.Sequential)]
public struct DEV_BROADCAST_HDR
{
    public int dbch_size;
    public DBT_DEVTYP dbch_devicetype;
    public int dbch_reserved;
}


[StructLayout(LayoutKind.Sequential)]
public struct BOOL : IEquatable<BOOL>
{
    int _intValue;
    public BOOL(bool value) => _intValue = value ? 1 : 0;
    public BOOL(int value) => _intValue = value > 0 ? 1 : 0;

    public bool Equals(BOOL other) => _intValue == other._intValue;

    public static implicit operator bool(BOOL value) => value._intValue > 0;
}

[StructLayout(LayoutKind.Sequential)]
public struct DEVPROPKEY
{
    public Guid fmtid;
    public uint pid;
}

/// <summary>
/// 定義済みデバイスプロパティキー
/// </summary>
public static class DEVPKEY
{
    public static readonly DEVPROPKEY NAME = new DEVPROPKEY
    {
        fmtid = new Guid("B725F130-47EF-101A-A5F1-02608C9EEBAC"),
        pid = 10
    };

    public static readonly DEVPROPKEY Device_DeviceDesc = new DEVPROPKEY
    {
        fmtid = new Guid("A45C254E-DF1C-4EFD-8020-67D146A850E0"), 
        pid = 2
    };

    public static readonly DEVPROPKEY Device_FriendlyName = new DEVPROPKEY
    {
        fmtid = new Guid("A45C254E-DF1C-4EFD-8020-67D146A850E0"),
        pid = 14
    };

    public static readonly DEVPROPKEY Device_Manufacturer = new DEVPROPKEY
    {
        fmtid = new Guid("A45C254E-DF1C-4EFD-8020-67D146A850E0"),
        pid = 13
    };
}

[StructLayout(LayoutKind.Sequential)]
public struct HRESULT
{
    private int _value;
    int Value => _value;

    public bool Succeeded => this.Value >= 0;
    public bool Failed => this.Value < 0;
    public bool IsOK => this.Value == 0;
    public bool IsNotOK => this.Value != 0;

    public HRESULT ThrowOnFailure(IntPtr errorInfo = default)
    {
        Marshal.ThrowExceptionForHR(this.Value, errorInfo);
        return this;
    }
}

[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct RAWINPUT
{
    [StructLayout(LayoutKind.Explicit)]
    public struct DEVICEDATA
    {
        [FieldOffset(0)]
        public RAWMOUSE mouse;
        [FieldOffset(0)]
        public RAWKEYBOARD keyboard;
    }

    public RAWINPUTHEADER header;
    public DEVICEDATA data;
}

public struct RAWINPUTDEVICE
{
    public short UsagePage;
    public short Usage;
    public RIDEV Flags;
    public HWND HWndTarget;

    public RAWINPUTDEVICE(RIM_TYPE deviceType, HWND hWndTarget, bool remove)
    {
        switch (deviceType)
        {
            case RIM_TYPE.RIM_TYPEMOUSE:
                UsagePage = 1;
                Usage = 2;
                break;

            case RIM_TYPE.RIM_TYPEKEYBOARD:
                UsagePage = 1;
                Usage = 6;
                break;

            default:
                throw new ArgumentException($"RIM_TYPE {deviceType:D} is not supported.");
        }

        if (remove)
        {
            Flags = RIDEV.RIDEV_REMOVE;
            HWndTarget = Null<HWND>.Value;
        }
        else
        {
            Flags = RIDEV.RIDEV_INPUTSINK | RIDEV.RIDEV_NOLEGACY;
            HWndTarget = hWndTarget;
        }
    }
}

[StructLayout(LayoutKind.Sequential)]
public struct RAWINPUTDEVICELIST
{
    public HDEVICE hDevice;
    public RIM_TYPE dwType;
}

[StructLayout(LayoutKind.Sequential)]
public struct RAWINPUTHEADER
{
    public RIM_TYPE Type;
    public uint dwSize;
    public HDEVICE hDevice;
    public IntPtr wParam;
}

[StructLayout(LayoutKind.Sequential, Pack = 2)]
public record struct RAWKEYBOARD
{
    public ushort MakeCode;
    public RI_KEY Flags;
    public ushort Reserved;
    public VKey VKey;
    public WindowMessage Message;
    public int ExtraInformation;
}

[StructLayout(LayoutKind.Sequential, Pack = 2)]
public record struct RAWMOUSE
{
    public MOUSE_MOVE Flags;
    private short _padding;
    public RI_MOUSE ButtonFlags;
    public short ButtonData;
    public int RawButtons;
    public int LastX;
    public int LastY;
    public int ExtraInformation;
}

[StructLayout(LayoutKind.Sequential)]
public struct RECT
{
    public int Left;
    public int Top;
    public int Right;
    public int Bottom;
}

[StructLayout(LayoutKind.Sequential)]
public struct SP_DEVICE_INTERFACE_DATA
{
    public uint cbSize;
    public Guid InterfaceClassGuid;
    public uint Flags;
    public IntPtr Reserved;
}

[StructLayout(LayoutKind.Sequential)]
public struct SP_DEVINFO_DATA
{
    public uint cbSize;
    public Guid ClassGuid;
    public uint DevInst;
    public IntPtr Reserved;
}

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
public struct WNDCLASSEX
{
    public uint cbSize;
    public uint style;
    public WNDPROC lpfnWndProc;
    public int cbClsExtra;
    public int cbWndExtra;
    public IntPtr hInstance;
    public IntPtr hIcon;
    public IntPtr hCursor;
    public IntPtr hbrBackground;
    public IntPtr lpszMenuName;
    [MarshalAs(UnmanagedType.LPWStr)]
    public string lpszClassName;
    public IntPtr hIconSm;
}

[StructLayout(LayoutKind.Sequential)]
public struct MSG
{
    public HWND hwnd;
    public WindowMessage message;
    public IntPtr wParam;
    public IntPtr lParam;
    public uint time;
    public int pt_x;
    public int pt_y;
}

public delegate IntPtr WNDPROC(HWND hwnd, WindowMessage msg, IntPtr wParam, IntPtr lParam);

#region Handles

public interface IPtrHandle : IEquatable<IntPtr>
{
    IntPtr Value { get; set; }
    bool IEquatable<IntPtr>.Equals(IntPtr other) => Value != other;
}

public static class Null<T> where T : unmanaged, IPtrHandle
{
    public static T Value = new T() { Value = IntPtr.Zero };
}

public static class IPtrHandleExtentions
{
    public static bool IsNull(this IPtrHandle handle) => handle.Value == IntPtr.Zero;
}

public record struct HWND(IntPtr Value) : IPtrHandle
{
    public static explicit operator IntPtr(HWND value) => value.Value;
    public static explicit operator HWND(IntPtr value) => new HWND(value);
}

public record struct HANDLE(IntPtr Value) : IPtrHandle
{
    public static explicit operator IntPtr(HANDLE value) => value.Value;
    public static explicit operator HANDLE(IntPtr value) => new HANDLE(value);
    public bool IsInvalid() => Value == -1;
}

public record struct HDEVICE(IntPtr Value) : IPtrHandle
{
    public static explicit operator IntPtr(HDEVICE value) => value.Value;
    public static explicit operator HDEVICE(IntPtr value) => new HDEVICE(value);
}


public record struct HRAWINPUT(IntPtr Value) : IPtrHandle
{
    public static explicit operator IntPtr(HRAWINPUT value) => value.Value;
    public static explicit operator HRAWINPUT(IntPtr value) => new HRAWINPUT(value);
}

public record struct HDEVINFO(IntPtr Value) : IPtrHandle
{
    public static explicit operator IntPtr(HDEVINFO value) => value.Value;
    public static explicit operator HDEVINFO(IntPtr value) => new HDEVINFO(value);
}

#endregion
