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

public class PropertyTest : BaseTest
{
    [Fact]
    public void PropertiesWithNameValue_DispId0IsUsed()
    {
        var result = CreateAssembly()
                        .WithInterface("TestInterface").WithCustomAttribute<InterfaceTypeAttribute>(ComInterfaceType.InterfaceIsIDispatch)
                            .WithProperty("Value", typeof(int))
                            .Build()
                         .Build()
                    .Build();

        using var releasableFuncDesc = result.TypeLib.GetTypeInfoByName("TestInterface")?.GetFuncDescByName("Value");
        releasableFuncDesc.Should().NotBeNull();
        releasableFuncDesc!.Value.invkind.Should().Be(INVOKEKIND.INVOKE_PROPERTYGET);
        releasableFuncDesc!.Value.memid.Should().Be(0);

        // Check that no unexpected warnings occurred 
        result.TypeLibExporterNotifySink.ReportedEvents.Should().OnlyContain(x => x.EventKind == ExporterEventKind.NOTIF_TYPECONVERTED);
    }

    [Theory]
    [InlineData(ComInterfaceType.InterfaceIsDual)]
    [InlineData(ComInterfaceType.InterfaceIsIDispatch)]
    [InlineData(ComInterfaceType.InterfaceIsIUnknown)]
    public void PropertyInInterfaceWithComInterfaceType_Found(ComInterfaceType interfaceType)
    {
        var result = CreateAssembly(new AssemblyName($"Dynamic{interfaceType}"))
                        .WithInterface("TestInterface")
                            .WithCustomAttribute<InterfaceTypeAttribute>(interfaceType)
                            .WithProperty("TestProperty", typeof(int))
                            .Build()
                        .Build()
                    .Build();

        //check for interface
        var typeInfo = result.TypeLib.GetTypeInfoByName("TestInterface");
        typeInfo.Should().NotBeNull("TestInterface not found");

        // Search for method
        typeInfo!.ContainsFuncDescByName("TestProperty").Should().Be(true);
    }

    [Fact]
    public void PropertyInInterfaceWithComInterface_ShouldHavePutRef()
    {
        var result = CreateAssembly()
                        .WithInterface("TestSourceInterface")
                        .Build(out var testSourceInterfaceType)
                       .WithInterface("TestInterface")
                           .WithCustomAttribute<InterfaceTypeAttribute>(ComInterfaceType.InterfaceIsDual)
                           .WithProperty("TestProperty", testSourceInterfaceType!)
                           .Build()
                       .Build()
                   .Build();

        //check for source-interface
        var typeSourceInfo = result.TypeLib.GetTypeInfoByName("TestSourceInterface");
        typeSourceInfo.Should().NotBeNull("TestSourceInterface not found");

        //check for test-interface
        var typeInfo = result.TypeLib.GetTypeInfoByName("TestInterface");
        typeInfo.Should().NotBeNull("TestInterface not found");

        //check for setter
        using var funcDescByName = typeInfo!.GetFuncDescByName("TestProperty", invokeKind: INVOKEKIND.INVOKE_PROPERTYPUTREF);
        funcDescByName.Should().NotBeNull();
    }

