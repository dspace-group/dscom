#pragma warning disable 1591

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace dSPACE.Runtime.InteropServices.ComTypes;

/// <summary>
/// For more information: https://docs.microsoft.com/en-us/windows/win32/api/_automat/
/// </summary>
[StructLayout(LayoutKind.Explicit)]
[System.Security.SecurityCritical]
[EditorBrowsable(EditorBrowsableState.Never)]
[ExcludeFromCodeCoverage]
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
