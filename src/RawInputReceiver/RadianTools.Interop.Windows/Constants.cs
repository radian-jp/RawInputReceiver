namespace RadianTools.Interop.Windows;

public enum CONFIGRET : uint
{
    CR_SUCCESS = 0x00000000,
    CR_DEFAULT = 0x00000001,
    CR_OUT_OF_MEMORY = 0x00000002,
    CR_INVALID_POINTER = 0x00000003,
    CR_INVALID_FLAG = 0x00000004,
    CR_INVALID_DEVNODE = 0x00000005,
    CR_INVALID_DEVINST = CR_INVALID_DEVNODE,
    CR_INVALID_RES_DES = 0x00000006,
    CR_INVALID_LOG_CONF = 0x00000007,
    CR_INVALID_ARBITRATOR = 0x00000008,
    CR_INVALID_NODELIST = 0x00000009,
    CR_DEVNODE_HAS_REQS = 0x0000000A,
    CR_INVALID_RESOURCEID = 0x0000000B,
    CR_DLVXD_NOT_FOUND = 0x0000000C,
    CR_NO_SUCH_DEVNODE = 0x0000000D,
    CR_NO_MORE_LOG_CONF = 0x0000000E,
    CR_NO_MORE_RES_DES = 0x0000000F,
    CR_ALREADY_SUCH_DEVNODE = 0x00000010,
    CR_INVALID_RANGE = 0x00000011,
    CR_INVALID_RANGE_LIST = 0x00000012,
    CR_INVALID_DUPLICATE = 0x00000013,
    CR_NO_SUCH_LOGICAL_DEV = 0x00000014,
    CR_CREATE_BLOCKED = 0x00000015,
    CR_NOT_SYSTEM_VM = 0x00000016,
    CR_REMOVE_VETOED = 0x00000017,
    CR_APM_VETOED = 0x00000018,
    CR_INVALID_LOAD_TYPE = 0x00000019,
    CR_BUFFER_SMALL = 0x0000001A,
    CR_NO_ARBITRATOR = 0x0000001B,
    CR_NO_REGISTRY_HANDLE = 0x0000001C,
    CR_REGISTRY_ERROR = 0x0000001D,
    CR_INVALID_DEVICE_ID = 0x0000001E,
    CR_INVALID_DATA = 0x0000001F,
    CR_INVALID_API = 0x00000020,
    CR_DEVLOADER_NOT_READY = 0x00000021,
    CR_NEED_RESTART = 0x00000022,
    CR_NO_MORE_HW_PROFILES = 0x00000023,
    CR_DEVICE_NOT_FOUND = 0x00000024,
    CR_NO_SUCH_HW_PROFILE = 0x00000025,
    CR_INVALID_HW_PROFILE = 0x00000026,
    CR_NULL_VPD = 0x00000027,
    CR_VPD_NOT_FOUND = 0x00000028,
    CR_INVALID_VPD = 0x00000029,
    CR_BUFFER_TOO_SMALL = 0x0000002A,
    CR_NO_MORE_DEVNODES = 0x0000002B,
    CR_NO_MORE_RESOURCES = 0x0000002C,
    CR_NOT_DISABLEABLE = 0x0000002D,
    CR_FREE_RESOURCES = 0x0000002E,
    CR_QUERY_VETOED = 0x0000002F,
    CR_CANT_SHARE_IRQ = 0x00000030,
    CR_NO_DEPENDENT = 0x00000031,
    CR_SAME_RESOURCES = 0x00000032,
    CR_NO_SUCH_REGISTRY_KEY = 0x00000033,
    CR_INVALID_MACHINENAME = 0x00000034,
    CR_REMOTE_COMM_FAILURE = 0x00000035,
    CR_MACHINE_UNAVAILABLE = 0x00000036,
    CR_NO_CM_SERVICES = 0x00000037,
    CR_ACCESS_DENIED = 0x00000038,
    CR_CALL_NOT_IMPLEMENTED = 0x00000039,
    CR_INVALID_PROPERTY = 0x0000003A,
    CR_DEVICE_INTERFACE_ACTIVE = 0x0000003B,
    CR_NO_SUCH_DEVICE_INTERFACE = 0x0000003C,
    CR_INVALID_REFERENCE_STRING = 0x0000003D,
    CR_INVALID_CONFLICT_LIST = 0x0000003E,
    CR_INVALID_INDEX = 0x0000003F,
    CR_INVALID_STRUCTURE_SIZE = 0x00000040
}

