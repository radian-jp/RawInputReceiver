using System.Collections.Concurrent;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace RadianTools.Interop.Windows.Utility;

/// <summary>
/// 不可視ウィンドウクラス(専用スレッド動作)
/// 派生クラスで WndProc をオーバーライドすることで、メッセージ処理を拡張可能。
/// </summary>
public class HiddenWindow : IDisposable
{
    private Thread? _thread;
    private const int DisposeWaitMsec = 5000;

    private static Lazy<string> _windowClassName = new Lazy<string>(() => "HiddenWindowClass_" + Guid.NewGuid().ToString("N"));
    private static ushort? _atomWindow;
    private static IntPtr _hInstance = Kernel32.GetModuleHandle(IntPtr.Zero);
    private static DelegateWndProc _wndProcDelegate = WndProcGateway;

    /// <summary>
    /// Dispose済みかどうか
    /// </summary>
    public bool IsDisposed => !_gcHandle.IsAllocated;
    private GCHandle _gcHandle;

    /// <summary>
    /// ウィンドウハンドル
    /// </summary>
    public HWND Handle => _hwnd;
    private HWND _hwnd;

    /// <summary>
    /// ウィンドウスレッドで最後に発生した例外
    /// </summary>
    public Exception? LastException { get; private set; }

    private readonly ConcurrentQueue<(Action action, TaskCompletionSource<object?> tcs)> _invokeQueue = new();
    private readonly uint _msgInvokeAsync = User32.RegisterWindowMessage("HiddenWindow.InvokeAsync");

    private object _lockObj = new object();

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public HiddenWindow()
    {
        _gcHandle = GCHandle.Alloc(this);
    }

    /// <summary>
    /// ウィンドウのメッセージループを起動する。
    /// </summary>
    /// <exception cref="ObjectDisposedException">インスタンスが解放済み</exception>
    public void Run()
    {
        lock (_lockObj)
        {
            if (IsDisposed)
                throw new ObjectDisposedException(nameof(HiddenWindow));

            if (_thread != null)
                return;

            using var eventInitialized = new CountdownEvent(1);

            _thread = new Thread(() => {
                try
                {
                    try
                    {
                        // ウィンドウクラスは一度だけ登録し、
                        // アプリケーションの終了と同時に OS により自動解放される。
                        // UnregisterClass は不要かつリスクがあるため、呼び出していない。
                        RegisterWindowClass();

                        // ウィンドウ作成（不可視）
                        _hwnd = User32.CreateWindowEx(
                            0,
                            _windowClassName.Value,
                            string.Empty,
                            0,
                            0, 0, 0, 0,
                            IntPtr.Zero,
                            IntPtr.Zero,
                            _hInstance,
                            GCHandle.ToIntPtr(_gcHandle));

                        if (_hwnd.Value == IntPtr.Zero)
                            throw new Win32Exception(Marshal.GetLastWin32Error());
                    }
                    finally
                    {
                        //ウィンドウ作成完了(または失敗)
                        eventInitialized.Signal();
                    }

                    // メッセージループ
                    MSG msg;
                    while (true)
                    {
                        int ret = User32.GetMessage(out msg, IntPtr.Zero, 0, 0);
                        if (ret > 0)
                        {
                            // 通常メッセージ
                            User32.TranslateMessage(ref msg);
                            User32.DispatchMessage(ref msg);
                        }
                        else if (ret == 0)
                        {
                            // WM_QUIT → 正常終了
                            break;
                        }
                        else // ret == -1
                        {
                            // エラー発生
                            int err = Marshal.GetLastWin32Error();
                            Debug.WriteLine($"GetMessage failed. Error={err}");
                            break;
                        }
                    }

                    // 終了処理
                    User32.DestroyWindow(_hwnd);
                }
                catch (Exception ex)
                {
                    LastException = ex;
                }
            });
            _thread.IsBackground = true;
            _thread.SetApartmentState(ApartmentState.STA);
            _thread.Start();

            // HWND ができるまで待つ
            eventInitialized.Wait();
        }
    }

    /// <summary>
    /// ウィンドウプロシージャを処理しているスレッドでActionを実行します。
    /// </summary>
    /// <param name="action">実行するAction</param>
    /// <returns></returns>
    /// <exception cref="ObjectDisposedException">Actionを実行しているTask</exception>
    public Task InvokeAsync(Action action)
    {
        if (IsDisposed)
            throw new ObjectDisposedException(nameof(HiddenWindow));

        var tcs = new TaskCompletionSource<object?>();
        _invokeQueue.Enqueue((action, tcs));
        User32.PostMessage(Handle, _msgInvokeAsync, IntPtr.Zero, IntPtr.Zero);
        return tcs.Task;
    }

