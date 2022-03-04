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

using System.Collections;
using System.Globalization;
using dSPACE.Runtime.InteropServices.Exporter;

namespace dSPACE.Runtime.InteropServices.Tests;

public class MethodTest : BaseTest
{
    [Fact]
    public void Method_WithNameGetEnumerator_DispIdMinus4IsUsed()
    {
        var result = CreateAssembly()
                        .WithInterface("TestInterface").WithCustomAttribute<InterfaceTypeAttribute>(ComInterfaceType.InterfaceIsIDispatch)
                            .WithMethod("GetEnumerator")
                                .WithReturnType<IEnumerator>()
                            .Build()
                        .Build()
                    .Build();

        using var releasableFuncDesc = result.TypeLib.GetTypeInfoByName("TestInterface")?.GetFuncDescByName("GetEnumerator");
        releasableFuncDesc.Should().NotBeNull();
        releasableFuncDesc!.Value.invkind.Should().Be(INVOKEKIND.INVOKE_FUNC);
        releasableFuncDesc!.Value.memid.Should().Be(-4);
    }

    [Theory]
    [InlineData(ComInterfaceType.InterfaceIsDual)]
    [InlineData(ComInterfaceType.InterfaceIsIDispatch)]
    [InlineData(ComInterfaceType.InterfaceIsIUnknown)]
    public void Method_InInterfaceWithComInterfaceType_Found(ComInterfaceType interfaceType)
    {
        var result = CreateAssembly(CreateAssemblyName(assemblyNameSuffix: $"{interfaceType}"))
                        .WithInterface("TestInterface")
                            .WithCustomAttribute<InterfaceTypeAttribute>(interfaceType)
                            .WithMethod("TestMethod")
                                .WithParameter<string>()
                            .Build()
                        .Build()
                    .Build();

        //check for interface
        var typeInfo = result.TypeLib.GetTypeInfoByName("TestInterface");
        typeInfo.Should().NotBeNull("TestInterface not found");

        // Search for method
        typeInfo!.ContainsFuncDescByName("TestMethod").Should().Be(true);
    }

    [Theory]
    [InlineData(typeof(byte))]
    [InlineData(typeof(sbyte))]
    [InlineData(typeof(short))]
    [InlineData(typeof(ushort))]
    [InlineData(typeof(uint))]
    [InlineData(typeof(int))]
    [InlineData(typeof(ulong))]
    [InlineData(typeof(long))]
    [InlineData(typeof(string))]
    [InlineData(typeof(bool))]
    public void MethodWithOneParameterAndReturnTypeVoid_ReturnTypeIsVoid(Type firstArgumentType)
    {
        var result = CreateAssembly(CreateAssemblyName(assemblyNameSuffix: $"{firstArgumentType}"))
                        .WithInterface("TestInterface").WithCustomAttribute<InterfaceTypeAttribute>(ComInterfaceType.InterfaceIsIDispatch)
                            .WithMethod("TestMethod")
                                .WithParameter<string>()
                           .Build()
                        .Build()
                    .Build();

        //check for interface
        var typeInfo = result.TypeLib.GetTypeInfoByName("TestInterface");
        typeInfo.Should().NotBeNull("TestInterface not found");

        // Get and check TestMethod
        using var funcDesc = typeInfo!.GetFuncDescByName("TestMethod");
        funcDesc.Should().NotBeNull();

        // Check if return type if void
        funcDesc!.Value.elemdescFunc.tdesc.vt.Should().Be((short)VarEnum.VT_VOID);
    }

    [Theory]
    [InlineData(typeof(byte))]
    [InlineData(typeof(sbyte))]
    [InlineData(typeof(short))]
    [InlineData(typeof(ushort))]
    [InlineData(typeof(uint))]
    [InlineData(typeof(int))]
    [InlineData(typeof(ulong))]
    [InlineData(typeof(long))]
    [InlineData(typeof(float))]
    [InlineData(typeof(double))]
    [InlineData(typeof(string))]
    [InlineData(typeof(bool))]
    [InlineData(typeof(char))]
    [InlineData(typeof(object))]
    [InlineData(typeof(object[]))]
    [InlineData(typeof(IEnumerator))]
    [InlineData(typeof(DateTime))]
    [InlineData(typeof(Guid))]
    [InlineData(typeof(System.Drawing.Color))]
    [InlineData(typeof(decimal))]
    public void IUnknownMethodWithNoParameterAndGivenReturnType_ReturnTypeIsHResultAndFirstParameterIsOriginalReturnType(Type returnType)
    {
        var result = CreateAssembly(new AssemblyName($"Dynamic{returnType}"))
                        .WithInterface("TestInterface")
                            .WithCustomAttribute<InterfaceTypeAttribute>(ComInterfaceType.InterfaceIsIUnknown)
                            .WithMethod("TestMethod")
                                .WithReturnType(returnType)
                            .Build()
                        .Build()
                    .Build();

        //check for interface
        var typeInfo = result.TypeLib.GetTypeInfoByName("TestInterface");
        typeInfo.Should().NotBeNull("TestInterface not found");

        //check for method
        using var funcDescByName = typeInfo!.GetFuncDescByName("TestMethod");
        funcDescByName.Should().NotBeNull();
        var funcDesc = funcDescByName!.Value;

        // Check number of parameters
        funcDesc.cParams.Should().Be(1);

        // Get first parameter
        var parameter = funcDesc.GetParameter(0);
        parameter.Should().NotBeNull();

        // type should be Ptr
        var ptr = parameter!.Value.tdesc;
        ptr.vt.Should().Be((short)VarEnum.VT_PTR);

        // Ptr type should be the final typ
        var value = Marshal.PtrToStructure<TYPEDESC>(ptr.lpValue);
        value.GetVarEnum().Should().Be(returnType.GetVarEnum());
    }

    [Theory]
    [InlineData(typeof(byte))]
    [InlineData(typeof(sbyte))]
    [InlineData(typeof(short))]
    [InlineData(typeof(ushort))]
    [InlineData(typeof(uint))]
    [InlineData(typeof(int))]
    [InlineData(typeof(ulong))]
    [InlineData(typeof(long))]
    [InlineData(typeof(float))]
    [InlineData(typeof(double))]
    [InlineData(typeof(string))]
    [InlineData(typeof(bool))]
    [InlineData(typeof(char))]
    [InlineData(typeof(object))]
    [InlineData(typeof(object[]))]
    [InlineData(typeof(IEnumerator))]
    [InlineData(typeof(DateTime))]
    [InlineData(typeof(Guid))]
    [InlineData(typeof(System.Drawing.Color))]
    [InlineData(typeof(decimal))]
    public void IDispatchMethodWithNoParameterAndGivenReturnType_ReturnTypeIsUsed(Type returnType)
    {
        var result = CreateAssembly(new AssemblyName($"Dynamic{returnType}"))
                       .WithInterface("TestInterface")
                            .WithCustomAttribute<InterfaceTypeAttribute>(ComInterfaceType.InterfaceIsIDispatch)
                            .WithMethod("TestMethod")
                                .WithReturnType(returnType)
                            .Build()
                       .Build()
                    .Build();

        //check for interface
        var typeInfo = result.TypeLib.GetTypeInfoByName("TestInterface");
        typeInfo.Should().NotBeNull("TestInterface not found");

        //check for method
        using var funcDescByName = typeInfo!.GetFuncDescByName("TestMethod");
        funcDescByName.Should().NotBeNull();
        var funcDesc = funcDescByName!.Value;

        funcDesc.elemdescFunc.tdesc.vt.Should().Be(returnType.GetShortVarEnum());
    }

