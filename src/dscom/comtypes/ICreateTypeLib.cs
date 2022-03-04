using System.Runtime.InteropServices;

namespace dSPACE.Runtime.InteropServices.ComTypes;

/// <summary>
/// For more information: https://docs.microsoft.com/en-us/windows/win32/api/oaidl/nn-oaidl-icreatetypelib
/// </summary>
[ComImport, Guid("00020406-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface ICreateTypeLib
{
    [PreserveSig]
    HRESULT CreateTypeInfo([In, MarshalAs(UnmanagedType.LPWStr)] string szName, TYPEKIND tkind, out ICreateTypeInfo ppCTInfo);

    [PreserveSig]
    HRESULT SetName([In, MarshalAs(UnmanagedType.LPWStr)] string szName);

    [PreserveSig]
    HRESULT SetVersion(ushort wMajorVerNum, ushort wMinorVerNum);

    [PreserveSig]
    HRESULT SetGuid(in Guid guid);

    [PreserveSig]
    HRESULT SetDocString([MarshalAs(UnmanagedType.LPWStr)] string szDoc);

    [PreserveSig]
    HRESULT SetHelpFileName([MarshalAs(UnmanagedType.LPWStr)] string szHelpFileName);

    [PreserveSig]
    HRESULT SetHelpContext(uint dwHelpContext);

    [PreserveSig]
    HRESULT SetLcid(int lcid);

    [PreserveSig]
    HRESULT SetLibFlags(uint uLibFlags);

    [PreserveSig]
    HRESULT SaveAllChanges();
}
