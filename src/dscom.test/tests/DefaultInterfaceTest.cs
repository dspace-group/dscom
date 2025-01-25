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
using Xunit;

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
        Assert.NotNull(typeInfo);

        using var typeAttr = typeInfo!.GetTypeInfoAttributes();
        Assert.Equal(2, typeAttr!.Value.cImplTypes);

        typeInfo!.GetRefTypeOfImplType(0, out var href);
        typeInfo.GetRefTypeInfo(href, out var refTypeInfo);
        typeInfo.GetImplTypeFlags(0, out var pImplTypeFlags);
        refTypeInfo.GetDocumentation(-1, out var name, out _, out _, out _);

        Assert.Equal("_TestClass", name);
        Assert.True(pImplTypeFlags.HasFlag(IMPLTYPEFLAGS.IMPLTYPEFLAG_FDEFAULT));
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
        Assert.NotNull(typeInfo);

        using var typeAttr = typeInfo!.GetTypeInfoAttributes();
        Assert.Equal(2, typeAttr!.Value.cImplTypes);

        typeInfo!.GetRefTypeOfImplType(1, out var href);
        typeInfo.GetRefTypeInfo(href, out var refTypeInfo);
        typeInfo.GetImplTypeFlags(1, out var pImplTypeFlags);
        refTypeInfo.GetDocumentation(-1, out var name, out _, out _, out _);

        Assert.Equal(TestInterface, name);
        Assert.True(pImplTypeFlags.HasFlag(IMPLTYPEFLAGS.IMPLTYPEFLAG_FDEFAULT));
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
        Assert.NotNull(typeInfo);

        using var typeAttr = typeInfo!.GetTypeInfoAttributes();
        Assert.Equal(2, typeAttr!.Value.cImplTypes);

        typeInfo!.GetRefTypeOfImplType(0, out var href);
        typeInfo.GetRefTypeInfo(href, out var refTypeInfo);
        typeInfo.GetImplTypeFlags(0, out var pImplTypeFlags);
        refTypeInfo.GetDocumentation(-1, out var name, out _, out _, out _);

        Assert.Equal("TestInterface1", name);
        Assert.True(pImplTypeFlags.HasFlag(IMPLTYPEFLAGS.IMPLTYPEFLAG_FDEFAULT));
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
        Assert.NotNull(typeInfo);

        using var typeAttr = typeInfo!.GetTypeInfoAttributes();
        Assert.Equal(2, typeAttr!.Value.cImplTypes);

        typeInfo!.GetRefTypeOfImplType(0, out var href);
        typeInfo.GetRefTypeInfo(href, out var refTypeInfo);
        typeInfo.GetImplTypeFlags(0, out var pImplTypeFlags);
        refTypeInfo.GetDocumentation(-1, out var name, out _, out _, out _);

        Assert.Equal("_DerivedClass", name);
        Assert.True(pImplTypeFlags.HasFlag(IMPLTYPEFLAGS.IMPLTYPEFLAG_FDEFAULT));
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
        Assert.NotNull(typeInfo);

        using var typeAttr = typeInfo!.GetTypeInfoAttributes();
        Assert.Equal(3, typeAttr!.Value.cImplTypes);

        typeInfo!.GetRefTypeOfImplType(1, out var href);
        typeInfo.GetRefTypeInfo(href, out var refTypeInfo);
        typeInfo.GetImplTypeFlags(1, out var pImplTypeFlags);
        refTypeInfo.GetDocumentation(-1, out var name, out _, out _, out _);

        Assert.Equal(BaseInterface, name);
        Assert.True(pImplTypeFlags.HasFlag(IMPLTYPEFLAGS.IMPLTYPEFLAG_FDEFAULT));
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
        Assert.NotNull(typeInfo);

        using var typeAttr = typeInfo!.GetTypeInfoAttributes();
        Assert.Equal(2, typeAttr!.Value.cImplTypes);

        typeInfo!.GetRefTypeOfImplType(1, out var href);
        typeInfo.GetRefTypeInfo(href, out var refTypeInfo);
        typeInfo.GetImplTypeFlags(1, out var pImplTypeFlags);
        refTypeInfo.GetDocumentation(-1, out var name, out _, out _, out _);

        Assert.Equal(InterfaceName, name);
        Assert.True(pImplTypeFlags.HasFlag(IMPLTYPEFLAGS.IMPLTYPEFLAG_FDEFAULT));
    }
}
