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
        typeLibInfo.Should().NotBeNull();

        var kv = typeLibInfo!.GetAllEnumValues();

        kv.Should().Contain(new KeyValuePair<string, object>("TestEnum_A", 1));
        kv.Should().Contain(new KeyValuePair<string, object>("TestEnum_B", 20));
        kv.Should().Contain(new KeyValuePair<string, object>("TestEnum_C", 50));
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
        typeLibInfo.Should().BeNull();
    }

    [Fact]
    public void TwoEnumsWithTheSameNameInDifferentNamespaces_EnumFieldsShouldUseNamespaceAsPrefix()
    {
        var result = CreateAssembly()
            .WithEnum<int>("TestEnum").WithNamespace("dspace.test.namspace1")
                .WithLiteral("A", 1)
                .WithLiteral("B", 2)
                .Build()
            .WithEnum<int>("TestEnum").WithNamespace("dspace.test.namspace2")
                .WithLiteral("A", 1)
                .WithLiteral("B", 2)
                .Build()
            .Build();

        var typeLibInfo = result.TypeLib.GetTypeInfoByName("dspace_test_namspace1_TestEnum");
        typeLibInfo.Should().NotBeNull();
        var kv = typeLibInfo!.GetAllEnumValues();
        kv.ToList().Select(kv => kv.Key).Should().Contain("dspace_test_namspace1_TestEnum_A");
        kv.ToList().Select(kv => kv.Key).Should().Contain("dspace_test_namspace1_TestEnum_B");
        kv.ToList().Select(kv => kv.Key).Should().NotContain("TestEnum_A");
        kv.ToList().Select(kv => kv.Key).Should().NotContain("TestEnum_B");

        typeLibInfo = result.TypeLib.GetTypeInfoByName("dspace_test_namspace2_TestEnum");
        typeLibInfo.Should().NotBeNull();
        kv = typeLibInfo!.GetAllEnumValues();
        kv.ToList().Select(kv => kv.Key).Should().Contain("dspace_test_namspace2_TestEnum_A");
        kv.ToList().Select(kv => kv.Key).Should().Contain("dspace_test_namspace2_TestEnum_B");
        kv.ToList().Select(kv => kv.Key).Should().NotContain("TestEnum_A");
        kv.ToList().Select(kv => kv.Key).Should().NotContain("TestEnum_B");
    }
}
