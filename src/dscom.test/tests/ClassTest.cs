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

using dSPACE.Runtime.InteropServices.Exporter;

namespace dSPACE.Runtime.InteropServices.Tests;

public class ClassTest : BaseTest
{
    [Fact]
    public void ComVisibleClassWithInterface_TKINDIsCOCLASS()
    {
        var result = CreateAssembly()
                        .WithInterface("InterfaceTest").WithCustomAttribute<InterfaceTypeAttribute>(ComInterfaceType.InterfaceIsIDispatch)
                            .Build()
                        .WithClass("TestClass", new string[] { "InterfaceTest" })
                            .Build()
                        .Build();

        var typeInfo = result.TypeLib.GetTypeInfoByName("TestClass");
        typeInfo.Should().NotBeNull("TestClass not found");

        using var attributes = typeInfo!.GetTypeInfoAttributes();
        attributes.Should().NotBeNull();
        attributes!.Value.typekind.Should().Be(TYPEKIND.TKIND_COCLASS);
    }

    [Fact]
    public void ComVisibleClassWithDefaultInterface_TKINDIsCOCLASS()
    {
        var result = CreateAssembly()
            .WithInterface("InterfaceTest").WithCustomAttribute<InterfaceTypeAttribute>(ComInterfaceType.InterfaceIsIDispatch)
                .Build(out var interfaceType)
            .WithClass("TestClass", new string[] { "InterfaceTest" })
                .WithCustomAttribute(typeof(ComDefaultInterfaceAttribute), interfaceType!)
                .Build()
            .Build();

        var typeInfo = result.TypeLib.GetTypeInfoByName("TestClass");
        typeInfo.Should().NotBeNull("TestClass not found");

        using var attributes = typeInfo!.GetTypeInfoAttributes();
        attributes.Should().NotBeNull();
        attributes!.Value.typekind.Should().Be(TYPEKIND.TKIND_COCLASS);

        var index = 0;
        while (index < 2)
        {
            typeInfo!.GetRefTypeOfImplType(index, out var href);
            typeInfo!.GetRefTypeInfo(href, out var ppTI);
            ppTI.GetDocumentation(-1, out var refTypeName, out var refTypeDocString, out var refTypeHelpContext, out var refTypeHelpFile);
            refTypeName.Should().BeOneOf("InterfaceTest", "_TestClass");
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
        typeInfo.Should().NotBeNull("TestClass not found");

        using var attributes = typeInfo!.GetTypeInfoAttributes();
        attributes.Should().NotBeNull();
        attributes!.Value.typekind.Should().Be(TYPEKIND.TKIND_COCLASS);
    }

    [Fact]
    public void ComVisibleClassWithoutInterfaceWithoutClassInterfaceAttribute_HasDispClassInterface()
    {
        var result = CreateAssembly()
                        .WithClass("TestClass", Array.Empty<string>())
                            .Build()
                        .Build();

        var typeInfo = result.TypeLib.GetTypeInfoByName("_TestClass");
        typeInfo.Should().NotBeNull("_TestClass not found");

        using var attributes = typeInfo!.GetTypeInfoAttributes();
        attributes.Should().NotBeNull();
        attributes!.Value.typekind.Should().Be(TYPEKIND.TKIND_DISPATCH);
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
        typeInfo.Should().BeNull("_TestClass found");
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
        typeInfo.Should().NotBeNull("_TestClass not found");

        using var attributes = typeInfo!.GetTypeInfoAttributes();
        attributes.Should().NotBeNull();
        attributes!.Value.typekind.Should().Be(TYPEKIND.TKIND_DISPATCH);
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
        classInterface.Should().NotBeNull();

        using var attribute = classInterface!.GetTypeInfoAttributes();
        attribute.Should().NotBeNull();

        attribute!.Value.wMajorVerNum.Should().Be(0);
        attribute!.Value.wMinorVerNum.Should().Be(0);
    }

    [Fact]
    public void GenericClass_ClassIsIgnored()
    {
        var result = CreateAssembly()
                .WithClass("GenericTestClass")
                    .WithGenericTypeParameter("T")
                    .Build()
                .Build();

        result.TypeLib.GetTypeInfoCount().Should().Be(0, "Generic CoClass should be ignored");
    }

    [Fact]
    public void ClassDerivedFromGenericClass_DerivedClassIsCreated()
    {
        var dynamicTypeBuilder = CreateAssembly()
                .WithClass("GenericBaseClass")
                    .WithGenericTypeParameter("T")
                    .Build(out var genericType);
        genericType = genericType!.MakeGenericType(new Type[] { typeof(string) });
        var result = dynamicTypeBuilder
                .WithClass("DerivedClass", Array.Empty<string>(), genericType)
                .Build()
            .Build();

        result.TypeLib.GetTypeInfoCount().Should().Be(1, "Generic CoClass should be ignored, Derived class should be created.");
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

        result.TypeLib.GetTypeInfoCount().Should().Be(0);
    }

    [Fact]
    public void ClassDerivedFromGenericClassWithClassInterfaceAttribute_DerivedClassIsCreated()
    {
        var dynamicTypeBuilder = CreateAssembly()
                .WithClass("GenericBaseClass")
                    .WithGenericTypeParameter("T")
                    .Build(out var genericType);
        genericType = genericType!.MakeGenericType(new Type[] { typeof(string) });
        var result = dynamicTypeBuilder
                .WithClass("DerivedClass", Array.Empty<string>(), genericType)
                .WithCustomAttribute<ClassInterfaceAttribute>(ClassInterfaceType.AutoDispatch)
                .Build()
            .Build();

        result.TypeLib.GetTypeInfoCount().Should().Be(1, "Derived class should be created.");
    }

    [Fact]
    public void MethodWithParameterTypeOfUserDefinedClass_CoClassDefaultInterfaceIsUsedAsParameterType()
    {
        var result = CreateAssembly()
            .WithInterface("CustomInterface")
                .Build(out _)
            .WithClass("CustomClass", new string[] { "CustomInterface" })
                .WithCustomAttribute<ClassInterfaceAttribute>(ClassInterfaceType.None)
                .Build(out var customClass)
            .WithInterface("TestInterface")
                .WithMethod("TestMethod")
                    .WithParameter(customClass!)
                    .Build()
                .Build()
            .Build();

        var type = result.TypeLib.GetTypeInfoByName("TestInterface");
        type.Should().NotBeNull();

        var funcDesc = type!.GetFuncDescByName("TestMethod");
        funcDesc!.Should().NotBeNull();

        var firstParam = Marshal.PtrToStructure<ELEMDESC>(funcDesc!.Value.lprgelemdescParam);
        firstParam.tdesc.GetVarEnum().Should().Be(VarEnum.VT_PTR, $"First parameter of TestMethod should be VT_PTR");

        var subtypeDesc = Marshal.PtrToStructure<TYPEDESC>(firstParam.tdesc.lpValue);
        subtypeDesc.GetVarEnum().Should().Be(VarEnum.VT_USERDEFINED, $"Inner type of first parameter of TestMethod should be availble VarEnum.VT_USERDEFINED");

        var hrefType = subtypeDesc.lpValue;

        var typeInfo64Bit = (ITypeInfo64Bit)type!;
        typeInfo64Bit.GetRefTypeInfo(hrefType, out var refTypeInfo64Bit);
        refTypeInfo64Bit.GetDocumentation(-1, out var name, out _, out _, out _);
        name.Should().Be("CustomInterface");
    }

    [Fact]
    public void ClassImplements3InterfacesClassInterfaceTypeNone_FirstInterfaceIsTheDefaultInterface()
    {
        var result = CreateAssembly()
            .WithInterface("CustomInterface1")
                .Build(out _)
            .WithInterface("CustomInterface2")
                .Build(out _)
            .WithInterface("CustomInterface3")
                .Build(out _)
            .WithClass("CustomClass", new string[] { "CustomInterface1", "CustomInterface2", "CustomInterface3" })
                .WithCustomAttribute<ClassInterfaceAttribute>(ClassInterfaceType.None)
                .Build()
            .Build();

        var typeInfo = result.TypeLib.GetTypeInfoByName("CustomClass");
        typeInfo.Should().NotBeNull();

        using var typeAttr = typeInfo!.GetTypeInfoAttributes();

        for (var i = 0; i < typeAttr!.Value.cImplTypes; i++)
        {
            typeInfo!.GetRefTypeOfImplType(i, out var href);
            typeInfo.GetRefTypeInfo(href, out var refTypeInfo);
            typeInfo.GetImplTypeFlags(i, out var pImplTypeFlags);

            refTypeInfo.GetDocumentation(-1, out var name, out _, out _, out _);

            if (name == "CustomInterface1")
            {
                pImplTypeFlags.Should().HaveFlag(IMPLTYPEFLAGS.IMPLTYPEFLAG_FDEFAULT, "First interface should be the default interface");
            }
            else
            {
                pImplTypeFlags.Should().NotHaveFlag(IMPLTYPEFLAGS.IMPLTYPEFLAG_FDEFAULT, "Only first interface should be the default interface");
            }
        }
    }

    [Fact]
    public void CoClassWithComSourceInterfaces_ComSourceInterfacesIsIMPLTYPEFLAGFSOURCE()
    {
        var result = CreateAssembly()
                .WithInterface("SourceInterfaces").Build(out var sourceInterfaces)
                .WithClass("TestInterface")
                    .WithCustomAttribute<ComSourceInterfacesAttribute>(sourceInterfaces!)
                    .Build()
                .Build();

        var typeInfo = result.TypeLib.GetTypeInfoByName("TestInterface");
        typeInfo.Should().NotBeNull();

        using var typeAttr = typeInfo!.GetTypeInfoAttributes();

        var found = false;
        for (var i = 0; i < typeAttr!.Value.cImplTypes; i++)
        {
            typeInfo!.GetRefTypeOfImplType(i, out var href);
            typeInfo.GetRefTypeInfo(href, out var refTypeInfo);
            typeInfo.GetImplTypeFlags(i, out var pImplTypeFlags);

            refTypeInfo.GetDocumentation(-1, out var name, out _, out _, out _);

            if (name == "SourceInterfaces")
            {
                found = true;
                pImplTypeFlags.Should().HaveFlag(IMPLTYPEFLAGS.IMPLTYPEFLAG_FDEFAULT);
                pImplTypeFlags.Should().HaveFlag(IMPLTYPEFLAGS.IMPLTYPEFLAG_FSOURCE);
            }
        }

        found.Should().BeTrue("SourceInterfaces should is available as IMPLTYPEFLAG_FDEFAULT and IMPLTYPEFLAG_FSOURCE");
    }

    [Fact]
    public void CoClassWithComSourceInterfacesAndComInterfaceIsUsed_ComSourceInterfacesIsIMPLTYPEFLAGFSOURCEOneTime()
    {
        var result = CreateAssembly()
                .WithInterface("SourceInterfaces").Build(out var sourceInterfaces)
                .WithClass("TestClass", new string[] { "SourceInterfaces" })
                    .WithCustomAttribute<ComSourceInterfacesAttribute>(sourceInterfaces!)
                    .Build()
                .Build();

        var typeInfo = result.TypeLib.GetTypeInfoByName("TestClass");
        typeInfo.Should().NotBeNull();

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

            if (name == "SourceInterfaces")
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

        found.Should().Be(2, "SourceInterfaces should be availble two times");
        foundIMPLTYPEFLAGFDEFAULT.Should().Be(1, "Multiple IMPLTYPEFLAGS.IMPLTYPEFLAG_FSOURCE found");
        foundIMPLTYPEFLAGFSOURCE.Should().Be(1, "Multiple IMPLTYPEFLAGS.IMPLTYPEFLAG_FDEFAULT found");
    }
}
