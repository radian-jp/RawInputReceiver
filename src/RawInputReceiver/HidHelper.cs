using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using RadianTools.Interop.Windows;

namespace RadianTools.Hardware.Input.Windows;

/// <summary>
/// HID用補助クラス
/// </summary>
public static class HidHelper
{
    /// <summary>
    /// RegisterRawInputDevicesの簡易呼び出し
    /// </summary>
    /// <param name="usagePage">HIDデバイスのUsagePage値</param>
    /// <param name="usage">HIDデバイスのUsage値</param>
    /// <param name="hwnd">登録するウィンドウ</param>
    /// <param name="remove">登録時はFalseを指定、解除時はTrueを指定</param>
    public static int RegisterRawInputDeviceSlim(HidUsagePage usagePage, HidUsage usage, HWND hwnd, bool remove)
    {
        var device = new RAWINPUTDEVICE(usagePage, usage, hwnd, remove);
        var result = User32.RegisterRawInputDevices(in device, 1, Marshal.SizeOf<RAWINPUTDEVICE>());
        Debug.WriteLine($"{nameof(RegisterRawInputDeviceSlim)}({usagePage}, {usage}, hwnd:{hwnd.Value:X16}, remove:{remove}) = {result}");
        return result;
    }

    /// <summary>
    /// 存在するRAWINPUTデバイスのデバイスハンドルを取得する。
    /// </summary>
    /// <returns>デバイスハンドルのList。取得失敗時は空のList。</returns>
    public static IReadOnlyList<HDEVICE> GetRawInputDeviceHandles()
    {
        uint numDevices = 0;
        uint cbSize = (uint)Marshal.SizeOf<RAWINPUTDEVICELIST>();
        if (User32.GetRawInputDeviceList(IntPtr.Zero, ref numDevices, cbSize) == -1)
            return Array.Empty<HDEVICE>();

        Span<RAWINPUTDEVICELIST> pDevList = stackalloc RAWINPUTDEVICELIST[(int)numDevices];
        if (User32.GetRawInputDeviceList(ref pDevList[0], ref numDevices, cbSize) == -1)
            return Array.Empty<HDEVICE>();

        var listHandles = new List<HDEVICE>();
        foreach (var dev in pDevList)
        {
            listHandles.Add(dev.hDevice);
        }
        return listHandles;
    }

    /// <summary>
    /// 仮想キーコードをスキャンコード、スキャンコード情報フラグを元に修正する。
    /// </summary>
    /// <param name="vKey">仮想キーコード</param>
    /// <param name="makeCode">スキャンコード</param>
    /// <param name="flags">スキャンコード情報フラグ</param>
    /// <returns>修正済み仮想キーコード</returns>
    /// <remarks>
    /// この実装は PPSSPP (https://github.com/hrydgard/ppsspp) の RawInput.cpp を参考にしています。
    /// ライセンスは BSD スタイルで、著作権表示を保持しています。
    /// </remarks>
    public static VKey GetTrueVKey(VKey vKey, ushort makeCode, RI_KEY flags)
    {
        switch (vKey)
        {
            case VKey.VK_SHIFT:
                vKey = (VKey)User32.MapVirtualKey(makeCode, MAPVK.MAPVK_VSC_TO_VK_EX);
                break;

            case VKey.VK_CONTROL:
                if ((flags & RI_KEY.RI_KEY_E0) != 0)
                    vKey = VKey.VK_RCONTROL;
                else
                    vKey = VKey.VK_LCONTROL;
                break;

            case VKey.VK_MENU:
                if ((flags & RI_KEY.RI_KEY_E0) != 0)
                    vKey = VKey.VK_RMENU;
                else
                    vKey = VKey.VK_LMENU;
                break;

            case VKey.VK_NUMLOCK:
                vKey = (VKey)(User32.MapVirtualKey((uint)vKey, MAPVK.MAPVK_VK_TO_VSC) | 0x100);
                break;
        }

        return vKey;
    }

