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

public class EventTests : BaseTest
{
    private const string EventInterface1 = "EventInterface1";
    private const string EventInterface2 = "EventInterface2";

    [Fact]
    public void ClassWithSourceInterfacesAttribute_InterfaceIsDefaultSource()
    {
        var result = CreateAssembly().WithInterface(EventInterface1)
                .Build(out var interfaceType)
            .WithClass("CustomClass")
                .WithCustomAttribute<ComSourceInterfacesAttribute>(interfaceType!)
                .Build()
            .Build();

        var typeInfo = result.TypeLib.GetTypeInfoByName("CustomClass");
        Assert.NotNull(typeInfo);

        using var typeAttr = typeInfo!.GetTypeInfoAttributes();

        Assert.Equal(2, typeAttr!.Value.cImplTypes);

        typeInfo!.GetRefTypeOfImplType(1, out var href);
        typeInfo.GetRefTypeInfo(href, out var refTypeInfo);
        typeInfo.GetImplTypeFlags(1, out var pImplTypeFlags);
        refTypeInfo.GetDocumentation(-1, out var name, out _, out _, out _);

        Assert.Equal(EventInterface1, name);
        Assert.True(pImplTypeFlags.HasFlag(IMPLTYPEFLAGS.IMPLTYPEFLAG_FDEFAULT));
        Assert.True(pImplTypeFlags.HasFlag(IMPLTYPEFLAGS.IMPLTYPEFLAG_FSOURCE));
    }

    [Fact]
    public void ClassWithDuplicateSourceInterfacesAttribute_InterfaceIsOnceDefaultSource()
    {
        var result = CreateAssembly().WithInterface(EventInterface1)
                .Build(out var interfaceType)
            .WithClass("CustomClass")
                .WithCustomAttribute<ComSourceInterfacesAttribute>(interfaceType!, interfaceType!)
                .Build()
            .Build();

        var typeInfo = result.TypeLib.GetTypeInfoByName("CustomClass");
        Assert.NotNull(typeInfo);

        using var typeAttr = typeInfo!.GetTypeInfoAttributes();

        Assert.Equal(2, typeAttr!.Value.cImplTypes);

        typeInfo!.GetRefTypeOfImplType(1, out var href);
        typeInfo.GetRefTypeInfo(href, out var refTypeInfo);
        typeInfo.GetImplTypeFlags(1, out var pImplTypeFlags);
        refTypeInfo.GetDocumentation(-1, out var name, out _, out _, out _);

        Assert.Equal(EventInterface1, name);
        Assert.True(pImplTypeFlags.HasFlag(IMPLTYPEFLAGS.IMPLTYPEFLAG_FDEFAULT));
        Assert.True(pImplTypeFlags.HasFlag(IMPLTYPEFLAGS.IMPLTYPEFLAG_FSOURCE));
    }

    [Fact]
    public void ClassWithTwoSourceInterfacesAttribute_FirstInterfaceIsDefaultSourceSecondIsSource()
    {
        var result = CreateAssembly().WithInterface(EventInterface1)
                .Build(out var interfaceType1)
            .WithInterface(EventInterface2)
                .Build(out var interfaceType2)
            .WithClass("CustomClass")
                .WithCustomAttribute<ComSourceInterfacesAttribute>(interfaceType1!, interfaceType2!)
                .Build()
            .Build();

        var typeInfo = result.TypeLib.GetTypeInfoByName("CustomClass");
        Assert.NotNull(typeInfo);

        using var typeAttr = typeInfo!.GetTypeInfoAttributes();

        Assert.Equal(3, typeAttr!.Value.cImplTypes);

        typeInfo!.GetRefTypeOfImplType(1, out var href);
        typeInfo.GetRefTypeInfo(href, out var refTypeInfo);
        typeInfo.GetImplTypeFlags(1, out var pImplTypeFlags);
        refTypeInfo.GetDocumentation(-1, out var name, out _, out _, out _);

        Assert.Equal(EventInterface1, name);
        Assert.True(pImplTypeFlags.HasFlag(IMPLTYPEFLAGS.IMPLTYPEFLAG_FDEFAULT));
        Assert.True(pImplTypeFlags.HasFlag(IMPLTYPEFLAGS.IMPLTYPEFLAG_FSOURCE));

        typeInfo!.GetRefTypeOfImplType(2, out href);
        typeInfo.GetRefTypeInfo(href, out refTypeInfo);
        typeInfo.GetImplTypeFlags(2, out pImplTypeFlags);
        refTypeInfo.GetDocumentation(-1, out name, out _, out _, out _);

        Assert.Equal(EventInterface2, name);
        Assert.Equal(IMPLTYPEFLAGS.IMPLTYPEFLAG_FSOURCE, pImplTypeFlags);
    }

