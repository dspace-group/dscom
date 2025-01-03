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
using dSPACE.Runtime.InteropServices.Attributes;

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
    public void EnumWithAlias_IsChanged()
    {
        var result = CreateAssembly()
            .WithEnum("touppercase", typeof(int))
                .WithCustomAttribute<ComVisibleAttribute>(true)
                .WithCustomAttribute<ComAliasAttribute>("froofroo")
                .WithLiteral("A", 1)
                .WithLiteral("B", 20)
                .WithLiteral("C", 50)
                .Build()
            .Build(useComAlias: true);

        var typeLibInfo = result.TypeLib.GetTypeInfoByName("touppercase");
        typeLibInfo.Should().BeNull();
        typeLibInfo = result.TypeLib.GetTypeInfoByName("froofroo");
        typeLibInfo.Should().NotBeNull();
        var kv = typeLibInfo!.GetAllEnumValues();
        kv.Should().OnlyContain(z => z.Key.StartsWith("froofroo"));
    }

    [Fact]
    public void EnumWithAliasBothLevels_AreIndependent()
    {
        var result = CreateAssembly()
            .WithEnum("touppercase", typeof(int))
                .WithCustomAttribute<ComVisibleAttribute>(true)
                .WithCustomAttribute<ComAliasAttribute>("froofroo")
                .WithLiteralAndAttribute("A", 1, typeof(ComAliasAttribute), new Type[] { typeof(string) }, new object[] { "fizz" })
                .WithLiteralAndAttribute("B", 20, typeof(ComAliasAttribute), new Type[] { typeof(string) }, new object[] { "buzz" })
                .WithLiteralAndAttribute("C", 50, typeof(ComAliasAttribute), new Type[] { typeof(string) }, new object[] { "fizzbuzz" })
                .Build()
            .Build(useComAlias: true);

        var typeLibInfo = result.TypeLib.GetTypeInfoByName("touppercase");
        typeLibInfo.Should().BeNull();
        typeLibInfo = result.TypeLib.GetTypeInfoByName("froofroo");
        typeLibInfo.Should().NotBeNull();
        var kv = typeLibInfo!.GetAllEnumValues();
        kv.Should().OnlyContain(z => z.Key == "fizz" || z.Key == "buzz" || z.Key == "fizzbuzz");
    }

    [Fact]
    public void EnumReferences_AreAliasedCorectly()
    {
        var result = CreateAssembly()
            .WithEnum("touppercase", typeof(int))
                .WithCustomAttribute<ComVisibleAttribute>(true)
                .WithCustomAttribute<ComAliasAttribute>("froofroo")
                .WithLiteralAndAttribute("A", 1, typeof(ComAliasAttribute), new Type[] { typeof(string) }, new object[] { "fizz" })
                .WithLiteralAndAttribute("B", 20, typeof(ComAliasAttribute), new Type[] { typeof(string) }, new object[] { "buzz" })
                .WithLiteralAndAttribute("C", 50, typeof(ComAliasAttribute), new Type[] { typeof(string) }, new object[] { "fizzbuzz" })
                .Build(out var enumType)
            .WithInterface("_enumAlias")
                .WithMethod("getFruit")
                    .WithParameter(new ParameterItem(enumType!, null, new DefaultValue(20)))
                    .WithReturnType(enumType!)
                    .Build()
                .Build()
            .Build(useComAlias: true);

        var typeLibInfo = result.TypeLib.GetTypeInfoByName("_enumAlias");
        typeLibInfo.Should().NotBeNull();

        var funcDescValue = typeLibInfo!.GetFuncDescByName("getFruit");
        funcDescValue!.Value.Should().NotBeNull();
        var funcDesc = funcDescValue.Value;

        var elemDescParam = funcDesc.GetParameter(0)!.Value;
        var paramDesc = elemDescParam.desc.paramdesc;
        paramDesc.Should().NotBeNull();

        // the lpVarValue points to a PARAMDESCEX, but we don't care about the size, so dereference the VARIANTARG
        // structure directly at the offset'd address.
        var varValue = Marshal.GetObjectForNativeVariant(paramDesc.lpVarValue + sizeof(ulong));
        varValue.Should().Be(20);

        typeLibInfo!.GetRefTypeInfo(funcDesc.elemdescFunc.tdesc.lpValue.ExtractInt32(), out var returnType);
        returnType.GetName().Should().Be("froofroo");

        typeLibInfo.GetRefTypeInfo(elemDescParam.tdesc.lpValue.ExtractInt32(), out var paramType);
        paramType.GetName().Should().Be("froofroo");
    }

    [Fact]
    public void EnumWithMemberAlias_IsChanged()
    {
        var result = CreateAssembly()
            .WithEnum("touppercase", typeof(int))
                .WithCustomAttribute<ComVisibleAttribute>(true)
                .WithLiteralAndAttribute("A", 1, typeof(ComAliasAttribute), new Type[] { typeof(string) }, new object[] { "fizz" })
                .WithLiteralAndAttribute("B", 20, typeof(ComAliasAttribute), new Type[] { typeof(string) }, new object[] { "buzz" })
                .WithLiteralAndAttribute("C", 50, typeof(ComAliasAttribute), new Type[] { typeof(string) }, new object[] { "fizzbuzz" })
                .Build()
            .Build(useComAlias: true);

        var typeLibInfo = result.TypeLib.GetTypeInfoByName("TOUPPERCASE");
        typeLibInfo.Should().BeNull();
        typeLibInfo = result.TypeLib.GetTypeInfoByName("touppercase");
        typeLibInfo.Should().NotBeNull();
        var kv = typeLibInfo!.GetAllEnumValues();
        kv.Should().OnlyContain(z => z.Key == "fizz" || z.Key == "buzz" || z.Key == "fizzbuzz");
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

    [Fact]
    public void InterfaceWithInvalidMarshalAsParameter_NameSuffixCounts()
    {
        var result = CreateAssembly()
                        .WithInterface("TestInterface")
                            .WithMethod("TestMethod")
                                .WithParameter<bool>()
                            .Build()
                            .WithMethod("TestMethod")
                                .WithParameter<bool>()
                                .WithParameter<bool>()
                                .WithParameterCustomAttribute<MarshalAsAttribute>(0, UnmanagedType.Interface)
                            .Build()
                            .WithMethod("TestMethod")
                                .WithParameter<bool>()
                                .WithParameter<bool>()
                                .WithParameter<bool>()
                            .Build()
                        .Build()
                    .Build();

        var typeInfo = result.TypeLib.GetTypeInfoByName("TestInterface");
        typeInfo.Should().NotBeNull();

        typeInfo!.GetFuncDescByName("TestMethod").Should().NotBeNull("TestMethod should be available");
        typeInfo!.GetFuncDescByName("TestMethod")!.Value.cParams.Should().Be(1);
        typeInfo!.GetFuncDescByName("TestMethod_3").Should().NotBeNull("TestMethod should be available");
        typeInfo!.GetFuncDescByName("TestMethod_3")!.Value.cParams.Should().Be(3);

        typeInfo!.GetFuncDescByName("TestMethod_2").Should().BeNull("TestMethod should not be available");
    }

    [Fact]
    public void InterfaceWithInvalidGenericParameter_NameSuffixCounts()
    {
        var result = CreateAssembly()
                        .WithInterface("TestInterface")
                            .WithMethod("TestMethod")
                                .WithParameter<bool>()
                            .Build()
                            .WithMethod("TestMethod")
                                .WithParameter<Func<bool>>()
                            .Build()
                            .WithMethod("TestMethod")
                                .WithParameter<bool>()
                                .WithParameter<bool>()
                                .WithParameter<bool>()
                            .Build()
                        .Build()
                    .Build();

        var typeInfo = result.TypeLib.GetTypeInfoByName("TestInterface");
        typeInfo.Should().NotBeNull();

        typeInfo!.GetFuncDescByName("TestMethod").Should().NotBeNull("TestMethod should be available");
        typeInfo!.GetFuncDescByName("TestMethod")!.Value.cParams.Should().Be(1);
        typeInfo!.GetFuncDescByName("TestMethod_3").Should().NotBeNull("TestMethod should be available");
        typeInfo!.GetFuncDescByName("TestMethod_3")!.Value.cParams.Should().Be(3);

        typeInfo!.GetFuncDescByName("TestMethod_2").Should().BeNull("TestMethod should not be available");
    }
}
