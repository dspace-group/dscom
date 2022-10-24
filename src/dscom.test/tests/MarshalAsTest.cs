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

namespace dSPACE.Runtime.InteropServices.Tests;

public class MarshalAsTest : BaseTest
{

    [Theory]
    [InlineData(ComInterfaceType.InterfaceIsDual)]
    [InlineData(ComInterfaceType.InterfaceIsIDispatch)]
    [InlineData(ComInterfaceType.InterfaceIsIUnknown)]
    public void MethodWithParameterAndMarshalAsAttribute_ParameterTypeIsCorrect(ComInterfaceType interfaceType)
    {
        var interface2 = CreateAssembly(CreateAssemblyName(assemblyNameSuffix: $"{interfaceType}"))
            .WithInterface("TestInterface")
            .WithCustomAttribute<InterfaceTypeAttribute>(interfaceType);

        foreach (var kv in MarshalHelper.UnmanagedTypeMapDict)
        {
            interface2.WithMethod(ValidChars.Replace($"TestMethod_{kv.Key.Type}_{kv.Key.UnmanagedType}", "_"))
                .WithParameter(kv.Key.Type)
                .WithParameterCustomAttribute<MarshalAsAttribute>(0, kv.Key.UnmanagedType)
            .Build();
        }
        var result = interface2.Build().Build();

        var typeInfo = result.TypeLib.GetTypeInfoByName("TestInterface");
        typeInfo.Should().NotBeNull("TestInterface should be generated");

        foreach (var kv in MarshalHelper.UnmanagedTypeMapDict)
        {
            var methodName = ValidChars.Replace($"TestMethod_{kv.Key.Type}_{kv.Key.UnmanagedType}", "_");
            using var funcDesc = typeInfo!.GetFuncDescByName(methodName);

            if (kv.Value == null)
            {
                // Is not supported TLBX_E_BAD_NATIVETYPE
                funcDesc.Should().BeNull($"{methodName} should not no be available");
            }
            else
            {
                // Is is supported
                funcDesc.Should().NotBeNull($"{methodName} should be available");

                // check first parameter
                var firstParam = Marshal.PtrToStructure<ELEMDESC>(funcDesc!.Value.lprgelemdescParam);
                firstParam.tdesc.GetVarEnum().Should().Be(kv.Value, $"First parameter of {methodName} should be available {kv.Value}");
            }
        }
    }

    [Theory]
    [InlineData(ComInterfaceType.InterfaceIsDual)]
    [InlineData(ComInterfaceType.InterfaceIsIDispatch)]
    [InlineData(ComInterfaceType.InterfaceIsIUnknown)]
    public void PropertyWithMarshalAsAttribute_ParameterTypeIsCorrect(ComInterfaceType interfaceType)
    {
        var type = CreateAssembly(CreateAssemblyName(assemblyNameSuffix: $"{interfaceType}"))
            .WithInterface("TestInterface")
            .WithCustomAttribute<InterfaceTypeAttribute>(interfaceType);

        foreach (var kv in MarshalHelper.UnmanagedTypeMapDict)
        {
            type.WithProperty(ValidChars.Replace($"TestMethod_{kv.Key.Type}_{kv.Key.UnmanagedType}", "_"), kv.Key.Type)
                .WithReturnTypeCustomAttribute<MarshalAsAttribute>(kv.Key.UnmanagedType)
                .WithParameterCustomAttribute<MarshalAsAttribute>(kv.Key.UnmanagedType)
            .Build();
        }
        var result = type.Build().Build();

        var typeInfo = result.TypeLib.GetTypeInfoByName("TestInterface");
        typeInfo.Should().NotBeNull("TestInterface should be generated");

        foreach (var kv in MarshalHelper.UnmanagedTypeMapDict)
        {
            var methodName = ValidChars.Replace($"TestMethod_{kv.Key.Type}_{kv.Key.UnmanagedType}", "_");

            // Check setter
            using var funcDescSet = typeInfo!.GetFuncDescByName(methodName, INVOKEKIND.INVOKE_PROPERTYPUT | INVOKEKIND.INVOKE_PROPERTYPUTREF);

            if (kv.Value == null)
            {
                // Is not supported TLBX_E_BAD_NATIVETYPE
                funcDescSet.Should().BeNull($"{methodName} should not no be available");
            }
            else
            {
                // Is is supported
                funcDescSet.Should().NotBeNull($"{methodName} should be available");

                // check first parameter
                var firstParam = Marshal.PtrToStructure<ELEMDESC>(funcDescSet!.Value.lprgelemdescParam);
                firstParam.tdesc.GetVarEnum().Should().Be(kv.Value, $"First parameter of {methodName} should be available {kv.Value}");
            }

            // Check getter
            using var funcDescGet = typeInfo!.GetFuncDescByName(methodName, INVOKEKIND.INVOKE_PROPERTYGET);

            if (kv.Value == null)
            {
                // Is not supported TLBX_E_BAD_NATIVETYPE
                funcDescGet.Should().BeNull($"{methodName} should not no be available");
            }
            else
            {
                // Is is supported
                funcDescGet.Should().NotBeNull($"{methodName} should be available");

                if (interfaceType == ComInterfaceType.InterfaceIsIUnknown)
                {
                    // Return type should be VT_HRESULT
                    funcDescGet!.Value.elemdescFunc.tdesc.GetVarEnum().Should().Be(VarEnum.VT_HRESULT, $"Return type of {methodName} should be available {kv.Value}");

                    // check first parameter
                    var firstParam = Marshal.PtrToStructure<ELEMDESC>(funcDescSet!.Value.lprgelemdescParam);
                    firstParam.tdesc.GetVarEnum().Should().Be(kv.Value, $"First parameter of {methodName} should be available {kv.Value}");
                }
                else
                {
                    // Check return value
                    funcDescGet!.Value.elemdescFunc.tdesc.GetVarEnum().Should().Be(kv.Value, $"Return type of {methodName} should be available {kv.Value}");
                }
            }
        }
    }

