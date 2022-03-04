using System.Runtime.InteropServices;

namespace dSPACE.Runtime.InteropServices.ComTypes;

/// <summary>
/// VARIANTARG describes arguments passed within DISPPARAMS, and VARIANT to specify variant data that cannot be passed by reference.
/// <para>
/// When a variant refers to another variant by using the VT_VARIANT | VT_BYREF vartype, the variant being referred to cannot also
/// be of type VT_VARIANT | VT_BYREF.VARIANTs can be passed by value, even if VARIANTARGs cannot.
/// </para>
/// </summary>
[StructLayout(LayoutKind.Explicit)]
[System.Security.SecurityCritical]
public struct VARIANT
{
    [FieldOffset(0)]
    public VarEnum vt;

    [FieldOffset(2)]
    public ushort wReserved1;

    [FieldOffset(4)]
    public ushort wReserved2;

    [FieldOffset(6)]
    public ushort wReserved3;

    [FieldOffset(8)]
    public IntPtr byref;

    [FieldOffset(8)]
    private readonly Record _rec;

    [FieldOffset(8)]
    internal ulong longValue;

    [StructLayout(LayoutKind.Sequential)]
    private struct Record
    {
        private readonly IntPtr _record;

        private readonly IntPtr _recordInfo;
    }
}
