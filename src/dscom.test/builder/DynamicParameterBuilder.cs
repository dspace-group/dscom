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

internal record struct DefaultValue(
        object? Value
    );

internal record struct ParameterItem(
        Type Type,
        ParameterAttributes? ParameterAttributes,
        DefaultValue? DefaultValue = null
    );

internal class DynamicParameterBuilder
{
    public int Count => _parameterItems.Count;

    private readonly List<ParameterItem> _parameterItems = new();

    private Tuple<Type, object?>[]? _paramAttributes;

    private readonly Dictionary<int, Tuple<FieldInfo[], object?[]>> _paramAttributesFieldValues = new();

    public void AddParameterAttribute<T>(int parameterIndex, object value)
    {
        var attributes = _paramAttributes ?? new Tuple<Type, object?>[_parameterItems == null ? 0 : _parameterItems.Count];
        attributes[parameterIndex] = new(typeof(T), value);
        _paramAttributes = attributes;
    }

    public void AddParameterCustomAttributeFieldValue(int parameterIndex, FieldInfo[] namedFields, object?[] fieldValues)
    {
        _paramAttributesFieldValues[parameterIndex] = new(namedFields, fieldValues);
    }

    public Type[] GetParameterTypes()
    {
        return _parameterItems.Select(p => p.Type).ToArray();
    }
    public void AddParameter(ParameterItem parameter)
    {
        _parameterItems.Add(parameter);
    }

    public void AddParameters(MethodBuilder methodBuilder, int index = 1)
    {
        for (var iParameter = 0; iParameter < _parameterItems.Count; iParameter++)
        {
            var item = _parameterItems[iParameter];

            var hasDefaultValue = item.DefaultValue != null;

            var parameterAttribute = item.ParameterAttributes;
            parameterAttribute ??= hasDefaultValue ? ParameterAttributes.HasDefault : ParameterAttributes.None;

            var paramBuilder = methodBuilder.DefineParameter(index, parameterAttribute.Value, $"Param{index}");
            if (_paramAttributes != null)
            {
                //use parameter attributes
                if ((_paramAttributes.Length > iParameter) && _paramAttributes.GetValue(iParameter) != null)
                {
                    var attributeParam = _paramAttributes[iParameter].Item2;
                    var typeAttributeParam = _paramAttributes[iParameter].Item2 == null ? typeof(object) : _paramAttributes[iParameter].Item2?.GetType();
                    var typeAttribute = _paramAttributes[iParameter].Item1;

                    var attributeConstructor = attributeParam != null && typeAttributeParam != null ? typeAttribute.GetConstructor(new Type[] { typeAttributeParam }) : typeAttribute.GetConstructor(Array.Empty<Type>());

                    _paramAttributesFieldValues.TryGetValue(iParameter, out var fields);

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
    }
}
