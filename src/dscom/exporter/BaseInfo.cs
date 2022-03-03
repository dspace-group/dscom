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

namespace dSPACE.Runtime.InteropServices.Exporter;

internal class BaseInfo
{
    public BaseInfo(BaseInfo? parent, string itemName)
    {
        Parent = parent;
        ItemName = itemName;
    }

    [Ignore]
    public string ItemName { get; set; }

    [Ignore]
    public IEnumerable<BaseInfo>? OwningCollection { get; set; }

    [Ignore]
    public BaseInfo? Parent { get; }

    public string GetPath(bool considerIndex = false)
    {
        var name = !string.IsNullOrEmpty(ItemName) ? char.ToLowerInvariant(ItemName[0]) + ItemName.Substring(1) : string.Empty;
        if (Parent != null)
        {
            if (OwningCollection != null)
            {
                if (considerIndex)
                {
                    var index = OwningCollection.ToList().IndexOf(this);
                    if (index != -1)
                    {
                        return $"{Parent.GetPath(considerIndex)}.{name}[{index}]";
                    }
                }

                return $"{Parent.GetPath(considerIndex)}.{name}[]";
            }

            return $"{Parent.GetPath(considerIndex)}.{name}";
        }

        return name;
    }
}
