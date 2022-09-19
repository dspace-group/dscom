#pragma warning disable 1591

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace dSPACE.Runtime.InteropServices.ComTypes;

/// <summary>
/// For more information: https://docs.microsoft.com/en-us/windows/win32/api/_automat/
/// </summary>
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
[EditorBrowsable(EditorBrowsableState.Never)]
[ExcludeFromCodeCoverage]
public struct IDLDESC
{
    public IntPtr dwReserved;

    public IDLFLAG wIDLFlags;
}