    [Theory]
    [InlineData(ComInterfaceType.InterfaceIsIUnknown)]
    public void MethodWithReturnValueAndMarshalAsAttribute_ParameterTypePtrTypeAnsSubTypeIsCorrect(ComInterfaceType interfaceType)
    {
        var interface2 = CreateAssembly(CreateAssemblyName(assemblyNameSuffix: $"{interfaceType}"))
            .WithInterface("TestInterface")
            .WithCustomAttribute<InterfaceTypeAttribute>(interfaceType);

        foreach (var kv in MarshalHelper.UnmanagedTypeMapDict)
        {
            interface2.WithMethod(ValidChars.Replace($"TestMethod_{kv.Key.Type}_{kv.Key.UnmanagedType}", "_"))
                .WithReturnType(kv.Key.Type)
                .WithReturnTypeCustomAttribute<MarshalAsAttribute>(kv.Key.UnmanagedType)
            .Build();
        }
        var result = interface2.Build().Build();

        var typeInfo = result.TypeLib.GetTypeInfoByName("TestInterface");
        typeInfo.Should().NotBeNull("TestInterface should be generated");

        foreach (var kv in MarshalHelper.UnmanagedTypeMapDict)
        {
            var methodName = ValidChars.Replace($"TestMethod_{kv.Key.Type}_{kv.Key.UnmanagedType}", "_");
            using var funcDesc = typeInfo!.GetFuncDescByName(methodName);

            if (kv.Value == null)
            {
                // Is not supported TLBX_E_BAD_NATIVETYPE
                funcDesc.Should().BeNull($"{methodName} should not no be available");
            }
            else
            {
                // Is is supported
                funcDesc.Should().NotBeNull($"{methodName} should be available");

                // return value should be VT_HRESULT
                funcDesc!.Value.elemdescFunc.tdesc.GetVarEnum().Should().Be(VarEnum.VT_HRESULT, $"Return value of {methodName} should be available and VarEnum.VT_HRESULT");

                // first parameter should be VT_PTR
                var firstParam = Marshal.PtrToStructure<ELEMDESC>(funcDesc!.Value.lprgelemdescParam);
                firstParam.tdesc.GetVarEnum().Should().Be(VarEnum.VT_PTR, $"First parameter of {methodName} should be available VarEnum.VT_PTR");

                // first parameter inner type
                var subtypeDesc = Marshal.PtrToStructure<TYPEDESC>(firstParam.tdesc.lpValue);
                subtypeDesc.GetVarEnum().Should().Be(kv.Value, $"First parameter of {methodName} should be available VarEnum.VT_PTR");
            }
        }
    }

