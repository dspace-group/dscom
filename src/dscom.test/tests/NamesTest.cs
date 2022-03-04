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

public class NamesTest : BaseTest
{
    [Fact]
    public void EnumWithNameToChange_IsChanged()
    {
        var result = CreateAssembly()
            .WithEnum("touppercase", typeof(int))
                .WithLiteral("A", 1)
                .WithLiteral("B", 20)
                .WithLiteral("C", 50)
                .Build()
            .WithEnum("TOLOWERCASE", typeof(int))
                .WithLiteral("A", 1)
                .WithLiteral("B", 20)
                .WithLiteral("C", 50)
                .Build()
            .Build();

        var typeLibInfo = result.TypeLib.GetTypeInfoByName("touppercase");
        typeLibInfo.Should().BeNull();
        typeLibInfo = result.TypeLib.GetTypeInfoByName("TOUPPERCASE");
        typeLibInfo.Should().NotBeNull();
        var kv = typeLibInfo!.GetAllEnumValues();
        kv.Should().OnlyContain(z => z.Key.StartsWith("TOUPPERCASE"));

        typeLibInfo = result.TypeLib.GetTypeInfoByName("TOLOWERCASE");
        typeLibInfo.Should().BeNull();
        typeLibInfo = result.TypeLib.GetTypeInfoByName("tolowercase");
        typeLibInfo.Should().NotBeNull();
        kv = typeLibInfo!.GetAllEnumValues();
        kv.Should().OnlyContain(z => z.Key.StartsWith("tolowercase"));
    }

    [Fact]
    public void InterfaceWithNameToChange_IsChanged()
    {
        var result = CreateAssembly()
                        .WithInterface("touppercase")
                            .Build()
                        .WithInterface("TOLOWERCASE")
                            .Build()
                        .Build();

        var typeInfo = result.TypeLib.GetTypeInfoByName("touppercase");
        typeInfo.Should().BeNull("touppercase found");
        typeInfo = result.TypeLib.GetTypeInfoByName("TOUPPERCASE");
        typeInfo.Should().NotBeNull("TOUPPERCASE not found");

        typeInfo = result.TypeLib.GetTypeInfoByName("TOLOWERCASE");
        typeInfo.Should().BeNull("TOLOWERCASE case found");
        typeInfo = result.TypeLib.GetTypeInfoByName("tolowercase");
        typeInfo.Should().NotBeNull("tolowercase not found");
    }

    [Fact]
    public void InterfaceWithAComInvisibleMethodAndComVisibleMethodSameName_NameIsGeneratedWithoutSuffix()
    {
        var result = CreateAssembly()
                        .WithInterface("TestInterface")
                            .WithMethod("TestMethod")
                                .WithCustomAttribute<ComVisibleAttribute>(false)
                                .WithParameter<int>()
                            .Build()
                            .WithMethod("TestMethod")
                                .WithParameter<int>()
                                .WithParameter<int>()
                            .Build()
                            .WithMethod("TestMethod")
                                .WithCustomAttribute<ComVisibleAttribute>(false)
                                .WithParameter<int>()
                                .WithParameter<int>()
                                .WithParameter<int>()
                            .Build()
                            .WithMethod("TestMethod")
                                .WithParameter<int>()
                                .WithParameter<int>()
                                .WithParameter<int>()
                                .WithParameter<int>()
                            .Build()
                        .Build()
                    .Build();

        var typeInfo = result.TypeLib.GetTypeInfoByName("TestInterface");
        typeInfo.Should().NotBeNull();

        typeInfo!.GetFuncDescByName("TestMethod").Should().NotBeNull("TestMethod should be available");
        typeInfo!.GetFuncDescByName("TestMethod")!.Value.cParams.Should().Be(2);
        typeInfo!.GetFuncDescByName("TestMethod_2").Should().NotBeNull("TestMethod_2 should be available");
        typeInfo!.GetFuncDescByName("TestMethod_2")!.Value.cParams.Should().Be(4);
        typeInfo!.GetFuncDescByName("TestMethod_3").Should().BeNull("TestMethod_3 should not be available");
        typeInfo!.GetFuncDescByName("TestMethod_4").Should().BeNull("TestMethod_4 should not be available");
    }
}
