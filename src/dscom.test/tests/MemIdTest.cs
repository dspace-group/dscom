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
using System.Runtime.InteropServices;
using Xunit;

namespace dSPACE.Runtime.InteropServices.Tests;

public class MemIdTest : BaseTest
{
    [Theory]
    [InlineData(ComInterfaceType.InterfaceIsIDispatch, 0x60020000)]
    [InlineData(ComInterfaceType.InterfaceIsDual, 0x60020000)]
    [InlineData(ComInterfaceType.InterfaceIsIUnknown, 0x60010000)]
    public void PropertiesWithoutDispIdAttribute_DispIdsShouldGeneratedAndShouldStartWithExplicitValue(ComInterfaceType interfaceType, int dispIdStart)
    {
        var result = CreateAssembly(CreateAssemblyName(assemblyNameSuffix: $"{interfaceType}{dispIdStart}"))
                        .WithInterface("TestInterface")
                            .WithCustomAttribute<InterfaceTypeAttribute>(interfaceType)
                            .WithProperty("Property1", typeof(string)).Build()
                            .WithProperty("Property2", typeof(string)).Build()
                            .WithProperty("Property3", typeof(string)).Build()
                        .Build()
                    .Build();

        using var property1 = result!.TypeLib.GetTypeInfoByName("TestInterface")!.GetFuncDescByName("Property1");
        using var property2 = result!.TypeLib.GetTypeInfoByName("TestInterface")!.GetFuncDescByName("Property2");
        using var property3 = result!.TypeLib.GetTypeInfoByName("TestInterface")!.GetFuncDescByName("Property3");

        Assert.NotNull(property1);
        Assert.NotNull(property2);
        Assert.NotNull(property3);

        Assert.Equal(dispIdStart, property1!.Value.memid);
        Assert.Equal(dispIdStart + 2, property2!.Value.memid);
        Assert.Equal(dispIdStart + 4, property3!.Value.memid);
    }

    [Theory]
    [InlineData(ComInterfaceType.InterfaceIsIDispatch, 0x60020000)]
    [InlineData(ComInterfaceType.InterfaceIsDual, 0x60020000)]
    [InlineData(ComInterfaceType.InterfaceIsIUnknown, 0x60010000)]
    public void MethodsWithoutDispIdAttribute_DispIdsShouldGeneratedAndShouldStartWithExplicitValue(ComInterfaceType interfaceType, int dispIdStart)
    {
        var result = CreateAssembly(CreateAssemblyName(assemblyNameSuffix: $"{interfaceType}{dispIdStart}"))
                        .WithInterface("TestInterface")
                            .WithCustomAttribute<InterfaceTypeAttribute>(interfaceType)
                            .WithMethod("Method1").WithReturnType<string>().Build()
                            .WithMethod("Method2").WithReturnType<string>().Build()
                            .WithMethod("Method3").WithReturnType<string>().Build()
                        .Build()
                    .Build();

        using var method1 = result!.TypeLib.GetTypeInfoByName("TestInterface")!.GetFuncDescByName("Method1");
        using var method2 = result!.TypeLib.GetTypeInfoByName("TestInterface")!.GetFuncDescByName("Method2");
        using var method3 = result!.TypeLib.GetTypeInfoByName("TestInterface")!.GetFuncDescByName("Method3");

        Assert.NotNull(method1);
        Assert.NotNull(method2);
        Assert.NotNull(method3);

        Assert.Equal(dispIdStart, method1!.Value.memid);
        Assert.Equal(dispIdStart + 1, method2!.Value.memid);
        Assert.Equal(dispIdStart + 2, method3!.Value.memid);
    }