    [Theory]
    [InlineData(typeof(byte))]
    [InlineData(typeof(sbyte))]
    [InlineData(typeof(short))]
    [InlineData(typeof(ushort))]
    [InlineData(typeof(uint))]
    [InlineData(typeof(int))]
    [InlineData(typeof(ulong))]
    [InlineData(typeof(long))]
    [InlineData(typeof(float))]
    [InlineData(typeof(double))]
    [InlineData(typeof(string))]
    [InlineData(typeof(bool))]
    [InlineData(typeof(char))]
    [InlineData(typeof(object))]
    [InlineData(typeof(object[]))]
    [InlineData(typeof(IEnumerator))]
    [InlineData(typeof(DateTime))]
    [InlineData(typeof(Guid))]
    [InlineData(typeof(System.Drawing.Color))]
    [InlineData(typeof(decimal))]

    public void DualMethodWithNoParameterAndGivenReturnType_ReturnTypeIsUsed(Type returnType)
    {
        var result = CreateAssembly(new AssemblyName($"Dynamic{returnType}"))
                       .WithInterface("TestInterface")
                            .WithCustomAttribute<InterfaceTypeAttribute>(ComInterfaceType.InterfaceIsDual)
                            .WithMethod("TestMethod")
                                .WithReturnType(returnType)
                            .Build()
                       .Build()
                    .Build();

        //check for interface
        var typeInfo = result.TypeLib.GetTypeInfoByName("TestInterface");
        typeInfo.Should().NotBeNull("TestInterface not found");

        //check for method
        typeInfo!.GetFuncDesc(7, out var ppFuncDesc);
        ppFuncDesc.Should().NotBe(IntPtr.Zero);

        var funcDesc = Marshal.PtrToStructure<FUNCDESC>(ppFuncDesc);
        funcDesc.elemdescFunc.tdesc.vt.Should().Be(returnType.GetShortVarEnum());
        funcDesc.cParams.Should().Be(0);
    }

    [Fact]
    public void MethodWithDescriptionAttribute_HasDocString()
    {
        var result = CreateAssembly()
                       .WithInterface("TestInterface").WithCustomAttribute<InterfaceTypeAttribute>(ComInterfaceType.InterfaceIsIDispatch)
                            .WithMethod("TestMethod")
                                .WithReturnType(typeof(void))
                                .WithCustomAttribute(typeof(System.ComponentModel.DescriptionAttribute), "Description")
                           .Build()
                       .Build()
                    .Build();
        //check for interface
        var typeInfo = result.TypeLib.GetTypeInfoByName("TestInterface");
        typeInfo.Should().NotBeNull("TestInterface not found");

        //check for method
        using var funcDescByName = typeInfo!.GetFuncDescByName("TestMethod");
        funcDescByName.Should().NotBeNull();
        var funcDesc = funcDescByName!.Value;

        typeInfo!.GetDocumentation(funcDesc.memid, out var methodString, out var docString, out var helpContext, out var helpFile);
        docString.Should().NotBeNull();
        docString.Should().BeEquivalentTo("Description");
    }

    [Theory]
    [InlineData(typeof(byte), (byte)42, typeof(byte))]
    [InlineData(typeof(sbyte), (sbyte)-42, typeof(sbyte))]
    [InlineData(typeof(short), (short)-42, typeof(short))]
    [InlineData(typeof(ushort), (ushort)42, typeof(ushort))]
    [InlineData(typeof(uint), (uint)42, typeof(uint))]
    [InlineData(typeof(int), (-42), typeof(int))]
    [InlineData(typeof(ulong), (ulong)42, typeof(ulong))]
    [InlineData(typeof(long), (long)-42, typeof(long))]
    [InlineData(typeof(float), (float)-42.42, typeof(float))]
    [InlineData(typeof(double), (-42.42), typeof(double))]
    [InlineData(typeof(string), "42", typeof(string))]
    [InlineData(typeof(bool), true, typeof(bool))]
    [InlineData(typeof(char), '4', typeof(ushort))]
    [InlineData(typeof(object), null, typeof(object))]
    public void MethodWithParameterWithDefaultValue_HasDefaultValue(Type parameterType, object value, Type acceptedReturnType)
    {
        var result = CreateAssembly(CreateAssemblyName(assemblyNameSuffix: $"{parameterType}{value}{acceptedReturnType}"))
                        .WithInterface("TestInterface").WithCustomAttribute<InterfaceTypeAttribute>(ComInterfaceType.InterfaceIsIDispatch)
                            .WithMethod("TestMethod")
                                .WithReturnType(typeof(void))
                                .WithParameter(new DynamicMethodBuilder.ParameterItem { Type = parameterType, ParameterAttributes = ParameterAttributes.Optional | ParameterAttributes.HasDefault, DefaultValue = new DynamicMethodBuilder.DefaultValue(value) })
                            .Build()
                        .Build()
                     .Build();

        //check for interface
        var typeInfo = result.TypeLib.GetTypeInfoByName("TestInterface");
        typeInfo.Should().NotBeNull("TestInterface not found");

        //check for method
        using var funcDesc = typeInfo!.GetFuncDescByName("TestMethod");
        funcDesc.Should().NotBeNull();
        funcDesc!.Value.lprgelemdescParam.Should().NotBeNull();

        var firstParam = Marshal.PtrToStructure<ELEMDESC>(funcDesc.Value.lprgelemdescParam);
        firstParam.tdesc.vt.Should().Be(parameterType.GetShortVarEnum());
        if (value != null)
        {
            firstParam.desc.paramdesc.lpVarValue.Should().NotBe(IntPtr.Zero);
            var defaultValue = Marshal.GetObjectForNativeVariant(firstParam.desc.paramdesc.lpVarValue + 8);
            defaultValue.Should().NotBeNull();
            defaultValue!.GetType().Should().Be(acceptedReturnType);
            defaultValue!.Should().Be(Convert.ChangeType(value, acceptedReturnType, CultureInfo.InvariantCulture));
        }
    }

    [Fact]
    public void MethodWithEnumReturnValueAndNoParameter_HasEnumReturnValue()
    {
        var typeBuilder = CreateAssembly()
            .WithEnum("TestEnum", typeof(int))
                .WithLiteral("A", 1)
                .WithLiteral("B", 20)
                .WithLiteral("C", 50)
            .Build(out var createdEnumType);
        var result = typeBuilder
            .WithInterface("TestInterface").WithCustomAttribute<InterfaceTypeAttribute>(ComInterfaceType.InterfaceIsIDispatch)
                .WithMethod("TestMethod")
                    .WithReturnType(createdEnumType!)
                .Build()
            .Build()
        .Build();

        //check for interface
        var typeInfo = result.TypeLib.GetTypeInfoByName("TestInterface");
        typeInfo.Should().NotBeNull("TestInterface not found");

        //check for method
        using var funcDesc = typeInfo!.GetFuncDescByName("TestMethod");
        funcDesc.Should().NotBeNull();

        funcDesc!.Value.elemdescFunc.tdesc.vt.Should().Be(createdEnumType!.GetShortVarEnum());
        funcDesc!.Value.elemdescFunc.tdesc.lpValue.Should().NotBeNull();
        funcDesc!.Value.cParams.Should().Be(0);

        ((ITypeInfo64Bit)typeInfo!).GetRefTypeInfo(funcDesc!.Value.elemdescFunc.tdesc.lpValue, out var typeInfoEnum);
        typeInfoEnum.Should().NotBeNull();
        typeInfoEnum.GetDocumentation(-1, out var strName, out var strDocString, out var dwHelpContext, out var strHelpFile);
        strName.Should().NotBeNull();
        strName.Should().BeEquivalentTo("TestEnum");
        typeInfoEnum.GetTypeAttr(out var ppTypeAttr);
        ppTypeAttr.Should().NotBeNull();
        var typeAttr = Marshal.PtrToStructure<TYPEATTR>(ppTypeAttr);
        typeAttr.Should().NotBeNull();
        typeAttr.typekind.Should().Be(TYPEKIND.TKIND_ENUM);
    }

