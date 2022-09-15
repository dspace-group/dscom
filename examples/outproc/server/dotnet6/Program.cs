using System.Runtime.InteropServices;
using dSPACE.Runtime.InteropServices;
using dSPACE.Runtime.InteropServices.ComTypes;
internal class Program
{
    [DllImport("ole32.dll")]
    public static extern int CoResumeClassObjects();

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

        var cookie = RegistrationServices.RegisterTypeForComClients(typeof(Server.Common.Greeter), RegistrationClassContext.LocalServer, RegistrationConnectionType.MultipleUse | RegistrationConnectionType.Suspended);

        var hr = CoResumeClassObjects();
        if (hr < 0)
        {
            Marshal.ThrowExceptionForHR(hr);
        }

        Console.WriteLine("Running...");
        Console.ReadLine();
        RegistrationServices.UnregisterTypeForComClients(cookie);
    }
}
