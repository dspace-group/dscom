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

public class ClassTest : BaseTest
{
    private const string CustomInterface = "CustomInterface";
    private const string SourceInterfaces = "SourceInterfaces";
    private const string InterfaceTest = "InterfaceTest";

    [Fact]
    public void ComVisibleClassWithInterface_TKINDIsCOCLASS()
    {
        var result = CreateAssembly()
                        .WithInterface(InterfaceTest).WithCustomAttribute<InterfaceTypeAttribute>(ComInterfaceType.InterfaceIsIDispatch)
                            .Build()
                        .WithClass("TestClass", new[] { InterfaceTest })
                            .Build()
                        .Build();

        var typeInfo = result.TypeLib.GetTypeInfoByName("TestClass");
        Assert.NotNull(typeInfo);

        using var attributes = typeInfo!.GetTypeInfoAttributes();
        Assert.NotNull(attributes);
        Assert.Equal(TYPEKIND.TKIND_COCLASS, attributes!.Value.typekind);
    }

    [Fact]
    public void ComVisibleClassWithDefaultInterface_TKINDIsCOCLASS()
    {
        var result = CreateAssembly()
            .WithInterface(InterfaceTest).WithCustomAttribute<InterfaceTypeAttribute>(ComInterfaceType.InterfaceIsIDispatch)
                .Build(out var interfaceType)
            .WithClass("TestClass", new[] { InterfaceTest })
                .WithCustomAttribute(typeof(ComDefaultInterfaceAttribute), interfaceType!)
                .Build()
            .Build();

        var typeInfo = result.TypeLib.GetTypeInfoByName("TestClass");
        Assert.NotNull(typeInfo);

        using var attributes = typeInfo!.GetTypeInfoAttributes();
        Assert.NotNull(attributes);
        Assert.Equal(TYPEKIND.TKIND_COCLASS, attributes!.Value.typekind);

        var index = 0;
        while (index < 2)
        {
            typeInfo!.GetRefTypeOfImplType(index, out var href);
            typeInfo!.GetRefTypeInfo(href, out var ppTI);
            ppTI.GetDocumentation(-1, out var refTypeName, out var refTypeDocString, out var refTypeHelpContext, out var refTypeHelpFile);
            Assert.Contains(refTypeName, new[] { InterfaceTest, "_TestClass" });
            index++;
        }
    }

    [Fact]
    public void ComVisibleClassWithoutInterface_TKINDIsCOCLASS()
    {
        var result = CreateAssembly()
                        .WithClass("TestClass", Array.Empty<string>())
                            .Build()
                        .Build();

        var typeInfo = result.TypeLib.GetTypeInfoByName("TestClass");
        Assert.NotNull(typeInfo);

        using var attributes = typeInfo!.GetTypeInfoAttributes();
        Assert.NotNull(attributes);
        Assert.Equal(TYPEKIND.TKIND_COCLASS, attributes!.Value.typekind);
    }

    [Fact]
    public void ComVisibleClassWithoutInterfaceWithoutClassInterfaceAttribute_HasDispClassInterface()
    {
        var result = CreateAssembly()
                        .WithClass("TestClass", Array.Empty<string>())
                            .Build()
                        .Build();

        var typeInfo = result.TypeLib.GetTypeInfoByName("_TestClass");
        Assert.NotNull(typeInfo);

        using var attributes = typeInfo!.GetTypeInfoAttributes();
        Assert.NotNull(attributes);
        Assert.Equal(TYPEKIND.TKIND_DISPATCH, attributes!.Value.typekind);
    }

    [Fact]
    public void ComVisibleClassWithoutInterfaceWithClassInterfaceAttributeNon_HasNoClassInterface()
    {
        var result = CreateAssembly()
                        .WithClass("TestClass", Array.Empty<string>())
                            .WithCustomAttribute(typeof(ClassInterfaceAttribute), (short)ClassInterfaceType.None)
                            .Build()
                        .Build();

        var typeInfo = result.TypeLib.GetTypeInfoByName("_TestClass");
        Assert.Null(typeInfo);
    }

    [Fact]
    public void ComVisibleClassWithoutInterfaceWithClassInterfaceAttributeDisp_HasDispClassInterface()
    {
        var result = CreateAssembly()
                        .WithClass("TestClass", Array.Empty<string>())
                            .WithCustomAttribute<ClassInterfaceAttribute>((short)ClassInterfaceType.AutoDispatch)
                            .Build()
                        .Build();

        var typeInfo = result.TypeLib.GetTypeInfoByName("_TestClass");
        Assert.NotNull(typeInfo);

        using var attributes = typeInfo!.GetTypeInfoAttributes();
        Assert.NotNull(attributes);
        Assert.Equal(TYPEKIND.TKIND_DISPATCH, attributes!.Value.typekind);
    }

