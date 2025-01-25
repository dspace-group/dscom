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
        Assert.NotNull(typeInfo);

        using var attributes = typeInfo!.GetTypeInfoAttributes();
        Assert.NotNull(attributes);
        Assert.Equal(TYPEKIND.TKIND_DISPATCH, attributes!.Value.typekind);

        // Check that no unexpected warnings occurred 
        Assert.All(result.TypeLibExporterNotifySink.ReportedEvents, x => Assert.Equal(ExporterEventKind.NOTIF_TYPECONVERTED, x.EventKind));
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
        Assert.NotNull(typeInfo);

        using var attributes = typeInfo!.GetTypeInfoAttributes();
        Assert.NotNull(attributes);
        Assert.Equal(TYPEKIND.TKIND_DISPATCH, attributes!.Value.typekind);

        // Check that no unexpected warnings occurred 
        Assert.All(result.TypeLibExporterNotifySink.ReportedEvents, x => Assert.Equal(ExporterEventKind.NOTIF_TYPECONVERTED, x.EventKind));
    }

    [Fact]
    public void InterfaceWithout_ComInterfaceType_IsTKIND_DISPATCH()
    {
        var result = CreateAssembly()
                        .WithInterface("TestInterface")
                            .Build()
                        .Build();

        var typeInfo = result.TypeLib.GetTypeInfoByName("TestInterface");
        Assert.NotNull(typeInfo);

        using var attributes = typeInfo!.GetTypeInfoAttributes();
        Assert.NotNull(attributes);
        Assert.Equal(TYPEKIND.TKIND_DISPATCH, attributes!.Value.typekind);

        // Check that no unexpected warnings occurred 
        Assert.All(result.TypeLibExporterNotifySink.ReportedEvents, x => Assert.Equal(ExporterEventKind.NOTIF_TYPECONVERTED, x.EventKind));
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
        Assert.NotNull(typeInfo);
        typeInfo!.GetDocumentation(-1, out _, out var strDocString, out _, out _);
        Assert.NotNull(strDocString);
        Assert.Equal("Description", strDocString);

        // Check that no unexpected warnings occurred 
        Assert.All(result.TypeLibExporterNotifySink.ReportedEvents, x => Assert.Equal(ExporterEventKind.NOTIF_TYPECONVERTED, x.EventKind));
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
        Assert.NotNull(typeInfo);

        using var attributes = typeInfo!.GetTypeInfoAttributes();
        Assert.NotNull(attributes);
        Assert.Equal(TYPEFLAGS.TYPEFLAG_FDISPATCHABLE, attributes!.Value.wTypeFlags);

        // Check that no unexpected warnings occurred 
        Assert.All(result.TypeLibExporterNotifySink.ReportedEvents, x => Assert.Equal(ExporterEventKind.NOTIF_TYPECONVERTED, x.EventKind));
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
        Assert.NotNull(typeInfo);

        using var attributes = typeInfo!.GetTypeInfoAttributes();
        Assert.NotNull(attributes);
        Assert.Equal(TYPEFLAGS.TYPEFLAG_FDISPATCHABLE | TYPEFLAGS.TYPEFLAG_FDUAL, attributes!.Value.wTypeFlags);

        // Check that no unexpected warnings occurred 
        Assert.All(result.TypeLibExporterNotifySink.ReportedEvents, x => Assert.Equal(ExporterEventKind.NOTIF_TYPECONVERTED, x.EventKind));
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

        Assert.NotNull(typeInfo);

        typeInfo!.GetRefTypeOfImplType(0, out var href);
        typeInfo!.GetRefTypeInfo(href, out var ppTI);
        ppTI.GetDocumentation(-1, out var refTypeName, out _, out _, out _);

        Assert.Equal(expectedTypeString, refTypeName);

        // Check that no unexpected warnings occurred 
        Assert.All(result.TypeLibExporterNotifySink.ReportedEvents, x => Assert.Equal(ExporterEventKind.NOTIF_TYPECONVERTED, x.EventKind));
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

        Assert.NotNull(result.TypeLib.GetTypeInfoByName("Namespace1_TestInterface"));
        Assert.NotNull(result.TypeLib.GetTypeInfoByName("Namespace2_TestInterface"));
        Assert.Null(result.TypeLib.GetTypeInfoByName("TestInterface"));

        // Check that no unexpected warnings occurred 
        Assert.All(result.TypeLibExporterNotifySink.ReportedEvents, x => Assert.Equal(ExporterEventKind.NOTIF_TYPECONVERTED, x.EventKind));
    }
}
