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
using System.Reflection;

namespace dSPACE.Runtime.InteropServices;
internal sealed class SimpleNameResolver : INameResolver
{
    private readonly IDictionary<string, string> _names = new Dictionary<string, string>();

    public SimpleNameResolver(IEnumerable<string> names)
    {
        foreach (var name in names)
        {
            if (!_names.ContainsKey(name.ToLower(CultureInfo.InvariantCulture)))
            {
                _names.Add(name.ToLower(CultureInfo.InvariantCulture), name);
            }
        }
    }

    public string GetMappedName(Type type, string name)
    {
        var lowerCaseName = type.Name.ToLower(CultureInfo.InvariantCulture);
        if (_names.TryGetValue(lowerCaseName, out var mappedName))
        {
            return mappedName;
        }
        return name;
    }

    public string GetMappedName(MethodInfo method, string name)
    {
        var lowerCaseName = method.Name.ToLower(CultureInfo.InvariantCulture);
        if (_names.TryGetValue(lowerCaseName, out var mappedName))
        {
            return mappedName;
        }
        return name;
    }

    public string GetMappedName(FieldInfo field, string name)
    {
        // Enums requires additional handling due to the fact that they are handled
        // slightly differently in the type library so we must dismabiguate between 
        // a regular field on any other elements from a field of an Enum type and 
        // ensure the names of the fields are handled correctly. 

        var isEnum = false;
        var enumName = string.Empty;

        if (field.DeclaringType != null && field.DeclaringType.IsEnum)
        {
            isEnum = field.DeclaringType.IsEnum;
            enumName = field.DeclaringType.Name;
        }

        if (_names.TryGetValue(enumName.ToLower(CultureInfo.InvariantCulture), out var aliasName))
        {
            enumName = aliasName;
        }

        var builtName = isEnum
            ? enumName + "_" + field.Name
            : field.Name;

        if (_names.TryGetValue(builtName.ToLower(CultureInfo.InvariantCulture), out var mappedName))
        {
            return mappedName;
        }

        if (isEnum)
        {
            // Handle the case where there are namespace collisions in which case, the provided 
            // name parameter that has the disambiguated name built. 
            if (name.EndsWith("_" + builtName, StringComparison.InvariantCultureIgnoreCase))
            {
                return name;
            }

            return builtName;
        }

        return name;
    }

    public string GetMappedName(ParameterInfo parameter, string name)
    {
        var lowerCaseName = parameter?.Name?.ToLower(CultureInfo.InvariantCulture) ?? string.Empty;
        if (!string.IsNullOrWhiteSpace(lowerCaseName) && _names.TryGetValue(lowerCaseName, out var mappedName))
        {
            return mappedName;
        }
        return name;
    }

    public string GetMappedName(PropertyInfo prop, string name)
    {
        var lowerCaseName = prop.Name.ToLower(CultureInfo.InvariantCulture);
        if (_names.TryGetValue(lowerCaseName, out var mappedName))
        {
            return mappedName;
        }
        return name;
    }
}
