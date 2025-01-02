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

        if (isSettable)
        {
            _setterParameterBuilder = new();
            _setterParameterBuilder.AddParameter(new ParameterItem(PropertyType, ParameterAttributes.HasFieldMarshal));
        }

        if (isGettable)
        {
            _getterParameterBuilder = new();
        }
    }

    public Type PropertyType { get; }

    private readonly DynamicTypeBuilder _dynamicTypeBuilder;

    private Tuple<Type, object>? _returnTypeCustomAttributes;

    protected override AttributeTargets AttributeTarget => AttributeTargets.Property;

    private readonly DynamicParameterBuilder? _getterParameterBuilder;

    private readonly DynamicParameterBuilder? _setterParameterBuilder;

    private static readonly MethodAttributes _methodAttributes =
        MethodAttributes.Public
        | MethodAttributes.SpecialName
        | MethodAttributes.HideBySig
        | MethodAttributes.Abstract
        | MethodAttributes.Virtual;

    public DynamicTypeBuilder CreateProperty()
    {
        var propertyBuilder = _dynamicTypeBuilder.TypeBuilder!.DefineProperty(Name, PropertyAttributes.None, CallingConventions.HasThis, PropertyType, null);

        if (_getterParameterBuilder is not null)
        {
            var specialName = $"get_{Name}";
            var getterMethodBuilder = _dynamicTypeBuilder.TypeBuilder.DefineMethod(
                specialName,
                _methodAttributes,
                CallingConventions.HasThis,
                PropertyType,
                _getterParameterBuilder.GetParameterTypes());

            if (_returnTypeCustomAttributes != null)
            {
                var attributeParam = _returnTypeCustomAttributes.Item2;
                var typeAttributeParam = _returnTypeCustomAttributes.Item2.GetType();
                var typeAttribute = _returnTypeCustomAttributes.Item1;
                var attributeConstructor = typeAttribute.GetConstructor(new Type[] { typeAttributeParam });
                var attributeBuilder = new CustomAttributeBuilder(attributeConstructor!, new object[] { attributeParam });

                var returnValueParameterBuilder =
                    getterMethodBuilder.DefineParameter(0, ParameterAttributes.Retval, null);
                returnValueParameterBuilder.SetCustomAttribute(attributeBuilder);
            }

            _getterParameterBuilder.AddParameters(getterMethodBuilder);

            propertyBuilder.SetGetMethod(getterMethodBuilder);
        }

        if (_setterParameterBuilder is not null)
        {
            var specialName = $"set_{Name}";
            var setterMethodBuilder = _dynamicTypeBuilder.TypeBuilder.DefineMethod(
                specialName,
                _methodAttributes,
                CallingConventions.HasThis,
                null,
                _setterParameterBuilder.GetParameterTypes());

            _setterParameterBuilder.AddParameters(setterMethodBuilder);

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
        _setterParameterBuilder?.AddParameterAttribute<T>(0, value);
        return this;
    }

    public DynamicPropertyBuilder WithIndexParameter<T>()
    {
        return WithIndexParameter(typeof(T));
    }

    public DynamicPropertyBuilder WithIndexParameter(Type parameterType)
    {
        return WithIndexParameter(new ParameterItem(parameterType, ParameterAttributes.In));
    }

    public DynamicPropertyBuilder WithIndexParameter(ParameterItem parameterItem)
    {
        _setterParameterBuilder?.AddParameter(parameterItem);
        _getterParameterBuilder?.AddParameter(parameterItem);
        return this;
    }

    public DynamicTypeBuilder Build()
    {
        return _dynamicTypeBuilder;
    }
}
