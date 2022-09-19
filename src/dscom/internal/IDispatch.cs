#pragma warning disable 1591

using System.ComponentModel;
using System.Runtime.InteropServices;

namespace dSPACE.Runtime.InteropServices.ComTypes.Internal;

/// <summary>
/// For more information: https://docs.microsoft.com/en-us/windows/win32/api/oaidl/nn-oaidl-idispatch
/// </summary>
[System.Security.SuppressUnmanagedCodeSecurity, ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("00020400-0000-0000-C000-000000000046")]
[EditorBrowsable(EditorBrowsableState.Never)]
public interface IDispatch
{
    [System.Security.SecurityCritical]
    void GetTypeInfoCount(out uint pctinfo);

    [System.Security.SecurityCritical]
    void GetTypeInfo(uint iTInfo, int lcid, out ITypeInfo ppTInfo);

    [System.Security.SecurityCritical]
    void GetIDsOfNames([Optional] in Guid riid, [In, MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPWStr, SizeParamIndex = 2)] string[] rgszNames,
        uint cNames, int lcid, [Out, MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.I4, SizeParamIndex = 2)] int[] rgDispId);

    [System.Security.SecurityCritical]
    void Invoke(int dispIdMember, [Optional] in Guid riid, int lcid, INVOKEKIND wFlags, ref DISPPARAMS pDispParams, [Optional] IntPtr pVarResult, [Optional] IntPtr pExcepInfo, [Optional] IntPtr puArgErr);
}