    [Theory]
    [InlineData(ComInterfaceType.InterfaceIsIDispatch, 0x60020000)]
    [InlineData(ComInterfaceType.InterfaceIsDual, 0x60020000)]
    [InlineData(ComInterfaceType.InterfaceIsIUnknown, 0x60010000)]
    public void MethodWithBasePlusOneDispIdAttributeAndWithoutDispIDAttribute_DispIdShouldBeGeneratedAndShouldBeBasePlusTwo(ComInterfaceType interfaceType, int dispIdStart)
    {
        // tlbexp.exe will fail (TlbExp : error TX0000) in case of InterfaceIsDual or InterfaceIsIDispatch
        // Dscom will use the next available MemId (DispId) and will not fail.
        var result = CreateAssembly(CreateAssemblyName(assemblyNameSuffix: $"{interfaceType}{dispIdStart}"))
                        .WithInterface("TestInterface")
                            .WithCustomAttribute<InterfaceTypeAttribute>(interfaceType)
                            .WithMethod("Method1").WithReturnType<string>().WithCustomAttribute<DispIdAttribute>(dispIdStart + 1).Build()
                            .WithMethod("Method2").WithReturnType<string>().Build()
                        .Build()
                    .Build();

        using var method1 = result!.TypeLib.GetTypeInfoByName("TestInterface")!.GetFuncDescByName("Method1");
        using var method2 = result!.TypeLib.GetTypeInfoByName("TestInterface")!.GetFuncDescByName("Method2");

        Assert.NotNull(method1);
        Assert.NotNull(method2);

        Assert.Equal(dispIdStart + 1, method1!.Value.memid);
        Assert.Equal(dispIdStart + 2, method2!.Value.memid);
    }

    [Fact]
    public void GetEnumeratorMethodWithReturnValueIEnumerator_DispIdMinus4IsUsed()
    {
        var result = CreateAssembly()
                        .WithInterface("TestInterface")
                            .WithMethod("GetEnumerator").WithReturnType<IEnumerator>()
                            .Build()
                        .Build()
                    .Build();

        using var func = result!.TypeLib.GetTypeInfoByName("TestInterface")!.GetFuncDescByName("GetEnumerator");
        Assert.NotNull(func);
        Assert.Equal(-4, func!.Value.memid);
    }

    [Fact]
    public void GetEnumeratorMethodWithReturnValueIEnumeratorAndDispIdAttributeValue123_DispId123Used()
    {
        var result = CreateAssembly()
                        .WithInterface("TestInterface")
                            .WithMethod("GetEnumerator")
                                .WithReturnType<IEnumerator>()
                                .WithCustomAttribute<DispIdAttribute>(123)
                            .Build()
                        .Build()
                    .Build();

        using var func = result!.TypeLib.GetTypeInfoByName("TestInterface")!.GetFuncDescByName("GetEnumerator");
        Assert.NotNull(func);
        Assert.Equal(123, func!.Value.memid);
    }

    [Fact]
    public void GetEnumerator2MethodAndReturnValueIEnumerator_DispIdMinus4IsNotUsed()
    {
        var result = CreateAssembly()
                        .WithInterface("TestInterface")
                            .WithMethod("GetEnumerator2")
                                .WithReturnType<IEnumerator>()
                            .Build()
                        .Build()
                    .Build();

        using var func = result!.TypeLib.GetTypeInfoByName("TestInterface")!.GetFuncDescByName("GetEnumerator2");
        Assert.NotNull(func);
        Assert.NotEqual(-4, func!.Value.memid);
    }

    [Fact]
    public void GetEnumeratorMethodWithReturnValueString_DispIdMinus4IsNotUsed()
    {
        var result = CreateAssembly()
                        .WithInterface("TestInterface")
                            .WithMethod("GetEnumerator")
                                .WithReturnType<string>()
                            .Build()
                        .Build()
                    .Build();

        using var func = result!.TypeLib.GetTypeInfoByName("TestInterface")!.GetFuncDescByName("GetEnumerator");
        Assert.NotNull(func);
        Assert.NotEqual(-4, func!.Value.memid);
    }

    [Fact]
    public void ValueProperty_DispId0IsUsed()
    {
        var result = CreateAssembly()
                        .WithInterface("TestInterface")
                            .WithProperty("Value", typeof(string))
                            .Build()
                        .Build()
                    .Build();

        using var func = result!.TypeLib.GetTypeInfoByName("TestInterface")!.GetFuncDescByName("Value");
        Assert.NotNull(func);
        Assert.Equal(0, func!.Value.memid);
    }

    [Fact]
    public void ValuePropertyWithDispIdAttributeValue123_DispId123IsUsed()
    {
        var result = CreateAssembly()
                        .WithInterface("TestInterface")
                            .WithProperty("Value", typeof(string))
                                .WithCustomAttribute<DispIdAttribute>(123)
                            .Build()
                        .Build()
                    .Build();

        using var func = result!.TypeLib.GetTypeInfoByName("TestInterface")!.GetFuncDescByName("Value");
        Assert.NotNull(func);
        Assert.Equal(123, func!.Value.memid);
    }