    [Theory]
    [InlineData(typeof(byte), INVOKEKIND.INVOKE_PROPERTYPUT)]
    [InlineData(typeof(sbyte), INVOKEKIND.INVOKE_PROPERTYPUT)]
    [InlineData(typeof(short), INVOKEKIND.INVOKE_PROPERTYPUT)]
    [InlineData(typeof(ushort), INVOKEKIND.INVOKE_PROPERTYPUT)]
    [InlineData(typeof(uint), INVOKEKIND.INVOKE_PROPERTYPUT)]
    [InlineData(typeof(int), INVOKEKIND.INVOKE_PROPERTYPUT)]
    [InlineData(typeof(ulong), INVOKEKIND.INVOKE_PROPERTYPUT)]
    [InlineData(typeof(long), INVOKEKIND.INVOKE_PROPERTYPUT)]
    [InlineData(typeof(float), INVOKEKIND.INVOKE_PROPERTYPUT)]
    [InlineData(typeof(double), INVOKEKIND.INVOKE_PROPERTYPUT)]
    [InlineData(typeof(string), INVOKEKIND.INVOKE_PROPERTYPUT)]
    [InlineData(typeof(bool), INVOKEKIND.INVOKE_PROPERTYPUT)]
    [InlineData(typeof(char), INVOKEKIND.INVOKE_PROPERTYPUT)]
    [InlineData(typeof(object), INVOKEKIND.INVOKE_PROPERTYPUTREF)]
    [InlineData(typeof(object[]), INVOKEKIND.INVOKE_PROPERTYPUTREF)]
    [InlineData(typeof(System.Collections.IEnumerator), INVOKEKIND.INVOKE_PROPERTYPUTREF)]
    [InlineData(typeof(DateTime), INVOKEKIND.INVOKE_PROPERTYPUT)]
    [InlineData(typeof(Guid), INVOKEKIND.INVOKE_PROPERTYPUT)]
    [InlineData(typeof(System.Drawing.Color), INVOKEKIND.INVOKE_PROPERTYPUT)]
    [InlineData(typeof(decimal), INVOKEKIND.INVOKE_PROPERTYPUT)]
    [InlineData(typeof(Delegate), INVOKEKIND.INVOKE_PROPERTYPUTREF)]
    [InlineData(typeof(IntPtr), INVOKEKIND.INVOKE_PROPERTYPUT)]
    [InlineData(typeof(IDispatch), INVOKEKIND.INVOKE_PROPERTYPUTREF)]
    [InlineData(typeof(IUnknown), INVOKEKIND.INVOKE_PROPERTYPUTREF)]
    public void SetterPropertyInInterface_ShouldHaveInvokeKind(Type type, INVOKEKIND invokeKind)
    {
        var result = CreateAssembly()
                       .WithInterface("TestInterface")
                           .WithCustomAttribute<InterfaceTypeAttribute>(ComInterfaceType.InterfaceIsDual)
                           .WithProperty("TestProperty", type, true, false)
                           .Build()
                       .Build()
                   .Build();

        //check for interface
        var typeInfo = result.TypeLib.GetTypeInfoByName("TestInterface");
        typeInfo.Should().NotBeNull("TestInterface not found");

        //check for setter
        using var funcDescByName = typeInfo!.GetFuncDescByName("TestProperty");
        funcDescByName.Should().NotBeNull();

        //check for invokekind
        funcDescByName!.Value.invkind.Should().Be(invokeKind);
    }

    [Theory]
    [InlineData(typeof(byte))]
    [InlineData(typeof(sbyte))]
    [InlineData(typeof(short))]
    [InlineData(typeof(ushort))]
    [InlineData(typeof(uint))]
    [InlineData(typeof(int))]
    [InlineData(typeof(ulong))]
    [InlineData(typeof(long))]
    [InlineData(typeof(float))]
    [InlineData(typeof(double))]
    [InlineData(typeof(string))]
    [InlineData(typeof(bool))]
    [InlineData(typeof(char))]
    [InlineData(typeof(object))]
    [InlineData(typeof(object[]))]
    [InlineData(typeof(System.Collections.IEnumerator))]
    [InlineData(typeof(DateTime))]
    [InlineData(typeof(Guid))]
    [InlineData(typeof(System.Drawing.Color))]
    [InlineData(typeof(decimal))]
    [InlineData(typeof(Delegate))]
    [InlineData(typeof(IntPtr))]
    [InlineData(typeof(IDispatch))]
    [InlineData(typeof(IUnknown))]
    public void IDispatchPropertyWithGetter_IsAvailable(Type type)
    {
        var result = CreateAssembly(new AssemblyName($"Dynamic{type}"))
                       .WithInterface("TestInterface").WithCustomAttribute<InterfaceTypeAttribute>(ComInterfaceType.InterfaceIsIDispatch)
                           .WithProperty("Property1", type, false, true)
                           .Build()
                        .Build()
                    .Build();

        //check for interface
        var typeInfo = result.TypeLib.GetTypeInfoByName("TestInterface");
        typeInfo.Should().NotBeNull("TestInterface not found");

        // //check for only one method
        Assert.Throws<COMException>(() => typeInfo!.GetFuncDesc(1, out var ppFuncDesc));

        //check for method
        using var funcDescByName = typeInfo!.GetFuncDescByName("Property1");
        funcDescByName.Should().NotBeNull();
        var funcDesc = funcDescByName!.Value;

        funcDesc.elemdescFunc.tdesc.vt.Should().Be(type.GetShortVarEnum());

        typeInfo!.GetDocumentation(funcDesc.memid, out var propertyName, out var docString, out var helpContext, out var helpFile);
        propertyName.Should().Be("Property1");
    }

