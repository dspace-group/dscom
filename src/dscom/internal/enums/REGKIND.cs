#pragma warning disable 1591

using System.ComponentModel;

namespace dSPACE.Runtime.InteropServices.ComTypes;

/// <summary>
/// Controls how a type library is registered.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public enum REGKIND
{
    /// <summary>
    /// Use default register behavior.
    /// </summary>
    DEFAULT,

    /// <summary>
    /// Register this type library.
    /// </summary>
    REGISTER,

    /// 
    /// <summary>Do not register this type library.
    /// </summary>
    NONE,
}
