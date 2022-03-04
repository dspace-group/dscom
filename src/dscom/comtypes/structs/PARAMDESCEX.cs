using System.Runtime.InteropServices;

namespace dSPACE.Runtime.InteropServices.ComTypes;

[StructLayout(LayoutKind.Sequential)]
public struct PARAMDESCEX
{
    public ulong size;

    public VARIANT varValue;
}
