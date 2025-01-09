using System.Runtime.InteropServices;
using dSPACE.Runtime.InteropServices.Attributes;

namespace dSPACE.Runtime.InteropServices.Test;

[
    ComVisible(true),
    ComAlias("IFOO")
]
public interface IFoo
{
    [ComAlias("DOFOO")]
    void DoFoo();
}