    [Fact]
    public void MethodWithVoidReturnValueAndEnumParameter_HasEnumParameter()
    {
        var typeBuilder = CreateAssembly()
            .WithEnum("TestEnum", typeof(int))
                .WithLiteral("A", 1)
                .WithLiteral("B", 20)
                .WithLiteral("C", 50)
            .Build(out var createdEnumType);
        var result = typeBuilder
            .WithInterface("TestInterface").WithCustomAttribute<InterfaceTypeAttribute>(ComInterfaceType.InterfaceIsIDispatch)
                .WithMethod("TestMethod")
                    .WithReturnType(typeof(void))
                    .WithParameter(createdEnumType!)
                .Build()
            .Build()
        .Build();

        //check for interface
        var typeInfo = result.TypeLib.GetTypeInfoByName("TestInterface");
        typeInfo.Should().NotBeNull("TestInterface not found");

        //check for method
        using var funcDesc = typeInfo!.GetFuncDescByName("TestMethod");
        funcDesc.Should().NotBeNull();

        funcDesc!.Value.Should().NotBeNull();
        funcDesc!.Value.cParams.Should().Be(1);
        funcDesc!.Value.lprgelemdescParam.Should().NotBeNull();
        var parameter = Marshal.PtrToStructure<ELEMDESC>(funcDesc!.Value.lprgelemdescParam);

        parameter.tdesc.vt.Should().Be(createdEnumType!.GetShortVarEnum());
        parameter.tdesc.lpValue.Should().NotBeNull();

        ((ITypeInfo64Bit)typeInfo!).GetRefTypeInfo(parameter.tdesc.lpValue, out var typeInfoEnum);
        typeInfoEnum.Should().NotBeNull();
        typeInfoEnum.GetDocumentation(-1, out var strName, out var strDocString, out var dwHelpContext, out var strHelpFile);
        strName.Should().NotBeNull();
        strName.Should().BeEquivalentTo("TestEnum");
        typeInfoEnum.GetTypeAttr(out var ppTypeAttr);
        ppTypeAttr.Should().NotBeNull();
        var typeAttr = Marshal.PtrToStructure<TYPEATTR>(ppTypeAttr);
        typeAttr.Should().NotBeNull();
        typeAttr.typekind.Should().Be(TYPEKIND.TKIND_ENUM);
    }

    [Fact]
    public void MethodWithInterfaceReturnValueAndNoParameter_HasInterfaceReturnValue()
    {
        var typeBuilder = CreateAssembly()
            .WithInterface("ReferencedInterface").WithCustomAttribute<InterfaceTypeAttribute>(ComInterfaceType.InterfaceIsIDispatch)
            .Build(out var createdInterfaceType);
        var result = typeBuilder
            .WithInterface("TestInterface").WithCustomAttribute<InterfaceTypeAttribute>(ComInterfaceType.InterfaceIsIDispatch)
                .WithMethod("TestMethod")
                    .WithReturnType(createdInterfaceType!)
                .Build()
            .Build()
        .Build();

        //check for interface
        var typeInfo = result.TypeLib.GetTypeInfoByName("TestInterface");
        typeInfo.Should().NotBeNull("TestInterface not found");

        //Check mathod
        using var funcDesc = typeInfo!.GetFuncDescByName("TestMethod");
        funcDesc.Should().NotBeNull();

        funcDesc!.Value.elemdescFunc.tdesc.vt.Should().Be(createdInterfaceType!.GetShortVarEnum());
        funcDesc!.Value.elemdescFunc.tdesc.lpValue.Should().NotBeNull();
        funcDesc!.Value.cParams.Should().Be(0);

        var typeDesc = Marshal.PtrToStructure<TYPEDESC>(funcDesc!.Value.elemdescFunc.tdesc.lpValue);
        typeDesc.vt.Should().Be((short)VarEnum.VT_USERDEFINED);
        ((ITypeInfo64Bit)typeInfo!).GetRefTypeInfo(typeDesc.lpValue, out var typeInfoEnum);
        typeInfoEnum.Should().NotBeNull();
        typeInfoEnum.GetDocumentation(-1, out var strName, out var strDocString, out var dwHelpContext, out var strHelpFile);
        strName.Should().NotBeNull();
        strName.Should().BeEquivalentTo("ReferencedInterface");
        typeInfoEnum.GetTypeAttr(out var ppTypeAttr);
        ppTypeAttr.Should().NotBeNull();
        var typeAttr = Marshal.PtrToStructure<TYPEATTR>(ppTypeAttr);
        typeAttr.Should().NotBeNull();
        typeAttr.typekind.Should().Be(TYPEKIND.TKIND_DISPATCH);
    }

    [Fact]
    public void MethodWithVoidReturnValueAndInterfaceParameter_HasInterfaceParameter()
    {
        var typeBuilder = CreateAssembly()
            .WithInterface("ReferencedInterface")
            .Build(out var createdInterfaceType);
        var result = typeBuilder
            .WithInterface("TestInterface")
                .WithMethod("TestMethod")
                    .WithReturnType(typeof(void))
                    .WithParameter(createdInterfaceType!)
                .Build()
            .Build()
        .Build();

        //check for interface
        var typeInfo = result.TypeLib.GetTypeInfoByName("TestInterface");
        typeInfo.Should().NotBeNull("TestInterface not found");

        //check for method
        using var releasableFuncDesc = typeInfo!.GetFuncDescByName("TestMethod");
        releasableFuncDesc.Should().NotBeNull();
        var funcDesc = releasableFuncDesc!.Value;

        funcDesc.Should().NotBeNull();
        funcDesc.cParams.Should().Be(1);
        funcDesc.lprgelemdescParam.Should().NotBeNull();
        var parameter = Marshal.PtrToStructure<ELEMDESC>(funcDesc.lprgelemdescParam);

        parameter.tdesc.vt.Should().Be(createdInterfaceType!.GetShortVarEnum());
        parameter.tdesc.lpValue.Should().NotBeNull();

        var typeDesc = Marshal.PtrToStructure<TYPEDESC>(parameter.tdesc.lpValue);
        typeDesc.vt.Should().Be((short)VarEnum.VT_USERDEFINED);
        ((ITypeInfo64Bit)typeInfo!).GetRefTypeInfo(typeDesc.lpValue, out var typeInfoEnum);
        typeInfoEnum.Should().NotBeNull();
        typeInfoEnum.GetDocumentation(-1, out var strName, out var strDocString, out var dwHelpContext, out var strHelpFile);
        strName.Should().NotBeNull();
        strName.Should().BeEquivalentTo("ReferencedInterface");
        typeInfoEnum.GetTypeAttr(out var ppTypeAttr);
        ppTypeAttr.Should().NotBeNull();
        var typeAttr = Marshal.PtrToStructure<TYPEATTR>(ppTypeAttr);
        typeAttr.Should().NotBeNull();
        typeAttr.typekind.Should().Be(TYPEKIND.TKIND_DISPATCH);
    }