    [Fact]
    public void MethodWithReturnValueAndMarshalAsAttribute_NoWarning()
    {
        var result = CreateAssembly(CreateAssemblyName())
            .WithInterface("TestInterface")
                .WithMethod("TestMethod")
                    .WithReturnType(typeof(System.Collections.IList))
                    .WithReturnTypeCustomAttribute<MarshalAsAttribute>(UnmanagedType.IUnknown)
                    .Build()
               .Build()
            .Build();

        var typeInfo = result.TypeLib.GetTypeInfoByName("TestInterface");
        typeInfo.Should().NotBeNull("TestInterface should be generated");
        using var funcDesc = typeInfo!.GetFuncDescByName("TestMethod");
        funcDesc.Should().NotBeNull("TestMethod should be available");
        funcDesc!.Value.cParams.Should().Be(0);
        // ToDo: What additional properties can be tested here 
    }

    [Theory]
    [InlineData(ComInterfaceType.InterfaceIsDual)]
    [InlineData(ComInterfaceType.InterfaceIsIDispatch)]
    public void MethodWithReturnValueAndMarshalAsAttribute_ParameterTypeIsCorrect(ComInterfaceType interfaceType)
    {
        var interface2 = CreateAssembly(CreateAssemblyName(assemblyNameSuffix: $"{interfaceType}"))
            .WithInterface("TestInterface")
            .WithCustomAttribute<InterfaceTypeAttribute>(interfaceType);

        foreach (var kv in MarshalHelper.UnmanagedTypeMapDict)
        {
            interface2.WithMethod(ValidChars.Replace($"TestMethod_{kv.Key.Type}_{kv.Key.UnmanagedType}", "_"))
                .WithReturnType(kv.Key.Type)
                .WithReturnTypeCustomAttribute<MarshalAsAttribute>(kv.Key.UnmanagedType)
            .Build();
        }
        var result = interface2.Build().Build();

        var typeInfo = result.TypeLib.GetTypeInfoByName("TestInterface");
        typeInfo.Should().NotBeNull("TestInterface should be generated");

        foreach (var kv in MarshalHelper.UnmanagedTypeMapDict)
        {
            var methodName = ValidChars.Replace($"TestMethod_{kv.Key.Type}_{kv.Key.UnmanagedType}", "_");
            using var funcDesc = typeInfo!.GetFuncDescByName(methodName);

            if (kv.Value == null)
            {
                // Is not supported TLBX_E_BAD_NATIVETYPE
                funcDesc.Should().BeNull($"{methodName} should not no be available");
            }
            else
            {
                // Is is supported
                funcDesc.Should().NotBeNull($"{methodName} should be available");

                // check return value
                funcDesc!.Value.elemdescFunc.tdesc.GetVarEnum().Should().Be(kv.Value, $"First parameter of {methodName} should be available {kv.Value}");
            }
        }
    }

