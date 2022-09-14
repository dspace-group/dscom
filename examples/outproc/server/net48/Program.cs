using System;
using System.Runtime.InteropServices;
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
            // Dictionary<string, string> args = new Dictionary<string, string>();
            // foreach (Tuple<string, string> registrationArgument in registrationArguments)
            // {
            //     args[registrationArgument.Item1] = registrationArgument.Item2;
            // }

            // string appName = args["APPNAME"];
            // string appId = args["APPID"];
            // string appTitle = $"Greeter title";

            // string appDescription = $"Greeter Application";

            // string versionIndependentProgId = $"Greeter.Application";
            // string progId = $"{versionIndependentProgId}.{Version}";

            // string clsId = "{a9bd4abf-1518-4f3c-b017-6bc45f983ff0}".ToUpper();

            // using (RegistryKey progIdKey = Registry.ClassesRoot.CreateSubKey(progId, true))
            // {
            //     progIdKey.SetValue(null, appDescription);
            //     CreateSubKey(progIdKey, "CLSID", clsId);
            // }

            // using (
            //     RegistryKey versionIndependentProgIdKey = Registry.ClassesRoot.CreateSubKey(
            //         versionIndependentProgId,
            //         true))
            // {
            //     versionIndependentProgIdKey.SetValue(null, appDescription);
            //     CreateSubKey(versionIndependentProgIdKey, "CLSID", clsId);
            //     CreateSubKey(versionIndependentProgIdKey, "CurVer", progId);
            // }

            // using (RegistryKey clsIdRootKey = Registry.ClassesRoot.OpenSubKey("CLSID", true))
            // {
            //     using (RegistryKey clsIdKey = clsIdRootKey.CreateSubKey(clsId, true))
            //     {
            //         clsIdKey.SetValue(null, appDescription);
            //         CreateSubKey(clsIdKey, "ProgID", progId);
            //         CreateSubKey(clsIdKey, "VersionIndependentProgID", versionIndependentProgId);
            //         CreateSubKey(clsIdKey, "Programmable", null);
            //         CreateSubKey(clsIdKey, "LocalServer32", System.Reflection.Assembly.GetExecutingAssembly().Location);
            //         clsIdKey.SetValue("AppID", clsId);
            //     }
            // }

            // using (RegistryKey appIdRootKey = Registry.ClassesRoot.OpenSubKey("AppID", true))
            // {
            //     CreateSubKey(appIdRootKey, clsId, appTitle);
            //     using (RegistryKey exeKey = appIdRootKey.CreateSubKey(System.Reflection.Assembly.GetExecutingAssembly().Location, true))
            //     {
            //         exeKey.SetValue("AppID", clsId);
            //     }
            // }
            string serverKey = string.Format("SOFTWARE\\Classes\\CLSID\\{0:B}\\LocalServer32", new Guid("a9bd4abf-1518-4f3c-b017-6bc45f983ff0"));
            using RegistryKey regKey = Registry.LocalMachine.CreateSubKey(serverKey);
            regKey.SetValue(null, System.Reflection.Assembly.GetExecutingAssembly().Location);
    }

    [MTAThread()]
    static void Main(string[] args)
    {
        System.Console.WriteLine($"Args count:{args.Length}");
        foreach (var item in args)
        {
            System.Console.WriteLine($"Arg :{item}");
        }
        {
            if (args[0].Equals("/RegServer",StringComparison.InvariantCultureIgnoreCase))
            {
                Register();
                return;
            }
            else if (args[0].Equals("/UnRegServer",StringComparison.InvariantCultureIgnoreCase))
            {
                return;
            }
            
            var registration = new RegistrationServices();
            var cookie = registration.RegisterTypeForComClients(typeof(Greeter), RegistrationClassContext.LocalServer,RegistrationConnectionType.MultipleUse | RegistrationConnectionType.Suspended);
            System.Console.WriteLine($"RegisterTypeForComClients return cookie {cookie}");
            int hResult = CoResumeClassObjects();
            System.Console.WriteLine($"CoResumeClassObjects returned {hResult}");
            System.Console.ReadLine();
            registration.UnregisterTypeForComClients(cookie);
        }
    }
}