    [Fact]
    public void ClassWithInterfaceAndSourceInterfacesAttribute_InterfaceAppearsTwiceSecondIsDefaultSource()
    {
        var result = CreateAssembly().WithInterface(EventInterface1)
                .Build(out var interfaceType)
            .WithClass("CustomClass", new string[] { EventInterface1 })
                .WithCustomAttribute<ComSourceInterfacesAttribute>(interfaceType!)
                .Build()
            .Build();

        var typeInfo = result.TypeLib.GetTypeInfoByName("CustomClass");
        Assert.NotNull(typeInfo);

        using var typeAttr = typeInfo!.GetTypeInfoAttributes();

        Assert.Equal(3, typeAttr!.Value.cImplTypes);

        typeInfo!.GetRefTypeOfImplType(1, out var href);
        typeInfo.GetRefTypeInfo(href, out var refTypeInfo);
        typeInfo.GetImplTypeFlags(1, out var pImplTypeFlags);
        refTypeInfo.GetDocumentation(-1, out var name, out _, out _, out _);

        Assert.Equal(EventInterface1, name);
        Assert.Equal(0, (int)pImplTypeFlags);

        typeInfo!.GetRefTypeOfImplType(2, out href);
        typeInfo.GetRefTypeInfo(href, out refTypeInfo);
        typeInfo.GetImplTypeFlags(2, out pImplTypeFlags);
        refTypeInfo.GetDocumentation(-1, out name, out _, out _, out _);

        Assert.Equal(EventInterface1, name);
        Assert.True(pImplTypeFlags.HasFlag(IMPLTYPEFLAGS.IMPLTYPEFLAG_FDEFAULT));
        Assert.True(pImplTypeFlags.HasFlag(IMPLTYPEFLAGS.IMPLTYPEFLAG_FSOURCE));
    }

    [Fact]
    public void DerivedClassWithSourceInterfacesAttributeBaseClassWithSourceInterfaceAttribute_BothInterfacesSource()
    {
        var result = CreateAssembly().WithInterface(EventInterface1)
                .Build(out var interface1Type)
            .WithClass("CustomClass1")
                .WithCustomAttribute<ComSourceInterfacesAttribute>(interface1Type!)
                .Build(out var class1Type)
            .WithInterface(EventInterface2)
                .Build(out var interface2Type)
            .WithClass("CustomClass2", Array.Empty<string>(), class1Type)
                .WithCustomAttribute<ComSourceInterfacesAttribute>(interface2Type!)
                .Build()
            .Build();

        var typeInfo = result.TypeLib.GetTypeInfoByName("CustomClass2");
        Assert.NotNull(typeInfo);

        using var typeAttr = typeInfo!.GetTypeInfoAttributes();

        Assert.Equal(3, typeAttr!.Value.cImplTypes);

        typeInfo!.GetRefTypeOfImplType(1, out var href);
        typeInfo.GetRefTypeInfo(href, out var refTypeInfo);
        typeInfo.GetImplTypeFlags(1, out var pImplTypeFlags);
        refTypeInfo.GetDocumentation(-1, out var name, out _, out _, out _);

        Assert.Equal(EventInterface2, name);
        Assert.True(pImplTypeFlags.HasFlag(IMPLTYPEFLAGS.IMPLTYPEFLAG_FDEFAULT));
        Assert.True(pImplTypeFlags.HasFlag(IMPLTYPEFLAGS.IMPLTYPEFLAG_FSOURCE));

        typeInfo!.GetRefTypeOfImplType(2, out href);
        typeInfo.GetRefTypeInfo(href, out refTypeInfo);
        typeInfo.GetImplTypeFlags(2, out pImplTypeFlags);
        refTypeInfo.GetDocumentation(-1, out name, out _, out _, out _);

        Assert.Equal(EventInterface1, name);
        Assert.Equal(IMPLTYPEFLAGS.IMPLTYPEFLAG_FSOURCE, pImplTypeFlags);
    }
}