    /// <summary>
    /// デバイス情報詳細を取得する。
    /// </summary>
    /// <param name="hDevice">デバイスハンドル</param>
    /// <returns>デバイス詳細</returns>
    public static HidDeviceDetail GetDeviceDetail(HDEVICE hDevice)
    {
        var devicePath = GetDevicePath(hDevice);
        if (string.IsNullOrEmpty(devicePath))
            return HidDeviceDetail.Empty;

        var hidDevicePath = HidDevicePath.FromPathString(devicePath);
        var manufacturer = "";
        var productName = "";
        var fh = Kernel32.CreateFile(devicePath, 0, FILE_SHARE.FILE_SHARE_READ, IntPtr.Zero, FILE_DISPOSITION.OPEN_EXISTING, 0, IntPtr.Zero);
        if (!fh.IsInvalid())
        {
            try
            {
                const int MAX_BUF_SIZE = 4092;
                unsafe
                {
                    var pName = stackalloc char[MAX_BUF_SIZE / sizeof(char)];
                    if (Hid.HidD_GetIndexedString(fh, HIDStringIndex.Product, (IntPtr)pName, MAX_BUF_SIZE))
                        productName = new string(pName);

                    if (String.IsNullOrEmpty(productName))
                    {
                        if (Hid.HidD_GetProductString(fh, (IntPtr)pName, MAX_BUF_SIZE))
                            productName = new string(pName);
                    }

                    if (Hid.HidD_GetManufacturerString(fh, (IntPtr)pName, MAX_BUF_SIZE))
                        manufacturer = new string(pName);
                }
            }
            finally
            {
                Kernel32.CloseHandle(fh);
            }
        }

        var friendlyName = "";
        if (hidDevicePath != null)
        {
            var deviceInstanceId = DevicePathToDeviceInstanceId(devicePath);
            const int CM_LOCATE_DEVNODE_NORMAL = 0x00000000;
            var cr = Cfgmgr32.CM_Locate_DevNode(out var devInst, deviceInstanceId, CM_LOCATE_DEVNODE_NORMAL);
            if (cr == CONFIGRET.CR_SUCCESS)
            {
                friendlyName = GetDevNodeProperty(devInst, in DEVPKEY.NAME);

                if (string.IsNullOrEmpty(manufacturer))
                    manufacturer = GetDevNodeProperty(devInst, in DEVPKEY.Device_Manufacturer);
            }
        }

        return new HidDeviceDetail(hDevice, hidDevicePath, friendlyName, manufacturer, productName);
    }

    private static string GetDevicePath(HDEVICE hDevice)
    {
        int devicePathSize = 0;
        User32.GetRawInputDeviceInfo(hDevice, RIDI.DEVICENAME, IntPtr.Zero, ref devicePathSize);
        if (devicePathSize == 0)
            return "";

        unsafe
        {
            // MSのドキュメントでは、GetRawInputDeviceInfoW は文字のバイト数を返すとあるが、
            // 実装は文字数が返ってくるので deviceNameSize そのまま使用でOK
            var pDevPath = stackalloc char[devicePathSize];
            User32.GetRawInputDeviceInfo(hDevice, RIDI.DEVICENAME, (IntPtr)pDevPath, ref devicePathSize);
            var devicePath = new string(pDevPath).TrimEnd('\0');
            return devicePath;
        }
    }

