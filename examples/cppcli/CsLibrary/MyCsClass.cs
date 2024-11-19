using System.Runtime.InteropServices;

namespace CsNs
{
    [ComVisible(true)]
    [Guid("4468308A-1E42-4CE2-8BC0-4F6AFA1951E0")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IComClass
    {
        int Add(int a, int b);
    };

    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    [Guid("6610DBEC-DD26-416B-BF94-A5D0C7192F6C")]
    public class MyCsClass : CppNs.MyCppClass, IComClass
    {
    }
}
