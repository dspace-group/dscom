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

using System.Runtime.InteropServices;

namespace dSPACE.Runtime.InteropServices.Exporter;

internal class CustomDataItemInfo : BaseInfo
{
    private static readonly Dictionary<Guid, string> _guids = new();

    static CustomDataItemInfo()
    {
        foreach (var field in typeof(Guids).GetFields())
        {
            var value = field.GetValue(null);

            if (value != null)
            {
                try
                {
                    _guids[new Guid(value.ToString() ?? string.Empty)] = field.Name;
                }
                catch (FormatException) { }
            }
        }
    }

    public CustomDataItemInfo(CUSTDATAITEM item, BaseInfo? parent, string itemName) : base(parent, itemName)
    {
        Guid = item.guid.ToString();
        GuidConstant = GetNameOfGuidConstant(item.guid);

        var len = Marshal.SizeOf(item.varValue);
        var ptr = Marshal.AllocHGlobal(len);
        Marshal.StructureToPtr(item.varValue, ptr, false);
        var value = Marshal.GetObjectForNativeVariant(ptr);
        Marshal.FreeHGlobal(ptr);
        Value = value?.ToString() ?? string.Empty;
    }

    private static string GetNameOfGuidConstant(Guid guid)
    {
        _guids.TryGetValue(guid, out var name);
        return name ?? string.Empty;
    }

    public string Guid { get; }

    public string GuidConstant { get; }

    public string Value { get; }
}
