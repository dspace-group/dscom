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
using System.Runtime.InteropServices;
using Xunit;

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
        Assert.NotNull(releasableFuncDesc);
        Assert.Equal(INVOKEKIND.INVOKE_FUNC, releasableFuncDesc!.Value.invkind);
        Assert.Equal(-4, releasableFuncDesc!.Value.memid);

        // Check that no unexpected warnings occurred 
        Assert.All(result.TypeLibExporterNotifySink.ReportedEvents, x => Assert.Equal(ExporterEventKind.NOTIF_TYPECONVERTED, x.EventKind));
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
        Assert.NotNull(typeInfo);

        // Search for method
        Assert.True(typeInfo!.ContainsFuncDescByName("TestMethod"));

        // Check that no unexpected warnings occurred 
        Assert.All(result.TypeLibExporterNotifySink.ReportedEvents, x => Assert.Equal(ExporterEventKind.NOTIF_TYPECONVERTED, x.EventKind));
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
        Assert.NotNull(typeInfo);

        // Get and check TestMethod
        using var funcDesc = typeInfo!.GetFuncDescByName("TestMethod");
        Assert.NotNull(funcDesc);

        // Check if return type if void
        Assert.Equal((short)VarEnum.VT_VOID, funcDesc!.Value.elemdescFunc.tdesc.vt);

        // Check that no unexpected warnings occurred 
        Assert.All(result.TypeLibExporterNotifySink.ReportedEvents, x => Assert.Equal(ExporterEventKind.NOTIF_TYPECONVERTED, x.EventKind));

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
    [InlineData(typeof(Delegate))]
    [InlineData(typeof(IntPtr))]
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
        Assert.NotNull(typeInfo);

        //check for method
        using var funcDescByName = typeInfo!.GetFuncDescByName("TestMethod");
        Assert.NotNull(funcDescByName);
        var funcDesc = funcDescByName!.Value;

        // Check number of parameters
        Assert.Equal(1, funcDesc.cParams);

        // Get first parameter
        var parameter = funcDesc.GetParameter(0);
        Assert.NotNull(parameter);

        // type should be Ptr
        var ptr = parameter!.Value.tdesc;
        Assert.Equal((short)VarEnum.VT_PTR, ptr.vt);

        // Ptr type should be the final typ
        var value = Marshal.PtrToStructure<TYPEDESC>(ptr.lpValue);
        Assert.Equal(returnType.GetVarEnum(), value.GetVarEnum());

        // Check that no unexpected warnings occurred 
        Assert.All(result.TypeLibExporterNotifySink.ReportedEvents, x => Assert.Equal(ExporterEventKind.NOTIF_TYPECONVERTED, x.EventKind));
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
    [InlineData(typeof(Delegate))]
    [InlineData(typeof(IntPtr))]
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
        Assert.NotNull(typeInfo);

        //check for method
        using var funcDescByName = typeInfo!.GetFuncDescByName("TestMethod");
        Assert.NotNull(funcDescByName);
        var funcDesc = funcDescByName!.Value;

        Assert.Equal(returnType.GetShortVarEnum(), funcDesc.elemdescFunc.tdesc.vt);

        // Check that no unexpected warnings occurred 
        Assert.All(result.TypeLibExporterNotifySink.ReportedEvents, x => Assert.Equal(ExporterEventKind.NOTIF_TYPECONVERTED, x.EventKind));
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
    [InlineData(typeof(Delegate))]
    [InlineData(typeof(IntPtr))]

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
        Assert.NotNull(typeInfo);

        //check for method
        typeInfo!.GetFuncDesc(7, out var ppFuncDesc);
        Assert.NotEqual(IntPtr.Zero, ppFuncDesc);

        var funcDesc = Marshal.PtrToStructure<FUNCDESC>(ppFuncDesc);
        Assert.Equal(returnType.GetShortVarEnum(), funcDesc.elemdescFunc.tdesc.vt);
        Assert.Equal(0, funcDesc.cParams);

        // Check that no unexpected warnings occurred 
        Assert.All(result.TypeLibExporterNotifySink.ReportedEvents, x => Assert.Equal(ExporterEventKind.NOTIF_TYPECONVERTED, x.EventKind));
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
        Assert.NotNull(typeInfo);

        //check for method
        using var funcDescByName = typeInfo!.GetFuncDescByName("TestMethod");
        Assert.NotNull(funcDescByName);
        var funcDesc = funcDescByName!.Value;

        typeInfo!.GetDocumentation(funcDesc.memid, out var methodString, out var docString, out var helpContext, out var helpFile);
        Assert.NotNull(docString);
        Assert.Equal("Description", docString);

        // Check that no unexpected warnings occurred 
        Assert.All(result.TypeLibExporterNotifySink.ReportedEvents, x => Assert.Equal(ExporterEventKind.NOTIF_TYPECONVERTED, x.EventKind));
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
                                .WithParameter(new ParameterItem { Type = parameterType, ParameterAttributes = ParameterAttributes.Optional | ParameterAttributes.HasDefault, DefaultValue = new DefaultValue(value) })
                            .Build()
                        .Build()
                     .Build();

        //check for interface
        var typeInfo = result.TypeLib.GetTypeInfoByName("TestInterface");
        Assert.NotNull(typeInfo);

        //check for method
        using var funcDesc = typeInfo!.GetFuncDescByName("TestMethod");
        Assert.NotNull(funcDesc);

        var firstParam = Marshal.PtrToStructure<ELEMDESC>(funcDesc.Value.lprgelemdescParam);
        Assert.Equal(parameterType.GetShortVarEnum(), firstParam.tdesc.vt);
        if (value != null)
        {
            Assert.NotEqual(IntPtr.Zero, firstParam.desc.paramdesc.lpVarValue);
            var defaultValue = Marshal.GetObjectForNativeVariant(firstParam.desc.paramdesc.lpVarValue + 8);
            Assert.NotNull(defaultValue);
            Assert.Equal(acceptedReturnType, defaultValue!.GetType());
            Assert.Equal(Convert.ChangeType(value, acceptedReturnType, CultureInfo.InvariantCulture), defaultValue);
        }

        // Check that no unexpected warnings occurred 
        Assert.All(result.TypeLibExporterNotifySink.ReportedEvents, x => Assert.Equal(ExporterEventKind.NOTIF_TYPECONVERTED, x.EventKind));
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
        Assert.NotNull(typeInfo);

        //check for method
        using var funcDesc = typeInfo!.GetFuncDescByName("TestMethod");
        Assert.NotNull(funcDesc);

        Assert.Equal(createdEnumType!.GetShortVarEnum(), funcDesc!.Value.elemdescFunc.tdesc.vt);
        Assert.Equal(0, funcDesc!.Value.cParams);

        ((ITypeInfo64Bit)typeInfo!).GetRefTypeInfo(funcDesc!.Value.elemdescFunc.tdesc.lpValue, out var typeInfoEnum);
        Assert.NotNull(typeInfoEnum);
        typeInfoEnum.GetDocumentation(-1, out var strName, out var strDocString, out var dwHelpContext, out var strHelpFile);
        Assert.NotNull(strName);
        Assert.Equal("TestEnum", strName);
        typeInfoEnum.GetTypeAttr(out var ppTypeAttr);
        var typeAttr = Marshal.PtrToStructure<TYPEATTR>(ppTypeAttr);
        Assert.Equal(TYPEKIND.TKIND_ENUM, typeAttr.typekind);

        // Check that no unexpected warnings occurred 
        Assert.All(result.TypeLibExporterNotifySink.ReportedEvents, x => Assert.Equal(ExporterEventKind.NOTIF_TYPECONVERTED, x.EventKind));
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
        Assert.NotNull(typeInfo);

        //check for method
        using var funcDesc = typeInfo!.GetFuncDescByName("TestMethod");
        Assert.NotNull(funcDesc);

        Assert.Equal(1, funcDesc!.Value.cParams);
        var parameter = Marshal.PtrToStructure<ELEMDESC>(funcDesc!.Value.lprgelemdescParam);

        Assert.Equal(createdEnumType!.GetShortVarEnum(), parameter.tdesc.vt);

        ((ITypeInfo64Bit)typeInfo!).GetRefTypeInfo(parameter.tdesc.lpValue, out var typeInfoEnum);
        Assert.NotNull(typeInfoEnum);
        typeInfoEnum.GetDocumentation(-1, out var strName, out var strDocString, out var dwHelpContext, out var strHelpFile);
        Assert.NotNull(strName);
        Assert.Equal("TestEnum", strName);
        typeInfoEnum.GetTypeAttr(out var ppTypeAttr);
        var typeAttr = Marshal.PtrToStructure<TYPEATTR>(ppTypeAttr);
        Assert.Equal(TYPEKIND.TKIND_ENUM, typeAttr.typekind);

        // Check that no unexpected warnings occurred 
        Assert.All(result.TypeLibExporterNotifySink.ReportedEvents, x => Assert.Equal(ExporterEventKind.NOTIF_TYPECONVERTED, x.EventKind));
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
        Assert.NotNull(typeInfo);

        //Check mathod
        using var funcDesc = typeInfo!.GetFuncDescByName("TestMethod");
        Assert.NotNull(funcDesc);

        Assert.Equal(createdInterfaceType!.GetShortVarEnum(), funcDesc!.Value.elemdescFunc.tdesc.vt);
        Assert.Equal(0, funcDesc!.Value.cParams);

        var typeDesc = Marshal.PtrToStructure<TYPEDESC>(funcDesc!.Value.elemdescFunc.tdesc.lpValue);
        Assert.Equal((short)VarEnum.VT_USERDEFINED, typeDesc.vt);
        ((ITypeInfo64Bit)typeInfo!).GetRefTypeInfo(typeDesc.lpValue, out var typeInfoEnum);
        Assert.NotNull(typeInfoEnum);
        typeInfoEnum.GetDocumentation(-1, out var strName, out var strDocString, out var dwHelpContext, out var strHelpFile);
        Assert.NotNull(strName);
        Assert.Equal("ReferencedInterface", strName);
        typeInfoEnum.GetTypeAttr(out var ppTypeAttr);
        var typeAttr = Marshal.PtrToStructure<TYPEATTR>(ppTypeAttr);
        Assert.Equal(TYPEKIND.TKIND_DISPATCH, typeAttr.typekind);

        // Check that no unexpected warnings occurred 
        Assert.All(result.TypeLibExporterNotifySink.ReportedEvents, x => Assert.Equal(ExporterEventKind.NOTIF_TYPECONVERTED, x.EventKind));
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
        Assert.NotNull(typeInfo);

        //check for method
        using var releasableFuncDesc = typeInfo!.GetFuncDescByName("TestMethod");
        Assert.NotNull(releasableFuncDesc);
        var funcDesc = releasableFuncDesc!.Value;

        Assert.Equal(1, funcDesc.cParams);
        var parameter = Marshal.PtrToStructure<ELEMDESC>(funcDesc.lprgelemdescParam);

        Assert.Equal(createdInterfaceType!.GetShortVarEnum(), parameter.tdesc.vt);

        var typeDesc = Marshal.PtrToStructure<TYPEDESC>(parameter.tdesc.lpValue);
        Assert.Equal((short)VarEnum.VT_USERDEFINED, typeDesc.vt);
        ((ITypeInfo64Bit)typeInfo!).GetRefTypeInfo(typeDesc.lpValue, out var typeInfoEnum);
        Assert.NotNull(typeInfoEnum);
        typeInfoEnum.GetDocumentation(-1, out var strName, out var strDocString, out var dwHelpContext, out var strHelpFile);
        Assert.NotNull(strName);
        Assert.Equal("ReferencedInterface", strName);
        typeInfoEnum.GetTypeAttr(out var ppTypeAttr);
        var typeAttr = Marshal.PtrToStructure<TYPEATTR>(ppTypeAttr);
        Assert.Equal(TYPEKIND.TKIND_DISPATCH, typeAttr.typekind);

        // Check that no unexpected warnings occurred 
        Assert.All(result.TypeLibExporterNotifySink.ReportedEvents, x => Assert.Equal(ExporterEventKind.NOTIF_TYPECONVERTED, x.EventKind));
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
        Assert.NotNull(typeInfo);

        //check for method
        using var funcDescByName = typeInfo!.GetFuncDescByName("TestMethod");
        Assert.NotNull(funcDescByName);
        var funcDesc = funcDescByName!.Value;

        Assert.Equal(typeof(string[]).GetShortVarEnum(), funcDesc.elemdescFunc.tdesc.vt);
        Assert.Equal(0, funcDesc.cParams);

        var typeDesc = Marshal.PtrToStructure<TYPEDESC>(funcDesc.elemdescFunc.tdesc.lpValue);
        Assert.Equal(typeof(string).GetShortVarEnum(), typeDesc.vt);

        // Check that no unexpected warnings occurred 
        Assert.All(result.TypeLibExporterNotifySink.ReportedEvents, x => Assert.Equal(ExporterEventKind.NOTIF_TYPECONVERTED, x.EventKind));
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
        Assert.NotNull(typeInfo);

        //check for method
        using var funcDescByName = typeInfo!.GetFuncDescByName("TestMethod");
        Assert.NotNull(funcDescByName);
        var funcDesc = funcDescByName!.Value;

        Assert.Equal(1, funcDesc.cParams);
        var parameter = Marshal.PtrToStructure<ELEMDESC>(funcDesc.lprgelemdescParam);

        Assert.Equal(typeof(string[]).GetVarEnum(), parameter.tdesc!.GetVarEnum());

        var typeDesc = Marshal.PtrToStructure<TYPEDESC>(parameter.tdesc.lpValue);
        Assert.Equal(typeof(string).GetVarEnum(), typeDesc.GetVarEnum());

        // Check that no unexpected warnings occurred 
        Assert.All(result.TypeLibExporterNotifySink.ReportedEvents, x => Assert.Equal(ExporterEventKind.NOTIF_TYPECONVERTED, x.EventKind));
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
        Assert.NotNull(typeInfo);

        // Check method
        using var funcDesc = typeInfo!.GetFuncDescByName("TestMethod");
        Assert.NotNull(funcDesc);

        Assert.Equal(VarEnum.VT_UNKNOWN, funcDesc!.Value.elemdescFunc.tdesc.GetVarEnum());

        // Check that no unexpected warnings occurred 
        Assert.All(result.TypeLibExporterNotifySink.ReportedEvents, x => Assert.Equal(ExporterEventKind.NOTIF_TYPECONVERTED, x.EventKind));
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
        Assert.NotNull(method1);

        using var method2 = result.TypeLib.GetTypeInfoByName("TestInterface")?.GetFuncDescByName("TestMethod_2");
        Assert.NotNull(method2);

        using var method3 = result.TypeLib.GetTypeInfoByName("TestInterface")?.GetFuncDescByName("TestMethod_3");
        Assert.NotNull(method3);

        // Check that no unexpected warnings occurred 
        Assert.All(result.TypeLibExporterNotifySink.ReportedEvents, x => Assert.Equal(ExporterEventKind.NOTIF_TYPECONVERTED, x.EventKind));
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
        Assert.NotNull(function);

        var parameter = function!.Value.GetParameter(0);
        Assert.NotNull(parameter);

        Assert.Equal(PARAMFLAG.PARAMFLAG_FOUT | PARAMFLAG.PARAMFLAG_FIN, parameter!.Value.desc.paramdesc.wParamFlags);
        Assert.Equal(VarEnum.VT_PTR, parameter!.Value.tdesc.GetVarEnum());

        // Check that no unexpected warnings occurred 
        Assert.All(result.TypeLibExporterNotifySink.ReportedEvents, x => Assert.Equal(ExporterEventKind.NOTIF_TYPECONVERTED, x.EventKind));
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
        Assert.NotNull(typeInfo);

        //check for method
        using var funcDescByName = typeInfo!.GetFuncDescByName("TestMethod");
        Assert.NotNull(funcDescByName);
        var funcDesc = funcDescByName!.Value;

        Assert.Equal(1, funcDesc.cParams);
        var parameter = Marshal.PtrToStructure<ELEMDESC>(funcDesc.lprgelemdescParam);

        Assert.Equal((short)VarEnum.VT_SAFEARRAY, parameter.tdesc.vt);

        var typeDesc = Marshal.PtrToStructure<TYPEDESC>(parameter.tdesc.lpValue);
        Assert.Equal((short)VarEnum.VT_PTR, typeDesc.vt);

        var typeDescUserDefined = Marshal.PtrToStructure<TYPEDESC>(typeDesc.lpValue);
        Assert.Equal((short)VarEnum.VT_USERDEFINED, typeDescUserDefined.vt);

        var typeInfo64Bit = (ITypeInfo64Bit)typeInfo!;
        typeInfo64Bit.GetRefTypeInfo(typeDescUserDefined.lpValue, out var refTypeInfo64Bit);
        refTypeInfo64Bit.GetDocumentation(-1, out var typeInfoName, out var docString, out var helpContext, out var helpFile);

        Assert.Equal("TestInterfaceUsedInParameter", typeInfoName);

        // Check that no unexpected warnings occurred 
        Assert.All(result.TypeLibExporterNotifySink.ReportedEvents, x => Assert.Equal(ExporterEventKind.NOTIF_TYPECONVERTED, x.EventKind));
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
        Assert.NotNull(typeInfo);

        //check for method
        using var funcDescByName = typeInfo!.GetFuncDescByName("TestMethod");
        Assert.NotNull(funcDescByName);
        var funcDesc = funcDescByName!.Value;

        Assert.Equal(1, funcDesc.cParams);
        var parameter = Marshal.PtrToStructure<ELEMDESC>(funcDesc.lprgelemdescParam);

        Assert.Equal(VarEnum.VT_PTR, parameter.tdesc.GetVarEnum());

        var typeDesc = Marshal.PtrToStructure<TYPEDESC>(parameter.tdesc.lpValue);
        Assert.Equal(VarEnum.VT_SAFEARRAY, typeDesc.GetVarEnum());

        var typeDesc2 = Marshal.PtrToStructure<TYPEDESC>(typeDesc.lpValue);
        Assert.Equal(VarEnum.VT_PTR, typeDesc2.GetVarEnum());

        var typeDesc3 = Marshal.PtrToStructure<TYPEDESC>(typeDesc2.lpValue);
        Assert.Equal(VarEnum.VT_USERDEFINED, typeDesc3.GetVarEnum());

        var typeInfo64Bit = (ITypeInfo64Bit)typeInfo!;
        typeInfo64Bit.GetRefTypeInfo(typeDesc3.lpValue, out var refTypeInfo64Bit);
        refTypeInfo64Bit.GetDocumentation(-1, out var typeInfoName, out var docString, out var helpContext, out var helpFile);

        Assert.Equal("TestInterfaceReturnValue", typeInfoName);

        // Check that no unexpected warnings occurred 
        Assert.All(result.TypeLibExporterNotifySink.ReportedEvents, x => Assert.Equal(ExporterEventKind.NOTIF_TYPECONVERTED, x.EventKind));
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
        Assert.NotNull(typeInfo);

        //check for method
        using var funcDescByName = typeInfo!.GetFuncDescByName("TestMethod");
        Assert.NotNull(funcDescByName);
        var funcDesc = funcDescByName!.Value;

        Assert.Equal(1, funcDesc.cParams);
        var parameter = Marshal.PtrToStructure<ELEMDESC>(funcDesc.lprgelemdescParam);

        Assert.Equal(VarEnum.VT_SAFEARRAY, (VarEnum)parameter.tdesc.vt);

        var typeDesc = Marshal.PtrToStructure<TYPEDESC>(parameter.tdesc.lpValue);
        Assert.Equal(VarEnum.VT_PTR, (VarEnum)typeDesc.vt);

        var typeDescUserDefined = Marshal.PtrToStructure<TYPEDESC>(typeDesc.lpValue);
        Assert.Equal(VarEnum.VT_USERDEFINED, (VarEnum)typeDescUserDefined.vt);

        var typeInfo64Bit = (ITypeInfo64Bit)typeInfo!;
        typeInfo64Bit.GetRefTypeInfo(typeDescUserDefined.lpValue, out var refTypeInfo64Bit);
        refTypeInfo64Bit.GetDocumentation(-1, out var typeInfoName, out var docString, out var helpContext, out var helpFile);

        Assert.Equal("TestInterfaceUsedInParameter", typeInfoName);

        // Check that no unexpected warnings occurred 
        Assert.All(result.TypeLibExporterNotifySink.ReportedEvents, x => Assert.Equal(ExporterEventKind.NOTIF_TYPECONVERTED, x.EventKind));
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
        Assert.NotNull(typeInfo);

        //check for method
        using var funcDescByName = typeInfo!.GetFuncDescByName("TestMethod");
        Assert.NotNull(funcDescByName);

        // Get first parameter
        Assert.Equal(1, funcDescByName!.Value.cParams);
        var parameter = Marshal.PtrToStructure<ELEMDESC>(funcDescByName!.Value.lprgelemdescParam);

        //First parameter should be VT_UNKNOWN
        Assert.Equal(VarEnum.VT_UNKNOWN, parameter.tdesc.GetVarEnum());

        // Check that no unexpected warnings occurred 
        Assert.Single(result.TypeLibExporterNotifySink.ReportedEvents, x => x.EventKind == ExporterEventKind.NOTIF_CONVERTWARNING);
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
        Assert.NotNull(typeInfo);

        //check for method
        using var funcDescByName = typeInfo!.GetFuncDescByName("ReturnInvisibleInterface");
        Assert.NotNull(funcDescByName);
        var funcDesc = funcDescByName!.Value;

        Assert.Equal(1, funcDesc.cParams);
        var parameter = Marshal.PtrToStructure<ELEMDESC>(funcDesc.lprgelemdescParam);

        Assert.Equal(VarEnum.VT_PTR, parameter.tdesc.GetVarEnum());
        var childTypeDesc = Marshal.PtrToStructure<TYPEDESC>(parameter.tdesc.lpValue);

        Assert.Equal(VarEnum.VT_UNKNOWN, childTypeDesc.GetVarEnum());

        // Check that no unexpected warnings occurred 
        Assert.Single(result.TypeLibExporterNotifySink.ReportedEvents, x => x.EventKind == ExporterEventKind.NOTIF_CONVERTWARNING);
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
    [InlineData(typeof(Guid))]
    [InlineData(typeof(System.Drawing.Color))]
    [InlineData(typeof(decimal))]
    [InlineData(typeof(DateTime))]
    [InlineData(typeof(Delegate))]
    [InlineData(typeof(IntPtr))]
    public void InterfaceIsIUnknownWithMethodWithOutParameter_OutParameterIsCorrect(Type parameterType)
    {
        var result = CreateAssembly(new AssemblyName($"Dynamic{parameterType}"))
                        .WithInterface("TestInterface")
                            .WithCustomAttribute<InterfaceTypeAttribute>(ComInterfaceType.InterfaceIsIUnknown)
                            .WithMethod("TestMethod")
                                .WithParameter(new ParameterItem { Type = parameterType, ParameterAttributes = ParameterAttributes.Out })
                            .Build()
                        .Build()
                    .Build();

        //check for interface
        var typeInfo = result.TypeLib.GetTypeInfoByName("TestInterface");
        Assert.NotNull(typeInfo);

        //check for method
        using var funcDescByName = typeInfo!.GetFuncDescByName("TestMethod");
        Assert.NotNull(funcDescByName);
        var funcDesc = funcDescByName!.Value;

        // Check number of parameters
        Assert.Equal(1, funcDesc.cParams);

        // Get first parameter
        var parameter = funcDesc.GetParameter(0);
        Assert.NotNull(parameter);

        // type should be Ptr
        var ptr = parameter!.Value.tdesc;
        Assert.Equal(parameterType.GetVarEnum(), ptr.GetVarEnum());

        Assert.Equal(IDLFLAG.IDLFLAG_FOUT, parameter!.Value.desc.idldesc.wIDLFlags);
        Assert.Equal(PARAMFLAG.PARAMFLAG_FOUT, parameter!.Value.desc.paramdesc.wParamFlags);

        // Check that no unexpected warnings occurred 
        Assert.All(result.TypeLibExporterNotifySink.ReportedEvents, x => Assert.Equal(ExporterEventKind.NOTIF_TYPECONVERTED, x.EventKind));
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
    [InlineData(typeof(Delegate))]
    [InlineData(typeof(IntPtr))]
    public void InterfaceIsIDispatchWithMethodWithOutParameter_OutParameterIsCorrect(Type parameterType)
    {
        var result = CreateAssembly(new AssemblyName($"Dynamic{parameterType}"))
                        .WithInterface("TestInterface")
                            .WithCustomAttribute<InterfaceTypeAttribute>(ComInterfaceType.InterfaceIsIDispatch)
                            .WithMethod("TestMethod")
                                .WithParameter(new ParameterItem { Type = parameterType, ParameterAttributes = ParameterAttributes.Out })
                            .Build()
                        .Build()
                    .Build();

        //check for interface
        var typeInfo = result.TypeLib.GetTypeInfoByName("TestInterface");
        Assert.NotNull(typeInfo);

        //check for method
        using var funcDescByName = typeInfo!.GetFuncDescByName("TestMethod");
        Assert.NotNull(funcDescByName);
        var funcDesc = funcDescByName!.Value;

        // Check number of parameters
        Assert.Equal(1, funcDesc.cParams);

        // Get first parameter
        var parameter = funcDesc.GetParameter(0);
        Assert.NotNull(parameter);

        // type should be Ptr
        var ptr = parameter!.Value.tdesc;
        Assert.Equal(parameterType.GetVarEnum(), ptr.GetVarEnum());

        Assert.Equal(IDLFLAG.IDLFLAG_FOUT, parameter!.Value.desc.idldesc.wIDLFlags);
        Assert.Equal(PARAMFLAG.PARAMFLAG_FOUT, parameter!.Value.desc.paramdesc.wParamFlags);

        // Check that no unexpected warnings occurred 
        Assert.All(result.TypeLibExporterNotifySink.ReportedEvents, x => Assert.Equal(ExporterEventKind.NOTIF_TYPECONVERTED, x.EventKind));
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
    [InlineData(typeof(Delegate))]
    [InlineData(typeof(IntPtr))]
    public void InterfaceIsDualWithMethodWithOutParameter_OutParameterIsCorrect(Type parameterType)
    {
        var result = CreateAssembly(new AssemblyName($"Dynamic{parameterType}"))
                        .WithInterface("TestInterface")
                            .WithCustomAttribute<InterfaceTypeAttribute>(ComInterfaceType.InterfaceIsDual)
                            .WithMethod("TestMethod")
                                .WithParameter(new ParameterItem { Type = parameterType, ParameterAttributes = ParameterAttributes.Out })
                            .Build()
                        .Build()
                    .Build();

        //check for interface
        var typeInfo = result.TypeLib.GetTypeInfoByName("TestInterface");
        Assert.NotNull(typeInfo);

        //check for method
        using var funcDescByName = typeInfo!.GetFuncDescByName("TestMethod");
        Assert.NotNull(funcDescByName);
        var funcDesc = funcDescByName!.Value;

        // Check number of parameters
        Assert.Equal(1, funcDesc.cParams);

        // Get first parameter
        var parameter = funcDesc.GetParameter(0);
        Assert.NotNull(parameter);

        // type should be Ptr
        var ptr = parameter!.Value.tdesc;
        Assert.Equal(parameterType.GetVarEnum(), ptr.GetVarEnum());

        Assert.Equal(IDLFLAG.IDLFLAG_FOUT, parameter!.Value.desc.idldesc.wIDLFlags);
        Assert.Equal(PARAMFLAG.PARAMFLAG_FOUT, parameter!.Value.desc.paramdesc.wParamFlags);

        // Check that no unexpected warnings occurred 
        Assert.All(result.TypeLibExporterNotifySink.ReportedEvents, x => Assert.Equal(ExporterEventKind.NOTIF_TYPECONVERTED, x.EventKind));
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
    [InlineData(typeof(Delegate))]
    [InlineData(typeof(IntPtr))]
    public void InterfaceIsIUnknownWithMethodWithArrayOutParameter_OutParameterIsCorrect(Type parameterType)
    {
        var result = CreateAssembly(new AssemblyName($"Dynamic{parameterType}"))
                        .WithInterface("TestInterface")
                            .WithCustomAttribute<InterfaceTypeAttribute>(ComInterfaceType.InterfaceIsIUnknown)
                            .WithMethod("TestMethod")
                                .WithParameter(new ParameterItem { Type = parameterType.MakeArrayType(), ParameterAttributes = ParameterAttributes.Out })
                            .Build()
                        .Build()
                    .Build();

        //check for interface
        var typeInfo = result.TypeLib.GetTypeInfoByName("TestInterface");
        Assert.NotNull(typeInfo);

        //check for method
        using var funcDescByName = typeInfo!.GetFuncDescByName("TestMethod");
        Assert.NotNull(funcDescByName);
        var funcDesc = funcDescByName!.Value;

        // Check number of parameters
        Assert.Equal(1, funcDesc.cParams);

        // Get first parameter
        var parameter = funcDesc.GetParameter(0);
        Assert.NotNull(parameter);

        // Check that no unexpected warnings occurred 
        Assert.All(result.TypeLibExporterNotifySink.ReportedEvents, x => Assert.Equal(ExporterEventKind.NOTIF_TYPECONVERTED, x.EventKind));
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
    [InlineData(typeof(Delegate))]
    [InlineData(typeof(IntPtr))]
    public void InterfaceIsIDispatchWithMethodWithArrayOutParameter_OutParameterIsCorrect(Type parameterType)
    {
        var result = CreateAssembly(new AssemblyName($"Dynamic{parameterType}"))
                        .WithInterface("TestInterface")
                            .WithCustomAttribute<InterfaceTypeAttribute>(ComInterfaceType.InterfaceIsIDispatch)
                            .WithMethod("TestMethod")
                                .WithParameter(new ParameterItem { Type = parameterType.MakeArrayType(), ParameterAttributes = ParameterAttributes.Out })
                            .Build()
                        .Build()
                    .Build();

        //check for interface
        var typeInfo = result.TypeLib.GetTypeInfoByName("TestInterface");
        Assert.NotNull(typeInfo);

        //check for method
        using var funcDescByName = typeInfo!.GetFuncDescByName("TestMethod");
        Assert.NotNull(funcDescByName);
        var funcDesc = funcDescByName!.Value;

        // Check number of parameters
        Assert.Equal(1, funcDesc.cParams);

        // Get first parameter
        var parameter = funcDesc.GetParameter(0);
        Assert.NotNull(parameter);

        // Check that no unexpected warnings occurred 
        Assert.All(result.TypeLibExporterNotifySink.ReportedEvents, x => Assert.Equal(ExporterEventKind.NOTIF_TYPECONVERTED, x.EventKind));
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
    [InlineData(typeof(Delegate))]
    [InlineData(typeof(IntPtr))]
    public void InterfaceIsDualWithMethodWithArrayOutParameter_OutParameterIsCorrect(Type parameterType)
    {
        var result = CreateAssembly(new AssemblyName($"Dynamic{parameterType}"))
                        .WithInterface("TestInterface")
                            .WithCustomAttribute<InterfaceTypeAttribute>(ComInterfaceType.InterfaceIsDual)
                            .WithMethod("TestMethod")
                                .WithParameter(new ParameterItem { Type = parameterType.MakeArrayType(), ParameterAttributes = ParameterAttributes.Out })
                            .Build()
                        .Build()
                    .Build();

        //check for interface
        var typeInfo = result.TypeLib.GetTypeInfoByName("TestInterface");
        Assert.NotNull(typeInfo);

        //check for method
        using var funcDescByName = typeInfo!.GetFuncDescByName("TestMethod");
        Assert.NotNull(funcDescByName);
        var funcDesc = funcDescByName!.Value;

        // Check number of parameters
        Assert.Equal(1, funcDesc.cParams);

        // Get first parameter
        var parameter = funcDesc.GetParameter(0);
        Assert.NotNull(parameter);

        // Check that no unexpected warnings occurred 
        Assert.All(result.TypeLibExporterNotifySink.ReportedEvents, x => Assert.Equal(ExporterEventKind.NOTIF_TYPECONVERTED, x.EventKind));
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
                                .WithParameter(new ParameterItem { Type = customInterface!.MakeArrayType().MakeByRefType(), ParameterAttributes = ParameterAttributes.Out })
                            .Build()
                        .Build()
                    .Build();

        //check for interface
        var typeInfo = result.TypeLib.GetTypeInfoByName("TestInterface");
        Assert.NotNull(typeInfo);

        //check for method
        using var funcDescByName = typeInfo!.GetFuncDescByName("TestMethod");
        Assert.NotNull(funcDescByName);
        var funcDesc = funcDescByName!.Value;

        // Check number of parameters
        Assert.Equal(1, funcDesc.cParams);

        // Get first parameter
        var parameter = funcDesc.GetParameter(0);
        Assert.NotNull(parameter);

        // Check that no unexpected warnings occurred 
        Assert.All(result.TypeLibExporterNotifySink.ReportedEvents, x => Assert.Equal(ExporterEventKind.NOTIF_TYPECONVERTED, x.EventKind));
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
    [InlineData(typeof(Delegate))]
    [InlineData(typeof(IntPtr))]
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
        Assert.NotNull(typeInfo);

        //check for method
        using var funcDescByName = typeInfo!.GetFuncDescByName("TestMethod");
        Assert.NotNull(funcDescByName);
        var funcDesc = funcDescByName!.Value;

        // Check number of parameters
        Assert.Equal(1, funcDesc.cParams);

        // Get first parameter
        var parameter = funcDesc.GetParameter(0);
        Assert.NotNull(parameter);

        Assert.Equal(VarEnum.VT_PTR, parameter!.Value.tdesc.GetVarEnum());

        var subtypeDesc = Marshal.PtrToStructure<TYPEDESC>(parameter!.Value.tdesc.lpValue);
        Assert.Equal(parameterType.GetVarEnum(), subtypeDesc.GetVarEnum());

        if (parameterType.IsEnum || parameterType.IsInterface)
        {
            var subsubtypeDesc = Marshal.PtrToStructure<TYPEDESC>(subtypeDesc.lpValue);
            Assert.Equal(VarEnum.VT_USERDEFINED, subsubtypeDesc.GetVarEnum());
        }

        // Check that no unexpected warnings occurred 
        Assert.All(result.TypeLibExporterNotifySink.ReportedEvents, x => Assert.Equal(ExporterEventKind.NOTIF_TYPECONVERTED, x.EventKind));
    }

    [Fact]
    public void MethodWithOptionalObjectArrayParameter_MethodIsAvailable()
    {
        var result = CreateAssembly()
                        .WithInterface("CustomInterface").Build(out var customInterface)
                        .WithInterface("TestInterface")
                            .WithMethod("TestMethod")
                                .WithParameter(new ParameterItem()
                                {
                                    ParameterAttributes = ParameterAttributes.Optional | ParameterAttributes.HasDefault,
                                    DefaultValue = new DefaultValue() { Value = null },
                                    Type = customInterface!.MakeArrayType(),
                                })
                            .Build()
                        .Build()
                    .Build();

        var type = result.TypeLib.GetTypeInfoByName("TestInterface");
        Assert.NotNull(type);

        Assert.NotNull(type!.GetFuncDescByName("TestMethod"));

        // Check that no unexpected warnings occurred 
        Assert.All(result.TypeLibExporterNotifySink.ReportedEvents, x => Assert.Equal(ExporterEventKind.NOTIF_TYPECONVERTED, x.EventKind));
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
        Assert.NotNull(type);

        Assert.NotNull(type!.GetFuncDescByName("AddRef"));
        Assert.NotNull(type!.GetFuncDescByName("AddRef_2"));
        Assert.Null(type!.GetFuncDescByName("AddRef_3"));

        // Check that no unexpected warnings occurred 
        Assert.All(result.TypeLibExporterNotifySink.ReportedEvents, x => Assert.Equal(ExporterEventKind.NOTIF_TYPECONVERTED, x.EventKind));
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
    [InlineData(typeof(string), null)]
    [InlineData(typeof(bool), true)]
    public void MethodWithDefaultParameterValueAttribute_DefaultValueIsUsed(Type parameterType, object defaultValue)
    {
        var result = CreateAssembly(new AssemblyName($"Dynamic{parameterType}{defaultValue}"))
                        .WithInterface("TestInterface")
                            .WithMethod("TestMethod")
                                .WithParameter(new ParameterItem()
                                {
                                    ParameterAttributes = ParameterAttributes.HasDefault,
                                    DefaultValue = new DefaultValue() { Value = Convert.ChangeType(defaultValue, parameterType) },
                                    Type = parameterType,
                                })
                            .Build()
                        .Build()
                    .Build();

        //check for interface
        var typeInfo = result.TypeLib.GetTypeInfoByName("TestInterface");
        Assert.NotNull(typeInfo);

        //check for method
        using var funcDesc = typeInfo!.GetFuncDescByName("TestMethod");
        Assert.NotNull(funcDesc);

        // Get first parameter
        var parameter = funcDesc!.Value.GetParameter(0);
        Assert.NotNull(parameter);

        // Check type
        var typeDesc = parameter!.Value.tdesc;
        Assert.Equal(parameterType.GetVarEnum(), typeDesc.GetVarEnum());

        Assert.Equal(PARAMFLAG.PARAMFLAG_FIN | PARAMFLAG.PARAMFLAG_FHASDEFAULT, parameter!.Value.desc.paramdesc.wParamFlags);

        // Check that no unexpected warnings occurred 
        Assert.All(result.TypeLibExporterNotifySink.ReportedEvents, x => Assert.Equal(ExporterEventKind.NOTIF_TYPECONVERTED, x.EventKind));
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
    public void MethodWithRefInParameter_ParameterIsIDLFLAGFIN(Type parameterType)
    {
        var result = CreateAssembly(new AssemblyName($"Dynamic{parameterType}"))
                        .WithInterface("TestInterface")
                            .WithMethod("TestMethod")
                                .WithParameter(parameterType.MakeByRefType())
                                .WithParameterCustomAttribute<InAttribute>(0)
                            .Build()
                        .Build()
                    .Build();

        //check for interface
        var typeInfo = result.TypeLib.GetTypeInfoByName("TestInterface");
        Assert.NotNull(typeInfo);

        //check for method
        using var funcDescByName = typeInfo!.GetFuncDescByName("TestMethod");
        Assert.NotNull(funcDescByName);
        var funcDesc = funcDescByName!.Value;

        // Check number of parameters
        Assert.Equal(1, funcDesc.cParams);

        // Get first parameter
        var parameter = funcDesc.GetParameter(0);
        Assert.NotNull(parameter);

        Assert.Equal(IDLFLAG.IDLFLAG_FIN, parameter!.Value.desc.idldesc.wIDLFlags);

        // Check that no unexpected warnings occurred 
        Assert.All(result.TypeLibExporterNotifySink.ReportedEvents, x => Assert.Equal(ExporterEventKind.NOTIF_TYPECONVERTED, x.EventKind));
    }
}
