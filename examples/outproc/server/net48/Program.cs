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

    private const string Version = "1.0";
    private static void CreateSubKey(RegistryKey parentKey, string name, string value)
    {
        using (RegistryKey subKey = parentKey.CreateSubKey(name, true))
        {
            if (value != null)
            {
                subKey.SetValue(null, value);
            }
        }
    }
    static void Register()
    {
        string appTitle = $"Greeter title";

        string appDescription = $"Greeter Application";

        string versionIndependentProgId = $"Greeter.Application";
        string progId = $"{versionIndependentProgId}.{Version}";

        string clsId = "{a9bd4abf-1518-4f3c-b017-6bc45f983ff0}".ToUpper();

        using (RegistryKey progIdKey = Registry.ClassesRoot.CreateSubKey(progId, true))
        {
            progIdKey.SetValue(null, appDescription);
            CreateSubKey(progIdKey, "CLSID", clsId);
        }

        using (
            RegistryKey versionIndependentProgIdKey = Registry.ClassesRoot.CreateSubKey(
                versionIndependentProgId,
                true))
        {
            versionIndependentProgIdKey.SetValue(null, appDescription);
            CreateSubKey(versionIndependentProgIdKey, "CLSID", clsId);
            CreateSubKey(versionIndependentProgIdKey, "CurVer", progId);
        }

        using (RegistryKey clsIdRootKey = Registry.ClassesRoot.OpenSubKey("CLSID", true))
        {
            using (RegistryKey clsIdKey = clsIdRootKey.CreateSubKey(clsId, true))
            {
                clsIdKey.SetValue(null, appDescription);
                CreateSubKey(clsIdKey, "ProgID", progId);
                CreateSubKey(clsIdKey, "VersionIndependentProgID", versionIndependentProgId);
                CreateSubKey(clsIdKey, "Programmable", string.Empty);
                CreateSubKey(clsIdKey, "LocalServer32", System.Reflection.Assembly.GetExecutingAssembly().Location);
                clsIdKey.SetValue("AppID", clsId);
            }
        }

        using (RegistryKey appIdRootKey = Registry.ClassesRoot.OpenSubKey("AppID", true))
        {
            CreateSubKey(appIdRootKey, clsId, appTitle);

            var fileNameOfExecutingAssembly = Path.GetFileName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            using (RegistryKey exeKey = appIdRootKey.CreateSubKey(fileNameOfExecutingAssembly, true))
            {
                exeKey.SetValue("AppID", clsId);
            }
        }
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
        var cookie = registration.RegisterTypeForComClients(typeof(Greeter), RegistrationClassContext.LocalServer, RegistrationConnectionType.MultipleUse | RegistrationConnectionType.Suspended);

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
