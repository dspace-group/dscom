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

internal class TypeLibAttributesInfo : BaseInfo
{
    public TypeLibAttributesInfo(ITypeLib2 typeLib, BaseInfo? parent, string itemName) : base(parent, itemName)
    {
        var ppTLIBAttr = new IntPtr();
        try
        {
            typeLib.GetLibAttr(out ppTLIBAttr);
            var typeLibAttr = Marshal.PtrToStructure<TYPELIBATTR>(ppTLIBAttr);
            Guid = typeLibAttr.guid;
            Lcid = typeLibAttr.lcid;
            SysKind = typeLibAttr.syskind;
            LibFlags = typeLibAttr.wLibFlags;
            MajorVerNum = typeLibAttr.wMajorVerNum;
            MinorVerNum = typeLibAttr.wMinorVerNum;
        }
        finally
        {
            typeLib.ReleaseTLibAttr(ppTLIBAttr);
        }
    }

    public Guid Guid { get; set; }

    public int Lcid { get; set; }

    public SYSKIND SysKind { get; set; }

    public LIBFLAGS LibFlags { get; set; }

    public short MajorVerNum { get; set; }

    public short MinorVerNum { get; set; }
}
