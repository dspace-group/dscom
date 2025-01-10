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

internal sealed class DynamicAssemblyBuilder : DynamicBuilder<DynamicAssemblyBuilder>
{
    public DynamicAssemblyBuilder(string name, AssemblyBuilder assemblyBuilder) : base(name)
    {
        AssemblyBuilder = assemblyBuilder;
        ModuleBuilder = CreateModuleBuilder();
        AppDomain.CurrentDomain.AssemblyResolve += ResolveEventHandler;
    }

    private List<DynamicTypeBuilder> DynamicTypeBuilder { get; } = new List<DynamicTypeBuilder>();

    public ModuleBuilder ModuleBuilder { get; set; }

    public AssemblyBuilder AssemblyBuilder { get; set; }

    public Assembly Assembly => ModuleBuilder.Assembly;

    private readonly List<DynamicAssemblyBuilderResult> _dependencies = new();

    protected override AttributeTargets AttributeTarget => AttributeTargets.Assembly;

    private Assembly? ResolveEventHandler(object? sender, ResolveEventArgs args)
    {
        if (args!.RequestingAssembly!.FullName == AssemblyBuilder.FullName)
        {
            return AssemblyBuilder;
        }

        return null;
    }

    public DynamicAssemblyBuilderResult Build(bool useComAlias = false)
    {
        foreach (var customAttributeBuilder in CustomAttributeBuilder)
        {
            AssemblyBuilder.SetCustomAttribute(customAttributeBuilder);
        }

        DynamicTypeBuilder.ForEach(dynamicTypeBuilder => dynamicTypeBuilder.CreateType());

        var typeLibConverter = new TypeLibConverter();
        var assembly = ModuleBuilder.Assembly;
        var typeLibExporterNotifySink = new TypeLibExporterNotifySink(useComAlias ? assembly : null, _dependencies);
        if (typeLibConverter.ConvertAssemblyToTypeLib(assembly, string.Empty, typeLibExporterNotifySink) is not ITypeLib2 typelib)
        {
            throw new COMException("Cannot create type library for this dynamic assembly");
        }

        AppDomain.CurrentDomain.AssemblyResolve -= ResolveEventHandler;

        return new DynamicAssemblyBuilderResult(typelib, ModuleBuilder.Assembly, typeLibExporterNotifySink);
    }

    internal DynamicAssemblyBuilder AddDependency(DynamicAssemblyBuilderResult assembly)
    {
        _dependencies.Add(assembly);
        return this;
    }

    internal DynamicTypeBuilder WithInterface(string interfaceName)
    {
        var dynamicTypeBuilder = new DynamicTypeBuilder(this, interfaceName, TypeAttributes.Interface | TypeAttributes.Public | TypeAttributes.Abstract);
        DynamicTypeBuilder.Add(dynamicTypeBuilder);
        return dynamicTypeBuilder;
    }

    internal DynamicEnumBuilder WithEnum(string enumName, Type enumType)
    {
        return new DynamicEnumBuilder(this, enumName, enumType);
    }

    internal DynamicEnumBuilder WithEnum<T>(string enumName)
    {
        return WithEnum(enumName, typeof(T));
    }

    internal DynamicTypeBuilder WithClass(string className)
    {
        return WithClass(className, Array.Empty<string>());
    }

    internal DynamicTypeBuilder WithClass(string className, string[] interfaceNames, Type? parentType = null)
    {
        var dynamicTypeBuilder = new DynamicTypeBuilder(this, className, TypeAttributes.Class | TypeAttributes.Public, interfaceNames, parentType);
        DynamicTypeBuilder.Add(dynamicTypeBuilder);
        return dynamicTypeBuilder;
    }

    internal DynamicTypeBuilder WithStruct(string structName)
    {
#pragma warning disable SYSLIB0050 // Type or member is obsolete
        var dynamicTypeBuilder = new DynamicTypeBuilder(this, structName, TypeAttributes.Public | TypeAttributes.Sealed | TypeAttributes.SequentialLayout | TypeAttributes.Serializable, null, typeof(ValueType));
#pragma warning restore SYSLIB0050 // Type or member is obsolete
        DynamicTypeBuilder.Add(dynamicTypeBuilder);
        return dynamicTypeBuilder;
    }

    internal void ForceTypeCreation(DynamicTypeBuilder toCreate, out Type? createdType)
    {
        createdType = toCreate.CreateType();
        DynamicTypeBuilder.Remove(toCreate);
    }

    private ModuleBuilder CreateModuleBuilder()
    {
        return AssemblyBuilder.DefineDynamicModule("DynamicTestModule"); ;
    }
}
