using System.Runtime.InteropServices;
using dSPACE.Runtime.InteropServices.Attributes;

namespace dSPACE.Runtime.InteropServices.DemoAssembly4;

[
    ComVisible(true),
    ComAlias("IFOO")
]
public interface IFoo
{
    [ComAlias("DOFOO")]
    void DoFoo();
}
