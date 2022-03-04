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

using System.Reflection;
using System.Runtime.InteropServices;

namespace dSPACE.Runtime.InteropServices.Writer;

internal class StructWriter : TypeWriter
{
    public StructWriter(Type sourceType, LibraryWriter libraryWriter, WriterContext context) : base(sourceType, libraryWriter, context)
    {
        TypeKind = TYPEKIND.TKIND_RECORD;
    }

    public override void CreateTypeInfo()
    {
        base.CreateTypeInfo();
        if (TypeInfo != null)
        {
            TypeInfo.SetAlignment(8);
        }
    }
    public override void Create()
    {
        Context.LogVerbose($"Creating struct {Name}");

        uint index = 0;

        foreach (var item in SourceType.GetFields())
        {
            var comVisible = true;
            if (item.GetCustomAttribute<ComVisibleAttribute>() != null)
            {
                comVisible = item.GetCustomAttribute<ComVisibleAttribute>()!.Value;
            }

            // Create members only if the struct is visible to COM and the field public and not static.
            if (comVisible && item.IsPublic && !item.IsStatic)
            {
                var elemDescWriter = new ElemDescBasedWriter(item.FieldType, item, SourceType, TypeInfo, Context);
                elemDescWriter.Create();

                elemDescWriter.ReportEvent();

                var varDesc = new VARDESC()
                {
                    elemdescVar = elemDescWriter.ElementDescription,
                    varkind = item.IsStatic ? VARKIND.VAR_STATIC : VARKIND.VAR_PERINSTANCE,
                    memid = unchecked((int)Constants.DISPID_UNKNWN)
                };

                TypeInfo.AddVarDesc(index, varDesc).ThrowIfFailed($"Error adding {item.Name} to {SourceType.Name}.");
                TypeInfo.SetVarName(index, Context.NameResolver.GetMappedName(item.Name)).ThrowIfFailed($"Error setting name {item.Name} for {item.FieldType} in {SourceType.Name}.");
            }
            index++;
        }
    }
}