    [Fact]
    public void MethodWithArrayReturnValueAndNoParameter_HasSafeArrayReturnValue()
    {
        var result =
            CreateAssembly()
                .WithInterface("TestInterface")
                    .WithCustomAttribute<InterfaceTypeAttribute>(ComInterfaceType.InterfaceIsIDispatch)
                    .WithMethod("TestMethod")
                        .WithReturnType<string[]>()
                    .Build()
                .Build()
            .Build();

        //check for interface
        var typeInfo = result.TypeLib.GetTypeInfoByName("TestInterface");
        typeInfo.Should().NotBeNull("TestInterface not found");

        //check for method
        using var funcDescByName = typeInfo!.GetFuncDescByName("TestMethod");
        funcDescByName.Should().NotBeNull();
        var funcDesc = funcDescByName!.Value;

        funcDesc.elemdescFunc.tdesc.vt.Should().Be(typeof(string[]).GetShortVarEnum());
        funcDesc.elemdescFunc.tdesc.lpValue.Should().NotBeNull();
        funcDesc.cParams.Should().Be(0);

        var typeDesc = Marshal.PtrToStructure<TYPEDESC>(funcDesc.elemdescFunc.tdesc.lpValue);
        typeDesc.vt.Should().Be(typeof(string).GetShortVarEnum());
    }

    [Fact]
    public void IDispatchMethodWithVoidReturnValueAndArrayParameter_HasSafeArrayParameter()
    {
        var result =
            CreateAssembly()
                .WithInterface("TestInterface")
                    .WithCustomAttribute<InterfaceTypeAttribute>(ComInterfaceType.InterfaceIsIDispatch)
                    .WithMethod("TestMethod")
                        .WithReturnType(typeof(void))
                        .WithParameter<string[]>()
                    .Build()
                .Build()
            .Build();

        //check for interface
        var typeInfo = result.TypeLib.GetTypeInfoByName("TestInterface");
        typeInfo.Should().NotBeNull("TestInterface not found");

        //check for method
        using var funcDescByName = typeInfo!.GetFuncDescByName("TestMethod");
        funcDescByName.Should().NotBeNull();
        var funcDesc = funcDescByName!.Value;

        funcDesc.Should().NotBeNull();
        funcDesc.cParams.Should().Be(1);
        funcDesc.lprgelemdescParam.Should().NotBeNull();
        var parameter = Marshal.PtrToStructure<ELEMDESC>(funcDesc.lprgelemdescParam);

        parameter.tdesc!.GetVarEnum().Should().Be(typeof(string[]).GetVarEnum());
        parameter.tdesc.lpValue.Should().NotBeNull();

        var typeDesc = Marshal.PtrToStructure<TYPEDESC>(parameter.tdesc.lpValue);
        typeDesc.GetVarEnum().Should().Be(typeof(string).GetVarEnum());
    }

    [Fact]
    public void MethodWithInvisibleClassReturnValueAndNoParameter_ReturnValueIsVT_UNKNOWN()
    {
        var typeBuilder = CreateAssembly()
            .WithClass("ClassName", Array.Empty<string>())
            .WithCustomAttribute<ComVisibleAttribute>(false)
            .Build(out var createdClassType);
        var result = typeBuilder
            .WithInterface("TestInterface")
                .WithCustomAttribute<ComVisibleAttribute>(true)
                .WithCustomAttribute<InterfaceTypeAttribute>(ComInterfaceType.InterfaceIsIDispatch)
                .WithMethod("TestMethod")
                    .WithReturnType(createdClassType!)
                .Build()
            .Build()
        .Build();

        //check for interface
        var typeInfo = result.TypeLib.GetTypeInfoByName("TestInterface");
        typeInfo.Should().NotBeNull("TestInterface not found");

        // Check method
        using var funcDesc = typeInfo!.GetFuncDescByName("TestMethod");
        funcDesc.Should().NotBeNull();

        funcDesc!.Value.elemdescFunc.tdesc.GetVarEnum().Should().Be(VarEnum.VT_UNKNOWN);
    }

    [Fact]
    public void MethodOverload_GeneratedMethod_HasSuffix()
    {
        var result = CreateAssembly()
                        .WithInterface("TestInterface").WithCustomAttribute<InterfaceTypeAttribute>(ComInterfaceType.InterfaceIsIDispatch)
                            .WithMethod("TestMethod")
                                .WithReturnType(typeof(void))
                                .WithParameter<string>()
                            .Build()
                            .WithMethod("TestMethod")
                                .WithReturnType(typeof(void))
                                .WithParameter<string>()
                                .WithParameter<string>()
                            .Build()
                            .WithMethod("TestMethod")
                                .WithReturnType(typeof(void))
                                .WithParameter<string>()
                                .WithParameter<string>()
                                .WithParameter<string>()
                            .Build()
                        .Build()
                    .Build();

        using var method1 = result.TypeLib.GetTypeInfoByName("TestInterface")?.GetFuncDescByName("TestMethod");
        method1.Should().NotBeNull();

        using var method2 = result.TypeLib.GetTypeInfoByName("TestInterface")?.GetFuncDescByName("TestMethod_2");
        method1.Should().NotBeNull();

        using var method3 = result.TypeLib.GetTypeInfoByName("TestInterface")?.GetFuncDescByName("TestMethod_3");
        method1.Should().NotBeNull();
    }

    [Theory]
    [InlineData(ComInterfaceType.InterfaceIsDual)]
    [InlineData(ComInterfaceType.InterfaceIsIDispatch)]
    [InlineData(ComInterfaceType.InterfaceIsIUnknown)]
    public void IUnkownMethodWithMarshalAsUnmangedTypeErrorAndPreserveSigWithRefParameter_ParameterFlagsIsFINAndFOUT(ComInterfaceType interfaceType)
    {
        var result = CreateAssembly(new AssemblyName($"Dynamic{interfaceType}"))
                        .WithInterface("TestInterface")
                            .WithCustomAttribute<InterfaceTypeAttribute>(interfaceType)
                            .WithMethod("TestMethod")
                                .WithReturnTypeCustomAttribute<MarshalAsAttribute>(UnmanagedType.Error)
                                .WithCustomAttribute<PreserveSigAttribute>()
                                .WithReturnType<int>()
                                .WithParameter(typeof(string).MakeByRefType())
                            .Build()
                       .Build()
                    .Build();

        using var function = result.TypeLib.GetTypeInfoByName("TestInterface")!.GetFuncDescByName("TestMethod");
        function.Should().NotBeNull();

        var parameter = function!.Value.GetParameter(0);
        parameter.Should().NotBeNull("Parameter should be present");

        parameter!.Value.desc.paramdesc.wParamFlags.Should().Be(PARAMFLAG.PARAMFLAG_FOUT | PARAMFLAG.PARAMFLAG_FIN);
        parameter!.Value.tdesc.GetVarEnum().Should().Be(VarEnum.VT_PTR);
    }

