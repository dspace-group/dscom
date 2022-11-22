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

internal sealed class TypeInfo : BaseInfo
{
    public TypeInfo(ITypeInfo2 typeInfo, BaseInfo? parent, string itemName) : base(parent, itemName)
    {
        typeInfo.GetDocumentation(-1, out var typeName, out var docString, out var helpContext, out var helpFile);

        Name = typeName;
        DocString = docString;
        HelpContext = helpContext;
        HelpFile = helpFile;
        Attributes = new TypeAttributeInfo(typeInfo, this, nameof(Attributes));

        if (Attributes.Kind is TYPEKIND.TKIND_ENUM or TYPEKIND.TKIND_RECORD)
        {
            for (var i = 0; i < Attributes.NumberOfVariablesAndDatafields; i++)
            {
                typeInfo.GetVarDesc(i, out var ppvarDesc);
                var varDesc = Marshal.PtrToStructure<VARDESC>(ppvarDesc);
                VariableDescription.Add(new VariableDescriptionInfo(typeInfo, varDesc, this, nameof(VariableDescription)) { OwningCollection = VariableDescription });
                typeInfo.ReleaseVarDesc(ppvarDesc);
            }
        }

        for (var i = 0; i < Attributes.NumberOfImplementedInterfaces; i++)
        {
            typeInfo.GetRefTypeOfImplType(i, out var href);
            typeInfo.GetRefTypeInfo(href, out var refTypeInfo);
            var refTypeInfo2 = (ITypeInfo2)refTypeInfo;
            ImplementedInterfaces.Add(new ImplementationReferenceTypeInfo(refTypeInfo2, typeInfo, i, this, nameof(ImplementedInterfaces)) { OwningCollection = ImplementedInterfaces });
        }

        for (var i = 0; i < Attributes.NumberOfFunctions; i++)
        {
            typeInfo.GetFuncDesc(i, out var ppFuncDesc);
            var funcDesc = Marshal.PtrToStructure<FUNCDESC>(ppFuncDesc);
            var method = new FunctionInfo(typeInfo, funcDesc, this, nameof(Functions)) { OwningCollection = Functions };
            Functions.Add(method);
            typeInfo.ReleaseFuncDesc(ppFuncDesc);
        }
    }

    public string Name { get; private set; }

    public string DocString { get; private set; }

    public int HelpContext { get; private set; }

    public string HelpFile { get; private set; }

    public TypeAttributeInfo Attributes { get; private set; }

    public List<ReferenceTypeInfo> ImplementedInterfaces { get; } = new List<ReferenceTypeInfo>();

    public List<VariableDescriptionInfo> VariableDescription { get; } = new List<VariableDescriptionInfo>();

    public List<FunctionInfo> Functions { get; } = new List<FunctionInfo>();
}