    [Fact]
    public void ComVisibleClassWithoutInterfaceWithClassInterfaceAttributeDual_Fails()
    {
        Assert.Throws<NotSupportedException>(
                () => CreateAssembly()
                        .WithClass("TestClass", Array.Empty<string>())
                            .WithCustomAttribute(typeof(ClassInterfaceAttribute), (short)ClassInterfaceType.AutoDual)
                            .Build()
                        .Build()
        );
    }

    [Fact]
    public void ClassInterfaceWithoutVersion_ShoutReturnMajorMinor0()
    {
        var result = CreateAssembly()
                .WithClass("TestClass", Array.Empty<string>())
                    .Build()
                .Build();

        var classInterface = result.TypeLib.GetTypeInfoByName("_TestClass");
        Assert.NotNull(classInterface);

        using var attribute = classInterface!.GetTypeInfoAttributes();
        Assert.NotNull(attribute);

        Assert.Equal(0, attribute!.Value.wMajorVerNum);
        Assert.Equal(0, attribute!.Value.wMinorVerNum);
    }

    [Fact]
    public void GenericClass_ClassIsIgnored()
    {
        var result = CreateAssembly()
                .WithClass("GenericTestClass")
                    .WithGenericTypeParameter("T")
                    .Build()
                .Build();

        Assert.Equal(0, result.TypeLib.GetTypeInfoCount());
    }

    [Fact]
    public void ClassDerivedFromGenericClass_DerivedClassIsCreated()
    {
        var dynamicTypeBuilder = CreateAssembly()
                .WithClass("GenericBaseClass")
                    .WithGenericTypeParameter("T")
                    .Build(out var genericType);
        genericType = genericType!.MakeGenericType(new[] { typeof(string) });
        var result = dynamicTypeBuilder
                .WithClass("DerivedClass", Array.Empty<string>(), genericType)
                .Build()
            .Build();

        Assert.Equal(1, result.TypeLib.GetTypeInfoCount());
    }

    [Fact]
    public void GenericClassWithClassInterfaceAutoDispatch_ClassInterfaceIsNotCreated()
    {
        var result = CreateAssembly()
                .WithClass("GenericTestClass")
                    .WithGenericTypeParameter("T")
                    .WithCustomAttribute<ClassInterfaceAttribute>(ClassInterfaceType.AutoDispatch)
                    .Build()
                .Build();

        Assert.Equal(0, result.TypeLib.GetTypeInfoCount());
    }

    [Fact]
    public void ClassDerivedFromGenericClassWithClassInterfaceAttribute_DerivedClassIsCreated()
    {
        var dynamicTypeBuilder = CreateAssembly()
                .WithClass("GenericBaseClass")
                    .WithGenericTypeParameter("T")
                    .Build(out var genericType);
        genericType = genericType!.MakeGenericType(new[] { typeof(string) });
        var result = dynamicTypeBuilder
                .WithClass("DerivedClass", Array.Empty<string>(), genericType)
                .WithCustomAttribute<ClassInterfaceAttribute>(ClassInterfaceType.AutoDispatch)
                .Build()
            .Build();

        Assert.Equal(1, result.TypeLib.GetTypeInfoCount());
    }

    [Fact]
    public void MethodWithParameterTypeOfUserDefinedClass_CoClassDefaultInterfaceIsUsedAsParameterType()
    {
        var result = CreateAssembly()
            .WithInterface(CustomInterface)
                .Build(out _)
            .WithClass("CustomClass", new[] { CustomInterface })
                .WithCustomAttribute<ClassInterfaceAttribute>(ClassInterfaceType.None)
                .Build(out var customClass)
            .WithInterface("TestInterface")
                .WithMethod("TestMethod")
                    .WithParameter(customClass!)
                    .Build()
                .Build()
            .Build();

        var type = result.TypeLib.GetTypeInfoByName("TestInterface");
        Assert.NotNull(type);

        var funcDesc = type!.GetFuncDescByName("TestMethod");
        Assert.NotNull(funcDesc);

        var firstParam = Marshal.PtrToStructure<ELEMDESC>(funcDesc!.Value.lprgelemdescParam);
        Assert.Equal(VarEnum.VT_PTR, firstParam.tdesc.GetVarEnum());

        var subtypeDesc = Marshal.PtrToStructure<TYPEDESC>(firstParam.tdesc.lpValue);
        Assert.Equal(VarEnum.VT_USERDEFINED, subtypeDesc.GetVarEnum());

        var hrefType = subtypeDesc.lpValue;

        var typeInfo64Bit = (ITypeInfo64Bit)type!;
        typeInfo64Bit.GetRefTypeInfo(hrefType, out var refTypeInfo64Bit);
        refTypeInfo64Bit.GetDocumentation(-1, out var name, out _, out _, out _);
        Assert.Equal(CustomInterface, name);
    }

