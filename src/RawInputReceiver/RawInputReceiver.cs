namespace RadianTools.Hardware.Input.Windows;

/// <summary>
/// RawInput受信クラス
/// </summary>
public class RawInputReceiver : IDisposable
{
    private bool _disposed;

    /// <summary>
    /// マウス情報受信時イベント
    /// </summary>
    internal Action<RawMouseEventArgs>? _mouseReceived;
    public event Action<RawMouseEventArgs> MouseReceived
    {
        add
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(RawInputReceiver));

            Action<RawMouseEventArgs>? original, updated;
            do
            {
                original = _mouseReceived;
                updated = (Action<RawMouseEventArgs>?)Delegate.Combine(original, value);
            }
            while (Interlocked.CompareExchange(ref _mouseReceived, updated, original) != original);

            if (original == null)
                _receiverWindow.RegisterMouse();
        }
        remove
        {
            Action<RawMouseEventArgs>? original, updated;
            do
            {
                original = _mouseReceived;
                updated = (Action<RawMouseEventArgs>?)Delegate.Remove(original, value);
            }
            while (Interlocked.CompareExchange(ref _mouseReceived, updated, original) != original);

            if (updated == null)
                _receiverWindow.UnregisterMouse();
        }
    }

    /// <summary>
    /// キーボード情報受信時イベント
    /// </summary>
    internal Action<RawKeyboardEventArgs>? _keyboardReceived;
    public event Action<RawKeyboardEventArgs> KeyboardReceived
    {
        add
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(RawInputReceiver));

            Action<RawKeyboardEventArgs>? original, updated;
            do
            {
                original = _keyboardReceived;
                updated = (Action<RawKeyboardEventArgs>?)Delegate.Combine(original, value);
            }
            while (Interlocked.CompareExchange(ref _keyboardReceived, updated, original) != original);

            if (original == null)
                _receiverWindow.RegisterKeyboard();
        }
        remove
        {
            Action<RawKeyboardEventArgs>? original, updated;
            do
            {
                original = _keyboardReceived;
                updated = (Action<RawKeyboardEventArgs>?)Delegate.Remove(original, value);
            }
            while (Interlocked.CompareExchange(ref _keyboardReceived, updated, original) != original);

            if (updated == null)
                _receiverWindow.UnregisterKeyboard();
        }
    }

    /// <summary>
    /// RawInput情報受信ウィンドウ
    /// </summary>
    private RawInputReceiverWindow _receiverWindow;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public RawInputReceiver()
    {
        // RawInput受信ウィンドウを作成し、メッセージループを起動
        _receiverWindow = new RawInputReceiverWindow(this);
        _receiverWindow.Run();
    }

    /// <summary>
    /// 解放処理
    /// </summary>
    public void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;
        _mouseReceived = null;
        _keyboardReceived = null;
        _receiverWindow.UnregisterMouse();
        _receiverWindow.UnregisterKeyboard();
        _receiverWindow.Dispose();
    }
}