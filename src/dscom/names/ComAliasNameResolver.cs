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

using dSPACE.Runtime.InteropServices.Attributes;
using System.Reflection;
using System.Runtime.InteropServices;

namespace dSPACE.Runtime.InteropServices;

internal sealed class ComAliasNameResolver : INameResolver
{
    private readonly Dictionary<object, string> _names = new Dictionary<object, string>();

    public ComAliasNameResolver(Assembly assembly)
    {
        var comVisibleTypes = assembly.GetTypes().Where(t => t.IsPublic && t.GetCustomAttribute<ComVisibleAttribute>() != null);
        var types = comVisibleTypes
            .Where(t => t.GetCustomAttribute<ComAliasAttribute>() != null)
            .ToDictionary(t => t as object, t => t.GetCustomAttribute<ComAliasAttribute>()?.Alias ?? string.Empty)
        ;
        foreach(var kv in types)
        {
            _names.Add(kv.Key, kv.Value);
        }

        var members = comVisibleTypes
            .SelectMany(t => t.GetMembers().Where(m => m.GetCustomAttribute<ComAliasAttribute>() != null))
            .ToDictionary(m => m as object, m => m.GetCustomAttribute<ComAliasAttribute>()?.Alias ?? string.Empty)
        ;
        foreach (var kv in members)
        {
            _names.Add(kv.Key, kv.Value);
        }

        var parameters = comVisibleTypes
            .SelectMany(t => t.GetMethods())
            .SelectMany(m => m.GetParameters().Where(p => p.GetCustomAttribute<ComAliasAttribute>() != null))
            .ToDictionary(p => p as object, p => p.GetCustomAttribute<ComAliasAttribute>()?.Alias ?? string.Empty)
        ;
        foreach (var kv in parameters)
        {
            _names.Add(kv.Key, kv.Value);
        }
    }

    public string GetMappedName(Type type, string name)
    {
        if (_names.TryGetValue(type, out var mappedName))
        {
            return mappedName;
        }
        return name;
    }

    public string GetMappedName(MethodInfo method, string name)
    {
        if (_names.TryGetValue(method, out var mappedName))
        {
            return mappedName;
        }
        return name;
    }

    public string GetMappedName(FieldInfo field, string name)
    {
        if (_names.TryGetValue(field, out var mappedName))
        {
            return mappedName;
        }
        else if (field.DeclaringType != null && field.DeclaringType.IsEnum)
        {
            if (_names.TryGetValue(field.DeclaringType, out var aliasName))
            {
                if (name == field.DeclaringType.Name + "_" + field.Name)
                {
                    return aliasName + "_" + field.Name;
                }
            }
        }

        return name;
    }

    public string GetMappedName(ParameterInfo parameter, string name)
    {
        if (_names.TryGetValue(parameter, out var mappedName))
        {
            return mappedName;
        }
        return name;
    }

    public string GetMappedName(PropertyInfo property, string name)
    {
        if (_names.TryGetValue(property, out var mappedName))
        {
            return mappedName;
        }
        return name;
    }
}