    private static string DevicePathToDeviceInstanceId(string devicePath)
    {
        // 先頭の "\\?\" を除去
        if (devicePath.StartsWith(@"\\?\"))
            devicePath = devicePath.Substring(4);

        // GUID の前までを抽出（# で区切られている）
        int guidIndex = devicePath.IndexOf("#{");
        if (guidIndex >= 0)
            devicePath = devicePath.Substring(0, guidIndex);

        // インスタンス ID の形式に変換（# → \）
        return devicePath.Replace('#', '\\');
    }

    private static string GetDevNodeProperty(DEVINST devInst, in DEVPROPKEY devPropKey)
    {
        var cr = Cfgmgr32.CM_Get_DevNode_Property(devInst, in devPropKey, out var propertyType, IntPtr.Zero, out var propertySize, 0);
        if (cr != CONFIGRET.CR_BUFFER_SMALL)
            return "";

        Span<char> bufProp = stackalloc char[(int)propertySize / sizeof(char)];
        cr = Cfgmgr32.CM_Get_DevNode_Property(devInst, in devPropKey, out propertyType, ref bufProp[0], out propertySize, 0);
        if (cr != CONFIGRET.CR_SUCCESS)
            return "";

        string prop = new string(bufProp).TrimEnd('\0');
        return prop;
    }
}

/// <summary>
/// HIDデバイス詳細
/// </summary>
public record HidDeviceDetail(
    /// <summary>デバイスハンドル</summary>
    HDEVICE Handle,
    /// <summary>デバイスパス</summary>
    HidDevicePath DevicePath,
    /// <summary>フレンドリー名</summary>
    string FriendlyName = "",
    /// <summary>製造元</summary>
    string Manufacturer = "",
    /// <summary>プロダクト名</summary>
    string ProductName = "")
{
    /// <summary>空のインスタンス</summary>
    public static HidDeviceDetail Empty { get; } = new HidDeviceDetail(Null<HDEVICE>.Value, HidDevicePath.Empty);
}

/// <summary>
/// HIDデバイスパス情報
/// </summary>
public record HidDevicePath(
    /// <summary>デバイスパス</summary>
    string Value,

    /// <summary>デバイスインターフェースクラス GUID</summary>
    Guid? InterfaceClassGuid = null,

    /// <summary>ベンダーID（VID_XXXX）</summary>
    string VendorId = "",

    /// <summary>プロダクトID（PID_XXXX）</summary>
    string ProductId = "",

    /// <summary>インターフェース番号（MI_XX）</summary>
    string InterfaceNo = "",

    /// <summary>コレクション番号（ColXX）</summary>
    string CollectionNo = "",

    /// <summary>PnP識別子のサフィックス部分</summary>
    string PnpSuffix = ""
)
{
    /// <summary>空のインスタンス</summary>
    public static HidDevicePath Empty { get; } = new HidDevicePath("");

    private static readonly Regex _regexVendorId = new(@"VID_([0-9A-F]{4})", RegexOptions.IgnoreCase);
    private static readonly Regex _regexProductId = new(@"PID_([0-9A-F]{4})", RegexOptions.IgnoreCase);
    private static readonly Regex _regexInterfaceNo = new(@"MI_([0-9]{2})", RegexOptions.IgnoreCase);
    private static readonly Regex _regexCollectionNo = new(@"Col([0-9]{2})", RegexOptions.IgnoreCase);
    private static readonly Regex _regexInterfaceClassGuid = new(@"\{([0-9A-F\-]{36})\}", RegexOptions.IgnoreCase);
    private static readonly Regex _regexPnpSuffix = new(@"#([^#]+)#\{", RegexOptions.IgnoreCase);

    /// <summary>
    /// HID デバイスパス文字列から情報を抽出してインスタンスを生成します。
    /// </summary>
    /// <param name="devicePath">HID デバイスパス</param>
    /// <returns>抽出された情報を含む <see cref="HidDevicePath"/> インスタンス</returns>
    public static HidDevicePath FromPathString(string devicePath)
    {
        static string MatchGroup(string input, Regex regex)
        {
            var match = regex.Match(input);
            return match.Success ? match.Groups[1].Value : "";
        }

        string vid = MatchGroup(devicePath, _regexVendorId);
        string pid = MatchGroup(devicePath, _regexProductId);
        string mi = MatchGroup(devicePath, _regexInterfaceNo);
        string col = MatchGroup(devicePath, _regexCollectionNo);
        string pnp = MatchGroup(devicePath, _regexPnpSuffix);

        var guidMatch = _regexInterfaceClassGuid.Match(devicePath);
        Guid? guid = guidMatch.Success ? Guid.Parse(guidMatch.Groups[1].Value) : null;

        return new HidDevicePath(devicePath, guid, vid, pid, mi, col, pnp);
    }
}
