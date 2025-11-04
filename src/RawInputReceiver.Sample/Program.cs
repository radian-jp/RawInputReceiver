using RadianTools.Hardware.Input.Windows;

internal class Program
{
    static void Main(string[] args)
    {
        using var receiver = new RawInputReceiver();

        receiver.MouseReceived += (e) => Console.WriteLine(e.ToString());
        receiver.KeyboardReceived += (e) => Console.WriteLine(e.ToString());

        Console.WriteLine("Push ESC key to exit.");

        while (Console.ReadKey(true).Key != ConsoleKey.Escape)
        {
        }
    }
}
