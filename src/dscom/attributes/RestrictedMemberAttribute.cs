using System.Runtime.InteropServices.ComTypes;

namespace dSPACE.Runtime.InteropServices.Attributes;

/// <summary>
/// Indicates that a member is restricted and cannot be used in scripts or macros. This is equivalent to MIDL attribute <code>restricted</code> / <see cref="FUNCFLAGS.FUNCFLAG_FRESTRICTED"/>.
/// </summary>
public class RestrictedMemberAttribute : FuncFlagsAttribute
{
    /// <summary>
    /// Indicates whether the member is restricted or not.
    /// </summary>
    /// <param name="restricted">If <code>true</code>, the type will not be accessible in scripts or macros.</param>
    public RestrictedMemberAttribute(bool restricted = true) : base(restricted) { }

    /// <summary>
    /// Indicates whether the member is restricted or not.
    /// </summary>
    public override FUNCFLAGS Flags => FUNCFLAGS.FUNCFLAG_FRESTRICTED;
}
