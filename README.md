# RawInputReceiver

**RawInputReceiver** is a C# class that wraps the Windows Raw Input API to receive input events from devices like keyboards and mice. It’s especially useful when you need to handle input from multiple devices individually.

## Features

- Wraps Windows Raw Input API  
- Identifies multiple input devices  
- Supports keyboard and mouse input events

## Usage

Here’s a sample based on [`Program.cs`](https://github.com/radian-jp/RawInputReceiver/blob/main/src/RawInputReceiver.Sample/Program.cs):

```csharp
using RadianTools.Hardware.Input.Windows;

internal class Program
{
    static void Main(string[] args)
    {
        using var receiver = new RawInputReceiver();
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
```

## Example Output

```
Push ESC key to exit.
Handle = 0x0000000000010044, MouseOp = Move, PushState = False , MoveAbsolute = False, X = 0, Y = 2, WheelDelta = 0, MappingVirtualDesktop = False , ProductName = 2.4G INPUT DEVICE, Manufacturer = MOSART Semi.
Handle = 0x0000000000010044, MouseOp = Move, PushState = False , MoveAbsolute = False, X = 2, Y = 5, WheelDelta = 0, MappingVirtualDesktop = False , ProductName = 2.4G INPUT DEVICE, Manufacturer = MOSART Semi.
Handle = 0x0000000000010044, MouseOp = Move, PushState = False , MoveAbsolute = False, X = 1, Y = 2, WheelDelta = 0, MappingVirtualDesktop = False , ProductName = 2.4G INPUT DEVICE, Manufacturer = MOSART Semi.
Handle = 0x0000000000010044, MouseOp = Move, PushState = False , MoveAbsolute = False, X = 0, Y = 0, WheelDelta = 0, MappingVirtualDesktop = False , ProductName = 2.4G INPUT DEVICE, Manufacturer = MOSART Semi.
Handle = 0x0000000000010044, MouseOp = ButtonLeft, PushState = True , MoveAbsolute = False, X = 0, Y = 0, WheelDelta = 0, MappingVirtualDesktop = False , ProductName = 2.4G INPUT DEVICE, Manufacturer = MOSART Semi.
Handle = 0x0000000000010044, MouseOp = ButtonLeft, PushState = False , MoveAbsolute = False, X = 0, Y = 0, WheelDelta = 0, MappingVirtualDesktop = False , ProductName = 2.4G INPUT DEVICE, Manufacturer = MOSART Semi.
Handle = 0x00000000002223D9, VKey = VK_G, PushState = True, ProductName = REALFORCE 108JP, Manufacturer = Topre
Handle = 0x00000000002223D9, VKey = VK_G, PushState = False, ProductName = REALFORCE 108JP, Manufacturer = Topre
Handle = 0x00000000002223D9, VKey = VK_S, PushState = True, ProductName = REALFORCE 108JP, Manufacturer = Topre
Handle = 0x00000000002223D9, VKey = VK_S, PushState = False, ProductName = REALFORCE 108JP, Manufacturer = Topre
Handle = 0x00000000002223D9, VKey = VK_ESCAPE, PushState = True, ProductName = REALFORCE 108JP, Manufacturer = Topre
```

## Requirements

- Windows 10 or later  
- .NET 9+

## License

This project is licensed under the [BSD Zero Clause License](https://opensource.org/licenses/0BSD).
