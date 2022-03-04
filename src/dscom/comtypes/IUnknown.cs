using System.Runtime.InteropServices;

namespace dSPACE.Runtime.InteropServices.ComTypes;

[ComVisible(false)]
[ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("00000000-0000-0000-C000-000000000046")]
public interface IUnknown
{
    IntPtr QueryInterface(ref Guid riid);

    [PreserveSig]
    uint AddRef();

    [PreserveSig]
    uint Release();
}
