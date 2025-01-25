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
using Xunit;

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
        Assert.Null(typeLibInfo);
        typeLibInfo = result.TypeLib.GetTypeInfoByName("TOUPPERCASE");
        Assert.NotNull(typeLibInfo);
        var kv = typeLibInfo!.GetAllEnumValues();
        Assert.All(kv, z => Assert.StartsWith("TOUPPERCASE", z.Key));

        typeLibInfo = result.TypeLib.GetTypeInfoByName("TOLOWERCASE");
        Assert.Null(typeLibInfo);
        typeLibInfo = result.TypeLib.GetTypeInfoByName("tolowercase");
        Assert.NotNull(typeLibInfo);
        kv = typeLibInfo!.GetAllEnumValues();
        Assert.All(kv, z => Assert.StartsWith("tolowercase", z.Key));
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
        Assert.Null(typeLibInfo);
        typeLibInfo = result.TypeLib.GetTypeInfoByName("froofroo");
        Assert.NotNull(typeLibInfo);
        var kv = typeLibInfo!.GetAllEnumValues();
        Assert.All(kv, z => Assert.StartsWith("froofroo", z.Key));
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
        Assert.Null(typeLibInfo);
        typeLibInfo = result.TypeLib.GetTypeInfoByName("froofroo");
        Assert.NotNull(typeLibInfo);
        var kv = typeLibInfo!.GetAllEnumValues();
        Assert.All(kv, z => Assert.Contains(z.Key, new[] { "fizz", "buzz", "fizzbuzz" }));
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
        Assert.NotNull(typeLibInfo);

        var funcDescValue = typeLibInfo!.GetFuncDescByName("getFruit");
        Assert.NotNull(funcDescValue);
        var funcDesc = funcDescValue.Value;

        var elemDescParam = funcDesc.GetParameter(0)!.Value;
        var paramDesc = elemDescParam.desc.paramdesc;

        // the lpVarValue points to a PARAMDESCEX, but we don't care about the size, so dereference the VARIANTARG
        // structure directly at the offset'd address.
        var varValue = Marshal.GetObjectForNativeVariant(paramDesc.lpVarValue + sizeof(ulong));
        Assert.Equal(20, varValue);

        typeLibInfo!.GetRefTypeInfo(funcDesc.elemdescFunc.tdesc.lpValue.ExtractInt32(), out var returnType);
        Assert.Equal("froofroo", returnType.GetName());

        typeLibInfo.GetRefTypeInfo(elemDescParam.tdesc.lpValue.ExtractInt32(), out var paramType);
        Assert.Equal("froofroo", paramType.GetName());
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
        Assert.Null(typeLibInfo);
        typeLibInfo = result.TypeLib.GetTypeInfoByName("touppercase");
        Assert.NotNull(typeLibInfo);
        var kv = typeLibInfo!.GetAllEnumValues();
        Assert.All(kv, z => Assert.Contains(z.Key, new[] { "fizz", "buzz", "fizzbuzz" }));
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
        Assert.Null(typeInfo);
        typeInfo = result.TypeLib.GetTypeInfoByName("TOUPPERCASE");
        Assert.NotNull(typeInfo);

        typeInfo = result.TypeLib.GetTypeInfoByName("TOLOWERCASE");
        Assert.Null(typeInfo);
        typeInfo = result.TypeLib.GetTypeInfoByName("tolowercase");
        Assert.NotNull(typeInfo);
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
        Assert.NotNull(typeInfo);

        Assert.NotNull(typeInfo!.GetFuncDescByName("TestMethod"));
        Assert.Equal(2, typeInfo!.GetFuncDescByName("TestMethod")!.Value.cParams);
        Assert.NotNull(typeInfo!.GetFuncDescByName("TestMethod_2"));
        Assert.Equal(4, typeInfo!.GetFuncDescByName("TestMethod_2")!.Value.cParams);
        Assert.Null(typeInfo!.GetFuncDescByName("TestMethod_3"));
        Assert.Null(typeInfo!.GetFuncDescByName("TestMethod_4"));
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
        Assert.NotNull(typeInfo);

        Assert.NotNull(typeInfo!.GetFuncDescByName("TestMethod"));
        Assert.Equal(1, typeInfo!.GetFuncDescByName("TestMethod")!.Value.cParams);
        Assert.NotNull(typeInfo!.GetFuncDescByName("TestMethod_3"));
        Assert.Equal(3, typeInfo!.GetFuncDescByName("TestMethod_3")!.Value.cParams);

        Assert.Null(typeInfo!.GetFuncDescByName("TestMethod_2"));
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
        Assert.NotNull(typeInfo);

        Assert.NotNull(typeInfo!.GetFuncDescByName("TestMethod"));
        Assert.Equal(1, typeInfo!.GetFuncDescByName("TestMethod")!.Value.cParams);
        Assert.NotNull(typeInfo!.GetFuncDescByName("TestMethod_3"));
        Assert.Equal(3, typeInfo!.GetFuncDescByName("TestMethod_3")!.Value.cParams);

        Assert.Null(typeInfo!.GetFuncDescByName("TestMethod_2"));
    }
}
