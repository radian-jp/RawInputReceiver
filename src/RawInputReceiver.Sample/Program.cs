using RadianTools.Hardware.Input.Windows;
using RadianTools.Interop.Windows;

namespace RawInputReceiver.Sample;

internal class Program
{
    static void Main(string[] args)
    {
        using var receiver = new RadianTools.Hardware.Input.Windows.RawInputReceiver();
        Action<RawMouseEventArgs> mouseReceived = (e) =>
        {
            Console.WriteLine(e.ToString());
        };

        Action<RawKeyboardEventArgs> keyboardReceived = (e) =>
        {
            Console.WriteLine(e.ToString());
        };

        receiver.MouseReceived += mouseReceived;
        receiver.KeyboardReceived += keyboardReceived;

        Console.WriteLine("Push ESC key to exit.");

        while (Console.ReadKey(true).Key != ConsoleKey.Escape)
        {
        }
    }
}