public enum DBT_WPARAM
{
    DBT_DEVNODES_CHANGED = 0x0007,
    DBT_DEVICEREMOVECOMPLETE = 0x8004,
}

public enum DBT_DEVTYP
{
    DBT_DEVTYP_DEVICEINTERFACE = 0x00000005,
    DBT_DEVTYP_HANDLE = 0x00000006,
}

public enum FILE_ACCESS : uint
{
    GENERIC_ALL = 0x10000000,
    GENERIC_EXECUTE = 0x20000000,
    GENERIC_WRITE = 0x40000000,
    GENERIC_READ = 0x80000000,
}

[Flags]
public enum FILE_SHARE
{
    NOT_SET = 0,
    FILE_SHARE_READ = 0x00000001,
    FILE_SHARE_WRITE = 0x00000002,
    FILE_SHARE_DELETE = 0x00000004,
}

public enum FILE_DISPOSITION
{
    CREATE_NEW = 1,
    CREATE_ALWAYS = 2,
    OPEN_EXISTING = 3,
    OPEN_ALWAYS = 4,
    TRUNCATE_EXISTING = 5,
}

/// <summary>
/// GetWindowLongPtr / SetWindowLongPtr 用のインデックス定数（GWLP_）
/// </summary>
public enum GWLP : int
{
    /// <summary>
    /// ウィンドウプロシージャのアドレス（SetWindowLongPtr によって置き換え可能）
    /// </summary>
    GWLP_WNDPROC = -4,

    /// <summary>
    /// アプリケーションインスタンスのハンドル
    /// </summary>
    GWLP_HINSTANCE = -6,

    /// <summary>
    /// 親ウィンドウのハンドル（子ウィンドウやポップアップの親）
    /// </summary>
    GWLP_HWNDPARENT = -8,

    /// <summary>
    /// メニュー識別子またはメニューハンドル
    /// </summary>
    GWLP_ID = -12,

    /// <summary>
    /// ウィンドウスタイル（WS_ 系のフラグ）
    /// </summary>
    GWLP_STYLE = -16,

    /// <summary>
    /// 拡張ウィンドウスタイル（WS_EX_ 系のフラグ）
    /// </summary>
    GWLP_EXSTYLE = -20,

    /// <summary>
    /// アプリケーション定義のユーザーデータ
    /// </summary>
    GWLP_USERDATA = -21
}

public enum GPFIDL : uint
{
    GPFIDL_DEFAULT = 0U,
    GPFIDL_ALTNAME = 1U,
    GPFIDL_UNCPRINTER = 2U,
}

public enum HIDStringIndex : uint
{
    LangID = 0,
    Manufacturer,
    Product,
    SerialNumber
}

[Flags]
public enum MOUSE_MOVE : ushort
{
    MOUSE_MOVE_RELATIVE = 0x00,
    MOUSE_MOVE_ABSOLUTE = 0x01,
    MOUSE_VIRTUAL_DESKTOP = 0x02,
    MOUSE_ATTRIBUTES_CHANGED = 0x04,
    MOUSE_MOVE_NOCOALESCE = 0x08,
}

[Flags]
public enum RIDEV : int
{
    /// <summary>
    /// 受信を停止する
    /// </summary>
    RIDEV_REMOVE = 0x00000001,

    /// <summary>
    /// レガシメッセージを生成しない
    /// </summary>
    RIDEV_NOLEGACY = 0x00000030,

    /// <summary>
    /// 非フォアグラウンドでも入力を受け取る
    /// </summary>
    RIDEV_INPUTSINK = 0x00000100,
}

public enum MAPVK : uint
{
    MAPVK_VK_TO_VSC = 0,
    MAPVK_VSC_TO_VK,
    MAPVK_VK_TO_CHAR,
    MAPVK_VSC_TO_VK_EX,
    MAPVK_VK_TO_VSC_EX,
}

public enum RIDI : int
{
    PREPARSEDDATA = 0x20000005,
    DEVICENAME = 0x20000007,
    RIDI_DEVICEINFO = 0x2000000b,
}

public enum RIM_TYPE : int
{
    RIM_TYPEMOUSE = 0,
    RIM_TYPEKEYBOARD = 1,
    RIM_TYPEHID = 2,
}

public enum HidUsagePage : ushort
{
    Undefined = 0x00,
    GenericDesktop = 0x01,
    SimulationControls = 0x02,
    VRControls = 0x03,
    SportControls = 0x04,
    GameControls = 0x05,
    Keyboard = 0x07,
    LED = 0x08,
    Button = 0x09,
    Consumer = 0x0C,
    VendorDefined = 0xFF00
}