    [Fact]
    public void ValueProperty_DispId0IsNotUsed()
    {
        var result = CreateAssembly()
                        .WithInterface("TestInterface")
                            .WithProperty("Value", typeof(string))
                            .WithIndexParameter(typeof(string))
                            .Build()
                        .Build()
                    .Build();

        using var func = result!.TypeLib.GetTypeInfoByName("TestInterface")!.GetFuncDescByName("Value");
        Assert.NotNull(func);
        Assert.NotEqual(0, func!.Value.memid);
    }

    [Fact]
    public void ItemMethodWithDispIdAttributeValue123_DispId123IsUsed()
    {
        var result = CreateAssembly()
                        .WithInterface("TestInterface")
                            .WithMethod("Item")
                                .WithReturnType<string>()
                                .WithCustomAttribute<DispIdAttribute>(123)
                            .Build()
                        .Build()
                    .Build();

        using var func = result!.TypeLib.GetTypeInfoByName("TestInterface")!.GetFuncDescByName("Item");
        Assert.NotNull(func);
        Assert.Equal(123, func!.Value.memid);
    }

    [Fact]
    public void PropertiesWithDuplicateDispIds_GeneratedDispIdsAreUsed()
    {
        var result = CreateAssembly()
                        .WithInterface("TestInterface")
                            .WithProperty("Property1", typeof(string))
                                .WithCustomAttribute<DispIdAttribute>(123)
                            .Build()
                            .WithProperty("Property2", typeof(string))
                                .WithCustomAttribute<DispIdAttribute>(123)
                            .Build()
                        .Build()
                    .Build();

        using var property1 = result!.TypeLib.GetTypeInfoByName("TestInterface")!.GetFuncDescByName("Property1");
        Assert.NotNull(property1);

        using var property2 = result!.TypeLib.GetTypeInfoByName("TestInterface")!.GetFuncDescByName("Property2");
        Assert.NotNull(property2);

        Assert.NotEqual(123, property1!.Value.memid);
        Assert.NotEqual(123, property2!.Value.memid);

        Assert.NotEqual(property1!.Value.memid, property2!.Value.memid);
    }

    [Fact]
    public void PropertyWithDefaultMemberAttribute_DispId0IsUsed()
    {
        var result = CreateAssembly()
                        .WithInterface("TestInterface")
                            .WithCustomAttribute<DefaultMemberAttribute>("DefaultProperty")
                            .WithProperty("DefaultProperty", typeof(string))
                            .Build()
                        .Build()
                    .Build();

        using var valueProperty = result!.TypeLib.GetTypeInfoByName("TestInterface")!.GetFuncDescByName("DefaultProperty");
        Assert.NotNull(valueProperty);
        Assert.Equal(0, valueProperty!.Value.memid);
    }

    [Fact]
    public void Properties_WithDuplicateDispID99_DispIDsAreGenerated()
    {
        var result = CreateAssembly()
                        .WithInterface("TestInterface").WithCustomAttribute<InterfaceTypeAttribute>(ComInterfaceType.InterfaceIsIDispatch)
                            .WithProperty("TestProperty1", typeof(int))
                                .WithCustomAttribute(typeof(DispIdAttribute), 99)
                            .Build()
                            .WithProperty("TestProperty2", typeof(int))
                                .WithCustomAttribute(typeof(DispIdAttribute), 99)
                            .Build()
                        .Build()
                    .Build();

        new List<string> { "TestProperty1", "TestProperty2" }.ForEach(propertyName =>
        {
            using var releasableFuncDesc = result.TypeLib.GetTypeInfoByName("TestInterface")?.GetFuncDescByName(propertyName);
            Assert.NotNull(releasableFuncDesc);
            Assert.Equal(INVOKEKIND.INVOKE_PROPERTYGET, releasableFuncDesc!.Value.invkind);
            Assert.NotEqual(99, releasableFuncDesc!.Value.memid);
        });
    }

