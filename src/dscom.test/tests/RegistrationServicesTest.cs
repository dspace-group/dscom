// Copyright 2022 dSPACE GmbH, Mark Lechtermann, Matthias Nissen and Contributors
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System.Runtime.InteropServices;

namespace dSPACE.Runtime.InteropServices.Tests;

public class RegistrationServicesTest : BaseTest
{
    [Fact]
    public void ITestClassRegister_CoGetClassObject_ShouldNotBeNull()
    {
        var registrationServices = new RegistrationServices();
        var cookie = registrationServices.RegisterTypeForComClients(typeof(RegistrationServicesTestClass), ComTypes.RegistrationClassContext.LocalServer, ComTypes.RegistrationConnectionType.MultipleUse);

        var guid = new Guid(typeof(RegistrationServicesTestClass).GetCustomAttribute<GuidAttribute>()!.Value);
        var guidIClassFactory = new Guid(typeof(ComTypes.IClassFactory).GetCustomAttribute<GuidAttribute>()!.Value);
        var classFactory = ComTypes.Ole32.CoGetClassObject(guid, (uint)ComTypes.RegistrationClassContext.LocalServer, IntPtr.Zero, guidIClassFactory);

        // CreateInstance(null, interfaceGuid, out IntPtr ptr);
        // Marshal.GetObjectForIUnknown(ptr) as IGreeter;        

        classFactory.Should().NotBeNull();

        registrationServices.UnregisterTypeForComClients(cookie);
    }
}

[ComVisible(true)]
[InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
[Guid("b1ab8205-71c1-4fba-839b-1e8cbb857bd7")]
public interface IRegistrationServicesTestInterface
{
    string TestMethod();
}

[ComVisible(true)]
[Guid("ed1ad172-6f4e-4ae1-911b-2b6bf65c2cf3")]
public class RegistrationServicesTestClass : IRegistrationServicesTestInterface
{
    public string TestMethod()
    {
        return "TestString";
    }
}