public enum HidUsage : ushort
{
    Pointer = 0x01,
    Mouse = 0x02,
    Joystick = 0x04,
    Gamepad = 0x05,
    Keyboard = 0x06,
    Keypad = 0x07,
    SystemControl = 0x80
}

[Flags]
public enum RI_MOUSE : ushort
{
    RI_MOUSE_LEFT_BUTTON_DOWN = 0x0001,
    RI_MOUSE_LEFT_BUTTON_UP = 0x0002,
    RI_MOUSE_RIGHT_BUTTON_DOWN = 0x0004,
    RI_MOUSE_RIGHT_BUTTON_UP = 0x0008,
    RI_MOUSE_MIDDLE_BUTTON_DOWN = 0x0010,
    RI_MOUSE_MIDDLE_BUTTON_UP = 0x0020,
    RI_MOUSE_BUTTON_4_DOWN = 0x0040,
    RI_MOUSE_BUTTON_4_UP = 0x0080,
    RI_MOUSE_BUTTON_5_DOWN = 0x0100,
    RI_MOUSE_BUTTON_5_UP = 0x0200,
    RI_MOUSE_WHEEL = 0x0400,
    RI_MOUSE_HWHEEL = 0x0800,
}

[Flags]
public enum RI_KEY : ushort
{
    RI_KEY_MAKE = 0x0000,
    RI_KEY_BREAK = 0x0001,
    RI_KEY_E0 = 0x0002,
    RI_KEY_E1 = 0x0004,
}

public enum SM_INDEX
{
    SM_CXSCREEN = 0,
    SM_CYSCREEN = 1,
    SM_CXVIRTUALSCREEN = 78,
    SM_CYVIRTUALSCREEN = 79,
}

public enum SPDRP
{
    SPDRP_FRIENDLYNAME = 0x0000000C,
}

