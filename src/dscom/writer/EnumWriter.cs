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

using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;

namespace dSPACE.Runtime.InteropServices.Writer;

internal sealed class EnumWriter : TypeWriter
{
    public EnumWriter(Type sourceType, LibraryWriter libraryWriter, WriterContext context) : base(sourceType, libraryWriter, context)
    {
        TypeKind = TYPEKIND.TKIND_ENUM;
    }

    public override void Create()
    {
        ((ITypeInfo)TypeInfo).GetDocumentation(-1, out var name, out var docString, out var helpContext, out var helpFile);

        uint index = 0;
        var fields =
            SourceType.GetFields(BindingFlags.Public | BindingFlags.Static)
            .OrderBy(field => field.MetadataToken);
        foreach (var field in fields)
        {
            var varDesc = new VARDESC();
            var varDescSymbConst = new VARIANT();

            var enumValue = Enum.Parse(SourceType, field.Name.ToString());
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
                .ThrowIfFailed($"Failed to add variable description for enum {SourceType}.");
            TypeInfo.SetVarName(index, Context.NameResolver.GetMappedName(field, $"{Name}_{field.Name}"))
                .ThrowIfFailed($"Failed to set variable name {field.Name} for enum {SourceType}.");

            // If the field has a DescriptionAttribute, use it as the variable description.
            var descriptionAttribute = field.GetCustomAttribute<DescriptionAttribute>(false);
            TypeInfo.SetVarDocString(index, descriptionAttribute?.Description ?? string.Empty)
                .ThrowIfFailed($"Failed to set variable description for enum {SourceType}.");

            index++;
        }

        Context.LogTypeExported($"Enum '{Name}' exported.");
    }
}
