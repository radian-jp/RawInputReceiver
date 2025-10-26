using RadianTools.Hardware.Input.Windows;
using RadianTools.Interop.Windows;

namespace RawInputReceiver.Sample;

internal class Program
{
    static void Main(string[] args)
    {
        var mouseReceiver = new RadianTools.Hardware.Input.Windows.RawInputReceiver(DeviceType.Mouse);
        var keyboardReceiver = new RadianTools.Hardware.Input.Windows.RawInputReceiver(DeviceType.Keyboard);

        mouseReceiver.MouseReceived += (e) =>
        {
            Console.WriteLine(e.ToString());
        };

        keyboardReceiver.KeyboardReceived += (e) =>
        {
            Console.WriteLine(e.ToString());
        };

        Console.WriteLine("Push 'q' key to exit.");
        while (Console.ReadKey(true).Key != ConsoleKey.Q)
        {
        }
    }
}
