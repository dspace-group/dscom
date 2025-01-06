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

    private static int Main(string[] args)
    {
        Console.WriteLine("Command-line arguments:");
        foreach (var arg in args)
        {
            Console.WriteLine(arg);
        }

        Console.WriteLine("OutProc COM server starting...");


        if (args.Length == 1)
        {
            if (args[0].Equals("/regserver", StringComparison.OrdinalIgnoreCase) || args[0].Equals("-regserver", StringComparison.OrdinalIgnoreCase))
            {
                // Register
                Register();
                return 0;
            }
            else if (args[0].Equals("/unregserver", StringComparison.OrdinalIgnoreCase) || args[0].Equals("-unregserver", StringComparison.OrdinalIgnoreCase))
            {
                // Unregister
                Unregister();
                return 0;
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

        return 0;
    }

    private static void Register()
    {
        OutProcServer.Register<Greeter>(ProgId, Version, Title, Description);
    }

    private static void Unregister()
    {
        OutProcServer.Unregister<Greeter>(ProgId, Version);
    }
}
