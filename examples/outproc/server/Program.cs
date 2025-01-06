using System.Runtime.InteropServices;
using dSPACE.Runtime.InteropServices;
using dSPACE.Runtime.InteropServices.ComTypes;
using Server.Common;
internal sealed class Program
{
    [DllImport("ole32.dll")]
    public static extern int CoResumeClassObjects();

    private const string ProgId = "Greeter.Application";
    private const string Version = "1.0";
    private const string Title = "Greeter Application";
    private const string Description = "Greeter Demo Application";

    private static void Main(string[] args)
    {
        Console.WriteLine("OutProc COM server starting...");

        if (args.Length == 1)
        {
            if (args[0].Equals("/regserver", StringComparison.OrdinalIgnoreCase) || args[0].Equals("-regserver", StringComparison.OrdinalIgnoreCase))
            {
                OutProcServer.Register<Greeter>(ProgId, Version, Title, Description);
            }
            else if (args[0].Equals("/unregserver", StringComparison.OrdinalIgnoreCase) || args[0].Equals("-unregserver", StringComparison.OrdinalIgnoreCase))
            {
                OutProcServer.Unregister<Greeter>(ProgId, Version);
            }
        }

        var registration = new RegistrationServices();
        var cookie = registration.RegisterTypeForComClients(typeof(Greeter), RegistrationClassContext.LocalServer, RegistrationConnectionType.MultipleUse | RegistrationConnectionType.Suspended);

        Console.WriteLine($"OutProc COM server running. PID:{Environment.ProcessId}");
        Console.WriteLine($"RegisterTypeForComClients return cookie {cookie}");

        var hr = CoResumeClassObjects();

        if (hr < 0)
        {
            Marshal.ThrowExceptionForHR(hr);
        }

        Console.WriteLine($"CoResumeClassObjects returned {hr}");
        Console.WriteLine($"Press enter to stop");

        Console.ReadLine();
        registration.UnregisterTypeForComClients(cookie);
    }
}
