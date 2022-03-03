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

internal abstract class DynamicBuilder<T> where T : DynamicBuilder<T>
{
    public DynamicBuilder(string name)
    {
        Name = name;
    }

    public List<CustomAttributeBuilder> CustomAttributeBuilder { get; } = new List<CustomAttributeBuilder>();

    public string Name { get; }

    public T WithCustomAttribute(Type type, Type[]? constructorParamTypes, object[]? values)
    {
        var dispIDAttributeConstructor = type.GetConstructor(constructorParamTypes ?? Array.Empty<Type>());
        if (dispIDAttributeConstructor == null)
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

        var dispIDAttributeBuilder = new CustomAttributeBuilder(dispIDAttributeConstructor!, values ?? Array.Empty<object>());
        CustomAttributeBuilder.Add(dispIDAttributeBuilder);
        return (T)this;
    }

    protected abstract AttributeTargets AttributeTarget { get; }

    public T WithCustomAttribute(Type type, object value)
    {
        return WithCustomAttribute(type, new Type[] { value.GetType() }, new object[] { value });
    }

    public T WithCustomAttribute<K>(object value)
    {
        return WithCustomAttribute(typeof(K), new Type[] { value.GetType() }, new object[] { value });
    }

    public T WithCustomAttribute<K>()
    {
        return WithCustomAttribute(typeof(K), null, null);
    }
}
