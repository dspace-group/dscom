namespace dSPACE.Runtime.InteropServices.Attributes;

/// <summary>
/// Indicates that the class or interface is restricted and cannot be used in scripts or macros. This is equivalent to MIDL attribute <code>restricted</code>.
/// </summary>
public class RestrictedAttribute : TypeFlagsAttribute
{
    /// <summary>
    /// Indicates whether the type is restricted or not.
    /// </summary>
    /// <param name="restricted">If <code>true</code>, the type will not be accessible in scripts or macros.</param>
    public RestrictedAttribute(bool restricted = true) : base(restricted) { }

    /// <summary>
    /// Indicates whether the type is restricted or not.
    /// </summary>
    public override TYPEFLAGS Flags => TYPEFLAGS.TYPEFLAG_FRESTRICTED;
}
