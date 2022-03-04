using System.Runtime.InteropServices;

namespace dSPACE.Runtime.InteropServices.ComTypes;

[StructLayout(LayoutKind.Sequential)]
public struct CUSTDATAITEM
{
    public Guid guid;

    public VARIANT varValue;
}
