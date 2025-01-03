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

internal sealed class DynamicPropertyBuilder : DynamicBuilder<DynamicPropertyBuilder>
{
    public DynamicPropertyBuilder(DynamicTypeBuilder dynamicTypeBuilder, string name, Type propertyType, bool isSettable = true, bool isGettable = true) : base(name)
    {
        _dynamicTypeBuilder = dynamicTypeBuilder;
        PropertyType = propertyType;
        IsSettable = isSettable;
        IsGettable = isGettable;
    }

    public Type PropertyType { get; }

    public bool IsSettable { get; }

    public bool IsGettable { get; }

    private readonly DynamicTypeBuilder _dynamicTypeBuilder;

    private Tuple<Type, object>? _returnTypeCustomAttributes;

    private Tuple<Type, object>? _parameterCustomAttributes;

    protected override AttributeTargets AttributeTarget => AttributeTargets.Property;

    private readonly DynamicParameterBuilder _indexParameterBuilder = new();

    private static readonly MethodAttributes _methodAttributes =
        MethodAttributes.Public
        | MethodAttributes.SpecialName
        | MethodAttributes.HideBySig
        | MethodAttributes.Abstract
        | MethodAttributes.Virtual;

    public DynamicTypeBuilder CreateProperty()
    {
        var propertyBuilder = _dynamicTypeBuilder.TypeBuilder!.DefineProperty(Name, PropertyAttributes.None, CallingConventions.HasThis, PropertyType, null);

        if (IsGettable)
        {
            var specialName = $"get_{Name}";
            var getterMethodBuilder = _dynamicTypeBuilder.TypeBuilder.DefineMethod(
                specialName,
                _methodAttributes,
                CallingConventions.HasThis,
                PropertyType,
                _indexParameterBuilder.GetParameterTypes());

            _indexParameterBuilder.AddParameters(getterMethodBuilder, 0);

            if (_returnTypeCustomAttributes != null)
            {
                var attributeParam = _returnTypeCustomAttributes.Item2;
                var typeAttributeParam = _returnTypeCustomAttributes.Item2.GetType();
                var typeAttribute = _returnTypeCustomAttributes.Item1;
                var attributeConstructor = typeAttribute.GetConstructor(new Type[] { typeAttributeParam });
                var attributeBuilder = new CustomAttributeBuilder(attributeConstructor!, new object[] { attributeParam });

                var returnValueParameterBuilder =
                    getterMethodBuilder.DefineParameter(_indexParameterBuilder.Count, ParameterAttributes.Retval, null);
                returnValueParameterBuilder.SetCustomAttribute(attributeBuilder);
            }

            propertyBuilder.SetGetMethod(getterMethodBuilder);
        }

        if (IsSettable)
        {
            var parameterTypes = Enumerable
                .Repeat(PropertyType, 1)
                .Concat(_indexParameterBuilder.GetParameterTypes())
                .ToArray();

            var specialName = $"set_{Name}";
            var setterMethodBuilder = _dynamicTypeBuilder.TypeBuilder.DefineMethod(
                specialName,
                _methodAttributes,
                CallingConventions.HasThis,
                null,
                parameterTypes);

            _indexParameterBuilder.AddParameters(setterMethodBuilder, 1);

            if (_parameterCustomAttributes != null)
            {
                var attributeParam = _parameterCustomAttributes.Item2;
                var typeAttributeParam = _parameterCustomAttributes.Item2.GetType();
                var typeAttribute = _parameterCustomAttributes.Item1;
                var attributeConstructor = typeAttribute.GetConstructor(new Type[] { typeAttributeParam });
                var attributeBuilder = new CustomAttributeBuilder(attributeConstructor!, new object[] { attributeParam });

                var returnValueParameterBuilder =
                    setterMethodBuilder.DefineParameter(1, ParameterAttributes.HasFieldMarshal, null);

                returnValueParameterBuilder.SetCustomAttribute(attributeBuilder);
            }

            propertyBuilder.SetSetMethod(setterMethodBuilder);
        }

        foreach (var customAttributeBuilder in CustomAttributeBuilder)
        {
            propertyBuilder.SetCustomAttribute(customAttributeBuilder);
        }

        return _dynamicTypeBuilder;
    }

    public DynamicPropertyBuilder WithReturnTypeCustomAttribute<T>(object value)
    {
        _returnTypeCustomAttributes = new Tuple<Type, object>(typeof(T), value);
        return this;
    }

    public DynamicPropertyBuilder WithParameterCustomAttribute<T>(object value)
        where T : Attribute
    {
        _parameterCustomAttributes = new Tuple<Type, object>(typeof(T), value);
        return this;
    }

    public DynamicPropertyBuilder WithIndexParameter<T>()
    {
        return WithIndexParameter(typeof(T));
    }

    public DynamicPropertyBuilder WithIndexParameter(Type parameterType)
    {
        return WithIndexParameter(new ParameterItem(parameterType, null));
    }

    public DynamicPropertyBuilder WithIndexParameter(ParameterItem parameterItem)
    {
        _indexParameterBuilder?.AddParameter(parameterItem);
        return this;
    }

    public DynamicTypeBuilder Build()
    {
        return _dynamicTypeBuilder;
    }
}