    [Theory]
    [InlineData(ComInterfaceType.InterfaceIsDual)]
    [InlineData(ComInterfaceType.InterfaceIsIDispatch)]
    [InlineData(ComInterfaceType.InterfaceIsIUnknown)]
    public void MethodWithArrayOfUserDefinedTypeParameter_UsesTDescVT_SAFEARRAY_VT_PTR_VT_USERDEFINED(ComInterfaceType interfaceType)
    {
        var result = CreateAssembly(new AssemblyName($"Dynamic{interfaceType}"))
                        .WithInterface("TestInterfaceUsedInParameter")
                        .Build(out var interfaceUsedInParameter)
                        .WithInterface("TestInterface")
                            .WithMethod("TestMethod")
                                .WithParameter(interfaceUsedInParameter!.MakeArrayType())
                            .Build()
                       .Build()
                    .Build();

        //check for interface
        var typeInfo = result.TypeLib.GetTypeInfoByName("TestInterface");
        typeInfo.Should().NotBeNull("TestInterface not found");

        //check for method
        using var funcDescByName = typeInfo!.GetFuncDescByName("TestMethod");
        funcDescByName.Should().NotBeNull();
        var funcDesc = funcDescByName!.Value;

        funcDesc.Should().NotBeNull();
        funcDesc.cParams.Should().Be(1);
        funcDesc.lprgelemdescParam.Should().NotBeNull();
        var parameter = Marshal.PtrToStructure<ELEMDESC>(funcDesc.lprgelemdescParam);

        parameter.tdesc.vt.Should().Be((short)VarEnum.VT_SAFEARRAY);
        parameter.tdesc.lpValue.Should().NotBeNull();

        var typeDesc = Marshal.PtrToStructure<TYPEDESC>(parameter.tdesc.lpValue);
        typeDesc.GetVarEnum().Should().Be(VarEnum.VT_PTR);

        var typeDescUserDefined = Marshal.PtrToStructure<TYPEDESC>(typeDesc.lpValue);
        typeDescUserDefined.vt.Should().Be((short)VarEnum.VT_USERDEFINED);

        var typeInfo64Bit = (ITypeInfo64Bit)typeInfo!;
        typeInfo64Bit.GetRefTypeInfo(typeDescUserDefined.lpValue, out var refTypeInfo64Bit);
        refTypeInfo64Bit.GetDocumentation(-1, out var typeInfoName, out var docString, out var helpContext, out var helpFile);

        typeInfoName.Should().Be("TestInterfaceUsedInParameter");
    }


    [Fact]
    public void IUnkownMethodWithReturnValueArrayOfUserDefinedTypes_Returns_VT_PTR_VT_SAFEARRAY_VT_PTR_VT_USERDEFINED()
    {
        var result = CreateAssembly()
                        .WithInterface("TestInterfaceReturnValue")
                            .WithCustomAttribute<InterfaceTypeAttribute>(ComInterfaceType.InterfaceIsIUnknown)
                        .Build(out var interfaceUsedInParameter)
                        .WithInterface("TestInterface")
                            .WithCustomAttribute<InterfaceTypeAttribute>(ComInterfaceType.InterfaceIsIUnknown)
                            .WithMethod("TestMethod")
                                .WithReturnType(interfaceUsedInParameter!.MakeArrayType())
                            .Build()
                       .Build()
                    .Build();

        //check for interface
        var typeInfo = result.TypeLib.GetTypeInfoByName("TestInterface");
        typeInfo.Should().NotBeNull("TestInterface not found");

        //check for method
        using var funcDescByName = typeInfo!.GetFuncDescByName("TestMethod");
        funcDescByName.Should().NotBeNull();
        var funcDesc = funcDescByName!.Value;

        funcDesc.Should().NotBeNull();
        funcDesc.cParams.Should().Be(1);
        funcDesc.lprgelemdescParam.Should().NotBeNull();
        var parameter = Marshal.PtrToStructure<ELEMDESC>(funcDesc.lprgelemdescParam);

        parameter.tdesc.GetVarEnum().Should().Be(VarEnum.VT_PTR);

        var typeDesc = Marshal.PtrToStructure<TYPEDESC>(parameter.tdesc.lpValue);
        typeDesc.GetVarEnum().Should().Be(VarEnum.VT_SAFEARRAY);

        var typeDesc2 = Marshal.PtrToStructure<TYPEDESC>(typeDesc.lpValue);
        typeDesc2.GetVarEnum().Should().Be(VarEnum.VT_PTR);

        var typeDesc3 = Marshal.PtrToStructure<TYPEDESC>(typeDesc2.lpValue);
        typeDesc3.GetVarEnum().Should().Be(VarEnum.VT_USERDEFINED);

        var typeInfo64Bit = (ITypeInfo64Bit)typeInfo!;
        typeInfo64Bit.GetRefTypeInfo(typeDesc3.lpValue, out var refTypeInfo64Bit);
        refTypeInfo64Bit.GetDocumentation(-1, out var typeInfoName, out var docString, out var helpContext, out var helpFile);

        typeInfoName.Should().Be("TestInterfaceReturnValue");
    }

    [Theory]
    [InlineData(ComInterfaceType.InterfaceIsDual)]
    [InlineData(ComInterfaceType.InterfaceIsIDispatch)]
    public void MethodWithReturnValueArrayOfUserDefinedType_ReturnsSAFEARRAY_VT_PTR_VT_USERDEFINED(ComInterfaceType interfaceType)
    {
        var result = CreateAssembly(new AssemblyName($"Dynamic{interfaceType}"))
                        .WithInterface("TestInterfaceUsedInParameter")
                        .Build(out var interfaceUsedInParameter)
                        .WithInterface("TestInterface")
                            .WithMethod("TestMethod")
                                .WithParameter(interfaceUsedInParameter!.MakeArrayType())
                            .Build()
                       .Build()
                    .Build();

        //check for interface
        var typeInfo = result.TypeLib.GetTypeInfoByName("TestInterface");
        typeInfo.Should().NotBeNull("TestInterface not found");

        //check for method
        using var funcDescByName = typeInfo!.GetFuncDescByName("TestMethod");
        funcDescByName.Should().NotBeNull();
        var funcDesc = funcDescByName!.Value;

        funcDesc.Should().NotBeNull();
        funcDesc.cParams.Should().Be(1);
        funcDesc.lprgelemdescParam.Should().NotBeNull();
        var parameter = Marshal.PtrToStructure<ELEMDESC>(funcDesc.lprgelemdescParam);

        ((VarEnum)parameter.tdesc.vt).Should().Be(VarEnum.VT_SAFEARRAY);
        parameter.tdesc.lpValue.Should().NotBeNull();

        var typeDesc = Marshal.PtrToStructure<TYPEDESC>(parameter.tdesc.lpValue);
        ((VarEnum)typeDesc.vt).Should().Be(VarEnum.VT_PTR);

        var typeDescUserDefined = Marshal.PtrToStructure<TYPEDESC>(typeDesc.lpValue);
        ((VarEnum)typeDescUserDefined.vt).Should().Be(VarEnum.VT_USERDEFINED);

        var typeInfo64Bit = (ITypeInfo64Bit)typeInfo!;
        typeInfo64Bit.GetRefTypeInfo(typeDescUserDefined.lpValue, out var refTypeInfo64Bit);
        refTypeInfo64Bit.GetDocumentation(-1, out var typeInfoName, out var docString, out var helpContext, out var helpFile);

        typeInfoName.Should().Be("TestInterfaceUsedInParameter");
    }

    [Theory]
    [InlineData(ComInterfaceType.InterfaceIsDual)]
    [InlineData(ComInterfaceType.InterfaceIsIDispatch)]
    [InlineData(ComInterfaceType.InterfaceIsIUnknown)]
    public void MethodWithComVisibleFalseParameter_FirstParameterIsIUnkown(ComInterfaceType interfaceType)
    {
        var result = CreateAssembly(new AssemblyName($"Dynamic{interfaceType}"))
                        .WithInterface("InvisibleInterface")
                            .WithCustomAttribute<ComVisibleAttribute>(false)
                            .Build(out var invisibleInterface)
                        .WithInterface("TestInterface")
                            .WithCustomAttribute<InterfaceTypeAttribute>(interfaceType)
                            .WithMethod("TestMethod")
                                .WithParameter(invisibleInterface!)
                            .Build()
                        .Build()
                    .Build();

        //check for interface
        var typeInfo = result.TypeLib.GetTypeInfoByName("TestInterface");
        typeInfo.Should().NotBeNull("TestInterface not found");

        //check for method
        using var funcDescByName = typeInfo!.GetFuncDescByName("TestMethod");
        funcDescByName.Should().NotBeNull();

        // Get first parameter
        funcDescByName!.Value.cParams.Should().Be(1);
        funcDescByName!.Value.lprgelemdescParam.Should().NotBeNull();
        var parameter = Marshal.PtrToStructure<ELEMDESC>(funcDescByName!.Value.lprgelemdescParam);

        //First parameter should be VT_UNKNOWN
        parameter.tdesc.GetVarEnum().Should().Be(VarEnum.VT_UNKNOWN);
    }

