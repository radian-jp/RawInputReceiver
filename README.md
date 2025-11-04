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

        receiver.MouseReceived += (e) => Console.WriteLine(e.ToString());
        receiver.KeyboardReceived += (e) => Console.WriteLine(e.ToString());

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
Handle = 0x0000000000010044, MouseOp = Move, PushState = False, MoveAbsolute = False, X = -2, Y = 1, WheelDelta = 0, MappingVirtualDesktop = False, ProductName = 2.4G INPUT DEVICE, Manufacturer = MOSART Semi.
Handle = 0x0000000000010044, MouseOp = Move, PushState = False, MoveAbsolute = False, X = -1, Y = 1, WheelDelta = 0, MappingVirtualDesktop = False, ProductName = 2.4G INPUT DEVICE, Manufacturer = MOSART Semi.
Handle = 0x0000000000010044, MouseOp = Move, PushState = False, MoveAbsolute = False, X = 0, Y = 0, WheelDelta = 0, MappingVirtualDesktop = False, ProductName = 2.4G INPUT DEVICE, Manufacturer = MOSART Semi.
Handle = 0x0000000000010044, MouseOp = Move, PushState = False, MoveAbsolute = False, X = -1, Y = 1, WheelDelta = 0, MappingVirtualDesktop = False, ProductName = 2.4G INPUT DEVICE, Manufacturer = MOSART Semi.
Handle = 0x0000000000010044, MouseOp = Move, PushState = False, MoveAbsolute = False, X = 0, Y = 0, WheelDelta = 0, MappingVirtualDesktop = False, ProductName = 2.4G INPUT DEVICE, Manufacturer = MOSART Semi.
Handle = 0x00000000002223D9, VKey = VK_F, PushState = True, ProductName = REALFORCE 108JP, Manufacturer = Topre
Handle = 0x00000000002223D9, VKey = VK_F, PushState = False, ProductName = REALFORCE 108JP, Manufacturer = Topre
Handle = 0x00000000002223D9, VKey = VK_H, PushState = True, ProductName = REALFORCE 108JP, Manufacturer = Topre
Handle = 0x00000000002223D9, VKey = VK_H, PushState = False, ProductName = REALFORCE 108JP, Manufacturer = Topre
Handle = 0x0000000000010044, MouseOp = VWheel, PushState = False, MoveAbsolute = False, X = 0, Y = 0, WheelDelta = -120, MappingVirtualDesktop = False, ProductName = 2.4G INPUT DEVICE, Manufacturer = MOSART Semi.
Handle = 0x0000000000010044, MouseOp = VWheel, PushState = False, MoveAbsolute = False, X = 0, Y = 0, WheelDelta = 120, MappingVirtualDesktop = False, ProductName = 2.4G INPUT DEVICE, Manufacturer = MOSART Semi.
Handle = 0x00000000002223D9, VKey = VK_ESCAPE, PushState = True, ProductName = REALFORCE 108JP, Manufacturer = Topre
```

## Requirements

- Windows 10 or later  
- .NET 9+

## License

This project is licensed under the [BSD Zero Clause License](https://opensource.org/licenses/0BSD).
