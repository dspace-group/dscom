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

namespace dSPACE.Runtime.InteropServices.Tests;

public class InterfaceTest : BaseTest
{
    [Fact]
    public void InterfaceIsDual_TYPEKIND_IsTKIND_INTERFACE()
    {
        var result = CreateAssembly()
                        .WithInterface("TestInterface")
                        .WithCustomAttribute<InterfaceTypeAttribute>(ComInterfaceType.InterfaceIsDual)
                            .Build()
                        .Build();

        var typeInfo = result.TypeLib.GetTypeInfoByName("TestInterface");
        typeInfo.Should().NotBeNull("TestInterface not found");

        using var attributes = typeInfo!.GetTypeInfoAttributes();
        attributes.Should().NotBeNull();
        attributes!.Value.typekind.Should().Be(TYPEKIND.TKIND_DISPATCH);
    }

    [Fact]
    public void InterfaceIsDispatch_TYPEKIND_IsTKIND_DISPATCH()
    {
        var result = CreateAssembly()
                        .WithInterface("TestInterface")
                         .WithCustomAttribute<InterfaceTypeAttribute>(ComInterfaceType.InterfaceIsIDispatch)
                            .Build()
                        .Build();

        var typeInfo = result.TypeLib.GetTypeInfoByName("TestInterface");
        typeInfo.Should().NotBeNull("TestInterface not found");

        using var attributes = typeInfo!.GetTypeInfoAttributes();
        attributes.Should().NotBeNull();
        attributes!.Value.typekind.Should().Be(TYPEKIND.TKIND_DISPATCH);
    }

    [Fact]
    public void InterfaceWithout_ComInterfaceType_IsTKIND_DISPATCH()
    {
        var result = CreateAssembly()
                        .WithInterface("TestInterface")
                            .Build()
                        .Build();

        var typeInfo = result.TypeLib.GetTypeInfoByName("TestInterface");
        typeInfo.Should().NotBeNull("TestInterface not found");

        using var attributes = typeInfo!.GetTypeInfoAttributes();
        attributes.Should().NotBeNull();
        attributes!.Value.typekind.Should().Be(TYPEKIND.TKIND_DISPATCH);
    }

    [Fact]
    public void InterfaceWithDescriptionAttribute_HasDocString()
    {
        var result = CreateAssembly()
                        .WithInterface("TestInterface")
                        .WithCustomAttribute(typeof(System.ComponentModel.DescriptionAttribute), "Description")
                            .Build()
                        .Build();

        var typeInfo = result.TypeLib.GetTypeInfoByName("TestInterface");
        typeInfo.Should().NotBeNull("TestInterface not found");
        typeInfo!.GetDocumentation(-1, out _, out var strDocString, out _, out _);
        strDocString.Should().NotBeNull();
        strDocString.Should().BeEquivalentTo("Description");

    }

    [Fact]
    public void InterfaceWith_NameToChange_Ischanged()
    {
        var result = CreateAssembly()
                        .WithInterface("touppercase")
                            .Build()
                        .WithInterface("TOLOWERCASE")
                            .Build()
                        .Build();

        var typeInfo = result.TypeLib.GetTypeInfoByName("touppercase");
        typeInfo.Should().BeNull("toupper case found");
        typeInfo = result.TypeLib.GetTypeInfoByName("TOUPPERCASE");
        typeInfo.Should().NotBeNull("TOUPPERCASE not found");

        typeInfo = result.TypeLib.GetTypeInfoByName("TOLOWERCASE");
        typeInfo.Should().BeNull("TOLOWERCASE case found");
        typeInfo = result.TypeLib.GetTypeInfoByName("tolowercase");
        typeInfo.Should().NotBeNull("tolowercase not found");
    }

    [Fact]
    public void InterfaceIsIDispatch_TYPEFLAGS_IsFDISPATCHABLE()
    {
        var result = CreateAssembly()
                        .WithInterface("TestInterface")
                        .WithCustomAttribute<InterfaceTypeAttribute>(ComInterfaceType.InterfaceIsIDispatch)
                            .Build()
                        .Build();

        var typeInfo = result.TypeLib.GetTypeInfoByName("TestInterface");
        typeInfo.Should().NotBeNull("TestInterface not found");

        using var attributes = typeInfo!.GetTypeInfoAttributes();
        attributes.Should().NotBeNull();
        attributes!.Value.wTypeFlags.Should().Be(TYPEFLAGS.TYPEFLAG_FDISPATCHABLE);
    }

    [Fact]
    public void InterfaceIsDual_TYPEFLAGS_IsFDISPATCHABLE_AND_FDUAL()
    {
        var result = CreateAssembly()
                        .WithInterface("TestInterface")
                        .WithCustomAttribute<InterfaceTypeAttribute>(ComInterfaceType.InterfaceIsDual)
                            .Build()
                        .Build();

        var typeInfo = result.TypeLib.GetTypeInfoByName("TestInterface");
        typeInfo.Should().NotBeNull("TestInterface not found");

        using var attributes = typeInfo!.GetTypeInfoAttributes();
        attributes.Should().NotBeNull();
        attributes!.Value.wTypeFlags.Should().Be(TYPEFLAGS.TYPEFLAG_FDISPATCHABLE | TYPEFLAGS.TYPEFLAG_FDUAL);
    }

    [Fact]
    public void InterfaceIsInspectable_throws()
    {
        Assert.Throws<NotSupportedException>
        (() => CreateAssembly()
                        .WithInterface("TestInterface")
                        .WithCustomAttribute<InterfaceTypeAttribute>(ComInterfaceType.InterfaceIsIInspectable)
                            .Build()
                        .Build());
    }

    [Theory]
    [InlineData(ComInterfaceType.InterfaceIsIDispatch, "IDispatch")]
    [InlineData(ComInterfaceType.InterfaceIsDual, "IDispatch")]
    [InlineData(ComInterfaceType.InterfaceIsIUnknown, "IUnknown")]
    public void InterfaceIsIDispatchPrDual_BaseInterface_IsDispatch(ComInterfaceType interfaceType, string expectedTypeString)
    {
        var result = CreateAssembly(CreateAssemblyName(assemblyNameSuffix: $"{interfaceType}{expectedTypeString}"))
                        .WithInterface("TestInterface")
                         .WithCustomAttribute<InterfaceTypeAttribute>(interfaceType)
                            .Build()
                        .Build();

        var typeInfo = result.TypeLib.GetTypeInfoByName("TestInterface");

        typeInfo.Should().NotBeNull("TestInterface not found");

        typeInfo!.GetRefTypeOfImplType(0, out var href);
        typeInfo!.GetRefTypeInfo(href, out var ppTI);
        ppTI.GetDocumentation(-1, out var refTypeName, out _, out _, out _);

        refTypeName.Should().Be(expectedTypeString);
    }

    [Fact]
    public void InterfacesWithSameName_NamespacesAreUsedToGenerateTypeName()
    {
        var result = CreateAssembly()
                        .WithInterface("TestInterface")
                            .WithNamespace("Namespace1")
                        .Build()
                        .WithInterface("TestInterface")
                            .WithNamespace("Namespace2")
                        .Build()
                    .Build();

        result.TypeLib.GetTypeInfoByName("Namespace1_TestInterface").Should().NotBeNull();
        result.TypeLib.GetTypeInfoByName("Namespace2_TestInterface").Should().NotBeNull();

        result.TypeLib.GetTypeInfoByName("TestInterface").Should().BeNull();
    }
}
