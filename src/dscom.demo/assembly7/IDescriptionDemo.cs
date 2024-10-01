using System.ComponentModel;
using System.Runtime.InteropServices;

namespace dSPACE.Runtime.InteropServices.DemoAssembly7;

[ComVisible(true)]
public interface IDescriptionDemo
{
    int MyProperty1
    {
        [Description("MyPropertyDescription1")]
        get; set;
    }

    [Description("MyPropertyDescription2")]
    int MyProperty2 { get; set; }
}
