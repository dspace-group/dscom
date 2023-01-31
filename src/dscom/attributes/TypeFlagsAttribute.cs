namespace dSPACE.Runtime.InteropServices.Attributes;

/// <summary>
/// Sets the <see cref="TYPEFLAGS"/> on the COM-visible type. The values will be or'd together.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false)]
public abstract class TypeFlagsAttribute : Attribute
{
    private readonly bool _flagsSet;

    /// <summary>
    /// Assigns a specific <see cref="TYPEFLAGS"/> value for the type.
    /// </summary>
    /// <param name="flagsSet">Indicate whether the flag is set.</param>
    public TypeFlagsAttribute(bool flagsSet)
    {
        _flagsSet = flagsSet;
    }

    /// <summary>
    /// Returns the assigned <see cref="TYPEFLAGS"/> value.
    /// </summary>
    public abstract TYPEFLAGS Flags { get; }

    /// <summary>
    /// Use the method to calculate the new flags value for a given flags. 
    /// </summary>
    /// <param name="flags">Existing flags to be updated.</param>
    /// <returns>The modified flags.</returns>
    public TYPEFLAGS UpdateFlags(TYPEFLAGS flags)
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