    [Fact]
    public void MethodWithComVisibleFalseReturnParameter_IsPtrToUnknown()
    {
        var result = CreateAssembly()
            .WithInterface("InvisibleInterface")
                .WithCustomAttribute<ComVisibleAttribute>(false)
            .Build(out var invisibleType)
            .WithInterface("TestInterface")
                .WithCustomAttribute<InterfaceTypeAttribute>(ComInterfaceType.InterfaceIsIUnknown)
                .WithMethod("ReturnInvisibleInterface")
                    .WithReturnType(invisibleType!)
                .Build()
            .Build()
        .Build();

        //check for interface
        var typeInfo = result.TypeLib.GetTypeInfoByName("TestInterface");
        typeInfo.Should().NotBeNull("TestInterface not found");

        //check for method
        using var funcDescByName = typeInfo!.GetFuncDescByName("ReturnInvisibleInterface");
        funcDescByName.Should().NotBeNull();
        var funcDesc = funcDescByName!.Value;

        funcDesc.Should().NotBeNull();
        funcDesc.cParams.Should().Be(1);
        funcDesc.lprgelemdescParam.Should().NotBeNull();
        var parameter = Marshal.PtrToStructure<ELEMDESC>(funcDesc.lprgelemdescParam);

        parameter.tdesc.GetVarEnum().Should().Be(VarEnum.VT_PTR);
        var childTypeDesc = Marshal.PtrToStructure<TYPEDESC>(parameter.tdesc.lpValue);
        childTypeDesc.GetVarEnum().Should().Be(VarEnum.VT_UNKNOWN);
    }

    [Theory]
    [InlineData(typeof(byte))]
    [InlineData(typeof(sbyte))]
    [InlineData(typeof(short))]
    [InlineData(typeof(ushort))]
    [InlineData(typeof(uint))]
    [InlineData(typeof(int))]
    [InlineData(typeof(ulong))]
    [InlineData(typeof(long))]
    [InlineData(typeof(float))]
    [InlineData(typeof(double))]
    [InlineData(typeof(string))]
    [InlineData(typeof(bool))]
    [InlineData(typeof(char))]
    [InlineData(typeof(object))]
    [InlineData(typeof(object[]))]
    [InlineData(typeof(IEnumerator))]
    [InlineData(typeof(DateTime))]
    [InlineData(typeof(Guid))]
    [InlineData(typeof(System.Drawing.Color))]
    [InlineData(typeof(decimal))]
    public void InterfaceIsIUnknownWithMethodWithOutParameter_OutParameterIsCorrect(Type parameterType)
    {
        var result = CreateAssembly(new AssemblyName($"Dynamic{parameterType}"))
                        .WithInterface("TestInterface")
                            .WithCustomAttribute<InterfaceTypeAttribute>(ComInterfaceType.InterfaceIsIUnknown)
                            .WithMethod("TestMethod")
                                .WithParameter(new DynamicMethodBuilder.ParameterItem { Type = parameterType, ParameterAttributes = ParameterAttributes.Out })
                            .Build()
                        .Build()
                    .Build();

        //check for interface
        var typeInfo = result.TypeLib.GetTypeInfoByName("TestInterface");
        typeInfo.Should().NotBeNull("TestInterface not found");

        //check for method
        using var funcDescByName = typeInfo!.GetFuncDescByName("TestMethod");
        funcDescByName.Should().NotBeNull();
        var funcDesc = funcDescByName!.Value;

        // Check number of parameters
        funcDesc.cParams.Should().Be(1);

        // Get first parameter
        var parameter = funcDesc.GetParameter(0);
        parameter.Should().NotBeNull();

        // type should be Ptr
        var ptr = parameter!.Value.tdesc;
        ptr.GetVarEnum().Should().Be(parameterType.GetVarEnum());

        parameter!.Value.desc.idldesc.wIDLFlags.Should().Be(IDLFLAG.IDLFLAG_FOUT);
        parameter!.Value.desc.paramdesc.wParamFlags.Should().Be(PARAMFLAG.PARAMFLAG_FOUT);
    }

    [Theory]
    [InlineData(typeof(byte))]
    [InlineData(typeof(sbyte))]
    [InlineData(typeof(short))]
    [InlineData(typeof(ushort))]
    [InlineData(typeof(uint))]
    [InlineData(typeof(int))]
    [InlineData(typeof(ulong))]
    [InlineData(typeof(long))]
    [InlineData(typeof(float))]
    [InlineData(typeof(double))]
    [InlineData(typeof(string))]
    [InlineData(typeof(bool))]
    [InlineData(typeof(char))]
    [InlineData(typeof(object))]
    [InlineData(typeof(object[]))]
    [InlineData(typeof(IEnumerator))]
    [InlineData(typeof(DateTime))]
    [InlineData(typeof(Guid))]
    [InlineData(typeof(System.Drawing.Color))]
    [InlineData(typeof(decimal))]
    public void InterfaceIsIDispatchWithMethodWithOutParameter_OutParameterIsCorrect(Type parameterType)
    {
        var result = CreateAssembly(new AssemblyName($"Dynamic{parameterType}"))
                        .WithInterface("TestInterface")
                            .WithCustomAttribute<InterfaceTypeAttribute>(ComInterfaceType.InterfaceIsIDispatch)
                            .WithMethod("TestMethod")
                                .WithParameter(new DynamicMethodBuilder.ParameterItem { Type = parameterType, ParameterAttributes = ParameterAttributes.Out })
                            .Build()
                        .Build()
                    .Build();

        //check for interface
        var typeInfo = result.TypeLib.GetTypeInfoByName("TestInterface");
        typeInfo.Should().NotBeNull("TestInterface not found");

        //check for method
        using var funcDescByName = typeInfo!.GetFuncDescByName("TestMethod");
        funcDescByName.Should().NotBeNull();
        var funcDesc = funcDescByName!.Value;

        // Check number of parameters
        funcDesc.cParams.Should().Be(1);

        // Get first parameter
        var parameter = funcDesc.GetParameter(0);
        parameter.Should().NotBeNull();

        // type should be Ptr
        var ptr = parameter!.Value.tdesc;
        ptr.GetVarEnum().Should().Be(parameterType.GetVarEnum());

        parameter!.Value.desc.idldesc.wIDLFlags.Should().Be(IDLFLAG.IDLFLAG_FOUT);
        parameter!.Value.desc.paramdesc.wParamFlags.Should().Be(PARAMFLAG.PARAMFLAG_FOUT);
    }

