using System.Runtime.InteropServices;

namespace RadianTools.Interop.Windows;

public static class Hid
{
    [DllImport("hid.dll")]
    public static extern BOOL HidD_GetProductString(
        IntPtr HidDeviceObject,
        IntPtr Buffer,
        int BufferLength
    );

    [DllImport("hid.dll")]
    public static extern BOOL HidD_GetManufacturerString(
        IntPtr HidDeviceObject,
        IntPtr Buffer,
        int BufferLength
    );

    [DllImport("hid.dll")]
    public static extern BOOL HidD_GetSerialNumberString(
        IntPtr HidDeviceObject,
        IntPtr Buffer,
        int BufferLength
    );
}
