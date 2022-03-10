namespace dSPACE.Runtime.InteropServices;

/// <summary>
/// For more information: https://docs.microsoft.com/en-us/windows/win32/api/_com/
/// </summary>
internal static class Constants
{
    public const string Kernel32 = "kernel32.dll";

    public const string OleAut32 = "oleaut32.dll";

    public const int LCID_NEUTRAL = 0;

    public const uint BASE_OLEAUT_DISPID = 0x60020000;

    public const uint BASE_OLEAUT_IUNKNOWN = 0x60010000;

    // [Windows SDK]\um\oaidl.h" 
    // DISPID reserved for the "value" property
    public const uint DISPIP_VALUE = 0x0;

    // DISPID reserved for the standard "NewEnum" method
    public const uint DISPID_NEWENUM = 0xfffffffc;

    // DISPID reserved to indicate an "unknown" name
    // only reserved for data members (properties); reused as a method dispid below
    public const uint DISPID_UNKNWN = 0xffffffff;
}
