using System.Runtime.InteropServices;

namespace Server.Common;

[ComVisible(true)]
[ComDefaultInterface(typeof(IGreeter))]
[ClassInterface(ClassInterfaceType.None)]
[Guid("A9BD4ABF-1518-4F3C-B017-6BC45F983FF1")]
public class Greeter : IGreeter
{
    private readonly Guid _instanceGuid;

    public string GetInstanceId()
    {
        return _instanceGuid.ToString();
    }

    public Greeter()
    {
        _instanceGuid = Guid.NewGuid();
        Console.WriteLine($"Greeter object created. PID: {System.Diagnostics.Process.GetCurrentProcess().Id} ID:{_instanceGuid}");
    }

    public string SayHello(string name)
    {
        var text = $"Hello {name}. This is the {Environment.Version} OutProc server! PID: {System.Diagnostics.Process.GetCurrentProcess().Id}";
        Console.WriteLine(text);
        return text;
    }
}
