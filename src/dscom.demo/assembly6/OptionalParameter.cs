using System.Runtime.InteropServices;

namespace dSPACE.Runtime.InteropServices.DemoAssembly6;

[ComVisible(true)]
public interface IOptionalParameter
{
    public void Method1([Optional] int param1);
    public void Method2(int param1 = default);
    public void Method3(int param1 = 123);
    public void Method4(int param1);
}
