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

internal class DynamicMethodBuilder : DynamicBuilder<DynamicMethodBuilder>
{
    internal record struct DefaultValue(
        object? Value
    );

    internal record struct ParameterItem(
            Type Type,
            ParameterAttributes? ParameterAttributes,
            DefaultValue? DefaultValue = null
        );

    public DynamicMethodBuilder(DynamicTypeBuilder dynamicTypeBuilder, string name) : base(name)
    {
        DynamicTypeBuilder = dynamicTypeBuilder;
    }

    public DynamicTypeBuilder DynamicTypeBuilder { get; }

    public Type? ReturnType { get; set; } = typeof(void);

    private Tuple<Type, object>? ReturnTypeAttribute { get; set; }

    public List<ParameterItem> ParamterItems { get; } = new();

    public Tuple<Type, object?>[]? ParamAttributes { get; set; }

    public Dictionary<int, Tuple<FieldInfo[], object?[]>> ParamAttributesFieldValues { get; set; } = new();

    protected override AttributeTargets AttributeTarget => AttributeTargets.Method;

    public DynamicMethodBuilder WithReturnType(Type type)
    {
        ReturnType = type;
        return this;
    }

    public DynamicMethodBuilder WithReturnType<T>()
    {
        ReturnType = typeof(T);
        return this;
    }

    public DynamicMethodBuilder WithParameter<T>()
    {
        return WithParameter(typeof(T));
    }

    public DynamicMethodBuilder WithParameter(Type parameterType)
    {
        ParamterItems.Add(new ParameterItem(parameterType, null));
        return this;
    }

    public DynamicMethodBuilder WithParameter(ParameterItem parameterItem)
    {
        ParamterItems.Add(parameterItem);
        return this;
    }

    public DynamicMethodBuilder WithReturnTypeCustomAttribute<T>(object value)
    {
        ReturnTypeAttribute = new Tuple<Type, object>(typeof(T), value);
        return this;
    }

    public DynamicMethodBuilder WithParameterCustomAttribute<T>(int parameterIndex)
    {
        var attributes = ParamAttributes ?? new Tuple<Type, object?>[ParamterItems == null ? 0 : ParamterItems.Count];
        attributes[parameterIndex] = new(typeof(T), null);
        ParamAttributes = attributes;
        return this;
    }

    public DynamicMethodBuilder WithParameterCustomAttribute<T>(int parameterIndex, object value)
    {
        var attributes = ParamAttributes ?? new Tuple<Type, object?>[ParamterItems == null ? 0 : ParamterItems.Count];
        attributes[parameterIndex] = new(typeof(T), value);
        ParamAttributes = attributes;
        return this;
    }

    public DynamicMethodBuilder WithParameterCustomAttribute<T>(int parameterIndex, object value, FieldInfo[] namedFields, object?[] fieldValues)
    {
        var attributes = ParamAttributes ?? new Tuple<Type, object?>[ParamterItems == null ? 0 : ParamterItems.Count];
        attributes[parameterIndex] = new(typeof(T), value);
        ParamAttributes = attributes;
        ParamAttributesFieldValues[parameterIndex] = new(namedFields, fieldValues);
        return this;
    }

    public DynamicTypeBuilder CreateMethod()
    {
        var methodBuilder = DynamicTypeBuilder.TypeBuilder!.DefineMethod(Name,
              MethodAttributes.Abstract | MethodAttributes.Public | MethodAttributes.Virtual,
              CallingConventions.HasThis, ReturnType, ParamterItems.Select(p => p.Type).ToArray());

        var returnParamBuilder = methodBuilder.DefineParameter(0, ParameterAttributes.Retval, null);
        if (ReturnTypeAttribute != null)
        {
            var attributeParam = ReturnTypeAttribute.Item2;
            var typeAttributeParam = ReturnTypeAttribute.Item2.GetType();
            var typeAttribute = ReturnTypeAttribute.Item1;
            var attributeConstructor = typeAttribute.GetConstructor(new Type[] { typeAttributeParam });
            var attributeBuilder = new CustomAttributeBuilder(attributeConstructor!, new object[] { attributeParam });

            returnParamBuilder.SetCustomAttribute(attributeBuilder);
        }

        var index = 1;

        foreach (var item in ParamterItems)
        {
            var hasDefaultValue = item.DefaultValue != null;

            var parameterAttribute = item.ParameterAttributes;
            parameterAttribute ??= hasDefaultValue ? ParameterAttributes.HasDefault : ParameterAttributes.None;

            var paramBuilder = methodBuilder.DefineParameter(index, parameterAttribute.Value, $"Param{index}");
            if (ParamAttributes != null)
            {
                //use parameter attributes
                if ((ParamAttributes.Length > index - 1) && ParamAttributes.GetValue(index - 1) != null)
                {
                    var attributeParam = ParamAttributes[index - 1].Item2;
                    var typeAttributeParam = ParamAttributes[index - 1].Item2 == null ? typeof(object) : ParamAttributes[index - 1].Item2?.GetType();
                    var typeAttribute = ParamAttributes[index - 1].Item1;

                    var attributeConstructor = attributeParam != null && typeAttributeParam != null ? typeAttribute.GetConstructor(new Type[] { typeAttributeParam }) : typeAttribute.GetConstructor(Array.Empty<Type>());

                    ParamAttributesFieldValues.TryGetValue(index - 1, out var fields);

                    var attributeBuilder = attributeParam == null
                        ? new CustomAttributeBuilder(attributeConstructor!, Array.Empty<object>())
                        : fields != null
                            ? new CustomAttributeBuilder(attributeConstructor!, new object[] { attributeParam }, fields.Item1, fields.Item2)
                            : new CustomAttributeBuilder(attributeConstructor!, new object[] { attributeParam });
                    paramBuilder.SetCustomAttribute(attributeBuilder);
                }
            }
            if (hasDefaultValue)
            {
                paramBuilder.SetConstant(item.DefaultValue!.Value.Value);
            }
            index++;
        }

        foreach (var customAttributeBuilder in CustomAttributeBuilder)
        {
            methodBuilder.SetCustomAttribute(customAttributeBuilder);
        }

        return DynamicTypeBuilder;
    }

    public DynamicTypeBuilder Build()
    {
        return DynamicTypeBuilder;
    }
}
