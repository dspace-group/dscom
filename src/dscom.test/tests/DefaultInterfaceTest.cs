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

public class DefaultInterfaceTest : BaseTest
{
    private const string InterfaceName = "DerivedInterface";
    private const string TestInterface = "TestInterface";
    private const string BaseInterface = "BaseInterface";

    [Fact]
    public void ClassWithClassAndComInterface_DefaultInterfaceIsClassInterface()
    {
        var result = CreateAssembly()
                .WithInterface(TestInterface)
                    .Build(out _)
                .WithClass("TestClass", new[] { TestInterface })
                .Build()
            .Build();

        var typeInfo = result.TypeLib.GetTypeInfoByName("TestClass");
        typeInfo.Should().NotBeNull();

        using var typeAttr = typeInfo!.GetTypeInfoAttributes();
        typeAttr!.Value.cImplTypes.Should().Be(2);

        typeInfo!.GetRefTypeOfImplType(0, out var href);
        typeInfo.GetRefTypeInfo(href, out var refTypeInfo);
        typeInfo.GetImplTypeFlags(0, out var pImplTypeFlags);
        refTypeInfo.GetDocumentation(-1, out var name, out _, out _, out _);

        name.Should().Be("_TestClass");
        pImplTypeFlags.Should().HaveFlag(IMPLTYPEFLAGS.IMPLTYPEFLAG_FDEFAULT);
    }

    [Fact]
    public void ClassWithClassAndComInterfaceDefaultIsComInterface_DefaultInterfaceIsComInterface()
    {
        var result = CreateAssembly()
                .WithInterface(TestInterface)
                    .Build(out var testInterfaceType)
                .WithClass("TestClass", new[] { TestInterface })
                .WithCustomAttribute<ComDefaultInterfaceAttribute>(testInterfaceType!)
                .Build()
            .Build();

        var typeInfo = result.TypeLib.GetTypeInfoByName("TestClass");
        typeInfo.Should().NotBeNull();

        using var typeAttr = typeInfo!.GetTypeInfoAttributes();
        typeAttr!.Value.cImplTypes.Should().Be(2);

        typeInfo!.GetRefTypeOfImplType(1, out var href);
        typeInfo.GetRefTypeInfo(href, out var refTypeInfo);
        typeInfo.GetImplTypeFlags(1, out var pImplTypeFlags);
        refTypeInfo.GetDocumentation(-1, out var name, out _, out _, out _);

        name.Should().Be(TestInterface);
        pImplTypeFlags.Should().HaveFlag(IMPLTYPEFLAGS.IMPLTYPEFLAG_FDEFAULT);
    }

    [Fact]
    public void ClassWithTwoComInterfaceAndNoDefaultAttribute_DefaultInterfaceIsFirstComInterface()
    {
        const string testInterface2 = "TestInterface2";
        var result = CreateAssembly()
                .WithInterface("TestInterface1")
                    .Build(out _)
                .WithInterface(testInterface2)
                    .Build(out _)
                .WithClass("TestClass", new[] { "TestInterface1", testInterface2 })
                .WithCustomAttribute<ClassInterfaceAttribute>(ClassInterfaceType.None)
                .Build()
            .Build();

        var typeInfo = result.TypeLib.GetTypeInfoByName("TestClass");
        typeInfo.Should().NotBeNull();

        using var typeAttr = typeInfo!.GetTypeInfoAttributes();
        typeAttr!.Value.cImplTypes.Should().Be(2);

        typeInfo!.GetRefTypeOfImplType(0, out var href);
        typeInfo.GetRefTypeInfo(href, out var refTypeInfo);
        typeInfo.GetImplTypeFlags(0, out var pImplTypeFlags);
        refTypeInfo.GetDocumentation(-1, out var name, out _, out _, out _);

        name.Should().Be("TestInterface1");
        pImplTypeFlags.Should().HaveFlag(IMPLTYPEFLAGS.IMPLTYPEFLAG_FDEFAULT);
    }

