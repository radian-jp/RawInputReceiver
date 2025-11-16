using RadianTools.Interop.Windows;

namespace RadianTools.Hardware.Input.Windows;

/// <summary>
/// RawInputkキーボード情報
/// </summary>
public ref struct RawKeyboardEventArgs
{
    public RawKeyboardEventArgs(ref HidDeviceDetail detail, ref RAWINPUT rawInput)
    {
        Handle = ref rawInput.header.hDevice;
        Detail = ref detail;
        Keyboard = ref rawInput.data.keyboard;
        VKey = ref rawInput.data.keyboard.VKey;

        PushState = (Keyboard.Flags & RI_KEY.RI_KEY_BREAK) == 0;
    }

    /// <summary>
    /// HIDデバイス詳細
    /// </summary>
    public readonly ref HidDeviceDetail Detail;

    /// <summary>
    /// キーボード状態情報
    /// </summary>
    public readonly ref RAWKEYBOARD Keyboard;

    /// <summary>
    /// デバイスハンドル
    /// </summary>
    public readonly ref HDEVICE Handle;

    /// <summary>
    /// 仮想キーコード
    /// </summary>
    public readonly ref VKey VKey;

    /// <summary>
    /// On/Off状態
    /// </summary>
    public readonly bool PushState;

    /// <inheritdoc />
    public override string ToString() =>
        $"{nameof(Handle)} = 0x{Handle.Value:X16}"
        + $", {nameof(VKey)} = {VKey}"
        + $", {nameof(PushState)} = {PushState}"
        + $", {nameof(Detail.ProductName)} = {Detail.ProductName}"
        + $", {nameof(Detail.Manufacturer)} = {Detail.Manufacturer}"
        ;
}
