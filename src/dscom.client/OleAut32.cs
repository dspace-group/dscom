using System.Runtime.InteropServices;
using SYSKIND = System.Runtime.InteropServices.ComTypes.SYSKIND;

namespace dSPACE.Runtime.InteropServices.ComTypes;

/// <summary>
/// For more information: https://docs.microsoft.com/en-us/windows/win32/api/_com/
/// </summary>
internal class OleAut32
{
    private const string OleAut32Dll = "oleaut32.dll";

    [DllImport(OleAut32Dll, SetLastError = false, ExactSpelling = true)]
    public static extern HRESULT UnRegisterTypeLibForUser(in Guid libID, ushort wMajorVerNum, ushort wMinorVerNum, int lcid, SYSKIND syskind);

    [DllImport(OleAut32Dll, SetLastError = false, ExactSpelling = true)]
    public static extern HRESULT UnRegisterTypeLib(in Guid libID, ushort wVerMajor, ushort wVerMinor, int lcid, SYSKIND syskind);

    [DllImport(OleAut32Dll, SetLastError = false, ExactSpelling = true)]
    public static extern HRESULT RegisterTypeLib(ITypeLib ptlib, [MarshalAs(UnmanagedType.LPWStr)] string szFullPath, [MarshalAs(UnmanagedType.LPWStr), Optional] string szHelpDir);

    [DllImport(OleAut32Dll, SetLastError = false, ExactSpelling = true)]
    public static extern HRESULT RegisterTypeLibForUser(ITypeLib ptlib, [MarshalAs(UnmanagedType.LPWStr)] string szFullPath, [MarshalAs(UnmanagedType.LPWStr), Optional] string szHelpDir);

    [DllImport(OleAut32Dll, SetLastError = false, ExactSpelling = true)]
    public static extern HRESULT LoadTypeLibEx([MarshalAs(UnmanagedType.LPWStr)] string szFile, REGKIND regkind, out ITypeLib pptlib);
}
