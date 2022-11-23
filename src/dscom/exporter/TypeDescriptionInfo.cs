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

internal sealed class TypeDescriptionInfo : BaseInfo
{
    public TypeDescriptionInfo(ITypeInfo2 typeInfo, TYPEDESC typeDesc, BaseInfo? parent, string itemName) : base(parent, itemName)
    {
        var varEnumTypes = string.Empty;
        while ((VarEnum)typeDesc.vt is VarEnum.VT_PTR or VarEnum.VT_SAFEARRAY)
        {
            if (typeDesc.lpValue != IntPtr.Zero)
            {
                varEnumTypes += ((VarEnum)typeDesc.vt).ToString() + " -> ";

                var childTypeDesc = Marshal.PtrToStructure<TYPEDESC>(typeDesc.lpValue);
                typeDesc = childTypeDesc;
            }
        }

        try
        {
            if ((VarEnum)typeDesc.vt == VarEnum.VT_USERDEFINED)
            {
                var hrefType = typeDesc.lpValue;
                var typeInfo64Bit = (ITypeInfo64Bit)typeInfo;
                typeInfo64Bit.GetRefTypeInfo(hrefType, out var refTypeInfo64Bit);
                var refTypeInfo = (ITypeInfo2)refTypeInfo64Bit;

                RefType = new ReferenceTypeInfo(refTypeInfo, typeInfo, this, nameof(RefType));
            }

            varEnumTypes += ((VarEnum)typeDesc.vt).ToString();
        }
        catch (Exception e)
        {
            typeInfo.GetDocumentation(-1, out var typeName, out _, out _, out _);
            varEnumTypes += VarEnum.VT_UNKNOWN.ToString();
            Console.WriteLine($"Failed to get RefType {typeName} {e.Message}");
        }

        Type = varEnumTypes;
    }

    public string? Type { get; private set; }

    public ReferenceTypeInfo? RefType { get; private set; }
}
