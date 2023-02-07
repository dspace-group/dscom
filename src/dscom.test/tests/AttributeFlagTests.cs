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

using dSPACE.Runtime.InteropServices.Attributes;
using FUNCFLAGS = System.Runtime.InteropServices.ComTypes.FUNCFLAGS;

namespace dSPACE.Runtime.InteropServices.Tests;

public class AttributeFlagTests : BaseTest
{
    [Theory]
    [InlineData(null, false)]
    [InlineData(true, true)]
    [InlineData(false, false)]
    public void HiddenAttributes_Considered(bool? interfacesHiddenAttribute, bool interfaceIsHidden)
    {
        var builder = CreateAssembly(CreateAssemblyName(assemblyNameSuffix: $"{interfacesHiddenAttribute}{interfaceIsHidden}"));
        var typebuilder = builder.WithInterface("TestInterface");

        if (interfacesHiddenAttribute != null)
        {
            typebuilder.WithCustomAttribute(typeof(HiddenAttribute), interfacesHiddenAttribute.Value);
        }

        var result = typebuilder.Build().Build();
        var typeInfo = result.TypeLib.GetTypeInfoByName("TestInterface");
        var flags = typeInfo?.GetTypeInfoAttributes()?.Value.wTypeFlags;

        if (interfaceIsHidden)
        {
            flags.Should().HaveFlag(TYPEFLAGS.TYPEFLAG_FHIDDEN);
        }
        else
        {
            flags.Should().NotHaveFlag(TYPEFLAGS.TYPEFLAG_FHIDDEN);
        }
    }

    [Theory]
    [InlineData(null, false)]
    [InlineData(true, true)]
    [InlineData(false, false)]
    public void HiddenMemberAttributes_Considered(bool? memberHdden, bool memberIsHidden)
    {
        var builder = CreateAssembly(CreateAssemblyName(assemblyNameSuffix: $"{memberHdden}{memberIsHidden}"));
        var typebuilder = builder.WithInterface("TestInterface");
        var methodBuilder = typebuilder.WithMethod("TestMethod");

        if (memberHdden != null)
        {
            methodBuilder.WithCustomAttribute(typeof(HiddenMemberAttribute), memberHdden.Value);
        }

        var result = methodBuilder.Build().Build().Build();
        var funcInfo = result.TypeLib.GetTypeInfoByName("TestInterface")?.GetFuncDescByName("TestMethod");
        var flags = (FUNCFLAGS)(funcInfo?.Value.wFuncFlags ?? 0);

        if (memberIsHidden)
        {
            flags.Should().HaveFlag(FUNCFLAGS.FUNCFLAG_FHIDDEN);
        }
        else
        {
            flags.Should().NotHaveFlag(FUNCFLAGS.FUNCFLAG_FHIDDEN);
        }
    }

    [Theory]
    [InlineData(null, false)]
    [InlineData(true, true)]
    [InlineData(false, false)]
    public void RestrictedAttributes_Considered(bool? interfacesRestrictedAttribute, bool interfaceIsRestricted)
    {
        var builder = CreateAssembly(CreateAssemblyName(assemblyNameSuffix: $"{interfacesRestrictedAttribute}{interfaceIsRestricted}"));
        var typebuilder = builder.WithInterface("TestInterface");

        if (interfacesRestrictedAttribute != null)
        {
            typebuilder.WithCustomAttribute(typeof(RestrictedAttribute), interfacesRestrictedAttribute.Value);
        }

        var result = typebuilder.Build().Build();
        var typeInfo = result.TypeLib.GetTypeInfoByName("TestInterface");
        var flags = typeInfo?.GetTypeInfoAttributes()?.Value.wTypeFlags;

        if (interfaceIsRestricted)
        {
            flags.Should().HaveFlag(TYPEFLAGS.TYPEFLAG_FRESTRICTED);
        }
        else
        {
            flags.Should().NotHaveFlag(TYPEFLAGS.TYPEFLAG_FRESTRICTED);
        }
    }

    [Theory]
    [InlineData(null, false)]
    [InlineData(true, true)]
    [InlineData(false, false)]
    public void RestrictedMemberAttributes_Considered(bool? memberHdden, bool memberIsRestricted)
    {
        var builder = CreateAssembly(CreateAssemblyName(assemblyNameSuffix: $"{memberHdden}{memberIsRestricted}"));
        var typebuilder = builder.WithInterface("TestInterface");
        var methodBuilder = typebuilder.WithMethod("TestMethod");

        if (memberHdden != null)
        {
            methodBuilder.WithCustomAttribute(typeof(RestrictedMemberAttribute), memberHdden.Value);
        }

        var result = methodBuilder.Build().Build().Build();
        var funcInfo = result.TypeLib.GetTypeInfoByName("TestInterface")?.GetFuncDescByName("TestMethod");
        var flags = (FUNCFLAGS)(funcInfo?.Value.wFuncFlags ?? 0);

        if (memberIsRestricted)
        {
            flags.Should().HaveFlag(FUNCFLAGS.FUNCFLAG_FRESTRICTED);
        }
        else
        {
            flags.Should().NotHaveFlag(FUNCFLAGS.FUNCFLAG_FRESTRICTED);
        }
    }
}
