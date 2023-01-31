namespace dSPACE.Runtime.InteropServices.Attributes;

/// <summary>
/// Allow one to define COM alias for an element. This alias is used in the exported type library. 
/// Overriding the default name means that you are responsible for handling naming collision within the 
/// type library.
/// </summary>
[AttributeUsage(AttributeTargets.Class |
                AttributeTargets.Enum |
                AttributeTargets.Event |
                AttributeTargets.Field |
                AttributeTargets.Interface |
                AttributeTargets.Method |
                AttributeTargets.Parameter |
                AttributeTargets.Property |
                AttributeTargets.ReturnValue |
                AttributeTargets.Struct,
    AllowMultiple = false)]
public class ComAliasAttribute : Attribute
{
    /// <summary>
    /// Indicate the alias to be used for the element.
    /// </summary>
    /// <param name="alias">The name to use in the exported type library and consequently the COM client code.</param>
    public ComAliasAttribute(string alias) { Alias = alias; }

    /// <summary>
    /// Alias name to be used in the exported type library.
    /// </summary>
    public string Alias { get; }
}
