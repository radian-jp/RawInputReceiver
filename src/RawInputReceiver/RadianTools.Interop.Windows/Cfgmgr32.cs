using System.Runtime.InteropServices;

namespace RadianTools.Interop.Windows;

public static class Cfgmgr32
{
    [DllImport("cfgmgr32.dll", CharSet = CharSet.Unicode)]
    public static extern CONFIGRET CM_Locate_DevNode(
        out DEVINST pdnDevInst,
        string pDeviceID,
        uint ulFlags);

    [DllImport("cfgmgr32.dll", CharSet = CharSet.Unicode)]
    public static extern CONFIGRET CM_Get_DevNode_Property(
        DEVINST dnDevInst,
        in DEVPROPKEY PropertyKey,
        out uint PropertyType,
        ref char PropertyBuffer,
        out uint PropertyBufferSize,
        uint ulFlags);

    [DllImport("cfgmgr32.dll", CharSet = CharSet.Unicode)]
    public static extern CONFIGRET CM_Get_DevNode_Property(
        DEVINST dnDevInst,
        in DEVPROPKEY PropertyKey,
        out uint PropertyType,
        IntPtr PropertyBuffer,
        out uint PropertyBufferSize,
        uint ulFlags);
}
