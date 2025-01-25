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

public class TypeLibTest : BaseTest
{
    [Fact]
    public void TypeLibraryVersionWithTypeLibVersionAttribute_TypeLibVersionAttributeIsUsed()
    {
        var classCtorInfo = typeof(TypeLibVersionAttribute).GetConstructor(new Type[] { typeof(int), typeof(int) });
        Assert.NotNull(classCtorInfo);
        var customAttributeBuilder = new CustomAttributeBuilder(classCtorInfo!, new object[] { 999, 888 });

        var builder = CreateAssembly(new AssemblyName("DynamicTestAssembly"), new CustomAttributeBuilder[] { customAttributeBuilder });
        var result = builder.Build();

        using var attributes = result.TypeLib.GetTypeLibAttributes();
        Assert.NotNull(attributes);

        Assert.Equal(999, attributes!.Value.wMajorVerNum);
        Assert.Equal(888, attributes!.Value.wMinorVerNum);
    }

    [Fact]
    public void TypeLibraryVersionWithOutTypeLibVersionAttribute_AssemblyVersionIsUsed()
    {
        var assemblyName = CreateAssemblyName(major: 777, minor: 666);

        var builder = CreateAssembly(assemblyName);
        var result = builder.Build();

        using var attributes = result.TypeLib.GetTypeLibAttributes();
        Assert.NotNull(attributes);

        Assert.Equal(777, attributes!.Value.wMajorVerNum);
        Assert.Equal(666, attributes!.Value.wMinorVerNum);
    }

    [Fact]
    public void TypeLibraryVersion_WithComCompatibleVersionAttributeAndAssemblyVersion_AssemblyVersionIsUsed()
    {
        var classCtorInfo = typeof(ComCompatibleVersionAttribute).GetConstructor(new Type[] { typeof(int), typeof(int), typeof(int), typeof(int) });
        Assert.NotNull(classCtorInfo);
        var customAttributeBuilder = new CustomAttributeBuilder(classCtorInfo!, new object[] { 999, 888, 777, 666 });

        var builder = CreateAssembly(new AssemblyName("DynamicTestAssembly") { Version = new Version(555, 444) }, new CustomAttributeBuilder[] { customAttributeBuilder });
        var result = builder.Build();

        using var attributes = result.TypeLib.GetTypeLibAttributes();
        Assert.NotNull(attributes);

        Assert.Equal(555, attributes!.Value.wMajorVerNum);
        Assert.Equal(444, attributes!.Value.wMinorVerNum);
    }

    [Fact]
    public void TypeLibAttribute_wLibFlagsIs0()
    {
        var builder = CreateAssembly();
        var result = builder.Build();

        using var attributes = result.TypeLib.GetTypeLibAttributes();
        Assert.NotNull(attributes);

        Assert.Equal(0, (int)attributes!.Value.wLibFlags);
    }

    [Fact]
    public void TypeLibAttribute_SyskindIsSYS_WIN64()
    {
        var builder = CreateAssembly();
        var result = builder.Build();

        using var attributes = result.TypeLib.GetTypeLibAttributes();
        Assert.NotNull(attributes);

        Assert.Equal(SYSKIND.SYS_WIN64, attributes!.Value.syskind);
    }

    [Fact]
    public void TypeLibAttribute_LcidIs0()
    {
        var builder = CreateAssembly();
        var result = builder.Build();

        using var attributes = result.TypeLib.GetTypeLibAttributes();
        Assert.NotNull(attributes);

        Assert.Equal(0, attributes!.Value.lcid);
    }

    [Fact]
    public void TypeLibCustomAttributeValueWithGuidExportedFromComPlus_isEqualToAssemblyToString()
    {
        var builder = CreateAssembly();
        var result = builder.Build();

        // ExportedFromComPlus
        var guid = new Guid("90883F05-3D28-11D2-8F17-00A0C9A6186D");
        result.TypeLib.GetCustData(ref guid, out var pVarVal);
        Assert.NotNull(pVarVal);
        var customDataString = pVarVal.ToString();

        Assert.Equal(builder.AssemblyBuilder.ToString(), customDataString);
    }

