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
    private static Guid _guidOfRegistrationServicesTestClass;
    private static Guid _guidOfRegistrationServicesTestInterface;
    private static Guid _guidOfiClassFactory;

    public RegistrationServicesTest()
    {
        _guidOfRegistrationServicesTestClass = new Guid(typeof(RegistrationServicesTestClass).GetCustomAttribute<GuidAttribute>()!.Value);
        _guidOfRegistrationServicesTestInterface = new Guid(typeof(IRegistrationServicesTestInterface).GetCustomAttribute<GuidAttribute>()!.Value);
        _guidOfiClassFactory = new Guid(typeof(IClassFactory).GetCustomAttribute<GuidAttribute>()!.Value);
    }

    [Fact]
    public void RegisterTypeForComClients_TestClassAsLocalServer_ShouldBeRegistered()
    {
        var registrationServices = new RegistrationServices();
        var cookie = registrationServices.RegisterTypeForComClients(typeof(RegistrationServicesTestClass), ComTypes.RegistrationClassContext.LocalServer, ComTypes.RegistrationConnectionType.MultipleUse);
        var classFactory = Ole32.CoGetClassObject(_guidOfRegistrationServicesTestClass, (uint)ComTypes.RegistrationClassContext.LocalServer, IntPtr.Zero, _guidOfiClassFactory) as IClassFactory;

        // Create instance of IRegistrationServicesTestInterface
        classFactory.Should().NotBeNull();
        classFactory!.CreateInstance(null!, ref _guidOfRegistrationServicesTestInterface, out var ppvObject);

        // IRegistrationServicesTestInterface should be available
        var testInstance = Marshal.GetObjectForIUnknown(ppvObject) as IRegistrationServicesTestInterface;
        testInstance.Should().NotBeNull();
        testInstance!.GetSuccessString().Should().Be("Success");

        // Cleanup
        registrationServices.UnregisterTypeForComClients(cookie);
    }

    [Fact]
    public void RegisterTypeForComClients_MultipleUseOfTestClassAsLocalServer_InstanceShouldBeReused()
    {
        var registrationServices = new RegistrationServices();
        var cookie = registrationServices.RegisterTypeForComClients(typeof(RegistrationServicesTestClass), ComTypes.RegistrationClassContext.LocalServer, ComTypes.RegistrationConnectionType.MultipleUse);
        var classFactory = Ole32.CoGetClassObject(_guidOfRegistrationServicesTestClass, (uint)ComTypes.RegistrationClassContext.LocalServer, IntPtr.Zero, _guidOfiClassFactory) as IClassFactory;

        // Create instance of IRegistrationServicesTestInterface
        classFactory.Should().NotBeNull();
        classFactory!.CreateInstance(null!, ref _guidOfRegistrationServicesTestInterface, out var ppvObject1);
        var testInstance1 = Marshal.GetObjectForIUnknown(ppvObject1) as IRegistrationServicesTestInterface;
        testInstance1.Should().NotBeNull();
        var instanceId1 = testInstance1!.GetInstanceId();

        classFactory.Should().NotBeNull();
        classFactory!.CreateInstance(null!, ref _guidOfRegistrationServicesTestInterface, out var ppvObject2);
        var testInstance2 = Marshal.GetObjectForIUnknown(ppvObject2) as IRegistrationServicesTestInterface;
        testInstance2.Should().NotBeNull();
        var instanceId2 = testInstance2!.GetInstanceId();

        instanceId1.Should().NotBeNullOrEmpty();
        instanceId2.Should().NotBeNullOrEmpty();
        instanceId2.Should().NotBe(Guid.Empty.ToString());
        instanceId2.Should().NotBe(Guid.Empty.ToString());
        instanceId1.Should().Be(instanceId2);

        // Cleanup
        registrationServices.UnregisterTypeForComClients(cookie);
    }

    [Fact]
    public void RegisterTypeForComClients_SingleUseOfTestClassAsLocalServer_InstanceShouldNotBeReused()
    {
        var registrationServices = new RegistrationServices();
        var cookie = registrationServices.RegisterTypeForComClients(typeof(RegistrationServicesTestClass), ComTypes.RegistrationClassContext.LocalServer, ComTypes.RegistrationConnectionType.SingleUse);
        var classFactory = Ole32.CoGetClassObject(_guidOfRegistrationServicesTestClass, (uint)ComTypes.RegistrationClassContext.LocalServer, IntPtr.Zero, _guidOfiClassFactory) as IClassFactory;

        // Create instance of IRegistrationServicesTestInterface
        classFactory.Should().NotBeNull();
        classFactory!.CreateInstance(null!, ref _guidOfRegistrationServicesTestInterface, out var ppvObject);

        // IRegistrationServicesTestInterface should be available
        var testInstance1 = Marshal.GetObjectForIUnknown(ppvObject) as IRegistrationServicesTestInterface;
        testInstance1.Should().NotBeNull();
        var instanceId1 = testInstance1!.GetInstanceId();

        var testInstance2 = Marshal.GetObjectForIUnknown(ppvObject) as IRegistrationServicesTestInterface;
        testInstance2.Should().NotBeNull();
        var instanceId2 = testInstance2!.GetInstanceId();

        instanceId1.Should().NotBeNullOrEmpty();
        instanceId2.Should().NotBeNullOrEmpty();
        instanceId2.Should().NotBe(Guid.Empty.ToString());
        instanceId2.Should().NotBe(Guid.Empty.ToString());
        instanceId1.Should().Be(instanceId2);

        // Cleanup
        registrationServices.UnregisterTypeForComClients(cookie);
    }
}

[ComVisible(true)]
[InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
[Guid("b1ab8205-71c1-4fba-839b-1e8cbb857bd7")]
public interface IRegistrationServicesTestInterface
{
    /// <summary>
    /// Returns the string "Success"
    /// </summary>
    /// <returns>Returns the string "Success"</returns>
    string GetSuccessString();

    /// <summary>
    /// Returns a unique string for each instance of this class.
    /// </summary>
    /// <returns>A unique test string.</returns>
    string GetInstanceId();
}

[ComVisible(true)]
[Guid("ed1ad172-6f4e-4ae1-911b-2b6bf65c2cf3")]
public class RegistrationServicesTestClass : IRegistrationServicesTestInterface
{
    private readonly Guid _instanceGuid;

    public RegistrationServicesTestClass()
    {
        _instanceGuid = Guid.NewGuid();
    }

    public string GetInstanceId()
    {
        return _instanceGuid.ToString();
    }

    public string GetSuccessString()
    {
        return "Success";
    }
}
