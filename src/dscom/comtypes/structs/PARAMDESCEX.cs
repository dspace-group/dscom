using System.Runtime.InteropServices;

namespace dSPACE.Runtime.InteropServices.ComTypes;

/// <summary>
/// For more information: https://docs.microsoft.com/en-us/windows/win32/api/oaidl/ns-oaidl-paramdescex
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct PARAMDESCEX
{
    public ulong size;

    public VARIANT varValue;
}