    [Theory]
    [InlineData(VarEnum.VT_FILETIME)]
    [InlineData(VarEnum.VT_BLOB)]
    [InlineData(VarEnum.VT_STREAM)]
    [InlineData(VarEnum.VT_STORAGE)]
    [InlineData(VarEnum.VT_STREAMED_OBJECT)]
    [InlineData(VarEnum.VT_STORED_OBJECT)]
    [InlineData(VarEnum.VT_BLOB_OBJECT)]
    [InlineData(VarEnum.VT_CF)]
    [InlineData(VarEnum.VT_CLSID)]
    [InlineData(VarEnum.VT_VECTOR)]
    [InlineData(VarEnum.VT_ARRAY)]
    [InlineData(VarEnum.VT_BYREF)]
    [InlineData(VarEnum.VT_PTR)]
    [InlineData(VarEnum.VT_SAFEARRAY)]
    [InlineData(VarEnum.VT_CARRAY)]
    [InlineData(VarEnum.VT_RECORD)]
    public void MethodWithArrayParameterAndMarshalAsAttribute_SafeArraySubTypeNotSupported(VarEnum enumSubType)
    {
        var interface2 = CreateAssembly(CreateAssemblyName(assemblyNameSuffix: $"{enumSubType}"))
            .WithInterface("TestInterface")
            .WithCustomAttribute<InterfaceTypeAttribute>(ComInterfaceType.InterfaceIsDual);

        foreach (var kv in MarshalHelper.UnmanagedTypeMapDict)
        {
            var fieldInfos = new FieldInfo[] { typeof(MarshalAsAttribute).GetField("SafeArraySubType")! };
            var fieldValues = new object[] { (int)enumSubType };

            interface2.WithMethod(ValidChars.Replace($"TestMethod_ArrayIs_{kv.Key.Type}_SubTypeIs_{enumSubType}_Expected_SAFEARRAY", "_"))
                .WithParameter(kv.Key.Type.MakeArrayType())
                .WithParameterCustomAttribute<MarshalAsAttribute>(0, UnmanagedType.SafeArray, fieldInfos, fieldValues)
            .Build();
        }

        var result = interface2.Build().Build(true, true);

        var typeInfo = result.TypeLib.GetTypeInfoByName("TestInterface");
        typeInfo.Should().NotBeNull("TestInterface should be generated");

        foreach (var kv in MarshalHelper.UnmanagedTypeMapDict)
        {
            var methodName = ValidChars.Replace($"TestMethod_ArrayIs_{kv.Key.Type}_SubTypeIs_{enumSubType}_Expected_SAFEARRAY", "_");
            using var funcDesc = typeInfo!.GetFuncDescByName(methodName);
            funcDesc.Should().BeNull();
        }
    }

    public static IEnumerable<object[]> SafeArraySafeArraySubTypeCombination
    {
        get
        {
            var varEnumTypes = new object[] {
                VarEnum.VT_EMPTY,
                VarEnum.VT_NULL,
                VarEnum.VT_I2,
                VarEnum.VT_I4,
                VarEnum.VT_R4,
                VarEnum.VT_R8,
                VarEnum.VT_CY,
                VarEnum.VT_DATE,
                VarEnum.VT_BSTR,
                VarEnum.VT_DISPATCH,
                VarEnum.VT_ERROR,
                VarEnum.VT_BOOL,
                VarEnum.VT_VARIANT,
                VarEnum.VT_UNKNOWN,
                VarEnum.VT_DECIMAL,
                VarEnum.VT_I1,
                VarEnum.VT_UI1,
                VarEnum.VT_UI2,
                VarEnum.VT_UI4,
                VarEnum.VT_I8,
                VarEnum.VT_UI8,
                VarEnum.VT_INT,
                VarEnum.VT_UINT,
                VarEnum.VT_VOID,
                VarEnum.VT_HRESULT,
                VarEnum.VT_USERDEFINED,
                VarEnum.VT_LPSTR,
                VarEnum.VT_LPWSTR
                };
            var comInterfaceTypes = new object[] {
                ComInterfaceType.InterfaceIsDual,
                ComInterfaceType.InterfaceIsIDispatch,
                ComInterfaceType.InterfaceIsIUnknown};
            foreach (var varEnumType in varEnumTypes)
            {
                foreach (var comInterfaceType in comInterfaceTypes)
                {
                    yield return new object[] { varEnumType, comInterfaceType };
                }
            }
        }
    }

