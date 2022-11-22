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

internal sealed class ParameterDescriptionInfo : BaseInfo
{
    public ParameterDescriptionInfo(ELEMDESC descunion, BaseInfo? parent, string itemName) : this(descunion.desc, parent, itemName)
    {
    }

    public ParameterDescriptionInfo(ELEMDESC.DESCUNION descunion, BaseInfo? parent, string itemName) : this(descunion.paramdesc, parent, itemName)
    {
    }

    public ParameterDescriptionInfo(PARAMDESC paramdesc, BaseInfo? parent, string itemName) : base(parent, itemName)
    {
        ParameterFlags = paramdesc.wParamFlags.ToString();
        if (paramdesc.wParamFlags.HasFlag(PARAMFLAG.PARAMFLAG_FHASDEFAULT))
        {
            if (IntPtr.Zero != paramdesc.lpVarValue)
            {
                var defptr = new IntPtr((long)paramdesc.lpVarValue + sizeof(ulong));

                if (defptr != IntPtr.Zero)
                {
                    var valueObject = Marshal.GetObjectForNativeVariant(defptr);

                    DefaultValue = valueObject == null ? "null" : valueObject.ToString();
                }
            }
        }
    }

    public string? ParameterFlags { get; private set; }

    public string? DefaultValue { get; private set; }
}
