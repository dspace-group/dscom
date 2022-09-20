using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Threading;
using Microsoft.Win32;

namespace net48;

class Program
{
    [DllImport("ole32.dll")]
    internal static extern int CoResumeClassObjects();

    private const string ProgId = "Greeter.Net48";
    private const string Version = "1.0";

    private const string Title = "Net48 Greeter";

    private const string Description = "FullFramework Greeter App";



    [MTAThread()]
    static void Main(string[] args)
    {
        if (args.Length == 1)
        {
            if (args[0].Equals("/regserver", StringComparison.OrdinalIgnoreCase) || args[0].Equals("-regserver", StringComparison.OrdinalIgnoreCase))
            {
                // Register
                Register();
                return;
            }
            else if (args[0].Equals("/unregserver", StringComparison.OrdinalIgnoreCase) || args[0].Equals("-unregserver", StringComparison.OrdinalIgnoreCase))
            {
                // Unregister
                Unregister();
                return;
            }
        }

        var registration = new RegistrationServices();
        var cookie = registration.RegisterTypeForComClients(typeof(Server.Common.Greeter), RegistrationClassContext.LocalServer, RegistrationConnectionType.MultipleUse | RegistrationConnectionType.Suspended);

        System.Console.WriteLine($"OutProc COM server running. PID:{System.Diagnostics.Process.GetCurrentProcess().Id}");
        System.Console.WriteLine($"RegisterTypeForComClients return cookie {cookie}");
        
       var hr = CoResumeClassObjects();
        if (hr < 0)
        {
            Marshal.ThrowExceptionForHR(hr);
        }

        System.Console.WriteLine($"CoResumeClassObjects returned {hr}");
        System.Console.WriteLine($"Press enter to stop");

        Console.ReadLine();
        registration.UnregisterTypeForComClients(cookie);
    }

    static void Register()
    {
        // Register
        Server.Common.RegistryHelper.RegisterOutProcServer<Server.Common.Greeter>(ProgId, Version, Title, Description);
    }

    private static void Unregister()
    {
        // Unregister
        Server.Common.RegistryHelper.UnregisterOutProcServer<Server.Common.Greeter>(ProgId, Version);
    }
}