    [Fact]
    public void TypeLibName_IsEqualTo_AssemblyName()
    {
        var builder = CreateAssembly();
        var result = builder.Build();
        result.TypeLib.GetDocumentation(-1, out var strTypeLibName, out _, out _, out _);

        Assert.Equal(builder.Assembly.GetName().Name, strTypeLibName);
    }

    [Fact]
    public void AssembliesWithDiffernetMajorVersion_TypLibGuidsAreDifferent()
    {
        var firstresult = CreateAssembly(CreateAssemblyName(minor: 1, major: 0)).Build();

        using var firstattributes = firstresult.TypeLib.GetTypeLibAttributes();
        Assert.NotNull(firstattributes);
        var firstGuid = firstattributes!.Value.guid;

        var secondresult = CreateAssembly(CreateAssemblyName(minor: 2, major: 0)).Build();
        using var secondattributes = secondresult.TypeLib.GetTypeLibAttributes();
        Assert.NotNull(secondattributes);
        var secondGuid = secondattributes!.Value.guid;

        Assert.NotEqual(firstGuid, secondGuid);
    }

    [Fact]
    public void AssembliesWithDiffernetMinorVersion_TypLibGuidsAreDifferent()
    {
        var firstresult = CreateAssembly(CreateAssemblyName(minor: 1, major: 0)).Build();

        using var firstattributes = firstresult.TypeLib.GetTypeLibAttributes();
        Assert.NotNull(firstattributes);
        var firstGuid = firstattributes!.Value.guid;

        var secondresult = CreateAssembly(CreateAssemblyName(minor: 1, major: 1)).Build();
        using var secondattributes = secondresult.TypeLib.GetTypeLibAttributes();
        Assert.NotNull(secondattributes);

        var secondGuid = secondattributes!.Value.guid;
        Assert.NotEqual(firstGuid, secondGuid);
    }

    [Fact]
    public void Assemblies_VersionConsidered()
    {
        var result = CreateAssembly(CreateAssemblyName(major: 3, minor: 4)).Build();

        using var attributes = result.TypeLib.GetTypeLibAttributes();
        Assert.NotNull(attributes);

        Assert.Equal(3, attributes!.Value.wMajorVerNum);
        Assert.Equal(4, attributes!.Value.wMinorVerNum);
    }

    [Fact]
    public void AssembliesWithoutVersion_MajorVersionIs1AndMinorVersionIs0()
    {
        var result = CreateAssembly().Build();

        using var attributes = result.TypeLib.GetTypeLibAttributes();
        Assert.NotNull(attributes);

        Assert.Equal(1, attributes!.Value.wMajorVerNum);
        Assert.Equal(0, attributes!.Value.wMinorVerNum);
    }

    [Fact]
    public void Assemblies_NotStoreOnDiskLibFlagsIs0()
    {
        var result = CreateAssembly().Build(false);
        using var attributes = result.TypeLib.GetTypeLibAttributes();
        Assert.NotNull(attributes);

        Assert.Equal(0, (int)attributes!.Value.wLibFlags);
    }

    [Fact]
    public void AssembliesWithDiffernetAssemblyName_TypLibGuidsAreDifferent()
    {
        var firstresult = CreateAssembly(CreateAssemblyName(assemblyNameSuffix: "1")).Build();

        using var firstattributes = firstresult.TypeLib.GetTypeLibAttributes();
        Assert.NotNull(firstattributes);
        var firstGuid = firstattributes!.Value.guid;

        var secondresult = CreateAssembly(CreateAssemblyName(assemblyNameSuffix: "2")).Build();
        using var secondattributes = secondresult.TypeLib.GetTypeLibAttributes();
        Assert.NotNull(secondattributes);
        var secondGuid = secondattributes!.Value.guid;

        Assert.NotEqual(firstGuid, secondGuid);
    }

