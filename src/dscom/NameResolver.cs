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

namespace dSPACE.Runtime.InteropServices;

internal sealed class NameResolver
{
    private readonly Dictionary<string, string> _names = new();

    public NameResolver(string[] names)
    {
        foreach (var name in names)
        {
            if (!_names.ContainsKey(name.ToLower(CultureInfo.InvariantCulture)))
            {
                _names.Add(name.ToLower(CultureInfo.InvariantCulture), name);
            }
        }
    }

    public string GetMappedName(string name)
    {
        var lowerCaseName = name.ToLower(CultureInfo.InvariantCulture);
        if (_names.TryGetValue(lowerCaseName, out var mappedName))
        {
            return mappedName;
        }
        return name;
    }
}
