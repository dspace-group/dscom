#pragma warning disable 1591

using System.ComponentModel;
using System.Runtime.InteropServices;

namespace dSPACE.Runtime.InteropServices.ComTypes.Internal;

/// <summary>
/// For more information: https://docs.microsoft.com/en-us/windows/win32/api/_com/
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public class OleAut32
{
    [DllImport(Constants.OleAut32, SetLastError = false, ExactSpelling = true)]
    public static extern HRESULT LoadRegTypeLib(in Guid rguid, ushort wVerMajor, ushort wVerMinor, int lcid, out ITypeLib pptlib);

    [DllImport(Constants.OleAut32, SetLastError = false, ExactSpelling = true)]
    public static extern HRESULT CreateTypeLib2(SYSKIND syskind, [MarshalAs(UnmanagedType.LPWStr)] string szFile, out ICreateTypeLib2 ppctlib);

    [DllImport(Constants.OleAut32, SetLastError = false, ExactSpelling = true)]
    public static extern HRESULT UnRegisterTypeLibForUser(in Guid libID, ushort wMajorVerNum, ushort wMinorVerNum, int lcid, SYSKIND syskind);

    [DllImport(Constants.OleAut32, SetLastError = false, ExactSpelling = true)]
    public static extern HRESULT UnRegisterTypeLib(in Guid libID, ushort wVerMajor, ushort wVerMinor, int lcid, SYSKIND syskind);

    [DllImport(Constants.OleAut32, SetLastError = false, ExactSpelling = true)]
    public static extern HRESULT RegisterTypeLib(ITypeLib ptlib, [MarshalAs(UnmanagedType.LPWStr)] string szFullPath, [MarshalAs(UnmanagedType.LPWStr), Optional] string szHelpDir);

    [DllImport(Constants.OleAut32, SetLastError = false, ExactSpelling = true)]
    public static extern HRESULT RegisterTypeLibForUser(ITypeLib ptlib, [MarshalAs(UnmanagedType.LPWStr)] string szFullPath, [MarshalAs(UnmanagedType.LPWStr), Optional] string szHelpDir);

    [DllImport(Constants.OleAut32, SetLastError = false, ExactSpelling = true)]
    public static extern HRESULT LoadTypeLibEx([MarshalAs(UnmanagedType.LPWStr)] string szFile, REGKIND regkind, out ITypeLib pptlib);
}

