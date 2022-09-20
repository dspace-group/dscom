namespace dSPACE.Runtime.InteropServices.ComTypes;

#pragma warning disable 1591

/// <summary>
/// Registers the specified type with COM using the specified execution context and connection type.
/// </summary>
[Flags]
public enum RegistrationClassContext : uint
{
    InProcessServer = 1,
    InProcessHandler = 2,
    LocalServer = 4,
    InProcessServer16 = 8,
    RemoteServer = 16,
    InProcessHandler16 = 32,
    Reserved1 = 64,
    Reserved2 = 128,
    Reserved3 = 256,
    Reserved4 = 512,
    NoCodeDownload = 1024,
    Reserved5 = 2048,
    NoCustomMarshal = 4096,
    EnableCodeDownload = 8192,
    NoFailureLog = 16384,
    DisableActivateAsActivator = 32768,
    EnableActivateAsActivator = 65536,
    FromDefaultContext = 131072,
}