    [Theory]
    [InlineData(typeof(byte))]
    [InlineData(typeof(sbyte))]
    [InlineData(typeof(short))]
    [InlineData(typeof(ushort))]
    [InlineData(typeof(uint))]
    [InlineData(typeof(int))]
    [InlineData(typeof(ulong))]
    [InlineData(typeof(long))]
    [InlineData(typeof(float))]
    [InlineData(typeof(double))]
    [InlineData(typeof(string))]
    [InlineData(typeof(bool))]
    [InlineData(typeof(char))]
    [InlineData(typeof(object))]
    [InlineData(typeof(object[]))]
    [InlineData(typeof(System.Collections.IEnumerator))]
    [InlineData(typeof(DateTime))]
    [InlineData(typeof(Guid))]
    [InlineData(typeof(System.Drawing.Color))]
    [InlineData(typeof(decimal))]
    [InlineData(typeof(Delegate))]
    [InlineData(typeof(IntPtr))]
    [InlineData(typeof(IDispatch))]
    [InlineData(typeof(IUnknown))]
    public void IDispatchPropertyWithSetter_IsAvailable(Type type)
    {
        var result = CreateAssembly(new AssemblyName($"Dynamic{type}"))
                       .WithInterface("TestInterface").WithCustomAttribute<InterfaceTypeAttribute>(ComInterfaceType.InterfaceIsIDispatch)
                           .WithProperty("Property1", type, true, false)
                           .Build()
                        .Build()
                    .Build();

        //check for interface
        var typeInfo = result.TypeLib.GetTypeInfoByName("TestInterface");
        typeInfo.Should().NotBeNull("TestInterface not found");

        //check for only one method
        //Assert.Throws<COMException>(() => typeInfo!.GetFuncDesc(1, out var ppFuncDesc));

        //check for method
        using var funcDescByName = typeInfo!.GetFuncDescByName("Property1");
        funcDescByName.Should().NotBeNull();
        var funcDesc = funcDescByName!.Value;

        funcDesc.elemdescFunc.tdesc.vt.Should().Be((short)VarEnum.VT_VOID);

        var elemDesc = Marshal.PtrToStructure<ELEMDESC>(funcDesc.lprgelemdescParam);
        elemDesc.tdesc.vt.Should().Be(type.GetShortVarEnum());
    }

    [Theory]
    [InlineData(typeof(byte))]
    [InlineData(typeof(sbyte))]
    [InlineData(typeof(short))]
    [InlineData(typeof(ushort))]
    [InlineData(typeof(uint))]
    [InlineData(typeof(int))]
    [InlineData(typeof(ulong))]
    [InlineData(typeof(long))]
    [InlineData(typeof(float))]
    [InlineData(typeof(double))]
    [InlineData(typeof(string))]
    [InlineData(typeof(bool))]
    [InlineData(typeof(char))]
    [InlineData(typeof(object))]
    [InlineData(typeof(object[]))]
    [InlineData(typeof(System.Collections.IEnumerator))]
    [InlineData(typeof(DateTime))]
    [InlineData(typeof(Guid))]
    [InlineData(typeof(System.Drawing.Color))]
    [InlineData(typeof(decimal))]
    [InlineData(typeof(Delegate))]
    [InlineData(typeof(IntPtr))]
    [InlineData(typeof(IDispatch))]
    [InlineData(typeof(IUnknown))]
    public void IDispatchPropertyWithSetterAndGetter_IsAvailable(Type type)
    {
        var result = CreateAssembly(new AssemblyName($"Dynamic{type}"))
                       .WithInterface("TestInterface").WithCustomAttribute<InterfaceTypeAttribute>(ComInterfaceType.InterfaceIsIDispatch)
                           .WithProperty("Property1", type, true, true)
                           .Build()
                        .Build()
                    .Build();

        //check for interface
        var typeInfo = result.TypeLib.GetTypeInfoByName("TestInterface");
        typeInfo.Should().NotBeNull("TestInterface not found");

        //check for INVOKE_PROPERTYGET
        using var propGetDescByName = typeInfo!.GetFuncDescByName("Property1", INVOKEKIND.INVOKE_PROPERTYGET);
        propGetDescByName.Should().NotBeNull();
        var propget = propGetDescByName!.Value;
        propget.elemdescFunc.tdesc.GetVarEnum().Should().Be(type.GetVarEnum());

        //check for INVOKE_PROPERTYPUT
        using var propSetDescByName = typeInfo!.GetFuncDescByName("Property1", INVOKEKIND.INVOKE_PROPERTYPUTREF | INVOKEKIND.INVOKE_PROPERTYPUT);
        propSetDescByName.Should().NotBeNull();
        var propset = propGetDescByName!.Value;
        propset.elemdescFunc.tdesc.GetVarEnum().Should().Be(type.GetVarEnum());
    }