/// <summary>
/// 仮想キーコードの定義
/// </summary>
public enum VKey : ushort
{
    VK_LBUTTON = 0x01,
    VK_RBUTTON = 0x02,
    VK_CANCEL = 0x03,
    VK_MBUTTON = 0x04,
    VK_XBUTTON1 = 0x05,
    VK_XBUTTON2 = 0x06,
    VK_BACK = 0x08,
    VK_TAB = 0x09,
    VK_CLEAR = 0x0C,
    VK_RETURN = 0x0D,
    VK_SHIFT = 0x10,
    VK_CONTROL = 0x11,
    VK_MENU = 0x12,
    VK_PAUSE = 0x13,
    VK_CAPITAL = 0x14,
    VK_KANA = 0x15,
    VK_HANGUL = 0x15,
    VK_IME_ON = 0x16,
    VK_JUNJA = 0x17,
    VK_FINAL = 0x18,
    VK_HANJA = 0x19,
    VK_KANJI = 0x19,
    VK_IME_OFF = 0x1A,
    VK_ESCAPE = 0x1B,
    VK_CONVERT = 0x1C,
    VK_NONCONVERT = 0x1D,
    VK_ACCEPT = 0x1E,
    VK_MODECHANGE = 0x1F,
    VK_SPACE = 0x20,
    VK_PRIOR = 0x21,
    VK_NEXT = 0x22,
    VK_END = 0x23,
    VK_HOME = 0x24,
    VK_LEFT = 0x25,
    VK_UP = 0x26,
    VK_RIGHT = 0x27,
    VK_DOWN = 0x28,
    VK_SELECT = 0x29,
    VK_PRINT = 0x2A,
    VK_EXECUTE = 0x2B,
    VK_SNAPSHOT = 0x2C,
    VK_INSERT = 0x2D,
    VK_DELETE = 0x2E,
    VK_HELP = 0x2F,
    VK_0 = 0x30,
    VK_1 = 0x31,
    VK_2 = 0x32,
    VK_3 = 0x33,
    VK_4 = 0x34,
    VK_5 = 0x35,
    VK_6 = 0x36,
    VK_7 = 0x37,
    VK_8 = 0x38,
    VK_9 = 0x39,
    VK_A = 0x41,
    VK_B = 0x42,
    VK_C = 0x43,
    VK_D = 0x44,
    VK_E = 0x45,
    VK_F = 0x46,
    VK_G = 0x47,
    VK_H = 0x48,
    VK_I = 0x49,
    VK_J = 0x4A,
    VK_K = 0x4B,
    VK_L = 0x4C,
    VK_M = 0x4D,
    VK_N = 0x4E,
    VK_O = 0x4F,
    VK_P = 0x50,
    VK_Q = 0x51,
    VK_R = 0x52,
    VK_S = 0x53,
    VK_T = 0x54,
    VK_U = 0x55,
    VK_V = 0x56,
    VK_W = 0x57,
    VK_X = 0x58,
    VK_Y = 0x59,
    VK_Z = 0x5A,
    VK_LWIN = 0x5B,
    VK_RWIN = 0x5C,
    VK_APPS = 0x5D,
    VK_SLEEP = 0x5F,
    VK_NUMPAD0 = 0x60,
    VK_NUMPAD1 = 0x61,
    VK_NUMPAD2 = 0x62,
    VK_NUMPAD3 = 0x63,
    VK_NUMPAD4 = 0x64,
    VK_NUMPAD5 = 0x65,
    VK_NUMPAD6 = 0x66,
    VK_NUMPAD7 = 0x67,
    VK_NUMPAD8 = 0x68,
    VK_NUMPAD9 = 0x69,
    VK_MULTIPLY = 0x6A,
    VK_ADD = 0x6B,
    VK_SEPARATOR = 0x6C,
    VK_SUBTRACT = 0x6D,
    VK_DECIMAL = 0x6E,
    VK_DIVIDE = 0x6F,
    VK_F1 = 0x70,
    VK_F2 = 0x71,
    VK_F3 = 0x72,
    VK_F4 = 0x73,
    VK_F5 = 0x74,
    VK_F6 = 0x75,
    VK_F7 = 0x76,
    VK_F8 = 0x77,
    VK_F9 = 0x78,
    VK_F10 = 0x79,
    VK_F11 = 0x7A,
    VK_F12 = 0x7B,
    VK_F13 = 0x7C,
    VK_F14 = 0x7D,
    VK_F15 = 0x7E,
    VK_F16 = 0x7F,
    VK_F17 = 0x80,
    VK_F18 = 0x81,
    VK_F19 = 0x82,
    VK_F20 = 0x83,
    VK_F21 = 0x84,
    VK_F22 = 0x85,
    VK_F23 = 0x86,
    VK_F24 = 0x87,
    VK_NUMLOCK = 0x90,
    VK_SCROLL = 0x91,
    VK_LSHIFT = 0xA0,
    VK_RSHIFT = 0xA1,
    VK_LCONTROL = 0xA2,
    VK_RCONTROL = 0xA3,
    VK_LMENU = 0xA4,
    VK_RMENU = 0xA5,
    VK_BROWSER_BACK = 0xA6,
    VK_BROWSER_FORWARD = 0xA7,
    VK_BROWSER_REFRESH = 0xA8,
    VK_BROWSER_STOP = 0xA9,
    VK_BROWSER_SEARCH = 0xAA,
    VK_BROWSER_FAVORITES = 0xAB,
    VK_BROWSER_HOME = 0xAC,
    VK_VOLUME_MUTE = 0xAD,
    VK_VOLUME_DOWN = 0xAE,
    VK_VOLUME_UP = 0xAF,
    VK_MEDIA_NEXT_TRACK = 0xB0,
    VK_MEDIA_PREV_TRACK = 0xB1,
    VK_MEDIA_STOP = 0xB2,
    VK_MEDIA_PLAY_PAUSE = 0xB3,
    VK_LAUNCH_MAIL = 0xB4,
    VK_LAUNCH_MEDIA_SELECT = 0xB5,
    VK_LAUNCH_APP1 = 0xB6,
    VK_LAUNCH_APP2 = 0xB7,
    VK_OEM_1 = 0xBA,
    VK_OEM_PLUS = 0xBB,
    VK_OEM_COMMA = 0xBC,
    VK_OEM_MINUS = 0xBD,
    VK_OEM_PERIOD = 0xBE,
    VK_OEM_2 = 0xBF,
    VK_OEM_3 = 0xC0,
    VK_GAMEPAD_A = 0xC3,
    VK_GAMEPAD_B = 0xC4,
    VK_GAMEPAD_X = 0xC5,
    VK_GAMEPAD_Y = 0xC6,
    VK_GAMEPAD_RIGHT_SHOULDER = 0xC7,
    VK_GAMEPAD_LEFT_SHOULDER = 0xC8,
    VK_GAMEPAD_LEFT_TRIGGER = 0xC9,
    VK_GAMEPAD_RIGHT_TRIGGER = 0xCA,
    VK_GAMEPAD_DPAD_UP = 0xCB,
    VK_GAMEPAD_DPAD_DOWN = 0xCC,
    VK_GAMEPAD_DPAD_LEFT = 0xCD,
    VK_GAMEPAD_DPAD_RIGHT = 0xCE,
    VK_GAMEPAD_MENU = 0xCF,
    VK_GAMEPAD_VIEW = 0xD0,
    VK_GAMEPAD_LEFT_THUMBSTICK_BUTTON = 0xD1,
    VK_GAMEPAD_RIGHT_THUMBSTICK_BUTTON = 0xD2,
    VK_GAMEPAD_LEFT_THUMBSTICK_UP = 0xD3,
    VK_GAMEPAD_LEFT_THUMBSTICK_DOWN = 0xD4,
    VK_GAMEPAD_LEFT_THUMBSTICK_RIGHT = 0xD5,
    VK_GAMEPAD_LEFT_THUMBSTICK_LEFT = 0xD6,
    VK_GAMEPAD_RIGHT_THUMBSTICK_UP = 0xD7,
    VK_GAMEPAD_RIGHT_THUMBSTICK_DOWN = 0xD8,
    VK_GAMEPAD_RIGHT_THUMBSTICK_RIGHT = 0xD9,
    VK_GAMEPAD_RIGHT_THUMBSTICK_LEFT = 0xDA,
    VK_OEM_4 = 0xDB,
    VK_OEM_5 = 0xDC,
    VK_OEM_6 = 0xDD,
    VK_OEM_7 = 0xDE,
    VK_OEM_8 = 0xDF,
    VK_OEM_102 = 0xE2,
    VK_PROCESSKEY = 0xE5,
    VK_PACKET = 0xE7,
    VK_ATTN = 0xF6,
    VK_CRSEL = 0xF7,
    VK_EXSEL = 0xF8,
    VK_EREOF = 0xF9,
    VK_PLAY = 0xFA,
    VK_ZOOM = 0xFB,
    VK_NONAME = 0xFC,
    VK_PA1 = 0xFD,
    VK_OEM_CLEAR = 0xFE,
    KEYBOARD_OVERRUN_MAKE_CODE = 0xFF,
}

