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

[
    ComVisible(true),
    ComAlias("FrooFroo")
]
public enum Fruits
{
    [ComAlias("frApple")]
    Apple,
    [ComAlias("frBanana")]
    Banana,
    [ComAlias("frCherry")]
    Cherry
}
