using System.Runtime.InteropServices;

namespace dSPACE.Runtime.InteropServices.ComTypes;

/// <summary>
/// For more information: https://docs.microsoft.com/en-us/windows/win32/api/oaidl/nn-oaidl-icreatetypelib2
/// </summary>
[ComImport, Guid("0002040F-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface ICreateTypeLib2 : ICreateTypeLib
{
    [PreserveSig]
    new HRESULT CreateTypeInfo([In, MarshalAs(UnmanagedType.LPWStr)] string szName, TYPEKIND tkind, out ICreateTypeInfo ppCTInfo);

    [PreserveSig]
    new HRESULT SetName([In, MarshalAs(UnmanagedType.LPWStr)] string? szName);

    [PreserveSig]
    new HRESULT SetVersion(ushort wMajorVerNum, ushort wMinorVerNum);

    [PreserveSig]
    new HRESULT SetGuid(in Guid guid);

    [PreserveSig]
    new HRESULT SetDocString([MarshalAs(UnmanagedType.LPWStr)] string szDoc);

    [PreserveSig]
    new HRESULT SetHelpFileName([MarshalAs(UnmanagedType.LPWStr)] string? szHelpFileName);

    [PreserveSig]
    new HRESULT SetHelpContext(uint dwHelpContext);

    [PreserveSig]
    new HRESULT SetLcid(int lcid);

    [PreserveSig]
    new HRESULT SetLibFlags(uint uLibFlags);

    [PreserveSig]
    new HRESULT SaveAllChanges();

    [PreserveSig]
    HRESULT DeleteTypeInfo([MarshalAs(UnmanagedType.LPWStr)] string? szName);

    [PreserveSig]
    HRESULT SetCustData(in Guid guid, [In, MarshalAs(UnmanagedType.Struct)] ref object? pVarVal);

    [PreserveSig]
    HRESULT SetHelpStringContext(uint dwHelpStringContext);

    [PreserveSig]
    HRESULT SetHelpStringDll([MarshalAs(UnmanagedType.LPWStr)] string szFileName);
}
