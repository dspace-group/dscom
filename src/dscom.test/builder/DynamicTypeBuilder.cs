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

namespace dSPACE.Runtime.InteropServices.Tests;

internal sealed class DynamicTypeBuilder : DynamicBuilder<DynamicTypeBuilder>
{
    public DynamicTypeBuilder(DynamicAssemblyBuilder dynamicTypeBuilder, string name, TypeAttributes typeAttributes, string[]? addInterfaceImplementation = null, Type? parentType = null) : base(name)
    {
        DynamicTypeLibBuilder = dynamicTypeBuilder;
        AddInterfaceImplementation = addInterfaceImplementation;
        TypeAttributes = typeAttributes;
        ParentType = parentType;

        _attributeTargets = typeAttributes.HasFlag(TypeAttributes.Interface) ? AttributeTargets.Interface : AttributeTargets.Class;
    }

    public Type? ParentType { get; set; }

    public DynamicAssemblyBuilder DynamicTypeLibBuilder { get; }

    public bool IsClass { get; }

    public string[]? AddInterfaceImplementation { get; }

    public TypeBuilder? TypeBuilder { get; set; }

    public string Namespace { get; set; } = "dSPACE.Test";

    private readonly AttributeTargets _attributeTargets;

    private TypeAttributes TypeAttributes { get; }

    protected override AttributeTargets AttributeTarget => _attributeTargets;

    private List<DynamicFieldBuilder> Fields { get; } = new();

    private List<DynamicMethodBuilder> Methods { get; } = new();

    private List<DynamicPropertyBuilder> Properties { get; } = new();

    private List<string> GenericTypeParameters { get; } = new();

    public DynamicTypeBuilder WithGenericTypeParameter(string typeParameter)
    {
        GenericTypeParameters.Add(typeParameter);
        return this;
    }

    public DynamicTypeBuilder WithNamespace(string name)
    {
        Namespace = name;
        return this;
    }

    public DynamicMethodBuilder WithMethod(string name)
    {
        var dynamicMethodBuilder = new DynamicMethodBuilder(this, name);
        Methods.Add(dynamicMethodBuilder);
        return dynamicMethodBuilder;
    }

    public DynamicPropertyBuilder WithProperty(string name, Type propertyType, bool IsSettable = true, bool IsGettable = true)
    {
        var dynamicPropertyBuilder = new DynamicPropertyBuilder(this, name, propertyType, IsSettable, IsGettable);
        Properties.Add(dynamicPropertyBuilder);
        return dynamicPropertyBuilder;
    }

    public DynamicPropertyBuilder WithProperty<T>(string name, bool IsSettable = true, bool IsGettable = true)
    {
        return WithProperty(name, typeof(T), IsSettable, IsGettable);
    }

    public DynamicFieldBuilder WithField(string name, Type type)
    {
        var field = new DynamicFieldBuilder(this, name, type);
        Fields.Add(field);
        return field;
    }

    public DynamicFieldBuilder WithField<T>(string name)
    {
        return WithField(name, typeof(T));
    }

    public DynamicAssemblyBuilder Build()
    {
        return DynamicTypeLibBuilder;
    }

    public DynamicAssemblyBuilder Build(out Type? forcedTypeCreation)
    {
        DynamicTypeLibBuilder.ForceTypeCreation(this, out forcedTypeCreation);
        return DynamicTypeLibBuilder;
    }

    public Type? CreateType()
    {
        TypeBuilder = DynamicTypeLibBuilder.ModuleBuilder.DefineType($"{Namespace}.{Name}", TypeAttributes, ParentType);

        if (GenericTypeParameters.Any())
        {
            TypeBuilder.DefineGenericParameters(GenericTypeParameters.ToArray());
        }

        if (AddInterfaceImplementation != null)
        {
            foreach (var interfaceName in AddInterfaceImplementation)
            {
                TypeBuilder.AddInterfaceImplementation(DynamicTypeLibBuilder.ModuleBuilder.GetType($"{Namespace}.{interfaceName}")!);
            }
        }

        foreach (var customAttributeBuilder in CustomAttributeBuilder)
        {
            TypeBuilder.SetCustomAttribute(customAttributeBuilder);
        }

        Methods.ForEach(d => d.CreateMethod());
        Properties.ForEach(d => d.CreateProperty());
        Fields.ForEach(d => d.CreateField());

        return TypeBuilder.CreateType();
    }
}
