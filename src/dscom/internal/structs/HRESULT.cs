#pragma warning disable 1591

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Security;

namespace dSPACE.Runtime.InteropServices.ComTypes.Internal;

/// <summary>
/// For more information: https://docs.microsoft.com/en-us/windows/win32/api/_automat/
/// </summary>
[StructLayout(LayoutKind.Explicit)]
[ExcludeFromCodeCoverage]
[EditorBrowsable(EditorBrowsableState.Never)]
public struct HRESULT
{
    [FieldOffset(0)]
    private readonly int _value;

    public readonly bool Succeeded => _value >= 0;

    public readonly bool Failed => _value < 0;

    public HRESULT(int i)
    {
        _value = i;
    }

    public static bool operator !=(HRESULT hrLeft, HRESULT hrRight)
    {
        return !(hrLeft == hrRight);
    }

    public static bool operator !=(HRESULT hrLeft, int hrRight)
    {
        return !(hrLeft == hrRight);
    }

    public static bool operator !=(HRESULT hrLeft, uint hrRight)
    {
        return !(hrLeft == hrRight);
    }

    public static bool operator ==(HRESULT hrLeft, HRESULT hrRight)
    {
        return hrLeft.Equals(hrRight);
    }

    public static bool operator ==(HRESULT hrLeft, int hrRight)
    {
        return hrLeft.Equals(hrRight);
    }

    public static bool operator ==(HRESULT hrLeft, uint hrRight)
    {
        return hrLeft.Equals(hrRight);
    }

    public static implicit operator uint(HRESULT hr)
    {
        return (uint)hr._value;
    }

    public static explicit operator HRESULT(uint value)
    {
        return new((int)value);
    }

    public static implicit operator int(HRESULT hr)
    {
        return hr._value;
    }

    public static explicit operator HRESULT(int value)
    {
        return new(value);
    }

    [SecurityCritical, SecuritySafeCritical]
    public readonly void ThrowIfFailed(string? message = null)
    {
        if (Failed)
        {
            throw new COMException(message, Marshal.GetExceptionForHR(_value));
        }
    }

    public override readonly bool Equals(object? obj)
    {
        if (obj == null)
        {
            return false;
        }

        if (obj is uint uintValue)
        {
            return _value == uintValue;
        }

        if (obj is int intValue)
        {
            return _value == intValue;
        }

        if (obj is HRESULT hresultValue)
        {
            return _value == hresultValue._value;
        }

        return false;
    }

    public override readonly int GetHashCode()
    {
        return _value;
    }

    [SuppressMessage("Microsoft.Style", "IDE1006", Justification = "")]
    public const int S_OK = unchecked(0x00000000);

    [SuppressMessage("Microsoft.Style", "IDE1006", Justification = "")]
    public const int E_FAIL = unchecked((int)0x80004005);

    [SuppressMessage("Microsoft.Style", "IDE1006", Justification = "")]
    public const int E_INVALIDARG = unchecked((int)0x80070057);

    [SuppressMessage("Microsoft.Style", "IDE1006", Justification = "")]
    public const int TYPE_E_DUPLICATEID = unchecked((int)0x800288C6);

    [SuppressMessage("Microsoft.Style", "IDE1006", Justification = "")]
    public const int TLBX_E_GENERICINST_SIGNATURE = unchecked((int)0x8013117D);

    [SuppressMessage("Microsoft.Style", "IDE1006", Justification = "")]
    public const int TLBX_E_BAD_NATIVETYPE = unchecked((int)0x801311A6);

    [SuppressMessage("Microsoft.Style", "IDE1006", Justification = "")]
    public const int TLBX_I_USEIUNKNOWN = 0x0013116F;

    [SuppressMessage("Microsoft.Style", "IDE1006", Justification = "")]
    public const int CLASS_E_NOAGGREGATION = unchecked((int)0x80040110);
}
