namespace dSPACE.Runtime.InteropServices.ComTypes;

#pragma warning disable 1591

/// <summary>
/// Defines the types of connections to a class object.
/// </summary>
[Flags]
public enum RegistrationConnectionType : uint
{
    SingleUse = 0,
    MultipleUse = 1,
    MultiSeparate = 2,
    Suspended = 4,
    Surrogate = 8,
}
