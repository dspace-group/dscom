using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;

internal class Program
{
    [DllImport("ole32.dll")]
    private static extern int CoRegisterClassObject(
        [MarshalAs(UnmanagedType.LPStruct)] Guid rclsid,
        [MarshalAs(UnmanagedType.IUnknown)] object pUnk,
        uint dwClsContext,
        uint flags,
        out uint lpdwRegister);

    [DllImport("ole32.dll")]
    public static extern int CoResumeClassObjects();

    [Flags]
    public enum RegistrationClassContext : uint
    {
        InProcessServer = 1,
        InProcessHandler = 2,
        LocalServer = 4,
        InProcessServer16 = 8,
        RemoteServer = 16,
        InProcessHandler16 = 32,
        Reserved1 = 64,
        Reserved2 = 128,
        Reserved3 = 256,
        Reserved4 = 512,
        NoCodeDownload = 1024,
        Reserved5 = 2048,
        NoCustomMarshal = 4096,
        EnableCodeDownload = 8192,
        NoFailureLog = 16384,
        DisableActivateAsActivator = 32768,
        EnableActivateAsActivator = 65536,
        FromDefaultContext = 131072,
    }

    [Flags]
    public enum RegistrationConnectionType : uint
    {
        SingleUse = 0,
        MultipleUse = 1,
        MultiSeparate = 2,
        Suspended = 4,
        Surrogate = 8,
    }

    private const string ProgId = "Greeter.Dotnet6";
    private const string Version = "1.0";

    private const string Title = "Dotnet6 Greeter";

    private const string Description = "Dotnet 6 Greeter App";

    private static void Main(string[] args)
    {
        if (args.Length == 1)
        {
            if (args[0].Equals("/regserver", StringComparison.OrdinalIgnoreCase) || args[0].Equals("-regserver", StringComparison.OrdinalIgnoreCase))
            {
                // Register
                Server.Common.RegistryHelper.RegisterOutProcServer<Server.Common.Greeter>(ProgId, Version, Title, Description);
                return;
            }
            else if (args[0].Equals("/unregserver", StringComparison.OrdinalIgnoreCase) || args[0].Equals("-unregserver", StringComparison.OrdinalIgnoreCase))
            {
                // Unregister
                throw new NotImplementedException();
            }
        }

        var guid = new Guid(typeof(Server.Common.Greeter).GetCustomAttributes<GuidAttribute>().First().Value);
        var hr = CoRegisterClassObject(guid, new COMRegistration.BasicClassFactory<Server.Common.Greeter>(), (uint)RegistrationClassContext.LocalServer, (uint)(RegistrationConnectionType.MultipleUse | RegistrationConnectionType.Suspended), out var cookie);
        if (hr < 0)
        {
            Marshal.ThrowExceptionForHR(hr);
        }

        Console.WriteLine($"Cookie: {cookie}");

        hr = CoResumeClassObjects();
        if (hr < 0)
        {
            Marshal.ThrowExceptionForHR(hr);
        }

        Console.WriteLine("Running...");
        Console.ReadLine();
    }
}
