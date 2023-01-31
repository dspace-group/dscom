namespace dSPACE.Runtime.InteropServices.Attributes;

/// <summary>
/// Indicates that the class or interface is hidden from the object browsers. This is equivalent to MIDL attribute <code>hidden</code> / <see cref="TYPEFLAGS.TYPEFLAG_FHIDDEN"/>.
/// </summary>
public class HiddenAttribute : TypeFlagsAttribute
{
    /// <summary>
    /// Indicates whether the type is hidden or not.
    /// </summary>
    /// <param name="hidden">If <code>true</code>, the type will not be accessible in scripts or macros.</param>
    public HiddenAttribute(bool hidden = true) : base(hidden) { }

    /// <summary>
    /// Indicates whether the type is hidden or not.
    /// </summary>
    public override TYPEFLAGS Flags => TYPEFLAGS.TYPEFLAG_FHIDDEN;
}
