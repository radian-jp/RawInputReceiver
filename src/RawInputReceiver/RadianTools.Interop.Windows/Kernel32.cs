using System.Runtime.InteropServices;

namespace RadianTools.Interop.Windows;

public static class Kernel32
{
    [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
    public static extern HANDLE CreateFile(
        string lpFileName,
        FILE_ACCESS dwDesiredAccess,
        FILE_SHARE dwShareMode,
        IntPtr lpSecurityAttributes,
        FILE_DISPOSITION dwCreationDisposition,
        int dwFlagsAndAttributes,
        IntPtr hTemplateFile
    );

    [DllImport("kernel32.dll")]
    public static extern int CloseHandle(HANDLE hObject);

    [DllImport("kernel32.dll")]
    public static extern IntPtr GetModuleHandle(IntPtr lpModuleName);
}
