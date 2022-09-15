using System.Runtime.InteropServices;

namespace dSPACE.Runtime.InteropServices.ComTypes;

/// <summary>
/// For more information: https://docs.microsoft.com/en-us/windows/win32/api/_com/
/// </summary>
internal class Ole32
{
    [DllImport(Constants.Ole32)]
    public static extern int CoResumeClassObjects();

    [DllImport(Constants.Ole32)]
    public static extern int CoRegisterClassObject(
    [MarshalAs(UnmanagedType.LPStruct)] Guid rclsid,
    [MarshalAs(UnmanagedType.IUnknown)] object pUnk,
    uint dwClsContext,
    uint flags,
    out int lpdwRegister);

    [DllImport(Constants.Ole32)]
    public static extern int CoRevokeClassObject(int register);
}
