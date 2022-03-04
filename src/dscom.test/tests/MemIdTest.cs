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

        property1.Should().NotBeNull("Property1 property should be available");
        property2.Should().NotBeNull("Property2 property should be available");
        property3.Should().NotBeNull("Property3 property should be available");

        property1!.Value.memid.Should().Be(dispIdStart);
        property2!.Value.memid.Should().Be(dispIdStart + 2);
        property3!.Value.memid.Should().Be(dispIdStart + 4);
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

        method1.Should().NotBeNull("Method1 method should be available");
        method2.Should().NotBeNull("Method2 method should be available");
        method3.Should().NotBeNull("Method3 method should be available");

        method1!.Value.memid.Should().Be(dispIdStart);
        method2!.Value.memid.Should().Be(dispIdStart + 1);
        method3!.Value.memid.Should().Be(dispIdStart + 2);
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
                    .Build(true, true);

        using var method1 = result!.TypeLib.GetTypeInfoByName("TestInterface")!.GetFuncDescByName("Method1");
        using var method2 = result!.TypeLib.GetTypeInfoByName("TestInterface")!.GetFuncDescByName("Method2");

        method1.Should().NotBeNull("Method1 method should be available");
        method2.Should().NotBeNull("Method2 method should be available");

        method1!.Value.memid.Should().Be(dispIdStart + 1);
        method2!.Value.memid.Should().Be(dispIdStart + 2);
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
        func.Should().NotBeNull("GetEnumerator method should be available");

        func!.Value.memid.Should().Be(-4);
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
        func.Should().NotBeNull("GetEnumerator method should be available");

        func!.Value.memid.Should().Be(123);
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
        func.Should().NotBeNull("GetEnumerator2 method should be available");

        func!.Value.memid.Should().NotBe(-4);
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
        func.Should().NotBeNull("GetEnumerator method should be available");

        func!.Value.memid.Should().NotBe(-4);
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
        func.Should().NotBeNull("Value property should be available");

        func!.Value.memid.Should().Be(0);
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
        func.Should().NotBeNull("Value property should be available");

        func!.Value.memid.Should().Be(123);
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
        func.Should().NotBeNull("Item method should be available");

        func!.Value.memid.Should().Be(123);
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
        property1.Should().NotBeNull("Property1 property should be available");

        using var property2 = result!.TypeLib.GetTypeInfoByName("TestInterface")!.GetFuncDescByName("Property2");
        property2.Should().NotBeNull("Property2 property should be available");

        property1!.Value.memid.Should().NotBe(123);
        property2!.Value.memid.Should().NotBe(123);

        property1!.Value.memid.Should().NotBe(property2!.Value.memid);
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
        valueProperty.Should().NotBeNull("DefaultProperty property should be available");
        valueProperty!.Value.memid.Should().Be(0);
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
            releasableFuncDesc.Should().NotBeNull();
            releasableFuncDesc!.Value.invkind.Should().Be(INVOKEKIND.INVOKE_PROPERTYGET);
            releasableFuncDesc!.Value.memid.Should().NotBe(99);
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
        releasableFuncDesc.Should().NotBeNull();
        releasableFuncDesc!.Value.invkind.Should().Be(INVOKEKIND.INVOKE_PROPERTYGET);
        releasableFuncDesc!.Value.memid.Should().NotBe(0);
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
        releasableFuncDesc.Should().NotBeNull();
        releasableFuncDesc!.Value.invkind.Should().Be(INVOKEKIND.INVOKE_PROPERTYGET);
        releasableFuncDesc!.Value.memid.Should().Be(99);
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
            releasableFuncDesc.Should().NotBeNull();
            releasableFuncDesc!.Value.invkind.Should().Be(INVOKEKIND.INVOKE_FUNC);
            releasableFuncDesc!.Value.memid.Should().NotBe(99);
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
        releasableFuncDesc.Should().NotBeNull();
        releasableFuncDesc!.Value.invkind.Should().Be(INVOKEKIND.INVOKE_FUNC);
        releasableFuncDesc!.Value.memid.Should().NotBe(0);
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
        releasableFuncDesc.Should().NotBeNull();
        releasableFuncDesc!.Value.invkind.Should().Be(INVOKEKIND.INVOKE_FUNC);
        releasableFuncDesc!.Value.memid.Should().Be(99);
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
        testInterface.Should().NotBeNull();

        using var attributes = testInterface!.GetTypeInfoAttributes();
        attributes.Should().NotBeNull();
        attributes!.Value.cbSizeVft.Should().Be(48);

        using var method1 = testInterface!.GetFuncDescByName("Method1");
        method1.Should().NotBeNull();
        method1!.Value.oVft.Should().Be(24);

        using var method2 = testInterface!.GetFuncDescByName("Method2");
        method2.Should().BeNull();

        using var method3 = testInterface!.GetFuncDescByName("Method3");
        method3.Should().NotBeNull();
        method3!.Value.oVft.Should().Be(40);
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
        testInterface.Should().NotBeNull();

        using var attributes = testInterface!.GetTypeInfoAttributes();
        attributes.Should().NotBeNull();
        attributes!.Value.cbSizeVft.Should().Be(72);

        using var method1 = testInterface!.GetFuncDescByName("Property1");
        method1.Should().NotBeNull();
        method1!.Value.oVft.Should().Be(24);

        using var method2 = testInterface!.GetFuncDescByName("Property2");
        method2.Should().BeNull();

        using var method3 = testInterface!.GetFuncDescByName("Property3", INVOKEKIND.INVOKE_PROPERTYGET);
        method3.Should().NotBeNull();
        method3!.Value.oVft.Should().Be(56);
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
        testInterface.Should().NotBeNull();

        using var attributes = testInterface!.GetTypeInfoAttributes();
        attributes.Should().NotBeNull();
        attributes!.Value.cbSizeVft.Should().Be(56);

        using var method1 = testInterface!.GetFuncDescByName("Method1");
        method1.Should().NotBeNull();
        method1!.Value.oVft.Should().Be(0);

        using var method2 = testInterface!.GetFuncDescByName("Method2");
        method2.Should().BeNull();

        using var method3 = testInterface!.GetFuncDescByName("Method3");
        method3.Should().NotBeNull();
        method3!.Value.oVft.Should().Be(0);
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
        testInterface.Should().NotBeNull();

        using var attributes = testInterface!.GetTypeInfoAttributes();
        attributes.Should().NotBeNull();
        attributes!.Value.cbSizeVft.Should().Be(56);

        using var method1 = testInterface!.GetFuncDescByName("Property1");
        method1.Should().NotBeNull();
        method1!.Value.oVft.Should().Be(0);

        using var method2 = testInterface!.GetFuncDescByName("Property2");
        method2.Should().BeNull();

        using var method3 = testInterface!.GetFuncDescByName("Property3", INVOKEKIND.INVOKE_PROPERTYGET);
        method3.Should().NotBeNull();
        method3!.Value.oVft.Should().Be(0);
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
        testInterface.Should().NotBeNull();

        using var attributes = testInterface!.GetTypeInfoAttributes();
        attributes.Should().NotBeNull();
        attributes!.Value.cbSizeVft.Should().Be(56);

        using var method1 = testInterface!.GetFuncDescByName("Method1");
        method1.Should().NotBeNull();
        method1!.Value.oVft.Should().Be(56);

        using var method2 = testInterface!.GetFuncDescByName("Method2");
        method2.Should().BeNull();

        using var method3 = testInterface!.GetFuncDescByName("Method3");
        method3.Should().NotBeNull();
        method3!.Value.oVft.Should().Be(72);
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
        testInterface.Should().NotBeNull();

        using var attributes = testInterface!.GetTypeInfoAttributes();
        attributes.Should().NotBeNull();
        attributes!.Value.cbSizeVft.Should().Be(56);

        using var method1 = testInterface!.GetFuncDescByName("Property1");
        method1.Should().NotBeNull();
        method1!.Value.oVft.Should().Be(56);

        using var method2 = testInterface!.GetFuncDescByName("Property2");
        method2.Should().BeNull();

        using var method3 = testInterface!.GetFuncDescByName("Property3", INVOKEKIND.INVOKE_PROPERTYGET);
        method3.Should().NotBeNull();
        method3!.Value.oVft.Should().Be(88);
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
            testMethod.Should().NotBeNull();
            testMethod!.Value.memid.Should().Be(dispIdStart + i);
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
            testMethod.Should().NotBeNull();

            // tlbexp.exe will ignore the MemId for the method with a wrong MarshalAsAttribute in case of a IUnkownInterface.
            // This behavior is a little bit strange. 
            // This should be the compatible behavior for tlbexp.exe.
            // 
            //  if (interfaceType == ComInterfaceType.InterfaceIsIUnknown)
            //              testMethod!.Value.memid.Should().Be(dispIdStart + (i / 2));
            //
            testMethod!.Value.memid.Should().Be(dispIdStart + i);
        }
    }
}
