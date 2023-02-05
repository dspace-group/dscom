using dSPACE.Runtime.InteropServices.Attributes;
using System.Runtime.InteropServices;

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