    [Theory]
    [MemberData(nameof(SafeArraySafeArraySubTypeCombination))]
    public void MethodWithArrayParameterAndMarshalAsAttribute_SafeArraySubTypeIsSupported(VarEnum enumSubType, ComInterfaceType interfaceType)
    {
        var interface2 = CreateAssembly(CreateAssemblyName(assemblyNameSuffix: $"{interfaceType}{enumSubType}"))
            .WithInterface("TestInterface")
            .WithCustomAttribute<InterfaceTypeAttribute>(interfaceType);

        foreach (var parameterType in MarshalHelper.UnmanagedTypeMapDict.Keys.Select(y => y.Type).Distinct())
        {
            var fieldInfos = new FieldInfo[] { typeof(MarshalAsAttribute).GetField("SafeArraySubType")! };
            var fieldValues = new object[] { (int)enumSubType };

            interface2.WithMethod(ValidChars.Replace($"TestMethod_ArrayIs_{parameterType}_SubTypeIs_{enumSubType}_Expected_SAFEARRAY", "_"))
                .WithParameter(parameterType.MakeArrayType())
                .WithParameterCustomAttribute<MarshalAsAttribute>(0, UnmanagedType.SafeArray, fieldInfos, fieldValues)
            .Build();
        }

        var result = interface2.Build().Build();

        var typeInfo = result.TypeLib.GetTypeInfoByName("TestInterface");
        typeInfo.Should().NotBeNull("TestInterface should be generated");

        foreach (var parameterType in MarshalHelper.UnmanagedTypeMapDict.Keys.Select(y => y.Type).Distinct())
        {
            var methodName = ValidChars.Replace($"TestMethod_ArrayIs_{parameterType}_SubTypeIs_{enumSubType}_Expected_SAFEARRAY", "_");
            using var funcDesc = typeInfo!.GetFuncDescByName(methodName);

            // Array of Array not supported
            if (parameterType.IsArray)
            {
                // Is not supported TLBX_E_BAD_NATIVETYPE
                funcDesc.Should().BeNull($"{methodName} should not no be available");
            }
            else
            {
                // Is is supported
                funcDesc.Should().NotBeNull($"{methodName} should be available");

                // check first parameter
                var firstParam = Marshal.PtrToStructure<ELEMDESC>(funcDesc!.Value.lprgelemdescParam);
                firstParam.tdesc.GetVarEnum().Should().Be(VarEnum.VT_SAFEARRAY, $"First parameter of {methodName} should be available VarEnum.VT_SAFEARRAY");
                var firstParamSub = Marshal.PtrToStructure<ELEMDESC>(firstParam.tdesc.lpValue);
                if (firstParamSub.tdesc.lpValue != IntPtr.Zero && firstParamSub.tdesc.GetVarEnum() == VarEnum.VT_PTR)
                {
                    firstParamSub = Marshal.PtrToStructure<ELEMDESC>(firstParamSub.tdesc.lpValue);
                }
                if (enumSubType is VarEnum.VT_USERDEFINED or VarEnum.VT_EMPTY)
                {
                    firstParamSub.tdesc.GetVarEnum().Should().Be(MarshalHelper.TypeToVarEnum(parameterType), $"Sub parameter of {methodName} should be {MarshalHelper.TypeToVarEnum(parameterType)}");
                }
                else
                {
                    firstParamSub.tdesc.GetVarEnum().Should().Be(enumSubType, $"Sub parameter of {methodName} should be {enumSubType}");
                }
            }
        }
    }

    [Fact]
    public void MethodWithReturnTypeUndParameterTypeCustomInterface_TypesAreCorrect()
    {
        var builder = CreateAssembly()
             .WithInterface("CustomInterface").Build(out var customtype)
             .WithInterface("TestInterface");

        foreach (UnmanagedType value in Enum.GetValues(typeof(UnmanagedType)))
        {
            switch (value)
            {
                case UnmanagedType.ByValTStr:
                case UnmanagedType.ByValArray:
                case UnmanagedType.CustomMarshaler:
                    continue;
            }

            builder.WithMethod($"TestMethod_{value}")
                .WithReturnType(customtype!)
                .WithParameter(customtype!)
                .WithParameter(customtype!)
                .WithReturnTypeCustomAttribute<MarshalAsAttribute>(value)
                .WithParameterCustomAttribute<MarshalAsAttribute>(0, value)
                .WithParameterCustomAttribute<MarshalAsAttribute>(1, value)
            .Build();

        }

        var result = builder.Build().Build();

        var typeinfo = result.TypeLib.GetTypeInfoByName("TestInterface");
        typeinfo.Should().NotBeNull();

        foreach (UnmanagedType value in Enum.GetValues(typeof(UnmanagedType)))
        {
            switch (value)
            {
                case UnmanagedType.ByValTStr:
                case UnmanagedType.ByValArray:
                case UnmanagedType.CustomMarshaler:
                    continue;
            }

            var methodName = $"TestMethod_{value}";
            var funcDesc = typeinfo!.GetFuncDescByName(methodName);

            funcDesc.Should().NotBeNull();

            // return value should be VT_PTR
            funcDesc!.Value.elemdescFunc.tdesc.GetVarEnum().Should().Be(VarEnum.VT_PTR, $"Return value of {methodName} should be available and VarEnum.VT_HRESULT");

            // return value inner type should VT_USERDEFINED
            var subtypeDesc = Marshal.PtrToStructure<TYPEDESC>(funcDesc!.Value.elemdescFunc.tdesc.lpValue);
            subtypeDesc.GetVarEnum().Should().Be(VarEnum.VT_USERDEFINED, $"First parameter of {methodName} should be available VarEnum.VT_PTR");

            // first parameter should be VT_PTR
            var firstParam = Marshal.PtrToStructure<ELEMDESC>(funcDesc!.Value.lprgelemdescParam);
            firstParam.tdesc.GetVarEnum().Should().Be(VarEnum.VT_PTR, $"First parameter of {methodName} should be available VarEnum.VT_PTR");

            // first parameter inner type should be VT_USERDEFINED
            subtypeDesc = Marshal.PtrToStructure<TYPEDESC>(firstParam.tdesc.lpValue);
            subtypeDesc.GetVarEnum().Should().Be(VarEnum.VT_USERDEFINED, $"First parameter of {methodName} should be available VarEnum.VT_PTR");
        }
    }

