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

internal class DynamicPropertyBuilder : DynamicBuilder<DynamicPropertyBuilder>
{
    public DynamicPropertyBuilder(DynamicTypeBuilder dynamicTypeBuilder, string name, Type propertyType, bool isSettable = true, bool isGettable = true) : base(name)
    {
        DynamicTypeBuilder = dynamicTypeBuilder;
        PropertyType = propertyType;
        IsSettable = isSettable;
        IsGettable = isGettable;
    }

    public DynamicTypeBuilder DynamicTypeBuilder { get; }

    public Type PropertyType { get; }

    public bool IsSettable { get; }

    public bool IsGettable { get; }

    public Tuple<Type, object>? ReturnTypeCustomAttributes { get; private set; }

    public Tuple<Type, object>? ParameterCustomAttributes { get; private set; }

    protected override AttributeTargets AttributeTarget => AttributeTargets.Property;

    public DynamicTypeBuilder CreateProperty()
    {
        var propertyBuilder = DynamicTypeBuilder.TypeBuilder!.DefineProperty(Name, PropertyAttributes.None, CallingConventions.HasThis, PropertyType, null);

        if (IsGettable)
        {
            var specialName = $"get_{Name}";
            var getterMethdoBuilder = DynamicTypeBuilder.TypeBuilder.DefineMethod(specialName,
                MethodAttributes.Public | MethodAttributes.SpecialName |
                MethodAttributes.HideBySig | MethodAttributes.Abstract | MethodAttributes.Virtual,
                CallingConventions.HasThis,
                PropertyType, null);

            if (ReturnTypeCustomAttributes != null)
            {
                var attributeParam = ReturnTypeCustomAttributes.Item2;
                var typeAttributeParam = ReturnTypeCustomAttributes.Item2.GetType();
                var typeAttribute = ReturnTypeCustomAttributes.Item1;
                var attributeConstructor = typeAttribute.GetConstructor(new Type[] { typeAttributeParam });
                var attributeBuilder = new CustomAttributeBuilder(attributeConstructor!, new object[] { attributeParam });

                var returnValueParameterBuilder =
                    getterMethdoBuilder.DefineParameter(0, ParameterAttributes.Retval, null);

                returnValueParameterBuilder.SetCustomAttribute(attributeBuilder);
            }

            propertyBuilder.SetGetMethod(getterMethdoBuilder);
        }
        if (IsSettable)
        {
            var specialName = $"set_{Name}";
            var setterMethdoBuilder = DynamicTypeBuilder.TypeBuilder.DefineMethod(specialName,
                MethodAttributes.Public | MethodAttributes.SpecialName |
                MethodAttributes.HideBySig | MethodAttributes.Abstract | MethodAttributes.Virtual,
                CallingConventions.HasThis,
                null, new Type[] { PropertyType });

            if (ParameterCustomAttributes != null)
            {
                var attributeParam = ParameterCustomAttributes.Item2;
                var typeAttributeParam = ParameterCustomAttributes.Item2.GetType();
                var typeAttribute = ParameterCustomAttributes.Item1;
                var attributeConstructor = typeAttribute.GetConstructor(new Type[] { typeAttributeParam });
                var attributeBuilder = new CustomAttributeBuilder(attributeConstructor!, new object[] { attributeParam });

                var returnValueParameterBuilder =
                setterMethdoBuilder.DefineParameter(1, ParameterAttributes.HasFieldMarshal, null);

                returnValueParameterBuilder.SetCustomAttribute(attributeBuilder);
            }

            propertyBuilder.SetSetMethod(setterMethdoBuilder);
        }

        foreach (var customAttributeBuilder in CustomAttributeBuilder)
        {
            propertyBuilder.SetCustomAttribute(customAttributeBuilder);
        }

        return DynamicTypeBuilder;
    }


    public DynamicPropertyBuilder WithReturnTypeCustomAttribute<T>(object value)
    {
        ReturnTypeCustomAttributes = new Tuple<Type, object>(typeof(T), value);
        return this;

    }

    public DynamicPropertyBuilder WithParameterCustomAttribute<T>(object value)
    {
        ParameterCustomAttributes = new Tuple<Type, object>(typeof(T), value);
        return this;
    }

    public DynamicTypeBuilder Build()
    {
        return DynamicTypeBuilder;
    }
}
