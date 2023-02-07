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

using System.Globalization;

namespace dSPACE.Runtime.InteropServices.Tests;

internal sealed class DynamicEnumBuilder : DynamicBuilder<DynamicEnumBuilder>
{
    public DynamicEnumBuilder(DynamicAssemblyBuilder dynamicTypeLibBuilder, string name, Type enumType) : base(name)
    {
        DynamicTypeLibBuilder = dynamicTypeLibBuilder;
        EnumType = enumType;
    }

    public EnumBuilder? EnumBuilder { get; set; }

    public DynamicAssemblyBuilder DynamicTypeLibBuilder { get; }

    public Type EnumType { get; }

    public string Namespace { get; private set; } = "dSPACE.Test";

    public Dictionary<string, object?> EnumValues { get; } = new();
    public Dictionary<string, CustomAttributeBuilder> EnumMemberAttributes { get; } = new();

    protected override AttributeTargets AttributeTarget => AttributeTargets.Enum;

    public DynamicEnumBuilder WithLiteral(string name, object? value)
    {
        EnumValues[name] = value;
        return this;
    }

    public DynamicEnumBuilder WithLiteralAndAttribute(string name, object? value, Type type, Type[]? constructorParamTypes, object[]? values)
    {
        WithLiteral(name, value);

        var attributeConstructor = type.GetConstructor(constructorParamTypes ?? Array.Empty<Type>());
        if (attributeConstructor == null)
        {
            throw new ArgumentException($"Constructor for {type} not found");
        }

        var attributeUsageAttribute = type.GetCustomAttribute<AttributeUsageAttribute>();
        if (attributeUsageAttribute != null)
        {
            if (!attributeUsageAttribute.ValidOn.HasFlag(AttributeTarget))
            {
                throw new ArgumentException($"Attribute {type.Name} not allowed here.");
            }
        }

        var attributeBuilder = new CustomAttributeBuilder(attributeConstructor!, values ?? Array.Empty<object>());
        EnumMemberAttributes.Add(name, attributeBuilder);

        return this;
    }

    public DynamicAssemblyBuilder Build()
    {
        return Build(out _);
    }

    public DynamicAssemblyBuilder Build(out Type? createdType)
    {
        EnumBuilder = DynamicTypeLibBuilder.ModuleBuilder.DefineEnum($"{Namespace}.{Name}", TypeAttributes.Public, EnumType);

        foreach (var kv in EnumValues)
        {
            var enumValue = Convert.ChangeType(kv.Value, EnumBuilder.UnderlyingSystemType, CultureInfo.InvariantCulture);
            var member = EnumBuilder.DefineLiteral(kv.Key, enumValue);

            if (EnumMemberAttributes.TryGetValue(kv.Key, out var attributeBuilder))
            {
                member.SetCustomAttribute(attributeBuilder);
            }
        }

        createdType = EnumBuilder.CreateType();
        foreach (var customAttributeBuilder in CustomAttributeBuilder)
        {
            EnumBuilder.SetCustomAttribute(customAttributeBuilder);
        }

        return DynamicTypeLibBuilder;
    }

    public DynamicEnumBuilder WithNamespace(string name)
    {
        Namespace = name;
        return this;
    }
}
