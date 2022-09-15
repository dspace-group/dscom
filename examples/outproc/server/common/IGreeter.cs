using System;
using System.Runtime.InteropServices;

namespace Server.Common;

[ComVisible(true)]
[Guid("fe48e076-b535-4438-882c-a534a2c5df7e")]
[InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
public interface IGreeter
{
    string SayHello(string name);
}
