using System.Runtime.InteropServices;

namespace Server.Common;

[ComVisible(true)]
[ComDefaultInterface(typeof(IGreeter))]
[ClassInterface(ClassInterfaceType.None)]
[Guid("A9BD4ABF-1518-4F3C-B017-6BC45F983FF0")]
public class Greeter : IGreeter
{
    public Greeter()
    {

    }

    public string SayHello(string name)
    {
        var text = $"Hello {name}. This is the Net48 OutProc server! PID: {System.Diagnostics.Process.GetCurrentProcess().Id}";
        Console.WriteLine(text);
        return text;
    }
}
