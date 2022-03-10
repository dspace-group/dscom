using System.Runtime.InteropServices;

namespace dSPACE.Runtime.InteropServices.ComTypes;

/// <summary>
/// For more information: https://docs.microsoft.com/en-us/windows/win32/api/_automat/
/// </summary>
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
internal struct IDLDESC
{
    public IntPtr dwReserved;

    public IDLFLAG wIDLFlags;
}
