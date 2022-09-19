#pragma warning disable 1591

using System.ComponentModel;
using System.Runtime.InteropServices;

namespace dSPACE.Runtime.InteropServices.ComTypes;

/// <summary>
/// For more information: https://docs.microsoft.com/de-de/windows/win32/api/unknwn/nn-unknwn-iclassfactory
/// </summary>
[ComImport, Guid("00000001-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[EditorBrowsable(EditorBrowsableState.Never)]
public interface IClassFactory
{
    /// <summary>
    /// Creates an uninitialized object.
    /// </summary>
    void CreateInstance(
        [MarshalAs(UnmanagedType.Interface)] object pUnkOuter,
        ref Guid riid,
        out IntPtr ppvObject);

    /// <summary>
    /// The IClassFactory::LockServer method locks an object application open in memory. This enables instances to be created more quickly.
    /// </summary>
    void LockServer([MarshalAs(UnmanagedType.Bool)] bool fLock);
}
