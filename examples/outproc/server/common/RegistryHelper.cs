using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.Reflection;
namespace Server.Common;

public class RegistryHelper
{
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

    public static void RegisterOutProcServer<T>(string versionIndependentProgId, string version, string title, string description) where T : class
    {

        var exePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, AppDomain.CurrentDomain.FriendlyName);
        if (!(System.Environment.Version < new System.Version(5,0)))
        {
            exePath +=".exe";
        }
        string progId = $"{versionIndependentProgId}.{version}";
        GuidAttribute guid = (GuidAttribute)typeof(T).GetCustomAttributes<GuidAttribute>().FirstOrDefault();
        if (guid == null)
        {
            throw new ArgumentException("Coclass guid not set!");
        }
        string clsId = String.Format("{{{0}}}", guid.Value);

        using (RegistryKey progIdKey = Registry.ClassesRoot.CreateSubKey(progId, true))
        {
            progIdKey.SetValue(null, description);
            CreateSubKey(progIdKey, "CLSID", clsId);
        }

        using (
            RegistryKey versionIndependentProgIdKey = Registry.ClassesRoot.CreateSubKey(
                versionIndependentProgId,
                true))
        {
            versionIndependentProgIdKey.SetValue(null, description);
            CreateSubKey(versionIndependentProgIdKey, "CLSID", clsId);
            CreateSubKey(versionIndependentProgIdKey, "CurVer", progId);
        }

        using (RegistryKey? clsIdRootKey = Registry.ClassesRoot.OpenSubKey("CLSID", true))
        {
            using (RegistryKey? clsIdKey = clsIdRootKey?.CreateSubKey(clsId, true))
            {
                if (clsIdKey == null)
                {
                    throw new System.ArgumentException("clsIdKey is null!");
                }
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
        }

        using (RegistryKey? appIdRootKey = Registry.ClassesRoot.OpenSubKey("AppID", true))
        {
            if (appIdRootKey == null)
            {
                throw new System.ArgumentException("appIdRootKey is null!");
            }
            CreateSubKey(appIdRootKey, clsId, title);

            var fileNameOfExecutingAssembly = Path.Combine(AppDomain.CurrentDomain.FriendlyName);
            if (!(System.Environment.Version < new System.Version(5,0)))
            {
                fileNameOfExecutingAssembly +=".exe";
            }
            using (RegistryKey exeKey = appIdRootKey.CreateSubKey(fileNameOfExecutingAssembly, true))
            {
                exeKey.SetValue("AppID", clsId);
            }
        }
    }

    public static void UnregisterOutProcServer<T>(string versionIndependentProgId, string version)
    {
        GuidAttribute guid = (GuidAttribute)typeof(T).GetCustomAttributes<GuidAttribute>().FirstOrDefault();
        if (guid == null)
        {
            throw new ArgumentException("Coclass guid not set!");
        }
        string clsId = String.Format("{{{0}}}", guid.Value);
        using (RegistryKey? clsIdRootKey = Registry.ClassesRoot.OpenSubKey("CLSID", true))
        {
            System.Console.WriteLine($"Deleting {clsId} from {clsIdRootKey}");
            clsIdRootKey?.DeleteSubKeyTree(clsId,false);
        }
        using (RegistryKey? clsIdRootKey = Registry.ClassesRoot.OpenSubKey("AppID", true))
        {
            System.Console.WriteLine($"Deleting {clsId} from {clsIdRootKey}");
            clsIdRootKey?.DeleteSubKeyTree(clsId,false);
            var fileNameOfExecutingAssembly = Path.Combine(AppDomain.CurrentDomain.FriendlyName);
            if (!(System.Environment.Version < new System.Version(5,0)))
            {
                fileNameOfExecutingAssembly +=".exe";
            }
            System.Console.WriteLine($"Deleting {fileNameOfExecutingAssembly} from {clsIdRootKey}");
            clsIdRootKey?.DeleteSubKeyTree(fileNameOfExecutingAssembly,false);
        }

        string progId = $"{versionIndependentProgId}.{version}";
        Registry.ClassesRoot.DeleteSubKeyTree(versionIndependentProgId,false);
        Registry.ClassesRoot.DeleteSubKeyTree(progId,false);
    }
}
