#pragma warning disable 1591

using System.ComponentModel;
using System.Runtime.InteropServices;

namespace dSPACE.Runtime.InteropServices.ComTypes;

/// <summary>
/// For more information: https://docs.microsoft.com/en-us/windows/win32/api/oaidl/nn-oaidl-icreatetypeinfo2
/// </summary>
[ComImport, Guid("0002040E-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[EditorBrowsable(EditorBrowsableState.Never)]
public interface ICreateTypeInfo2 : ICreateTypeInfo
{
    [PreserveSig]
    new HRESULT SetGuid(in Guid guid);

    [PreserveSig]
    new HRESULT SetTypeFlags(uint uTypeFlags);

    [PreserveSig]
    new HRESULT SetDocString([MarshalAs(UnmanagedType.LPWStr)] string pStrDoc);

    [PreserveSig]
    new HRESULT SetHelpContext(uint dwHelpContext);

    [PreserveSig]
    new HRESULT SetVersion(ushort wMajorVerNum, ushort wMinorVerNum);

    [PreserveSig]
    new HRESULT AddRefTypeInfo([In] ITypeInfo? pTInfo, out uint phRefType);

    [PreserveSig]
    new HRESULT AddFuncDesc(uint index, in FUNCDESC pFuncDesc);

    [PreserveSig]
    new HRESULT AddImplType(uint index, uint hRefType);

    [PreserveSig]
    new HRESULT SetImplTypeFlags(uint index, IMPLTYPEFLAGS implTypeFlags);

    [PreserveSig]
    new HRESULT SetAlignment(ushort cbAlignment);

    [PreserveSig]
    new HRESULT SetSchema([MarshalAs(UnmanagedType.LPWStr)] string pStrSchema);

    [PreserveSig]
    new HRESULT AddVarDesc(uint index, in VARDESC pVarDesc);

    [PreserveSig]
    new HRESULT SetFuncAndParamNames(uint index, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2, ArraySubType = UnmanagedType.LPWStr)] string[] rgszNames, uint cNames);

    [PreserveSig]
    new HRESULT SetVarName(uint index, [MarshalAs(UnmanagedType.LPWStr)] string szName);

    [PreserveSig]
    new HRESULT SetTypeDescAlias(in TYPEDESC pTDescAlias);

    [PreserveSig]
    new HRESULT DefineFuncAsDllEntry(uint index, [MarshalAs(UnmanagedType.LPWStr)] string szDllName, [MarshalAs(UnmanagedType.LPWStr)] string szProcName);

    [PreserveSig]
    new HRESULT SetFuncDocString(uint index, [MarshalAs(UnmanagedType.LPWStr)] string szDocString);

    [PreserveSig]
    new HRESULT SetVarDocString(uint index, [MarshalAs(UnmanagedType.LPWStr)] string szDocString);

    [PreserveSig]
    new HRESULT SetFuncHelpContext(uint index, uint dwHelpContext);

    [PreserveSig]
    new HRESULT SetVarHelpContext(uint index, uint dwHelpContext);

    [PreserveSig]
    new HRESULT SetMops(uint index, [MarshalAs(UnmanagedType.BStr)] string bstrMops);

    [PreserveSig]
    new HRESULT SetTypeIdldesc(in IDLDESC pIdlDesc);

    [PreserveSig]
    new HRESULT LayOut();

    [PreserveSig]
    HRESULT DeleteFuncDesc(uint index);

    [PreserveSig]
    HRESULT DeleteFuncDescByMemId(int memid, INVOKEKIND invKind);

    [PreserveSig]
    HRESULT DeleteVarDesc(uint index);

    [PreserveSig]
    HRESULT DeleteVarDescByMemId(int memid);

    [PreserveSig]
    HRESULT DeleteImplType(uint index);

    [PreserveSig]
    HRESULT SetCustData(in Guid guid, [In, MarshalAs(UnmanagedType.Struct)] ref object pVarVal);

    [PreserveSig]
    HRESULT SetFuncCustData(uint index, in Guid guid, [In, MarshalAs(UnmanagedType.Struct)] ref object pVarVal);

    [PreserveSig]
    HRESULT SetParamCustData(uint indexFunc, uint indexParam, in Guid guid, [In, MarshalAs(UnmanagedType.Struct)] ref object pVarVal);

    [PreserveSig]
    HRESULT SetVarCustData(uint index, in Guid guid, [In, MarshalAs(UnmanagedType.Struct)] ref object pVarVal);

    [PreserveSig]
    HRESULT SetImplTypeCustData(uint index, in Guid guid, [In, MarshalAs(UnmanagedType.Struct)] ref object pVarVal);

    [PreserveSig]
    HRESULT SetHelpStringContext(uint dwHelpStringContext);

    [PreserveSig]
    HRESULT SetFuncHelpStringContext(uint index, uint dwHelpStringContext);

    [PreserveSig]
    HRESULT SetVarHelpStringContext(uint index, uint dwHelpStringContext);

    [PreserveSig]
    HRESULT Invalidate();

    [PreserveSig]
    HRESULT SetName([In, MarshalAs(UnmanagedType.LPWStr)] string szName);
}
