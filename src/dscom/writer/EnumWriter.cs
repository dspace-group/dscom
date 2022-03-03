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
using System.Runtime.InteropServices;
using dSPACE.Runtime.InteropServices.ComTypes;

namespace dSPACE.Runtime.InteropServices.Writer;

internal class EnumWriter : TypeWriter
{
    public EnumWriter(Type sourceType, LibraryWriter libraryWriter, WriterContext context) : base(sourceType, libraryWriter, context)
    {
        TypeKind = TYPEKIND.TKIND_ENUM;
    }

    public override void Create()
    {
        ((ITypeInfo)TypeInfo).GetDocumentation(-1, out var name, out var docString, out var helpContext, out var helpFile);

        uint index = 0;
        var enumNames = SourceType.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
            .OrderBy(field => field.MetadataToken).Select(fieldInfo => fieldInfo.Name);
        foreach (var item in enumNames)
        {
            var varDesc = new VARDESC();
            var varDescSymbConst = new VARIANT();

            var enumValue = Enum.Parse(SourceType, item.ToString());
            var enumLongValue = (long)Convert.ChangeType(enumValue, typeof(long), CultureInfo.InvariantCulture);
            varDescSymbConst.byref = new IntPtr(enumLongValue);
            varDescSymbConst.vt = VarEnum.VT_I4;

            varDesc.desc.lpvarValue = StructureToPtr(varDescSymbConst);
            varDesc.elemdescVar.desc.idldesc.dwReserved = IntPtr.Zero;
            varDesc.elemdescVar.desc.idldesc.wIDLFlags = IDLFLAG.IDLFLAG_NONE;
            varDesc.elemdescVar.desc.paramdesc.lpVarValue = IntPtr.Zero;
            varDesc.elemdescVar.desc.paramdesc.wParamFlags = PARAMFLAG.PARAMFLAG_NONE;
            varDesc.elemdescVar.tdesc.vt = (short)VarEnum.VT_I4;
            varDesc.lpstrSchema = null!;
            varDesc.memid = -1;
            varDesc.varkind = VARKIND.VAR_CONST;
            varDesc.wVarFlags = 0;

            TypeInfo.AddVarDesc(index, varDesc)
                .ThrowIfFailed($"Failed to add variable description for enum {Name}.");
            TypeInfo.SetVarName(index, $"{Context.NameResolver.GetMappedName(Name)}_{item}")
                .ThrowIfFailed($"Failed to set variable name for enum {Name}.");
            index++;
        }
    }
}