    [Fact]
    public void MethodWithParameterWithComInvisibleEnumAndValidMarshalAsAttribute_MethodIsAvailable()
    {
        var result = CreateAssembly()
                        .WithEnum<int>("TestEnum")
                            .WithLiteral("A", 1)
                            .WithCustomAttribute<ComVisibleAttribute>(false)
                        .Build(out var testEnum)
                        .WithInterface("TestInterface")
                            .WithMethod("TestMethod")
                                .WithParameter(testEnum!)
                                .WithParameterCustomAttribute<MarshalAsAttribute>(0, UnmanagedType.U4)
                                .Build()
                            .Build()
                        .Build();

        var type = result.TypeLib.GetTypeInfoByName("TestInterface");
        type.Should().NotBeNull();

        type!.GetFuncDescByName("TestMethod").Should().NotBeNull("TestMethod should be available");
    }

    [Theory]
    [InlineData(ComInterfaceType.InterfaceIsDual, UnmanagedType.I4, VarEnum.VT_I4)]
    [InlineData(ComInterfaceType.InterfaceIsIDispatch, UnmanagedType.I4, VarEnum.VT_I4)]
    [InlineData(ComInterfaceType.InterfaceIsIUnknown, UnmanagedType.I4, VarEnum.VT_I4)]
    [InlineData(ComInterfaceType.InterfaceIsDual, UnmanagedType.U4, VarEnum.VT_I4)]
    [InlineData(ComInterfaceType.InterfaceIsIDispatch, UnmanagedType.U4, VarEnum.VT_I4)]
    [InlineData(ComInterfaceType.InterfaceIsIUnknown, UnmanagedType.U4, VarEnum.VT_I4)]
    [InlineData(ComInterfaceType.InterfaceIsDual, UnmanagedType.Interface, VarEnum.VT_I4)]
    [InlineData(ComInterfaceType.InterfaceIsIDispatch, UnmanagedType.Interface, VarEnum.VT_I4)]
    [InlineData(ComInterfaceType.InterfaceIsIUnknown, UnmanagedType.Interface, VarEnum.VT_I4)]
    [InlineData(ComInterfaceType.InterfaceIsDual, UnmanagedType.AsAny, VarEnum.VT_I4)]
    [InlineData(ComInterfaceType.InterfaceIsIDispatch, UnmanagedType.AsAny, VarEnum.VT_I4)]
    [InlineData(ComInterfaceType.InterfaceIsIUnknown, UnmanagedType.AsAny, VarEnum.VT_I4)]
    public void PropertyWithEnumAndValidMarshalAsAttribute_ParameterTypeIsCorrect(ComInterfaceType interfaceType, UnmanagedType marshalTo, VarEnum resultType)
    {
        var result = CreateAssembly(CreateAssemblyName(assemblyNameSuffix: $"{interfaceType}{marshalTo}{resultType}"))
                .WithEnum<int>("TestEnum")
                    .WithLiteral("A", 1)
                .Build(out var testEnum)
                .WithInterface("TestInterface")
                    .WithCustomAttribute<InterfaceTypeAttribute>(interfaceType)
                    .WithProperty("TestProperty", testEnum!)
                        .WithReturnTypeCustomAttribute<MarshalAsAttribute>(marshalTo)
                        .WithParameterCustomAttribute<MarshalAsAttribute>(marshalTo)
                    .Build()
                .Build()
            .Build();

        var typeInfo = result.TypeLib.GetTypeInfoByName("TestInterface");
        typeInfo.Should().NotBeNull("TestInterface should be generated");

        // Check setter
        using var funcDescSet = typeInfo!.GetFuncDescByName("TestProperty", INVOKEKIND.INVOKE_PROPERTYPUT | INVOKEKIND.INVOKE_PROPERTYPUTREF);

        // check first parameter
        var firstParam = Marshal.PtrToStructure<ELEMDESC>(funcDescSet!.Value.lprgelemdescParam);
        firstParam.tdesc.GetVarEnum().Should().Be(resultType);

        // Check getter
        using var funcDescGet = typeInfo!.GetFuncDescByName("TestProperty", INVOKEKIND.INVOKE_PROPERTYGET);
        funcDescGet.Should().NotBeNull();

        if (interfaceType == ComInterfaceType.InterfaceIsIUnknown)
        {
            // Return type should be VT_HRESULT
            funcDescGet!.Value.elemdescFunc.tdesc.GetVarEnum().Should().Be(VarEnum.VT_HRESULT);

            // check first parameter
            firstParam = Marshal.PtrToStructure<ELEMDESC>(funcDescSet!.Value.lprgelemdescParam);
            firstParam.tdesc.GetVarEnum().Should().Be(resultType);
        }
        else
        {
            // Check return value
            funcDescGet!.Value.elemdescFunc.tdesc.GetVarEnum().Should().Be(resultType);
        }
    }