    [Fact]
    public void PropertiesWithoutDispIdAttribute_DispIDIsGenerated()
    {
        var result = CreateAssembly()
                        .WithInterface("TestInterface").WithCustomAttribute<InterfaceTypeAttribute>(ComInterfaceType.InterfaceIsIDispatch)
                            .WithProperty("TestProperty", typeof(int))
                            .Build()
                        .Build()
                    .Build();

        using var releasableFuncDesc = result.TypeLib.GetTypeInfoByName("TestInterface")?.GetFuncDescByName("TestProperty");
        Assert.NotNull(releasableFuncDesc);
        Assert.Equal(INVOKEKIND.INVOKE_PROPERTYGET, releasableFuncDesc!.Value.invkind);
        Assert.NotEqual(0, releasableFuncDesc!.Value.memid);
    }

    [Fact]
    public void PropertiesWithDispIdAttributeValue99_DispID99IsUsed()
    {
        var result = CreateAssembly()
                        .WithInterface("TestInterface").WithCustomAttribute<InterfaceTypeAttribute>(ComInterfaceType.InterfaceIsIDispatch)
                            .WithProperty("TestProperty", typeof(int))
                                .WithCustomAttribute(typeof(DispIdAttribute), 99)
                            .Build()
                        .Build()
                    .Build();

        using var releasableFuncDesc = result.TypeLib.GetTypeInfoByName("TestInterface")?.GetFuncDescByName("TestProperty");
        Assert.NotNull(releasableFuncDesc);
        Assert.Equal(INVOKEKIND.INVOKE_PROPERTYGET, releasableFuncDesc!.Value.invkind);
        Assert.Equal(99, releasableFuncDesc!.Value.memid);
    }

    [Fact]
    public void MethodWithDuplicateDispID99_DispIDsAreGenerated()
    {
        var result = CreateAssembly()
                        .WithInterface("TestInterface").WithCustomAttribute<InterfaceTypeAttribute>(ComInterfaceType.InterfaceIsIDispatch)
                            .WithMethod("TestMethod1")
                                .WithCustomAttribute(typeof(DispIdAttribute), 99)
                            .Build()
                            .WithMethod("TestMethod2")
                                .WithCustomAttribute(typeof(DispIdAttribute), 99)
                            .Build()
                        .Build()
                    .Build();

        new List<string> { "TestMethod1", "TestMethod2" }.ForEach(propertyName =>
        {
            using var releasableFuncDesc = result.TypeLib.GetTypeInfoByName("TestInterface")?.GetFuncDescByName(propertyName);
            Assert.NotNull(releasableFuncDesc);
            Assert.Equal(INVOKEKIND.INVOKE_FUNC, releasableFuncDesc!.Value.invkind);
            Assert.NotEqual(99, releasableFuncDesc!.Value.memid);
        });
    }

    [Fact]
    public void MethodWithoutDispIdAttribute_DispIDIsGenerated()
    {
        var result = CreateAssembly()
                        .WithInterface("TestInterface").WithCustomAttribute<InterfaceTypeAttribute>(ComInterfaceType.InterfaceIsIDispatch)
                            .WithMethod("TestMethod")
                                .WithReturnType(typeof(void))
                            .Build()
                        .Build()
                    .Build();

        using var releasableFuncDesc = result.TypeLib.GetTypeInfoByName("TestInterface")?.GetFuncDescByName("TestMethod");
        Assert.NotNull(releasableFuncDesc);
        Assert.Equal(INVOKEKIND.INVOKE_FUNC, releasableFuncDesc!.Value.invkind);
        Assert.NotEqual(0, releasableFuncDesc!.Value.memid);
    }

    [Fact]
    public void MethodWithDispIdAttributeValue99_DispID99IsUsed()
    {
        var result = CreateAssembly()
                        .WithInterface("TestInterface").WithCustomAttribute<InterfaceTypeAttribute>(ComInterfaceType.InterfaceIsIDispatch)
                            .WithMethod("TestMethod")
                                .WithReturnType(typeof(void))
                                .WithCustomAttribute(typeof(DispIdAttribute), 99)
                            .Build()
                        .Build()
                    .Build();

        using var releasableFuncDesc = result.TypeLib.GetTypeInfoByName("TestInterface")?.GetFuncDescByName("TestMethod");
        Assert.NotNull(releasableFuncDesc);
        Assert.Equal(INVOKEKIND.INVOKE_FUNC, releasableFuncDesc!.Value.invkind);
        Assert.Equal(99, releasableFuncDesc!.Value.memid);
    }

