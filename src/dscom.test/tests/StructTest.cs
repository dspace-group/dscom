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

namespace dSPACE.Runtime.InteropServices.Tests;

public class StructTest : BaseTest
{
    public StructTest()
    {
        var types = new List<Type>() {
            typeof(byte),
            typeof(sbyte),
            typeof(short),
            typeof(ushort),
            typeof(uint),
            typeof(int),
            typeof(ulong),
            typeof(long),
            typeof(float),
            typeof(double),
            typeof(string),
            typeof(bool),
            typeof(char),
            typeof(object),
            typeof(object[]),
            typeof(IEnumerator),
            typeof(DateTime),
            typeof(Guid),
            typeof(System.Drawing.Color),
            typeof(decimal)
        };

        var structBuilder = CreateAssembly()
                                .WithStruct(StructName)
                                .WithCustomAttribute<ComVisibleAttribute>(true)
                                .WithCustomAttribute<StructLayoutAttribute>(LayoutKind.Sequential);

        foreach (var typeItem in types)
        {
            structBuilder.WithField(ValidChars.Replace($"Field_{typeItem}", "_"), typeItem).Build();
        }

        AssemblyBuilderResult = structBuilder.Build().Build();
    }

    internal DynamicAssemblyBuilderResult AssemblyBuilderResult { get; }

    internal string StructName { get; } = "TestStruct";

    [Theory]
    [InlineData(typeof(byte), VarEnum.VT_UI1, null)]
    [InlineData(typeof(sbyte), VarEnum.VT_I1, null)]
    [InlineData(typeof(short), VarEnum.VT_I2, null)]
    [InlineData(typeof(ushort), VarEnum.VT_UI2, null)]
    [InlineData(typeof(uint), VarEnum.VT_UI4, null)]
    [InlineData(typeof(int), VarEnum.VT_I4, null)]
    [InlineData(typeof(ulong), VarEnum.VT_UI8, null)]
    [InlineData(typeof(long), VarEnum.VT_I8, null)]
    [InlineData(typeof(float), VarEnum.VT_R4, null)]
    [InlineData(typeof(double), VarEnum.VT_R8, null)]
    [InlineData(typeof(string), VarEnum.VT_LPSTR, null)]
    [InlineData(typeof(bool), VarEnum.VT_I4, null)]
    [InlineData(typeof(char), VarEnum.VT_UI1, null)]
    [InlineData(typeof(object), VarEnum.VT_UNKNOWN, null)]
    [InlineData(typeof(object[]), VarEnum.VT_SAFEARRAY, VarEnum.VT_UNKNOWN)]
    [InlineData(typeof(IEnumerator), VarEnum.VT_PTR, VarEnum.VT_USERDEFINED)]
    [InlineData(typeof(DateTime), VarEnum.VT_DATE, null)]
    [InlineData(typeof(Guid), VarEnum.VT_USERDEFINED, null)]
    [InlineData(typeof(System.Drawing.Color), VarEnum.VT_USERDEFINED, null)]
    [InlineData(typeof(decimal), VarEnum.VT_DECIMAL, null)]
    public void StructWithFields_CorrectVarEnumTypeExpected(Type type, VarEnum expectedType, VarEnum? expectedSubType)
    {
        var fieldName = ValidChars.Replace($"Field_{type}", "_");
        var testStruct = AssemblyBuilderResult.TypeLib.GetTypeInfoByName(StructName);
        testStruct.Should().NotBeNull($"Struct {StructName} should exist");

        // Check field exist
        using var vardesc = testStruct!.GetVarDescByName(fieldName);
        vardesc.Should().NotBeNull($"Field {fieldName} should exist in struct {StructName}");

        // Check field type
        vardesc!.Value.elemdescVar.tdesc.GetVarEnum().Should().Be(expectedType, $"Field {fieldName} should be {expectedType}");

        // Check field sub type
        if (expectedSubType != null)
        {
            var subtypeDesc = Marshal.PtrToStructure<TYPEDESC>(vardesc!.Value.elemdescVar.tdesc.lpValue);
            subtypeDesc.GetVarEnum().Should().Be(expectedSubType, $"Field {fieldName} inner type should be {expectedSubType}");
        }
    }

    [Fact]
    public void StructWithFields_SizeIs136()
    {
        var testStruct = AssemblyBuilderResult.TypeLib.GetTypeInfoByName(StructName);
        testStruct.Should().NotBeNull($"Struct {StructName} should exist");

        using var attributes = testStruct!.GetTypeInfoAttributes();
        attributes.Should().NotBeNull();

        attributes!.Value.cbSizeInstance.Should().Be(136);
    }
}
