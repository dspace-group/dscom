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

internal sealed class ElementDescriptionInfo : BaseInfo
{
    public ElementDescriptionInfo(ITypeInfo2 typeInfo, ELEMDESC elementDescription, string name, BaseInfo? parent, string itemName) : base(parent, itemName)
    {
        Name = string.IsNullOrEmpty(name) ? "rhs" : name;

        IdlFlags = elementDescription.desc.idldesc.wIDLFlags;
        ParameterFlags = elementDescription.desc.paramdesc.wParamFlags;

        TypeDescription = new TypeDescriptionInfo(typeInfo, elementDescription.tdesc, this, nameof(TypeDescription));
        ParameterDescription = new ParameterDescriptionInfo(elementDescription, this, nameof(ParameterDescription));
    }

    public string? Name { get; private set; }

    public IDLFLAG IdlFlags { get; private set; }

    public PARAMFLAG ParameterFlags { get; private set; }

    public TypeDescriptionInfo? TypeDescription { get; private set; }

    public ParameterDescriptionInfo? ParameterDescription { get; private set; }
}