    [Fact]
    public void IUnkownInterfaceWith3MethodsAndOneComVisibleFalse_MemIdAndVTableSizeAreCorrect()
    {
        var result = CreateAssembly()
                        .WithInterface("TestInterface")
                            .WithCustomAttribute<InterfaceTypeAttribute>(ComInterfaceType.InterfaceIsIUnknown)
                            .WithMethod("Method1").Build()
                            .WithMethod("Method2").WithCustomAttribute<ComVisibleAttribute>(false).Build()
                            .WithMethod("Method3").Build()
                        .Build()
                    .Build();

        var testInterface = result!.TypeLib.GetTypeInfoByName("TestInterface");
        Assert.NotNull(testInterface);

        using var attributes = testInterface!.GetTypeInfoAttributes();
        Assert.NotNull(attributes);
        Assert.Equal(48, attributes!.Value.cbSizeVft);

        using var method1 = testInterface!.GetFuncDescByName("Method1");
        Assert.NotNull(method1);
        Assert.Equal(24, method1!.Value.oVft);

        using var method2 = testInterface!.GetFuncDescByName("Method2");
        Assert.Null(method2);

        using var method3 = testInterface!.GetFuncDescByName("Method3");
        Assert.NotNull(method3);
        Assert.Equal(40, method3!.Value.oVft);
    }

    [Fact]
    public void IUnkownInterfaceWith3PropertiesAndOneComVisibleFalse_MemIdAndVTableSizeAreCorrect()
    {
        var result = CreateAssembly()
                        .WithInterface("TestInterface")
                            .WithCustomAttribute<InterfaceTypeAttribute>(ComInterfaceType.InterfaceIsIUnknown)
                            .WithProperty<string>("Property1").Build()
                            .WithProperty<string>("Property2").WithCustomAttribute<ComVisibleAttribute>(false).Build()
                            .WithProperty<string>("Property3").Build()
                        .Build()
                    .Build();

        var testInterface = result!.TypeLib.GetTypeInfoByName("TestInterface");
        Assert.NotNull(testInterface);

        using var attributes = testInterface!.GetTypeInfoAttributes();
        Assert.NotNull(attributes);
        Assert.Equal(72, attributes!.Value.cbSizeVft);

        using var method1 = testInterface!.GetFuncDescByName("Property1");
        Assert.NotNull(method1);
        Assert.Equal(24, method1!.Value.oVft);

        using var method2 = testInterface!.GetFuncDescByName("Property2");
        Assert.Null(method2);

        using var method3 = testInterface!.GetFuncDescByName("Property3", INVOKEKIND.INVOKE_PROPERTYGET);
        Assert.NotNull(method3);
        Assert.Equal(56, method3!.Value.oVft);
    }

    [Fact]
    public void IDispatchInterfaceWith3MethodsAndOneComVisibleFalse_MemIdAndVTableSizeAreCorrect()
    {
        var result = CreateAssembly()
                        .WithInterface("TestInterface")
                            .WithCustomAttribute<InterfaceTypeAttribute>(ComInterfaceType.InterfaceIsIDispatch)
                            .WithMethod("Method1").Build()
                            .WithMethod("Method2").WithCustomAttribute<ComVisibleAttribute>(false).Build()
                            .WithMethod("Method3").Build()
                        .Build()
                    .Build();

        var testInterface = result!.TypeLib.GetTypeInfoByName("TestInterface");
        Assert.NotNull(testInterface);

        using var attributes = testInterface!.GetTypeInfoAttributes();
        Assert.NotNull(attributes);
        Assert.Equal(56, attributes!.Value.cbSizeVft);

        using var method1 = testInterface!.GetFuncDescByName("Method1");
        Assert.NotNull(method1);
        Assert.Equal(0, method1!.Value.oVft);
        Assert.Equal((int)Constants.BASE_OLEAUT_DISPID, method1!.Value.memid);

        using var method2 = testInterface!.GetFuncDescByName("Method2");
        Assert.Null(method2);

        using var method3 = testInterface!.GetFuncDescByName("Method3");
        Assert.NotNull(method3);
        Assert.Equal(0, method3!.Value.oVft);
        Assert.Equal(((int)Constants.BASE_OLEAUT_DISPID) + 2, method3!.Value.memid);
    }

