using System.Runtime.InteropServices;

namespace dSPACE.Runtime.InteropServices.ComTypes;

[StructLayout(LayoutKind.Sequential)]
public struct CUSTDATA
{
    /// <summary>
    /// The number of custom data items in the prgCustData array.
    /// </summary>
    public uint cCustData;

    /// <summary>
    /// The array of custom data items.
    /// </summary>
    public IntPtr prgCustData;

    /// <summary>
    /// Gets the array of <see cref="CUSTDATAITEM"/> structures.
    /// </summary>
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