    [Theory]
    [InlineData(typeof(byte))]
    [InlineData(typeof(sbyte))]
    [InlineData(typeof(short))]
    [InlineData(typeof(ushort))]
    [InlineData(typeof(uint))]
    [InlineData(typeof(int))]
    [InlineData(typeof(ulong))]
    [InlineData(typeof(long))]
    [InlineData(typeof(float))]
    [InlineData(typeof(double))]
    [InlineData(typeof(string))]
    [InlineData(typeof(bool))]
    [InlineData(typeof(char))]
    [InlineData(typeof(object))]
    [InlineData(typeof(object[]))]
    [InlineData(typeof(IEnumerator))]
    [InlineData(typeof(DateTime))]
    [InlineData(typeof(Guid))]
    [InlineData(typeof(System.Drawing.Color))]
    [InlineData(typeof(decimal))]
    public void InterfaceIsDualWithMethodWithOutParameter_OutParameterIsCorrect(Type parameterType)
    {
        var result = CreateAssembly(new AssemblyName($"Dynamic{parameterType}"))
                        .WithInterface("TestInterface")
                            .WithCustomAttribute<InterfaceTypeAttribute>(ComInterfaceType.InterfaceIsDual)
                            .WithMethod("TestMethod")
                                .WithParameter(new DynamicMethodBuilder.ParameterItem { Type = parameterType, ParameterAttributes = ParameterAttributes.Out })
                            .Build()
                        .Build()
                    .Build();

        //check for interface
        var typeInfo = result.TypeLib.GetTypeInfoByName("TestInterface");
        typeInfo.Should().NotBeNull("TestInterface not found");

        //check for method
        using var funcDescByName = typeInfo!.GetFuncDescByName("TestMethod");
        funcDescByName.Should().NotBeNull();
        var funcDesc = funcDescByName!.Value;

        // Check number of parameters
        funcDesc.cParams.Should().Be(1);

        // Get first parameter
        var parameter = funcDesc.GetParameter(0);
        parameter.Should().NotBeNull();

        // type should be Ptr
        var ptr = parameter!.Value.tdesc;
        ptr.GetVarEnum().Should().Be(parameterType.GetVarEnum());

        parameter!.Value.desc.idldesc.wIDLFlags.Should().Be(IDLFLAG.IDLFLAG_FOUT);
        parameter!.Value.desc.paramdesc.wParamFlags.Should().Be(PARAMFLAG.PARAMFLAG_FOUT);
    }

    [Theory]
    [InlineData(typeof(byte))]
    [InlineData(typeof(sbyte))]
    [InlineData(typeof(short))]
    [InlineData(typeof(ushort))]
    [InlineData(typeof(uint))]
    [InlineData(typeof(int))]
    [InlineData(typeof(ulong))]
    [InlineData(typeof(long))]
    [InlineData(typeof(float))]
    [InlineData(typeof(double))]
    [InlineData(typeof(string))]
    [InlineData(typeof(bool))]
    [InlineData(typeof(char))]
    [InlineData(typeof(object))]
    [InlineData(typeof(IEnumerator))]
    [InlineData(typeof(DateTime))]
    [InlineData(typeof(Guid))]
    [InlineData(typeof(System.Drawing.Color))]
    [InlineData(typeof(decimal))]
    public void InterfaceIsIUnknownWithMethodWithArrayOutParameter_OutParameterIsCorrect(Type parameterType)
    {
        var result = CreateAssembly(new AssemblyName($"Dynamic{parameterType}"))
                        .WithInterface("TestInterface")
                            .WithCustomAttribute<InterfaceTypeAttribute>(ComInterfaceType.InterfaceIsIUnknown)
                            .WithMethod("TestMethod")
                                .WithParameter(new DynamicMethodBuilder.ParameterItem { Type = parameterType.MakeArrayType(), ParameterAttributes = ParameterAttributes.Out })
                            .Build()
                        .Build()
                    .Build();

        //check for interface
        var typeInfo = result.TypeLib.GetTypeInfoByName("TestInterface");
        typeInfo.Should().NotBeNull("TestInterface not found");

        //check for method
        using var funcDescByName = typeInfo!.GetFuncDescByName("TestMethod");
        funcDescByName.Should().NotBeNull();
        var funcDesc = funcDescByName!.Value;

        // Check number of parameters
        funcDesc.cParams.Should().Be(1);

        // Get first parameter
        var parameter = funcDesc.GetParameter(0);
        parameter.Should().NotBeNull();
    }

    [Theory]
    [InlineData(typeof(byte))]
    [InlineData(typeof(sbyte))]
    [InlineData(typeof(short))]
    [InlineData(typeof(ushort))]
    [InlineData(typeof(uint))]
    [InlineData(typeof(int))]
    [InlineData(typeof(ulong))]
    [InlineData(typeof(long))]
    [InlineData(typeof(float))]
    [InlineData(typeof(double))]
    [InlineData(typeof(string))]
    [InlineData(typeof(bool))]
    [InlineData(typeof(char))]
    [InlineData(typeof(object))]
    [InlineData(typeof(IEnumerator))]
    [InlineData(typeof(DateTime))]
    [InlineData(typeof(Guid))]
    [InlineData(typeof(System.Drawing.Color))]
    [InlineData(typeof(decimal))]
    public void InterfaceIsIDispatchWithMethodWithArrayOutParameter_OutParameterIsCorrect(Type parameterType)
    {
        var result = CreateAssembly(new AssemblyName($"Dynamic{parameterType}"))
                        .WithInterface("TestInterface")
                            .WithCustomAttribute<InterfaceTypeAttribute>(ComInterfaceType.InterfaceIsIDispatch)
                            .WithMethod("TestMethod")
                                .WithParameter(new DynamicMethodBuilder.ParameterItem { Type = parameterType.MakeArrayType(), ParameterAttributes = ParameterAttributes.Out })
                            .Build()
                        .Build()
                    .Build();

        //check for interface
        var typeInfo = result.TypeLib.GetTypeInfoByName("TestInterface");
        typeInfo.Should().NotBeNull("TestInterface not found");

        //check for method
        using var funcDescByName = typeInfo!.GetFuncDescByName("TestMethod");
        funcDescByName.Should().NotBeNull();
        var funcDesc = funcDescByName!.Value;

        // Check number of parameters
        funcDesc.cParams.Should().Be(1);

        // Get first parameter
        var parameter = funcDesc.GetParameter(0);
        parameter.Should().NotBeNull();
    }

    [Theory]
    [InlineData(typeof(byte))]
    [InlineData(typeof(sbyte))]
    [InlineData(typeof(short))]
    [InlineData(typeof(ushort))]
    [InlineData(typeof(uint))]
    [InlineData(typeof(int))]
    [InlineData(typeof(ulong))]
    [InlineData(typeof(long))]
    [InlineData(typeof(float))]
    [InlineData(typeof(double))]
    [InlineData(typeof(string))]
    [InlineData(typeof(bool))]
    [InlineData(typeof(char))]
    [InlineData(typeof(object))]
    [InlineData(typeof(IEnumerator))]
    [InlineData(typeof(DateTime))]
    [InlineData(typeof(Guid))]
    [InlineData(typeof(System.Drawing.Color))]
    [InlineData(typeof(decimal))]
    public void InterfaceIsDualWithMethodWithArrayOutParameter_OutParameterIsCorrect(Type parameterType)
    {
        var result = CreateAssembly(new AssemblyName($"Dynamic{parameterType}"))
                        .WithInterface("TestInterface")
                            .WithCustomAttribute<InterfaceTypeAttribute>(ComInterfaceType.InterfaceIsDual)
                            .WithMethod("TestMethod")
                                .WithParameter(new DynamicMethodBuilder.ParameterItem { Type = parameterType.MakeArrayType(), ParameterAttributes = ParameterAttributes.Out })
                            .Build()
                        .Build()
                    .Build();

        //check for interface
        var typeInfo = result.TypeLib.GetTypeInfoByName("TestInterface");
        typeInfo.Should().NotBeNull("TestInterface not found");

        //check for method
        using var funcDescByName = typeInfo!.GetFuncDescByName("TestMethod");
        funcDescByName.Should().NotBeNull();
        var funcDesc = funcDescByName!.Value;

        // Check number of parameters
        funcDesc.cParams.Should().Be(1);

        // Get first parameter
        var parameter = funcDesc.GetParameter(0);
        parameter.Should().NotBeNull();
    }