    [Theory]
    [InlineData(ComInterfaceType.InterfaceIsDual, UnmanagedType.BStr)]
    [InlineData(ComInterfaceType.InterfaceIsIDispatch, UnmanagedType.BStr)]
    [InlineData(ComInterfaceType.InterfaceIsIUnknown, UnmanagedType.BStr)]
    [InlineData(ComInterfaceType.InterfaceIsDual, UnmanagedType.HString)]
    [InlineData(ComInterfaceType.InterfaceIsIDispatch, UnmanagedType.HString)]
    [InlineData(ComInterfaceType.InterfaceIsIUnknown, UnmanagedType.HString)]
    [InlineData(ComInterfaceType.InterfaceIsDual, UnmanagedType.LPArray)]
    [InlineData(ComInterfaceType.InterfaceIsIDispatch, UnmanagedType.LPArray)]
    [InlineData(ComInterfaceType.InterfaceIsIUnknown, UnmanagedType.LPArray)]
    [InlineData(ComInterfaceType.InterfaceIsDual, UnmanagedType.SafeArray)]
    [InlineData(ComInterfaceType.InterfaceIsIDispatch, UnmanagedType.SafeArray)]
    [InlineData(ComInterfaceType.InterfaceIsIUnknown, UnmanagedType.SafeArray)]
    public void PropertyWithEnumAndInvalidMarshalAsAttribute_ParameterTypeIsCorrect(ComInterfaceType interfaceType, UnmanagedType marshalTo)
    {
        var result = CreateAssembly(CreateAssemblyName(assemblyNameSuffix: $"{interfaceType}{marshalTo}"))
                .WithEnum<int>("TestEnum")
                    .WithLiteral("A", 1)
                .Build(out var testEnum)
                .WithInterface("TestInterface")
                    .WithCustomAttribute<InterfaceTypeAttribute>(interfaceType)
                    .WithProperty("TestProperty", testEnum!)
                        .WithReturnTypeCustomAttribute<MarshalAsAttribute>(marshalTo)
                        .WithParameterCustomAttribute<MarshalAsAttribute>(marshalTo)
                    .Build()
                .Build()
            .Build();

        var typeInfo = result.TypeLib.GetTypeInfoByName("TestInterface");
        typeInfo.Should().NotBeNull();

        // Check setter
        using var funcDescSet = typeInfo!.GetFuncDescByName("TestProperty", INVOKEKIND.INVOKE_PROPERTYPUT | INVOKEKIND.INVOKE_PROPERTYPUTREF);
        funcDescSet.Should().BeNull();

        // Check getter
        using var funcDescGet = typeInfo!.GetFuncDescByName("TestProperty", INVOKEKIND.INVOKE_PROPERTYGET);
        funcDescGet.Should().BeNull();
    }
}
