using System;
using System.Runtime.InteropServices;

namespace net48;

[ComVisible(true)]
[Guid("fe48e076-b535-4438-882c-a534a2c5df7e")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface IGreeter
{
    string SayHello(string name);
}