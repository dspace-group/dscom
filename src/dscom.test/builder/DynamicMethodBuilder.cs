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

using System.Reflection.Emit;
using static dSPACE.Runtime.InteropServices.Tests.DynamicMethodBuilder;

namespace dSPACE.Runtime.InteropServices.Tests;

internal sealed class DynamicMethodBuilder : DynamicBuilder<DynamicMethodBuilder>
{
    public DynamicMethodBuilder(DynamicTypeBuilder dynamicTypeBuilder, string name) : base(name)
    {
        DynamicTypeBuilder = dynamicTypeBuilder;
    }

    public DynamicTypeBuilder DynamicTypeBuilder { get; }

    public Type? ReturnType { get; set; } = typeof(void);

    private Tuple<Type, object>? ReturnTypeAttribute { get; set; }

    protected override AttributeTargets AttributeTarget => AttributeTargets.Method;

    private readonly DynamicParameterBuilder parameterBuilder = new();

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
        parameterBuilder.AddParameter(new ParameterItem(parameterType, null));
        return this;
    }

    public DynamicMethodBuilder WithParameter(ParameterItem parameterItem)
    {
        parameterBuilder.AddParameter(parameterItem);
        return this;
    }

    public DynamicMethodBuilder WithReturnTypeCustomAttribute<T>(object value)
    {
        ReturnTypeAttribute = new Tuple<Type, object>(typeof(T), value);
        return this;
    }

    public DynamicMethodBuilder WithParameterCustomAttribute<T>(int parameterIndex)
    {
        return WithParameterCustomAttribute<T>(parameterIndex, null);
    }

    public DynamicMethodBuilder WithParameterCustomAttribute<T>(int parameterIndex, object value)
    {
        parameterBuilder.AddParameterAttribute<T>(parameterIndex, value);
        return this;
    }

    public DynamicMethodBuilder WithParameterCustomAttribute<T>(int parameterIndex, object value, FieldInfo[] namedFields, object?[] fieldValues)
    {
        parameterBuilder.AddParameterAttribute<T>(parameterIndex, value);
        parameterBuilder.AddParameterCustomAttributeFieldValue(parameterIndex, namedFields, fieldValues);
        return this;
    }

    public DynamicTypeBuilder CreateMethod()
    {
        var methodBuilder = DynamicTypeBuilder.TypeBuilder!.DefineMethod(Name,
              MethodAttributes.Abstract | MethodAttributes.Public | MethodAttributes.Virtual,
              CallingConventions.HasThis, ReturnType, parameterBuilder.GetParameterTypes());

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

        parameterBuilder.AddParameters(methodBuilder);

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
