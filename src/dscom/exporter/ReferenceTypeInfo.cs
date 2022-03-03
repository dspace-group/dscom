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

internal class ReferenceTypeInfo : BaseInfo
{
    public ReferenceTypeInfo(ITypeInfo2 typeInfo, ITypeInfo2 typeInfoUsingType, BaseInfo? parent, string itemName) : base(parent, itemName)
    {
        typeInfo.GetDocumentation(-1, out var typeInfoName, out var _, out var _, out var _);
        Name = typeInfoName;
        typeInfo.GetContainingTypeLib(out var typeLib, out var _);
        var typeLib2 = (ITypeLib2)typeLib;
        typeInfoUsingType.GetContainingTypeLib(out var typeLibUsingType, out var _);
        var typeLibUsingType2 = (ITypeLib2)typeLibUsingType;

        var typeLibAttributesInfoUsingType = new TypeLibAttributesInfo(typeLibUsingType2, null, string.Empty);

        var typeLibAttributes = new TypeLibAttributesInfo(typeLib2, null, string.Empty);
        var typeAttributes = new TypeAttributeInfo(typeInfo, null, string.Empty);

        if (typeLibAttributesInfoUsingType.Guid != typeLibAttributes.Guid)
        {
            typeLib2.GetDocumentation(-1, out var typeLibName, out var _, out var _, out var _);

            IsImported = true;
            ImportedReferenceType = new ImportedReferenceTypeInfo(typeAttributes.Guid, typeAttributes.Guid, typeLibName, this, nameof(ImportedReferenceType));
        }
        else
        {
            IsImported = false;
        }
    }

    public string? Name { get; private set; }

    public bool IsImported { get; private set; }

    public ImportedReferenceTypeInfo? ImportedReferenceType { get; private set; }
}