    [Theory]
    [InlineData(typeof(byte))]
    [InlineData(typeof(sbyte))]
    [InlineData(typeof(short))]
    [InlineData(typeof(ushort))]
    [InlineData(typeof(uint))]
    [InlineData(typeof(int))]
    [InlineData(typeof(ulong))]
    [InlineData(typeof(long))]
    [InlineData(typeof(float))]
    [InlineData(typeof(double))]
    [InlineData(typeof(string))]
    [InlineData(typeof(bool))]
    [InlineData(typeof(char))]
    [InlineData(typeof(object))]
    [InlineData(typeof(object[]))]
    [InlineData(typeof(System.Collections.IEnumerator))]
    [InlineData(typeof(DateTime))]
    [InlineData(typeof(Guid))]
    [InlineData(typeof(System.Drawing.Color))]
    [InlineData(typeof(decimal))]
    [InlineData(typeof(Delegate))]
    [InlineData(typeof(IntPtr))]
    [InlineData(typeof(IDispatch))]
    [InlineData(typeof(IUnknown))]
    public void DualInterfacePropertyWithSetterAndGetter_IsAvailable(Type type)
    {
        var result = CreateAssembly(new AssemblyName($"Dynamic{type}"))
                       .WithInterface("TestInterface").WithCustomAttribute<InterfaceTypeAttribute>(ComInterfaceType.InterfaceIsDual)
                           .WithProperty("Property1", type, true, true)
                           .Build()
                        .Build()
                    .Build();

        //check for interface
        var typeInfo = result.TypeLib.GetTypeInfoByName("TestInterface");
        typeInfo.Should().NotBeNull("TestInterface not found");

        //check for INVOKE_PROPERTYGET
        using var propGetDescByName = typeInfo!.GetFuncDescByName("Property1", INVOKEKIND.INVOKE_PROPERTYGET);
        propGetDescByName.Should().NotBeNull();
        var propget = propGetDescByName!.Value;
        propget.elemdescFunc.tdesc.vt.Should().Be(type.GetShortVarEnum());

        //check for INVOKE_PROPERTYPUT
        using var propSetDescByName = typeInfo!.GetFuncDescByName("Property1", INVOKEKIND.INVOKE_PROPERTYPUTREF | INVOKEKIND.INVOKE_PROPERTYPUT);
        propSetDescByName.Should().NotBeNull();
        var propset = propGetDescByName!.Value;
        propset.elemdescFunc.tdesc.vt.Should().Be(type.GetShortVarEnum());
    }

    [Theory]
    [InlineData(typeof(byte))]
    [InlineData(typeof(sbyte))]
    [InlineData(typeof(short))]
    [InlineData(typeof(ushort))]
    [InlineData(typeof(uint))]
    [InlineData(typeof(int))]
    [InlineData(typeof(ulong))]
    [InlineData(typeof(long))]
    [InlineData(typeof(float))]
    [InlineData(typeof(double))]
    [InlineData(typeof(string))]
    [InlineData(typeof(bool))]
    [InlineData(typeof(char))]
    [InlineData(typeof(object))]
    [InlineData(typeof(object[]))]
    [InlineData(typeof(System.Collections.IEnumerator))]
    [InlineData(typeof(DateTime))]
    [InlineData(typeof(Guid))]
    [InlineData(typeof(System.Drawing.Color))]
    [InlineData(typeof(decimal))]
    [InlineData(typeof(IDispatch))]
    [InlineData(typeof(IUnknown))]
    public void IUnkownPropertyWithSetterAndGetter_IsAvailable(Type type)
    {
        var result = CreateAssembly(new AssemblyName($"Dynamic{type}"))
                       .WithInterface("TestInterface").WithCustomAttribute<InterfaceTypeAttribute>(ComInterfaceType.InterfaceIsIUnknown)
                           .WithProperty("Property1", type, true, true)
                           .Build()
                        .Build()
                    .Build();

        //check for interface
        var typeInfo = result.TypeLib.GetTypeInfoByName("TestInterface");
        typeInfo.Should().NotBeNull("TestInterface not found");

        //check for INVOKE_PROPERTYGET Ptr type
        using var propGetDescByName = typeInfo!.GetFuncDescByName("Property1", INVOKEKIND.INVOKE_PROPERTYGET);
        propGetDescByName.Should().NotBeNull();
        var propget = propGetDescByName!.Value;
        propget.elemdescFunc.tdesc.vt.Should().Be((short)VarEnum.VT_HRESULT);

        var firstParameter = propget.GetParameter(0);
        firstParameter.Should().NotBeNull();
        firstParameter!.Value.tdesc.vt.Should().Be((short)VarEnum.VT_PTR);

        // Ptr type should be the final typ
        var value = Marshal.PtrToStructure<TYPEDESC>(firstParameter!.Value.tdesc.lpValue);
        value.vt.Should().Be(type.GetShortVarEnum());

        //check for INVOKE_PROPERTYPUT
        using var propSetDescByName = typeInfo!.GetFuncDescByName("Property1", INVOKEKIND.INVOKE_PROPERTYPUTREF | INVOKEKIND.INVOKE_PROPERTYPUT);
        propSetDescByName.Should().NotBeNull();
        var propset = propGetDescByName!.Value;
        propset.elemdescFunc.tdesc.vt.Should().Be((short)VarEnum.VT_HRESULT);

        firstParameter = propget.GetParameter(0);
        firstParameter.Should().NotBeNull();
        firstParameter!.Value.tdesc.vt.Should().Be((short)VarEnum.VT_PTR);

        // Ptr type should be the final typ
        value = Marshal.PtrToStructure<TYPEDESC>(firstParameter!.Value.tdesc.lpValue);
        value.vt.Should().Be(type.GetShortVarEnum());
    }

