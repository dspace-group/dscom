using System.Runtime.InteropServices;

namespace dSPACE.Runtime.InteropServices.ComTypes;

/// <summary>
/// For more information: https://docs.microsoft.com/en-us/windows/win32/api/oaidl/nn-oaidl-icreatetypeinfo
/// </summary>
[ComImport, Guid("00020405-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface ICreateTypeInfo
{
    [PreserveSig]
    HRESULT SetGuid(in Guid guid);

    [PreserveSig]
    HRESULT SetTypeFlags(uint uTypeFlags);

    [PreserveSig]
    HRESULT SetDocString([MarshalAs(UnmanagedType.LPWStr)] string pStrDoc);

    [PreserveSig]
    HRESULT SetHelpContext(uint dwHelpContext);

    [PreserveSig]
    HRESULT SetVersion(ushort wMajorVerNum, ushort wMinorVerNum);

    [PreserveSig]
    HRESULT AddRefTypeInfo([In] ITypeInfo? pTInfo, out uint phRefType);

    [PreserveSig]
    HRESULT AddFuncDesc(uint index, in FUNCDESC pFuncDesc);

    [PreserveSig]
    HRESULT AddImplType(uint index, uint hRefType);

    [PreserveSig]
    HRESULT SetImplTypeFlags(uint index, IMPLTYPEFLAGS implTypeFlags);

    [PreserveSig]
    HRESULT SetAlignment(ushort cbAlignment);

    [PreserveSig]
    HRESULT SetSchema([MarshalAs(UnmanagedType.LPWStr)] string? pStrSchema);

    [PreserveSig]
    HRESULT AddVarDesc(uint index, in VARDESC pVarDesc);

    [PreserveSig]
    HRESULT SetFuncAndParamNames(uint index, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2, ArraySubType = UnmanagedType.LPWStr)] string[] rgszNames, uint cNames);

    [PreserveSig]
    HRESULT SetVarName(uint index, [MarshalAs(UnmanagedType.LPWStr)] string szName);

    [PreserveSig]
    HRESULT SetTypeDescAlias(in TYPEDESC pTDescAlias);

    [PreserveSig]
    HRESULT DefineFuncAsDllEntry(uint index, [MarshalAs(UnmanagedType.LPWStr)] string szDllName, [MarshalAs(UnmanagedType.LPWStr)] string szProcName);

    [PreserveSig]
    HRESULT SetFuncDocString(uint index, [MarshalAs(UnmanagedType.LPWStr)] string szDocString);

    [PreserveSig]
    HRESULT SetVarDocString(uint index, [MarshalAs(UnmanagedType.LPWStr)] string szDocString);

    [PreserveSig]
    HRESULT SetFuncHelpContext(uint index, uint dwHelpContext);

    [PreserveSig]
    HRESULT SetVarHelpContext(uint index, uint dwHelpContext);

    [PreserveSig]
    HRESULT SetMops(uint index, [MarshalAs(UnmanagedType.BStr)] string bstrMops);

    [PreserveSig]
    HRESULT SetTypeIdldesc(in IDLDESC pIdlDesc);

    [PreserveSig]
    HRESULT LayOut();
}