    [Fact]
    public void ClassImplements3InterfacesClassInterfaceTypeNone_FirstInterfaceIsTheDefaultInterface()
    {
        const string customInterface2 = "CustomInterface2";
        var result = CreateAssembly()
            .WithInterface("CustomInterface1")
                .Build(out _)
            .WithInterface(customInterface2)
                .Build(out _)
            .WithInterface("CustomInterface3")
                .Build(out _)
            .WithClass("CustomClass", new[] { "CustomInterface1", customInterface2, "CustomInterface3" })
                .WithCustomAttribute<ClassInterfaceAttribute>(ClassInterfaceType.None)
                .Build()
            .Build();

        var typeInfo = result.TypeLib.GetTypeInfoByName("CustomClass");
        Assert.NotNull(typeInfo);

        using var typeAttr = typeInfo!.GetTypeInfoAttributes();

        for (var i = 0; i < typeAttr!.Value.cImplTypes; i++)
        {
            typeInfo!.GetRefTypeOfImplType(i, out var href);
            typeInfo.GetRefTypeInfo(href, out var refTypeInfo);
            typeInfo.GetImplTypeFlags(i, out var pImplTypeFlags);

            refTypeInfo.GetDocumentation(-1, out var name, out _, out _, out _);

            if (name == "CustomInterface1")
            {
                Assert.True(pImplTypeFlags.HasFlag(IMPLTYPEFLAGS.IMPLTYPEFLAG_FDEFAULT));
            }
            else
            {
                Assert.False(pImplTypeFlags.HasFlag(IMPLTYPEFLAGS.IMPLTYPEFLAG_FDEFAULT));
            }
        }
    }

    [Fact]
    public void CoClassWithComSourceInterfaces_ComSourceInterfacesIsIMPLTYPEFLAGFSOURCE()
    {
        var result = CreateAssembly()
                .WithInterface(SourceInterfaces).Build(out var sourceInterfaces)
                .WithClass("TestInterface")
                    .WithCustomAttribute<ComSourceInterfacesAttribute>(sourceInterfaces!)
                    .Build()
                .Build();

        var typeInfo = result.TypeLib.GetTypeInfoByName("TestInterface");
        Assert.NotNull(typeInfo);

        using var typeAttr = typeInfo!.GetTypeInfoAttributes();

        var found = false;
        for (var i = 0; i < typeAttr!.Value.cImplTypes; i++)
        {
            typeInfo!.GetRefTypeOfImplType(i, out var href);
            typeInfo.GetRefTypeInfo(href, out var refTypeInfo);
            typeInfo.GetImplTypeFlags(i, out var pImplTypeFlags);

            refTypeInfo.GetDocumentation(-1, out var name, out _, out _, out _);

            if (name == SourceInterfaces)
            {
                found = true;
                Assert.True(pImplTypeFlags.HasFlag(IMPLTYPEFLAGS.IMPLTYPEFLAG_FDEFAULT));
                Assert.True(pImplTypeFlags.HasFlag(IMPLTYPEFLAGS.IMPLTYPEFLAG_FSOURCE));
            }
        }

        Assert.True(found);
    }

    [Fact]
    public void CoClassWithComSourceInterfacesAndComInterfaceIsUsed_ComSourceInterfacesIsIMPLTYPEFLAGFSOURCEOneTime()
    {
        var result = CreateAssembly()
                .WithInterface(SourceInterfaces).Build(out var sourceInterfaces)
                .WithClass("TestClass", new[] { SourceInterfaces })
                    .WithCustomAttribute<ComSourceInterfacesAttribute>(sourceInterfaces!)
                    .Build()
                .Build();

        var typeInfo = result.TypeLib.GetTypeInfoByName("TestClass");
        Assert.NotNull(typeInfo);

        using var typeAttr = typeInfo!.GetTypeInfoAttributes();

        var found = 0;
        var foundIMPLTYPEFLAGFDEFAULT = 0;
        var foundIMPLTYPEFLAGFSOURCE = 0;
        for (var i = 0; i < typeAttr!.Value.cImplTypes; i++)
        {
            typeInfo!.GetRefTypeOfImplType(i, out var href);
            typeInfo.GetRefTypeInfo(href, out var refTypeInfo);
            typeInfo.GetImplTypeFlags(i, out var pImplTypeFlags);

            refTypeInfo.GetDocumentation(-1, out var name, out _, out _, out _);

            if (name == SourceInterfaces)
            {
                found++;

                if (pImplTypeFlags.HasFlag(IMPLTYPEFLAGS.IMPLTYPEFLAG_FDEFAULT))
                {
                    foundIMPLTYPEFLAGFDEFAULT++;
                }

                if (pImplTypeFlags.HasFlag(IMPLTYPEFLAGS.IMPLTYPEFLAG_FSOURCE))
                {
                    foundIMPLTYPEFLAGFSOURCE++;
                }
            }
        }

        Assert.Equal(2, found);
        Assert.Equal(1, foundIMPLTYPEFLAGFDEFAULT);
        Assert.Equal(1, foundIMPLTYPEFLAGFSOURCE);
    }
}