    /// <summary>
    /// ウィンドウクラス登録
    /// </summary>
    /// <exception cref="Win32Exception">WinAPI失敗時に発生</exception>
    private static void RegisterWindowClass()
    {
        if (_atomWindow.HasValue)
            return;

        WNDCLASSEX wc = new WNDCLASSEX();
        wc.cbSize = (uint)Marshal.SizeOf(typeof(WNDCLASSEX));
        wc.lpfnWndProc = _wndProcDelegate;
        wc.hInstance = _hInstance;
        wc.lpszClassName = _windowClassName.Value;
        ushort atom = User32.RegisterClassEx(ref wc);
        if (atom == 0)
        {
            throw new Win32Exception(Marshal.GetLastWin32Error());
        }

        _atomWindow = atom;
    }

    /// <summary>
    /// 内部静的ウィンドウプロシージャからインスタンスのウィンドウプロシージャへ
    /// ウィンドウメッセージの中継を行う。
    /// </summary>
    /// <param name="hwnd">ウィンドウハンドル</param>
    /// <param name="msg">ウィンドウメッセージ</param>
    /// <param name="wParam">メッセージ追加情報1</param>
    /// <param name="lParam">メッセージ追加情報2</param>
    /// <returns>メッセージ処理結果</returns>
    private static IntPtr WndProcGateway(HWND hwnd, WindowMessage msg, IntPtr wParam, IntPtr lParam)
    {
        switch (msg)
        {
            case WindowMessage.WM_CREATE:
                unsafe
                {
                    var createStruct = (CREATESTRUCT*)lParam;
                    var gcHandle = GCHandle.FromIntPtr(createStruct->lpCreateParams);
                    var instance = (HiddenWindow)gcHandle.Target!;
                    User32.SetWindowLongPtr(hwnd, GWLP.GWLP_USERDATA, createStruct->lpCreateParams);
                    return instance.WndProc(hwnd, msg, wParam, lParam);
                }

            default:
                {
                    var userData = User32.GetWindowLongPtr(hwnd, GWLP.GWLP_USERDATA);
                    if (userData == IntPtr.Zero)
                        break;

                    var gcHandle = GCHandle.FromIntPtr(userData);
                    var instance = (HiddenWindow)gcHandle.Target!;
                    if( instance!=null )
                    {
                        return instance.WndProc(hwnd, msg, wParam, lParam);
                    }
                }
                break;
        }

        return User32.DefWindowProc(hwnd, msg, wParam, lParam);
    }

    /// <summary>
    /// 継承可能なウィンドウプロシージャ。
    /// </summary>
    /// <param name="hwnd">ウィンドウハンドル</param>
    /// <param name="msg">ウィンドウメッセージ</param>
    /// <param name="wParam">メッセージ追加情報1</param>
    /// <param name="lParam">メッセージ追加情報2</param>
    /// <returns>メッセージ処理結果。
    /// 継承先では、基本的には base.WndProc(hwnd, msg, wParam, lParam) の戻り値を返すこと。</returns>
    protected virtual IntPtr WndProc(HWND hwnd, WindowMessage msg, IntPtr wParam, IntPtr lParam)
    {
        if ((uint)msg == _msgInvokeAsync)
        {
            while (_invokeQueue.TryDequeue(out var item))
            {
                try
                {
                    item.action();
                    item.tcs.SetResult(null);
                }
                catch (Exception ex)
                {
                    item.tcs.SetException(ex);
                }
            }
            return User32.DefWindowProc(hwnd, msg, wParam, lParam);
        }

        switch (msg)
        {
            case WindowMessage.WM_DESTROY:
                User32.PostQuitMessage(0);
                break;
        }

        return User32.DefWindowProc(hwnd, msg, wParam, lParam);
    }

    /// <summary>
    /// 解放処理
    /// </summary>
    public void Dispose()
    {
        lock (_lockObj)
        {
            if (_gcHandle.IsAllocated)
                _gcHandle.Free();

            if (_thread != null && _thread.IsAlive)
            {
                User32.PostMessage(_hwnd, (uint)WindowMessage.WM_QUIT, IntPtr.Zero, IntPtr.Zero);
                if (!_thread.Join(DisposeWaitMsec))
                {
                    Debug.WriteLine($"After Dispose, the thread did not stop within {DisposeWaitMsec} msec.");
                }
            }
            _thread = null;
        }
    }
}
