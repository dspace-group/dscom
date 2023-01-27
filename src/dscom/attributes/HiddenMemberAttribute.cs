using System.Runtime.InteropServices.ComTypes;

namespace dSPACE.Runtime.InteropServices.Attributes;

/// <summary>
/// Indicates that a member is hidden in the object browser. This is equivalent to MIDL attribute <code>hidden</code> / <see cref="FUNCFLAGS.FUNCFLAG_FHIDDEN"/>.
/// </summary>
public class HiddenMemberAttribute : FuncFlagsAttribute
{
    /// <summary>
    /// Indicates whether the member is hidden or not.
    /// </summary>
    /// <param name="hidden">If <code>true</code>, the type will not be visible in the object browser by default.</param>
    public HiddenMemberAttribute(bool hidden = true) : base(hidden) { }

    /// <summary>
    /// Indicates whether the member is hidden or not.
    /// </summary>
    public override FUNCFLAGS Flags => FUNCFLAGS.FUNCFLAG_FHIDDEN;
}
