#pragma warning disable 1591

using System.ComponentModel;
using System.Runtime.InteropServices;

namespace dSPACE.Runtime.InteropServices.ComTypes.Internal;

/// <summary>
/// For more information: https://docs.microsoft.com/en-us/windows/win32/api/unknwn/nn-unknwn-iunknown
/// </summary>
[ComVisible(false)]
[ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("00000000-0000-0000-C000-000000000046")]
//[EditorBrowsable(EditorBrowsableState.Never)]
public interface IUnknown
{
    IntPtr QueryInterface(ref Guid riid);

    [PreserveSig]
    uint AddRef();

    [PreserveSig]
    uint Release();
}