/// <summary>
/// ウィンドウメッセージ定数
/// </summary>
public enum WindowMessage : uint
{
    WM_NULL = 0x0000,
    WM_CREATE = 0x0001,
    WM_DESTROY = 0x0002,
    WM_MOVE = 0x0003,
    WM_SIZE = 0x0005,
    WM_ACTIVATE = 0x0006,
    WM_SETFOCUS = 0x0007,
    WM_KILLFOCUS = 0x0008,
    WM_ENABLE = 0x000A,
    WM_SETREDRAW = 0x000B,
    WM_SETTEXT = 0x000C,
    WM_GETTEXT = 0x000D,
    WM_GETTEXTLENGTH = 0x000E,
    WM_PAINT = 0x000F,
    WM_CLOSE = 0x0010,
    WM_QUIT = 0x0012,
    WM_ERASEBKGND = 0x0014,
    WM_SHOWWINDOW = 0x0018,
    WM_SETTINGCHANGE = 0x001A,
    WM_ACTIVATEAPP = 0x001C,
    WM_NCPAINT = 0x0085,
    WM_NCACTIVATE = 0x0086,
    WM_INPUT = 0x00FF,
    WM_KEYDOWN = 0x0100,
    WM_KEYUP = 0x0101,
    WM_CHAR = 0x0102,
    WM_SYSKEYDOWN = 0x0104,
    WM_SYSKEYUP = 0x0105,
    WM_SYSCHAR = 0x0106,
    WM_MOUSEMOVE = 0x0200,
    WM_LBUTTONDOWN = 0x0201,
    WM_LBUTTONUP = 0x0202,
    WM_RBUTTONDOWN = 0x0204,
    WM_RBUTTONUP = 0x0205,
    WM_MBUTTONDOWN = 0x0207,
    WM_MBUTTONUP = 0x0208,
    WM_MOUSEWHEEL = 0x020A,
    WM_XBUTTONDOWN = 0x020B,
    WM_XBUTTONUP = 0x020C,
    WM_MOUSEHWHEEL = 0x020E,
    WM_DEVICECHANGE = 0x0219,
}