    [Theory]
    [InlineData(ComInterfaceType.InterfaceIsDual)]
    [InlineData(ComInterfaceType.InterfaceIsIDispatch)]
    [InlineData(ComInterfaceType.InterfaceIsIUnknown)]
    public void InterfaceWithMethodWithArrayOutParameteMakeByRefTypeOfUserDefine_OutParameterIsCorrect(ComInterfaceType comInterfaceType)
    {
        var result = CreateAssembly(new AssemblyName($"Dynamic{comInterfaceType}"))
                        .WithInterface("CustomInterface")
                            .WithCustomAttribute<InterfaceTypeAttribute>(comInterfaceType)
                        .Build(out var customInterface)
                        .WithInterface("TestInterface")
                            .WithCustomAttribute<InterfaceTypeAttribute>(comInterfaceType)
                            .WithMethod("TestMethod")
                                .WithParameter(new DynamicMethodBuilder.ParameterItem { Type = customInterface!.MakeArrayType().MakeByRefType(), ParameterAttributes = ParameterAttributes.Out })
                            .Build()
                        .Build()
                    .Build();

        //check for interface
        var typeInfo = result.TypeLib.GetTypeInfoByName("TestInterface");
        typeInfo.Should().NotBeNull("TestInterface not found");

        //check for method
        using var funcDescByName = typeInfo!.GetFuncDescByName("TestMethod");
        funcDescByName.Should().NotBeNull();
        var funcDesc = funcDescByName!.Value;

        // Check number of parameters
        funcDesc.cParams.Should().Be(1);

        // Get first parameter
        var parameter = funcDesc.GetParameter(0);
        parameter.Should().NotBeNull();
    }

    [Theory]
    [InlineData(typeof(byte))]
    [InlineData(typeof(sbyte))]
    [InlineData(typeof(short))]
    [InlineData(typeof(ushort))]
    [InlineData(typeof(uint))]
    [InlineData(typeof(int))]
    [InlineData(typeof(ulong))]
    [InlineData(typeof(long))]
    [InlineData(typeof(float))]
    [InlineData(typeof(double))]
    [InlineData(typeof(string))]
    [InlineData(typeof(bool))]
    [InlineData(typeof(char))]
    [InlineData(typeof(object))]
    [InlineData(typeof(IEnumerator))]
    [InlineData(typeof(DateTime))]
    [InlineData(typeof(Guid))]
    [InlineData(typeof(System.Drawing.Color))]
    [InlineData(typeof(decimal))]
    public void InterfaceIsDualWithMethodWithRefParameter_RefParameterIsCorrect(Type parameterType)
    {
        var result = CreateAssembly(new AssemblyName($"Dynamic{parameterType}"))
                        .WithInterface("TestInterface")
                            .WithCustomAttribute<InterfaceTypeAttribute>(ComInterfaceType.InterfaceIsDual)
                            .WithMethod("TestMethod")
                                .WithParameter(parameterType.MakeByRefType())
                            .Build()
                        .Build()
                    .Build();

        //check for interface
        var typeInfo = result.TypeLib.GetTypeInfoByName("TestInterface");
        typeInfo.Should().NotBeNull("TestInterface not found");

        //check for method
        using var funcDescByName = typeInfo!.GetFuncDescByName("TestMethod");
        funcDescByName.Should().NotBeNull();
        var funcDesc = funcDescByName!.Value;

        // Check number of parameters
        funcDesc.cParams.Should().Be(1);

        // Get first parameter
        var parameter = funcDesc.GetParameter(0);
        parameter.Should().NotBeNull();

        parameter!.Value.tdesc.GetVarEnum().Should().Be(VarEnum.VT_PTR);

        var subtypeDesc = Marshal.PtrToStructure<TYPEDESC>(parameter!.Value.tdesc.lpValue);
        subtypeDesc.GetVarEnum().Should().Be(parameterType.GetVarEnum());

        if (parameterType.IsEnum || parameterType.IsInterface)
        {
            var subsubtypeDesc = Marshal.PtrToStructure<TYPEDESC>(subtypeDesc.lpValue);
            subsubtypeDesc.GetVarEnum().Should().Be(VarEnum.VT_USERDEFINED);
        }
    }

    [Fact]
    public void MethodWithOptionalObjectArrayParameter_MethodIsAvailable()
    {
        var result = CreateAssembly()
                        .WithInterface("CustomInterface").Build(out var customInterface)
                        .WithInterface("TestInterface")
                            .WithMethod("TestMethod")
                                .WithParameter(new DynamicMethodBuilder.ParameterItem()
                                {
                                    ParameterAttributes = ParameterAttributes.Optional | ParameterAttributes.HasDefault,
                                    DefaultValue = new DynamicMethodBuilder.DefaultValue() { Value = null },
                                    Type = customInterface!.MakeArrayType(),
                                })
                            .Build()
                        .Build()
                    .Build();

        var type = result.TypeLib.GetTypeInfoByName("TestInterface");
        type.Should().NotBeNull();

        type!.GetFuncDescByName("TestMethod").Should().NotBeNull();
    }

    [Fact]
    public void DualInterfaceWithAddRefMethod_NewAddRefMethodShouldUseSuffix()
    {
        var result = CreateAssembly()
                        .WithInterface("TestInterface")
                            .WithCustomAttribute<InterfaceTypeAttribute>(ComInterfaceType.InterfaceIsDual)
                            .WithMethod("AddRef")
                            .Build()
                        .Build()
                    .Build();

        var type = result.TypeLib.GetTypeInfoByName("TestInterface");
        type.Should().NotBeNull();

        type!.GetFuncDescByName("AddRef").Should().NotBeNull();
        type!.GetFuncDescByName("AddRef_2").Should().NotBeNull();
        type!.GetFuncDescByName("AddRef_3").Should().BeNull();
    }

    [Theory]
    [InlineData(typeof(byte), 1)]
    [InlineData(typeof(sbyte), 1)]
    [InlineData(typeof(short), 1)]
    [InlineData(typeof(ushort), 1)]
    [InlineData(typeof(uint), 1)]
    [InlineData(typeof(int), 1)]
    [InlineData(typeof(ulong), 1)]
    [InlineData(typeof(long), 1)]
    [InlineData(typeof(string), "Test")]
    [InlineData(typeof(bool), true)]
    public void MethodWithDefaultParameterValueAttribute_DefaultValueIsUsed(Type parameterType, object defaultValue)
    {
        var result = CreateAssembly(new AssemblyName($"Dynamic{parameterType}{defaultValue}"))
                        .WithInterface("TestInterface")
                            .WithMethod("TestMethod")
                                .WithParameter(new DynamicMethodBuilder.ParameterItem()
                                {
                                    ParameterAttributes = ParameterAttributes.HasDefault,
                                    DefaultValue = new DynamicMethodBuilder.DefaultValue() { Value = Convert.ChangeType(defaultValue, parameterType) },
                                    Type = parameterType,
                                })
                            .Build()
                        .Build()
                    .Build();

        //check for interface
        var typeInfo = result.TypeLib.GetTypeInfoByName("TestInterface");
        typeInfo.Should().NotBeNull("TestInterface not found");

        //check for method
        using var funcDesc = typeInfo!.GetFuncDescByName("TestMethod");
        funcDesc.Should().NotBeNull();
        funcDesc!.Value.lprgelemdescParam.Should().NotBeNull();

        // Get first parameter
        var parameter = funcDesc!.Value.GetParameter(0);
        parameter.Should().NotBeNull();

        // Check type
        var typeDesc = parameter!.Value.tdesc;
        typeDesc.GetVarEnum().Should().Be(parameterType.GetVarEnum());

        parameter!.Value.desc.paramdesc.wParamFlags.Should().Be(PARAMFLAG.PARAMFLAG_FIN | PARAMFLAG.PARAMFLAG_FHASDEFAULT);
    }
}
