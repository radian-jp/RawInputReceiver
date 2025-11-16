using RadianTools.Interop.Windows;

namespace RadianTools.Hardware.Input.Windows;

/// <summary>
/// マウス操作
/// </summary>
public enum MouseOp
{
    Move = 0,
    ButtonLeft,
    ButtonRight,
    ButtonMiddle,
    Button4,
    Button5,
    VWheel,
    HWheel,
}

/// <summary>
/// RawInputマウス情報
/// </summary>
public ref struct RawMouseEventArgs
{
    public RawMouseEventArgs(ref HidDeviceDetail detail, ref RAWINPUT rawInput)
    {
        Handle = ref rawInput.header.hDevice;
        Detail = ref detail;
        Mouse = ref rawInput.data.mouse;
        WheelDelta = ref Mouse.ButtonData;

        MoveAbsolute = (Mouse.Flags & MOUSE_MOVE.MOUSE_MOVE_ABSOLUTE) != 0;
        MappingVirtualDesktop = (Mouse.Flags & MOUSE_MOVE.MOUSE_VIRTUAL_DESKTOP) != 0;
        if (MoveAbsolute)
        {
            RECT rect;
            if (MappingVirtualDesktop)
            {
                rect.Left = User32.GetSystemMetrics(SM_INDEX.SM_CXVIRTUALSCREEN);
                rect.Top = User32.GetSystemMetrics(SM_INDEX.SM_CYVIRTUALSCREEN);
                rect.Right = User32.GetSystemMetrics(SM_INDEX.SM_CXVIRTUALSCREEN);
                rect.Bottom = User32.GetSystemMetrics(SM_INDEX.SM_CYVIRTUALSCREEN);
            }
            else
            {
                rect.Left = User32.GetSystemMetrics(SM_INDEX.SM_CXVIRTUALSCREEN);
                rect.Top = User32.GetSystemMetrics(SM_INDEX.SM_CYVIRTUALSCREEN);
                rect.Right = User32.GetSystemMetrics(SM_INDEX.SM_CXVIRTUALSCREEN);
                rect.Bottom = User32.GetSystemMetrics(SM_INDEX.SM_CYVIRTUALSCREEN);
            }

            X = User32.MulDiv(X, rect.Right, 65535) + rect.Left;
            Y = User32.MulDiv(Y, rect.Bottom, 65535) + rect.Top;
        }
        else
        {
            X = Mouse.LastX;
            Y = Mouse.LastY;
        }

        if ((Mouse.ButtonFlags & RI_MOUSE.RI_MOUSE_LEFT_BUTTON_DOWN) != 0)
        {
            MouseOp = MouseOp.ButtonLeft;
            PushState = true;
        }
        else if ((Mouse.ButtonFlags & RI_MOUSE.RI_MOUSE_LEFT_BUTTON_UP) != 0)
        {
            MouseOp = MouseOp.ButtonLeft;
        }
        else if ((Mouse.ButtonFlags & RI_MOUSE.RI_MOUSE_RIGHT_BUTTON_DOWN) != 0)
        {
            MouseOp = MouseOp.ButtonRight;
            PushState = true;
        }
        else if ((Mouse.ButtonFlags & RI_MOUSE.RI_MOUSE_RIGHT_BUTTON_UP) != 0)
        {
            MouseOp = MouseOp.ButtonRight;
        }
        else if ((Mouse.ButtonFlags & RI_MOUSE.RI_MOUSE_MIDDLE_BUTTON_DOWN) != 0)
        {
            MouseOp = MouseOp.ButtonRight;
            PushState = true;
        }
        else if ((Mouse.ButtonFlags & RI_MOUSE.RI_MOUSE_MIDDLE_BUTTON_UP) != 0)
        {
            MouseOp = MouseOp.ButtonMiddle;
        }
        else if ((Mouse.ButtonFlags & RI_MOUSE.RI_MOUSE_BUTTON_4_DOWN) != 0)
        {
            MouseOp = MouseOp.Button4;
            PushState = true;
        }
        else if ((Mouse.ButtonFlags & RI_MOUSE.RI_MOUSE_BUTTON_4_UP) != 0)
        {
            MouseOp = MouseOp.Button4;
        }
        else if ((Mouse.ButtonFlags & RI_MOUSE.RI_MOUSE_BUTTON_5_DOWN) != 0)
        {
            MouseOp = MouseOp.Button5;
            PushState = true;
        }
        else if ((Mouse.ButtonFlags & RI_MOUSE.RI_MOUSE_BUTTON_5_UP) != 0)
        {
            MouseOp = MouseOp.Button5;
        }
        else if ((Mouse.ButtonFlags & RI_MOUSE.RI_MOUSE_WHEEL) != 0)
        {
            MouseOp = MouseOp.VWheel;
        }
        else if ((Mouse.ButtonFlags & RI_MOUSE.RI_MOUSE_HWHEEL) != 0)
        {
            MouseOp = MouseOp.HWheel;
        }
    }

    /// <summary>
    /// HIDデバイス詳細
    /// </summary>
    public readonly ref HidDeviceDetail Detail;

    /// <summary>
    /// マウス情報情報
    /// </summary>
    public readonly ref RAWMOUSE Mouse;

    /// <summary>
    /// デバイスハンドル
    /// </summary>
    public readonly ref HDEVICE Handle;

    /// <summary>
    /// X移動量（MoveAbsolute=trueの場合、スクリーンX座標）
    /// </summary>
    public readonly int X;

    /// <summary>
    /// Y移動量（MoveAbsolute=trueの場合、スクリーンY座標）
    /// </summary>
    public readonly int Y;

    /// <summary>
    /// ホイール移動量
    /// </summary>
    public readonly ref short WheelDelta;

    /// <summary>
    /// マウス操作
    /// </summary>
    public readonly MouseOp MouseOp;

    /// <summary>
    /// On/Off状態
    /// </summary>
    public readonly bool PushState;

    /// <summary>
    /// LastX, LastY が絶対座標かどうか
    /// </summary>
    public readonly bool MoveAbsolute;

    /// <summary>
    /// マウス座標が仮想デスクトップにマッピングされているかどうか
    /// </summary>
    public readonly bool MappingVirtualDesktop;

    /// <inheritdoc />
    public override string ToString() =>
        $"{nameof(Handle)} = 0x{Handle.Value:X16}"
        + $", {nameof(MouseOp)} = {MouseOp}"
        + $", {nameof(PushState)} = {PushState}"
        + $", {nameof(MoveAbsolute)} = {MoveAbsolute}"
        + $", {nameof(X)} = {X}"
        + $", {nameof(Y)} = {Y}"
        + $", {nameof(WheelDelta)} = {WheelDelta}"
        + $", {nameof(MappingVirtualDesktop)} = {MappingVirtualDesktop}"
        + $", {nameof(Detail.ProductName)} = {Detail.ProductName}"
        + $", {nameof(Detail.Manufacturer)} = {Detail.Manufacturer}"
        ;
}