    [Fact]
    public void AssembliesWithGuidAttribute_TypLibGuidsAreFromGuidAttribute()
    {
        var guidString = "e3b31696-7d3d-4769-a208-445bbcb812d6";

        var classCtorInfo = typeof(GuidAttribute).GetConstructor(new Type[] { typeof(string) });
        Assert.NotNull(classCtorInfo);
        var customAttributeBuilder = new CustomAttributeBuilder(classCtorInfo!, new object[] { guidString });

        var firstresult = CreateAssembly(
            CreateAssemblyName(major: 1, minor: 2),
            new CustomAttributeBuilder[] { customAttributeBuilder }).Build();

        using var attributes = firstresult.TypeLib.GetTypeLibAttributes();
        Assert.NotNull(attributes);
        var firstGuid = attributes!.Value.guid;

        var secondresult = CreateAssembly(
            CreateAssemblyName(major: 3, minor: 4),
            new CustomAttributeBuilder[] { customAttributeBuilder }).Build();
        using var secondattributes = secondresult.TypeLib.GetTypeLibAttributes();
        Assert.NotNull(secondattributes);
        var secondGuid = secondattributes!.Value.guid;

        Assert.Equal(new Guid(guidString), firstGuid);
        Assert.Equal(new Guid(guidString), secondGuid);
    }

    [Fact]
    public void AssemblyWithAssemblyDescriptionAttribute_DocStringIsAvailable()
    {
        var result = CreateAssembly().WithCustomAttribute<AssemblyDescriptionAttribute>("Test").Build();
        result.TypeLib.GetDocumentation(-1, out _, out var docString, out _, out _);

        Assert.Equal("Test", docString);
    }

    [Fact]
    public void TypeLib_ShouldBeLoaded_By_Class()
    {
        var assemblyA = CreateAssembly(new AssemblyName("AssemblyA"))
            .WithClass("TestSourceClass")
                .WithCustomAttribute(typeof(ClassInterfaceAttribute), ClassInterfaceType.AutoDispatch)
                .Build(out var classType)
            .WithInterface("ITestSourceInterface")
                .Build(out var interfaceType)
            .Build();

        var assemblyB = CreateAssembly(new AssemblyName("AssemblyB"))
            .AddDependency(assemblyA)
            .WithInterface("TestClassB")
                .WithProperty("Some", classType!).Build()
                .WithProperty("Other", interfaceType!).Build()
                .Build()
            .Build();

        // check for class
        var typeInfo = assemblyB.TypeLib.GetTypeInfoByName("TestClassB");
        Assert.NotNull(typeInfo);

        // check for 'some' property
        using var some_property_funcdesc = typeInfo!.GetFuncDescByName("Some");
        Assert.NotNull(some_property_funcdesc);

        Assert.Equal((short)VarEnum.VT_PTR, some_property_funcdesc!.Value!.elemdescFunc.tdesc.vt);

        // check for 'other' property
        using var other_property_funcdesc = typeInfo!.GetFuncDescByName("Other");
        Assert.NotNull(other_property_funcdesc);

        Assert.Equal((short)VarEnum.VT_PTR, other_property_funcdesc!.Value!.elemdescFunc.tdesc.vt);
    }

    [Fact]
    public void TypeLib_ShouldBeLoaded_By_Interface()
    {
        var assemblyA = CreateAssembly(new AssemblyName("AssemblyA"))
            .WithClass("TestSourceClass")
                .WithCustomAttribute(typeof(ClassInterfaceAttribute), ClassInterfaceType.AutoDispatch)
                .Build(out var classType)
            .WithInterface("ITestSourceInterface")
                .Build(out var interfaceType)
            .Build();

        var assemblyB = CreateAssembly(new AssemblyName("AssemblyB"))
            .AddDependency(assemblyA)
            .WithInterface("TestClassB")
                .WithProperty("Other", interfaceType!).Build()
                .WithProperty("Some", classType!).Build()
                .Build()
            .Build();

        // check for class
        var typeInfo = assemblyB.TypeLib.GetTypeInfoByName("TestClassB");
        Assert.NotNull(typeInfo);

        // check for 'other' property
        using var other_property_funcdesc = typeInfo!.GetFuncDescByName("Other");
        Assert.NotNull(other_property_funcdesc);

        Assert.Equal((short)VarEnum.VT_PTR, other_property_funcdesc!.Value!.elemdescFunc.tdesc.vt);

        // check for 'some' property
        using var some_property_funcdesc = typeInfo!.GetFuncDescByName("Some");
        Assert.NotNull(some_property_funcdesc);

        Assert.Equal((short)VarEnum.VT_PTR, some_property_funcdesc!.Value!.elemdescFunc.tdesc.vt);
    }
}
