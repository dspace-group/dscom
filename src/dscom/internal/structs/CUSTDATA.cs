using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace dSPACE.Runtime.InteropServices.ComTypes.Internal;

/// <summary>
/// For more information: https://docs.microsoft.com/en-us/windows/win32/api/_automat/
/// </summary>
[StructLayout(LayoutKind.Sequential)]
[EditorBrowsable(EditorBrowsableState.Never)]
[ExcludeFromCodeCoverage]
public struct CUSTDATA
{
    /// <summary>
    /// The cCustData 
    /// </summary>
    public uint cCustData;

    /// <summary>
    /// And <see cref="IntPtr"/> to the prgCustData.
    /// </summary>
    public IntPtr prgCustData;

    /// <summary>
    /// The CUSTDATAITEM items
    /// </summary>
    [SuppressMessage("Style", "IDE0004:Remove Unnecessary Cast", Justification = "Required for ambiguous signature match.")]
    public CUSTDATAITEM[] Items
    {
        get
        {
            var ptr = new IntPtr((long)prgCustData);
            var items = new List<CUSTDATAITEM>();
            for (var i = 0; i < cCustData; i++)
            {
                var item = Marshal.PtrToStructure<CUSTDATAITEM>(ptr);
                items.Add(item);
                ptr = new IntPtr((long)ptr + Marshal.SizeOf<CUSTDATAITEM>());
            }

            return items.ToArray();
        }
    }
}
