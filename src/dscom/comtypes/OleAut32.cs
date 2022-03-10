using System.Runtime.InteropServices;

namespace dSPACE.Runtime.InteropServices.ComTypes;

/// <summary>
/// For more information: https://docs.microsoft.com/en-us/windows/win32/api/_com/
/// </summary>
internal class OleAut32
{
    [DllImport(Constants.OleAut32, SetLastError = false, ExactSpelling = true)]
    public static extern HRESULT LoadRegTypeLib(in Guid rguid, ushort wVerMajor, ushort wVerMinor, int lcid, out ITypeLib pptlib);

    [DllImport(Constants.OleAut32, SetLastError = false, ExactSpelling = true)]
    public static extern HRESULT CreateTypeLib2(SYSKIND syskind, [MarshalAs(UnmanagedType.LPWStr)] string szFile, out ICreateTypeLib2 ppctlib);

    [DllImport(Constants.OleAut32, SetLastError = false, ExactSpelling = true)]
    public static extern HRESULT LoadTypeLibEx([MarshalAs(UnmanagedType.LPWStr)] string szFile, REGKIND regkind, out ITypeLib pptlib);
}
