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

using System.ComponentModel;
using System.Runtime.InteropServices;
using Xunit;

namespace dSPACE.Runtime.InteropServices.Tests;

public class EnumTest : BaseTest
{
    [Theory]
    [InlineData(typeof(byte))]
    [InlineData(typeof(sbyte))]
    [InlineData(typeof(short))]
    [InlineData(typeof(ushort))]
    [InlineData(typeof(uint))]
    [InlineData(typeof(int))]
    [InlineData(typeof(ulong))]
    [InlineData(typeof(long))]
    public void EnumWithNumericValues_Available(Type type)
    {
        var result = CreateAssembly(CreateAssemblyName(assemblyNameSuffix: $"{type}"))
            .WithEnum("TestEnum", type)
                .WithLiteral("A", 1)
                .WithLiteral("B", 20)
                .WithLiteral("C", 50)
                .Build()
            .Build();

        var typeLibInfo = result.TypeLib.GetTypeInfoByName("TestEnum");
        Assert.NotNull(typeLibInfo);

        var kv = typeLibInfo!.GetAllEnumValues();

        Assert.Contains(new KeyValuePair<string, object>("TestEnum_A", 1), kv);
        Assert.Contains(new KeyValuePair<string, object>("TestEnum_B", 20), kv);
        Assert.Contains(new KeyValuePair<string, object>("TestEnum_C", 50), kv);
    }

    [Fact]
    public void EnumComVisibleFalse_EnumIsNotGenerated()
    {
        var result = CreateAssembly()
            .WithEnum<int>("TestEnum")
                .WithCustomAttribute<ComVisibleAttribute>(false)
                .WithLiteral("A", 1)
                .WithLiteral("B", 20)
                .WithLiteral("C", 50)
                .Build()
            .Build();

        var typeLibInfo = result.TypeLib.GetTypeInfoByName("TestEnum");
        Assert.Null(typeLibInfo);
    }

    [Fact]
    public void TwoEnumsWithTheSameNameInDifferentNamespaces_EnumFieldsShouldUseNamespaceAsPrefix()
    {
        var result = CreateAssembly()
            .WithEnum<int>("TestEnum").WithNamespace("dspace.test.namespace1")
                .WithLiteral("A", 1)
                .WithLiteral("B", 2)
                .Build()
            .WithEnum<int>("TestEnum").WithNamespace("dspace.test.namespace2")
                .WithLiteral("A", 1)
                .WithLiteral("B", 2)
                .Build()
            .Build();

        var typeLibInfo = result.TypeLib.GetTypeInfoByName("dspace_test_namespace1_TestEnum");
        Assert.NotNull(typeLibInfo);
        var kv = typeLibInfo!.GetAllEnumValues();
        Assert.Contains("dspace_test_namespace1_TestEnum_A", kv.ToList().Select(kv => kv.Key));
        Assert.Contains("dspace_test_namespace1_TestEnum_B", kv.ToList().Select(kv => kv.Key));
        Assert.DoesNotContain("TestEnum_A", kv.ToList().Select(kv => kv.Key));
        Assert.DoesNotContain("TestEnum_B", kv.ToList().Select(kv => kv.Key));

        typeLibInfo = result.TypeLib.GetTypeInfoByName("dspace_test_namespace2_TestEnum");
        Assert.NotNull(typeLibInfo);
        kv = typeLibInfo!.GetAllEnumValues();
        Assert.Contains("dspace_test_namespace2_TestEnum_A", kv.ToList().Select(kv => kv.Key));
        Assert.Contains("dspace_test_namespace2_TestEnum_B", kv.ToList().Select(kv => kv.Key));
        Assert.DoesNotContain("TestEnum_A", kv.ToList().Select(kv => kv.Key));
        Assert.DoesNotContain("TestEnum_B", kv.ToList().Select(kv => kv.Key));
    }

    [Fact]
    public void EnumsValuesWithDescription_EnumValueHasDocString()
    {
        var result = CreateAssembly()
            .WithEnum<int>("TestEnum").WithNamespace("dspace.test.namespace1")
                .WithLiteralAndAttribute("A", 1, typeof(DescriptionAttribute), new Type[] { typeof(string) }, new object[] { "TestDescription_A" })
                .WithLiteralAndAttribute("B", 1, typeof(DescriptionAttribute), new Type[] { typeof(string) }, new object[] { "TestDescription_B" })
                .Build()
            .Build();

        var typeInfo = result.TypeLib.GetTypeInfoByName("TestEnum");
        Assert.NotNull(typeInfo);

        using var attribute = typeInfo!.GetTypeInfoAttributes();
        Assert.NotNull(attribute);
        var count = attribute!.Value.cVars;
        Assert.Equal(2, count);
        typeInfo!.GetVarDesc(0, out var ppVarDesc0);
        try
        {
            var varDesc = Marshal.PtrToStructure<VARDESC>(ppVarDesc0);
            Assert.Equal(VARKIND.VAR_CONST, varDesc.varkind);
            typeInfo!.GetDocumentation(varDesc.memid, out _, out var strDocString, out _, out _);
            Assert.Equal("TestDescription_A", strDocString);
        }
        finally
        {
            typeInfo.ReleaseVarDesc(ppVarDesc0);
        }

        typeInfo!.GetVarDesc(1, out var ppVarDesc1);
        try
        {
            var varDesc = Marshal.PtrToStructure<VARDESC>(ppVarDesc1);
            Assert.Equal(VARKIND.VAR_CONST, varDesc.varkind);
            typeInfo!.GetDocumentation(varDesc.memid, out _, out var strDocString, out _, out _);
            Assert.Equal("TestDescription_B", strDocString);
        }
        finally
        {
            typeInfo.ReleaseVarDesc(ppVarDesc1);
        }
    }
}