    [Fact]
    public void IDispatchInterfaceWith3PropertiesAndOneComVisibleFalse_MemIdAndVTableSizeAreCorrect()
    {
        var result = CreateAssembly()
                        .WithInterface("TestInterface")
                            .WithCustomAttribute<InterfaceTypeAttribute>(ComInterfaceType.InterfaceIsIDispatch)
                            .WithProperty<string>("Property1").Build()
                            .WithProperty<string>("Property2").WithCustomAttribute<ComVisibleAttribute>(false).Build()
                            .WithProperty<string>("Property3").Build()
                        .Build()
                    .Build();

        var testInterface = result!.TypeLib.GetTypeInfoByName("TestInterface");
        Assert.NotNull(testInterface);

        using var attributes = testInterface!.GetTypeInfoAttributes();
        Assert.NotNull(attributes);
        Assert.Equal(56, attributes!.Value.cbSizeVft);

        using var method1 = testInterface!.GetFuncDescByName("Property1");
        Assert.NotNull(method1);
        Assert.Equal(0, method1!.Value.oVft);
        Assert.Equal((int)Constants.BASE_OLEAUT_DISPID, method1!.Value.memid);

        using var method2 = testInterface!.GetFuncDescByName("Property2");
        Assert.Null(method2);

        using var method3 = testInterface!.GetFuncDescByName("Property3", INVOKEKIND.INVOKE_PROPERTYGET);
        Assert.NotNull(method3);
        Assert.Equal(0, method3!.Value.oVft);
        Assert.Equal(((int)Constants.BASE_OLEAUT_DISPID) + 4, method3!.Value.memid);
    }

    [Fact]
    public void DualInterfaceWith3MethodsAndOneComVisibleFalse_MemIdAndVTableSizeAreCorrect()
    {
        var result = CreateAssembly()
                        .WithInterface("TestInterface")
                            .WithCustomAttribute<InterfaceTypeAttribute>(ComInterfaceType.InterfaceIsDual)
                            .WithMethod("Method1").Build()
                            .WithMethod("Method2").WithCustomAttribute<ComVisibleAttribute>(false).Build()
                            .WithMethod("Method3").Build()
                        .Build()
                    .Build();

        var testInterface = result!.TypeLib.GetTypeInfoByName("TestInterface");
        Assert.NotNull(testInterface);

        using var attributes = testInterface!.GetTypeInfoAttributes();
        Assert.NotNull(attributes);
        Assert.Equal(56, attributes!.Value.cbSizeVft);

        using var method1 = testInterface!.GetFuncDescByName("Method1");
        Assert.NotNull(method1);
        Assert.Equal(56, method1!.Value.oVft);
        Assert.Equal((int)Constants.BASE_OLEAUT_DISPID, method1!.Value.memid);

        using var method2 = testInterface!.GetFuncDescByName("Method2");
        Assert.Null(method2);

        using var method3 = testInterface!.GetFuncDescByName("Method3");
        Assert.NotNull(method3);
        Assert.Equal(72, method3!.Value.oVft);
        Assert.Equal(((int)Constants.BASE_OLEAUT_DISPID) + 2, method3!.Value.memid);
    }

    [Fact]
    public void DualInterfaceWith3PropertiesAndOneComVisibleFalse_MemIdAndVTableSizeAreCorrect()
    {
        var result = CreateAssembly()
                        .WithInterface("TestInterface")
                            .WithCustomAttribute<InterfaceTypeAttribute>(ComInterfaceType.InterfaceIsDual)
                            .WithProperty<string>("Property1").Build()
                            .WithProperty<string>("Property2").WithCustomAttribute<ComVisibleAttribute>(false).Build()
                            .WithProperty<string>("Property3").Build()
                        .Build()
                    .Build();

        var testInterface = result!.TypeLib.GetTypeInfoByName("TestInterface");
        Assert.NotNull(testInterface);

        using var attributes = testInterface!.GetTypeInfoAttributes();
        Assert.NotNull(attributes);
        Assert.Equal(56, attributes!.Value.cbSizeVft);

        using var method1 = testInterface!.GetFuncDescByName("Property1");
        Assert.NotNull(method1);
        Assert.Equal(56, method1!.Value.oVft);
        Assert.Equal((int)Constants.BASE_OLEAUT_DISPID, method1!.Value.memid);

        using var method2 = testInterface!.GetFuncDescByName("Property2");
        Assert.Null(method2);

        using var method3 = testInterface!.GetFuncDescByName("Property3", INVOKEKIND.INVOKE_PROPERTYGET);
        Assert.NotNull(method3);
        Assert.Equal(88, method3!.Value.oVft);
        Assert.Equal(((int)Constants.BASE_OLEAUT_DISPID) + 4, method3!.Value.memid);
    }

