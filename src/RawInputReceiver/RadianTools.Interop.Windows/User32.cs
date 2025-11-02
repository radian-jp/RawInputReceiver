using System.Runtime.InteropServices;

namespace RadianTools.Interop.Windows;

public static class User32
{
    [DllImport("user32.dll", SetLastError = true)]
    public static extern int RegisterRawInputDevices(
        in RAWINPUTDEVICE devices,
        int number,
        int size);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern int GetRawInputData(
        HRAWINPUT hRawInput,
        int command,
        out RAWINPUT data,
        ref int size,
        int headerSize);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern int GetRawInputData(
        HRAWINPUT hRawInput,
        int command,
        IntPtr pData,
        ref int size,
        int headerSize);

    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern int GetRawInputDeviceInfo(
        IntPtr hDevice,
        RIDI uiCommand,
        IntPtr pData,
        ref int pcbSize
    );

    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern int GetRawInputDeviceInfo(
        IntPtr hDevice,
        RIDI uiCommand,
        ref char pData,
        ref int pcbSize
    );

    [DllImport("user32.dll", SetLastError = true)]
    public static extern IntPtr DefWindowProc(HWND hWnd, WindowMessage msg, IntPtr wParam, IntPtr lParam);  

    [DllImport("user32.dll", EntryPoint = "SetWindowLongPtrW", SetLastError = true)]
    private static extern IntPtr SetWindowLongPtr64(HWND hWnd, GWLP nIndex, IntPtr dwNewLong);

    [DllImport("user32.dll", EntryPoint = "SetWindowLongW", SetLastError = true)]
    private static extern IntPtr SetWindowLong32(HWND hWnd, GWLP nIndex, IntPtr dwNewLong);

    public delegate IntPtr DelegateSetWindowLongPtr(HWND hWnd, GWLP nIndex, IntPtr dwNewLong);

    public static DelegateSetWindowLongPtr SetWindowLongPtr = (IntPtr.Size == 8) ? SetWindowLongPtr64 : SetWindowLong32;

    [DllImport("user32.dll", EntryPoint = "GetWindowLongPtrW", SetLastError = true)]
    private static extern IntPtr GetWindowLongPtr64(HWND hWnd, GWLP nIndex);

    [DllImport("user32.dll", EntryPoint = "GetWindowLongW", SetLastError = true)]
    private static extern IntPtr GetWindowLong32(HWND hWnd, GWLP nIndex);

    public delegate IntPtr DelegateGetWindowLongPtr64(HWND hWnd, GWLP nIndex);

    public static DelegateGetWindowLongPtr64 GetWindowLongPtr = (IntPtr.Size == 8) ? GetWindowLongPtr64 : GetWindowLong32;

    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern ushort RegisterClassEx([In] ref WNDCLASSEX lpwcx);

    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern bool UnregisterClass(string lpClassName, IntPtr hInstance);

    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern uint RegisterWindowMessage(string messageName);

    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern HWND CreateWindowEx(
        int dwExStyle,
        string lpClassName,
        string lpWindowName,
        int dwStyle,
        int x, int y, int nWidth, int nHeight,
        IntPtr hWndParent,
        IntPtr hMenu,
        IntPtr hInstance,
        IntPtr lpParam);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern int DestroyWindow(HWND hwnd);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern int GetMessage(out MSG lpMsg, IntPtr hWnd, uint min, uint max);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern int TranslateMessage([In] ref MSG lpMsg);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern IntPtr DispatchMessage([In] ref MSG lpMsg);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern int PostMessage(HWND hwnd, uint msg, IntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern void PostQuitMessage(uint nExitCode);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    public static extern uint MapVirtualKey(uint uCode, MAPVK uMapType);

}
