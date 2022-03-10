namespace dSPACE.Runtime.InteropServices.ComTypes;

/// <summary>
/// Controls how a type library is registered.
/// </summary>
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
