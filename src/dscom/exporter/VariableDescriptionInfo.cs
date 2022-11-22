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

internal sealed class VariableDescriptionInfo : BaseInfo
{
    public VariableDescriptionInfo(ITypeInfo2 typeInfo, VARDESC varDesc, BaseInfo? parent, string itemName) : base(parent, itemName)
    {
        if (varDesc.varkind is VARKIND.VAR_CONST or VARKIND.VAR_PERINSTANCE)
        {
            typeInfo!.GetDocumentation(varDesc.memid, out var name, out _, out _, out _);
            Name = string.IsNullOrEmpty(name) ? null : name;
            Value = new TypeDescriptionInfo(typeInfo, varDesc.elemdescVar.tdesc, this, nameof(Value));
            Type = (VarEnum)varDesc.elemdescVar.tdesc.vt;
        }
    }

    public string? Name { get; private set; }

    public VarEnum? Type { get; private set; }

    public object? Value { get; private set; }
}
