#pragma warning disable 1591

using System.ComponentModel;
using System.Runtime.InteropServices;

namespace dSPACE.Runtime.InteropServices.ComTypes.Internal;

/// <summary>
/// For more information: https://docs.microsoft.com/en-us/windows/win32/api/_com/
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public class Ole32
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

    [DllImport("ole32.dll", CharSet = CharSet.Unicode, ExactSpelling = true, PreserveSig = false)]
    [return: MarshalAs(UnmanagedType.Interface)]
    public static extern object CoGetClassObject(
    [In, MarshalAs(UnmanagedType.LPStruct)] Guid rclsid,
    uint dwClsContext,
    IntPtr pServerInfo,
    [In, MarshalAs(UnmanagedType.LPStruct)] Guid riid);
}
