using System.Runtime.InteropServices.ComTypes;

namespace dSPACE.Runtime.InteropServices.Attributes;

/// <summary>
/// Sets the <see cref="FUNCFLAGS"/> on the COM-visible type. The values will be or'd together.
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = false)]
public abstract class FuncFlagsAttribute : Attribute
{
    private readonly bool _flagsSet;

    /// <summary>
    /// Assigns a specific <see cref="FUNCFLAGS"/> value for the type.
    /// </summary>
    /// <param name="flagsSet">Indicate whether the flag is set.</param>
    public FuncFlagsAttribute(bool flagsSet)
    {
        _flagsSet = flagsSet;
    }

    /// <summary>
    /// Returns the assigned <see cref="FUNCFLAGS"/> value.
    /// </summary>
    public abstract FUNCFLAGS Flags { get; }

    /// <summary>
    /// Use the method to calculate the new flags value for a given flags. 
    /// </summary>
    /// <param name="flags">Existing flags to be updated.</param>
    /// <returns>The modified flags.</returns>
    public FUNCFLAGS UpdateFlags(FUNCFLAGS flags)
    {
        if (_flagsSet)
        {
            return flags | Flags;
        }
        else
        {
            return flags & ~Flags;
        }
    }
}
