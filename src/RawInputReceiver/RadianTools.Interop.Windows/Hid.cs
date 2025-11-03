using System.Runtime.InteropServices;

namespace RadianTools.Interop.Windows;

public static class Hid
{
    [DllImport("hid.dll")]
    public static extern BOOL HidD_GetProductString(
        HANDLE HidDeviceObject,
        IntPtr Buffer,
        int BufferLength
    );

    [DllImport("hid.dll")]
    public static extern BOOL HidD_GetManufacturerString(
        HANDLE HidDeviceObject,
        IntPtr Buffer,
        int BufferLength
    );

    [DllImport("hid.dll")]
    public static extern BOOL HidD_GetSerialNumberString(
        HANDLE HidDeviceObject,
        IntPtr Buffer,
        int BufferLength
    );
}