    [Theory]
    [InlineData(ComInterfaceType.InterfaceIsIDispatch, 0x60020000)]
    [InlineData(ComInterfaceType.InterfaceIsDual, 0x60020000)]
    [InlineData(ComInterfaceType.InterfaceIsIUnknown, 0x60010000)]
    public void InterfaceWith500Methods_MemIdsAreCorrect(ComInterfaceType interfaceType, int dispIdStart)
    {
        var typeBuilder = CreateAssembly(CreateAssemblyName(assemblyNameSuffix: $"{interfaceType}{dispIdStart}"))
                        .WithInterface("TestInterface")
                            .WithCustomAttribute<InterfaceTypeAttribute>(interfaceType);

        for (var i = 0; i < 500; i++)
        {
            typeBuilder.WithMethod($"TestMethod{dispIdStart + i}").WithReturnType(typeof(void)).Build();
        }

        var result = typeBuilder.Build().Build();

        for (var i = 0; i < 500; i++)
        {
            var testMethod = result!.TypeLib.GetTypeInfoByName("TestInterface")!.GetFuncDescByName($"TestMethod{dispIdStart + i}");
            Assert.NotNull(testMethod);
            Assert.Equal(dispIdStart + i, testMethod!.Value.memid);
        }

        using var property1 = result!.TypeLib.GetTypeInfoByName("TestInterface")!.GetFuncDescByName("Property1");
    }

    [Theory]
    [InlineData(ComInterfaceType.InterfaceIsIDispatch, 0x60020000)]
    [InlineData(ComInterfaceType.InterfaceIsDual, 0x60020000)]
    [InlineData(ComInterfaceType.InterfaceIsIUnknown, 0x60010000)]
    public void InterfaceWith200MethodsEverySecondMethodDropped_MemIdsAreCorrect(ComInterfaceType interfaceType, int dispIdStart)
    {
        var typeBuilder = CreateAssembly(CreateAssemblyName(assemblyNameSuffix: $"{interfaceType}{dispIdStart}"))
                        .WithInterface("InvisibleInterface")
                            .WithCustomAttribute<ComVisibleAttribute>(false)
                            .Build(out _)
                        .WithInterface("TestInterface")
                            .WithCustomAttribute<InterfaceTypeAttribute>(interfaceType);

        for (var i = 0; i < 200; i++)
        {
            if (i % 2 == 0)
            {
                typeBuilder.WithMethod($"TestMethod{dispIdStart + i}").WithReturnType(typeof(void)).Build();
            }
            else
            {
                var fieldInfos = new FieldInfo[] { typeof(MarshalAsAttribute).GetField("SafeArraySubType")! };
                var fieldValues = new object[] { (int)VarEnum.VT_STREAMED_OBJECT };

                typeBuilder.WithMethod($"TestMethod{dispIdStart + i}")
                    .WithParameter(typeof(object))
                    .WithParameterCustomAttribute<MarshalAsAttribute>(0, UnmanagedType.SafeArray, fieldInfos, fieldValues)
                    .Build();
            }
        }

        var result = typeBuilder.Build().Build();

        for (var i = 0; i < 100; i += 2)
        {
            var testMethod = result!.TypeLib.GetTypeInfoByName("TestInterface")!.GetFuncDescByName($"TestMethod{dispIdStart + i}");
            Assert.NotNull(testMethod);

            // tlbexp.exe will ignore the MemId for the method with a wrong MarshalAsAttribute in case of a IUnkownInterface.
            // This behavior is a little bit strange. 
            // This should be the compatible behavior for tlbexp.exe.
            // 
            //  if (interfaceType == ComInterfaceType.InterfaceIsIUnknown)
            //              testMethod!.Value.memid.Should().Be(dispIdStart + (i / 2));
            //
            Assert.Equal(dispIdStart + i, testMethod!.Value.memid);
        }
    }
}
