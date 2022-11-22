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

internal sealed class TypeAttributeInfo : BaseInfo
{
    public TypeAttributeInfo(ITypeInfo2 typeInfo, BaseInfo? parent, string itemName) : base(parent, itemName)
    {
        var ppTypeAttr = new IntPtr();
        try
        {
            typeInfo.GetTypeAttr(out ppTypeAttr);
            var typeAttr = Marshal.PtrToStructure<TYPEATTR>(ppTypeAttr);
            Guid = typeAttr.guid;
            TypeFlags = typeAttr.wTypeFlags.ToString();
            Kind = typeAttr.typekind;
            Alignment = typeAttr.cbAlignment;
            Size = typeAttr.cbSizeInstance;
            VirtualMemoryTableSize = typeAttr.cbSizeVft;
            NumberOfFunctions = typeAttr.cFuncs;
            NumberOfImplementedInterfaces = typeAttr.cImplTypes;
            NumberOfVariablesAndDatafields = typeAttr.cVars;
            Lcid = typeAttr.lcid;
            ConstructorId = typeAttr.memidConstructor;
            DestructorId = typeAttr.memidDestructor;
            MajorVersionNumber = typeAttr.wMajorVerNum;
            MinorVersionNumber = typeAttr.wMinorVerNum;
            IdlFlag = typeAttr.idldescType.wIDLFlags.ToString();
        }
        finally
        {
            typeInfo.ReleaseTypeAttr(ppTypeAttr);
        }
    }

    public Guid Guid { get; private set; }

    public string? IdlFlag { get; private set; }

    public TYPEKIND Kind { get; private set; }

    public string TypeFlags { get; private set; }

    public short Alignment { get; private set; }

    public int Size { get; private set; }

    public int SizeInstance { get; private set; }

    public short VirtualMemoryTableSize { get; private set; }

    public short NumberOfFunctions { get; private set; }

    public short NumberOfImplementedInterfaces { get; private set; }

    public short NumberOfVariablesAndDatafields { get; private set; }

    public int Lcid { get; private set; }

    public int ConstructorId { get; private set; }

    public int DestructorId { get; private set; }

    public short MajorVersionNumber { get; private set; }

    public short MinorVersionNumber { get; private set; }
}