    [Fact]
    public void DerivedClassWithClassInterfaceFromClassWithDefaultAttribute_DefaultIsClassInterface()
    {
        var result = CreateAssembly()
                .WithInterface(BaseInterface)
                    .Build(out var baseInterfaceType)
                .WithClass("BaseClass", new[] { BaseInterface })
                .WithCustomAttribute<ComDefaultInterfaceAttribute>(baseInterfaceType!)
                .Build(out var baseClass)
                .WithClass("DerivedClass", Array.Empty<string>(), baseClass)
                .Build()
            .Build();

        var typeInfo = result.TypeLib.GetTypeInfoByName("DerivedClass");
        typeInfo.Should().NotBeNull();

        using var typeAttr = typeInfo!.GetTypeInfoAttributes();
        typeAttr!.Value.cImplTypes.Should().Be(2);

        typeInfo!.GetRefTypeOfImplType(0, out var href);
        typeInfo.GetRefTypeInfo(href, out var refTypeInfo);
        typeInfo.GetImplTypeFlags(0, out var pImplTypeFlags);
        refTypeInfo.GetDocumentation(-1, out var name, out _, out _, out _);

        name.Should().Be("_DerivedClass");
        pImplTypeFlags.Should().HaveFlag(IMPLTYPEFLAGS.IMPLTYPEFLAG_FDEFAULT);
    }

    [Fact]
    public void DerivedClassWithDefaultInterfaceFromBaseClass_DefualtIsBaseComInterface()
    {
        var result = CreateAssembly()
                .WithInterface(BaseInterface)
                    .Build(out var baseInterfaceType)
                .WithInterface(InterfaceName)
                    .Build(out _)
                .WithClass("BaseClass", new[] { BaseInterface })
                    .Build(out var baseClass)
                .WithClass("DerivedClass", new[] { InterfaceName }, baseClass)
                    .WithCustomAttribute<ComDefaultInterfaceAttribute>(baseInterfaceType!)
                .Build()
            .Build();

        var typeInfo = result.TypeLib.GetTypeInfoByName("DerivedClass");
        typeInfo.Should().NotBeNull();

        using var typeAttr = typeInfo!.GetTypeInfoAttributes();
        typeAttr!.Value.cImplTypes.Should().Be(3);

        typeInfo!.GetRefTypeOfImplType(1, out var href);
        typeInfo.GetRefTypeInfo(href, out var refTypeInfo);
        typeInfo.GetImplTypeFlags(1, out var pImplTypeFlags);
        refTypeInfo.GetDocumentation(-1, out var name, out _, out _, out _);

        name.Should().Be(BaseInterface);
        pImplTypeFlags.Should().HaveFlag(IMPLTYPEFLAGS.IMPLTYPEFLAG_FDEFAULT);
    }

    [Fact]
    public void DerivedClassWithComInterfaceWithoutClassInterfaceFromClassWithComInterface_DefaultIsDerivedComInterface()
    {
        var result = CreateAssembly()
                .WithInterface(BaseInterface)
                    .Build(out _)
                .WithInterface(InterfaceName)
                    .Build(out _)
                .WithClass("BaseClass", new[] { BaseInterface })
                    .Build(out var baseClass)
                .WithClass("DerivedClass", new[] { InterfaceName }, baseClass)
                    .WithCustomAttribute<ClassInterfaceAttribute>(ClassInterfaceType.None)
                    .Build()
            .Build();

        var typeInfo = result.TypeLib.GetTypeInfoByName("DerivedClass");
        typeInfo.Should().NotBeNull();

        using var typeAttr = typeInfo!.GetTypeInfoAttributes();
        typeAttr!.Value.cImplTypes.Should().Be(2);

        typeInfo!.GetRefTypeOfImplType(1, out var href);
        typeInfo.GetRefTypeInfo(href, out var refTypeInfo);
        typeInfo.GetImplTypeFlags(1, out var pImplTypeFlags);
        refTypeInfo.GetDocumentation(-1, out var name, out _, out _, out _);

        name.Should().Be(InterfaceName);
        pImplTypeFlags.Should().HaveFlag(IMPLTYPEFLAGS.IMPLTYPEFLAG_FDEFAULT);
    }
}
