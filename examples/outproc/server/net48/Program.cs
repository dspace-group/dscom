using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
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

    static void Register()
    {
        Server.Common.RegistryHelper.RegisterOutProcServer<Server.Common.Greeter>(ProgId, Version, Title, Description);
    }

    //-Embedding

    [MTAThread()]
    //[STAThread()]
    static void Main(string[] args)
    {
        System.Windows.Forms.MessageBox.Show(args.Length > 0 ? args[0] : "");

        bool useConsole = true;
        if (args.Length > 0)
        {
            switch (args[0].ToLower())
            {
                case "/regserver":
                case "-regserver":
                    Register();
                    return;
                case "/unregserver":
                case "-unregserver":
                    Unregister();
                    return;
                case "/embedding":
                case "-embedding":
                    useConsole = false;
                    break;
                default:
                    break;
            }
        }

        var registration = new RegistrationServices();
        var cookie = registration.RegisterTypeForComClients(typeof(Server.Common.Greeter), RegistrationClassContext.LocalServer, RegistrationConnectionType.MultipleUse | RegistrationConnectionType.Suspended);

        if (useConsole)
        {
            System.Console.WriteLine(System.Diagnostics.Process.GetCurrentProcess().Id);
            System.Console.WriteLine($"RegisterTypeForComClients return cookie {cookie}");
        }
        int hResult = CoResumeClassObjects();

        if (useConsole)
        {
            System.Console.WriteLine($"CoResumeClassObjects returned {hResult}");
            System.Console.WriteLine($"Press CTRL+C to stop");
        }

        //System.Console.ReadLine();

        Application.Run(new Form());
        //System.Environment.Exit(0);

        // Dispatcher.Run();
        // //new System.Threading.AutoResetEvent(false).WaitOne();
        // registration.UnregisterTypeForComClients(cookie);
    }

    private static void Unregister()
    {
        throw new NotImplementedException();
    }
}
