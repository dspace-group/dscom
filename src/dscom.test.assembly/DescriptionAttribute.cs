using System.ComponentModel;
using System.Runtime.InteropServices;

namespace dSPACE.Runtime.InteropServices.Test;

[ComVisible(true)]
public interface ITestForDescriptionAttribute
{
    int MyProperty1
    {
        [Description("MyPropertyDescription1")]
        get; set;
    }

    [Description("MyPropertyDescription2")]
    int MyProperty2 { get; set; }
}
