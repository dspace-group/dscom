using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.Reflection;

namespace Server.Common;

public class OutProcServer
{
    public static void Register<T>(string versionIndependentProgId, string version, string title, string description) where T : class
    {
        static void CreateSubKey(RegistryKey parentKey, string name, string value)
        {
            using var subKey = parentKey.CreateSubKey(name, true);
            if (value != null)
            {
                subKey.SetValue(null, value);
            }
        }

        var exePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, AppDomain.CurrentDomain.FriendlyName);
        if (!(Environment.Version < new Version(5, 0)))
        {
            exePath += ".exe";
        }
        var progId = $"{versionIndependentProgId}.{version}";
        var guid = typeof(T).GetCustomAttributes<GuidAttribute>().FirstOrDefault() ?? throw new ArgumentException("Coclass guid not set!");
        var clsId = string.Format("{{{0}}}", guid.Value);

        using (var progIdKey = Registry.ClassesRoot.CreateSubKey(progId, true))
        {
            progIdKey.SetValue(null, description);
            CreateSubKey(progIdKey, "CLSID", clsId);
        }

        using (
            var versionIndependentProgIdKey = Registry.ClassesRoot.CreateSubKey(
                versionIndependentProgId,
                true))
        {
            versionIndependentProgIdKey.SetValue(null, description);
            CreateSubKey(versionIndependentProgIdKey, "CLSID", clsId);
            CreateSubKey(versionIndependentProgIdKey, "CurVer", progId);
        }

        using (var clsIdRootKey = Registry.ClassesRoot.OpenSubKey("CLSID", true))
        {
            using var clsIdKey = (clsIdRootKey?.CreateSubKey(clsId, true)) ?? throw new ArgumentException("clsIdKey is null!");
            clsIdKey.SetValue(null, description);
            CreateSubKey(clsIdKey, "ProgID", progId);
            CreateSubKey(clsIdKey, "VersionIndependentProgID", versionIndependentProgId);
            CreateSubKey(clsIdKey, "Programmable", string.Empty);
            CreateSubKey(clsIdKey, "LocalServer32", exePath);

            clsIdKey.SetValue("AppID", clsId);

            Console.WriteLine("Register LocalServer32: " + exePath);
            Console.WriteLine("Register AppID: " + clsId);
            Console.WriteLine("Register ProgID: " + progId);
            Console.WriteLine("Register VersionIndependentProgID: " + versionIndependentProgId);
        }

        using var appIdRootKey = Registry.ClassesRoot.OpenSubKey("AppID", true) ?? throw new ArgumentException("appIdRootKey is null!");
        CreateSubKey(appIdRootKey, clsId, title);

        var fileNameOfExecutingAssembly = Path.Combine(AppDomain.CurrentDomain.FriendlyName);
        if (!(Environment.Version < new Version(5, 0)))
        {
            fileNameOfExecutingAssembly += ".exe";
        }
        using var exeKey = appIdRootKey.CreateSubKey(fileNameOfExecutingAssembly, true);
        exeKey.SetValue("AppID", clsId);
    }

    public static void Unregister<T>(string versionIndependentProgId, string version)
    {
        var guid = typeof(T).GetCustomAttributes<GuidAttribute>().FirstOrDefault() ?? throw new ArgumentException("Coclass guid not set!");
        var clsId = string.Format("{{{0}}}", guid.Value);
        using (var clsIdRootKey = Registry.ClassesRoot.OpenSubKey("CLSID", true))
        {
            Console.WriteLine($"Deleting {clsId} from {clsIdRootKey}");
            clsIdRootKey?.DeleteSubKeyTree(clsId, false);
        }
        using (var clsIdRootKey = Registry.ClassesRoot.OpenSubKey("AppID", true))
        {
            Console.WriteLine($"Deleting {clsId} from {clsIdRootKey}");
            clsIdRootKey?.DeleteSubKeyTree(clsId, false);
            var fileNameOfExecutingAssembly = Path.Combine(AppDomain.CurrentDomain.FriendlyName);
            if (!(Environment.Version < new Version(5, 0)))
            {
                fileNameOfExecutingAssembly += ".exe";
            }
            Console.WriteLine($"Deleting {fileNameOfExecutingAssembly} from {clsIdRootKey}");
            clsIdRootKey?.DeleteSubKeyTree(fileNameOfExecutingAssembly, false);
        }

        var progId = $"{versionIndependentProgId}.{version}";
        Registry.ClassesRoot.DeleteSubKeyTree(versionIndependentProgId, false);
        Registry.ClassesRoot.DeleteSubKeyTree(progId, false);
    }
}