    [Fact]
    public void NameAndValuePropertyWithSetterAndGetter_IsAvailable()
    {
        var result = CreateAssembly()
                       .WithInterface("TestInterface").WithCustomAttribute<InterfaceTypeAttribute>(ComInterfaceType.InterfaceIsIDispatch)
                           .WithProperty("Name", typeof(string), true, true)
                           .Build()
                           .WithProperty("Value", typeof(int), true, true)
                           .Build()
                         .Build()
                       .Build();

        //check for interface
        var typeInfo = result.TypeLib.GetTypeInfoByName("TestInterface");
        typeInfo.Should().NotBeNull("TestInterface not found");

        // ################### set_Name

        typeInfo!.GetFuncDesc(0, out var ppFuncDesc);
        ppFuncDesc.Should().NotBe(IntPtr.Zero);
        var funcDesc = Marshal.PtrToStructure<FUNCDESC>(ppFuncDesc);
        funcDesc.elemdescFunc.tdesc.vt.Should().Be(typeof(string).GetShortVarEnum());
        typeInfo.GetDocumentation(funcDesc.memid, out var propertyName, out _, out _, out _);
        propertyName.Should().Be("Name");

        // ################### get_Name

        typeInfo!.GetFuncDesc(1, out ppFuncDesc);
        ppFuncDesc.Should().NotBe(IntPtr.Zero);
        funcDesc = Marshal.PtrToStructure<FUNCDESC>(ppFuncDesc);
        funcDesc.elemdescFunc.tdesc.vt.Should().Be((short)VarEnum.VT_VOID);

        var elemDesc = Marshal.PtrToStructure<ELEMDESC>(funcDesc.lprgelemdescParam);
        elemDesc.tdesc.vt.Should().Be(typeof(string).GetShortVarEnum());

        Marshal.PtrToStructure<FUNCDESC>(ppFuncDesc);

        typeInfo.GetDocumentation(funcDesc.memid, out propertyName, out _, out _, out _);
        propertyName.Should().Be("Name");

        // ################### set_Value

        typeInfo!.GetFuncDesc(2, out ppFuncDesc);
        ppFuncDesc.Should().NotBe(IntPtr.Zero);
        funcDesc = Marshal.PtrToStructure<FUNCDESC>(ppFuncDesc);
        funcDesc.elemdescFunc.tdesc.vt.Should().Be(typeof(int).GetShortVarEnum());

        typeInfo.GetDocumentation(funcDesc.memid, out propertyName, out _, out _, out _);
        propertyName.Should().Be("Value");

        // ################### get_Value

        typeInfo!.GetFuncDesc(3, out ppFuncDesc);
        ppFuncDesc.Should().NotBe(IntPtr.Zero);
        funcDesc = Marshal.PtrToStructure<FUNCDESC>(ppFuncDesc);
        funcDesc.elemdescFunc.tdesc.vt.Should().Be((short)VarEnum.VT_VOID);

        elemDesc = Marshal.PtrToStructure<ELEMDESC>(funcDesc.lprgelemdescParam);
        elemDesc.tdesc.vt.Should().Be(typeof(int).GetShortVarEnum());

        Marshal.PtrToStructure<FUNCDESC>(ppFuncDesc);

        typeInfo.GetDocumentation(funcDesc.memid, out propertyName, out _, out _, out _);
        propertyName.Should().Be("Value");
    }
}
