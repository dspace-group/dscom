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

namespace dSPACE.Runtime.InteropServices.Writer;

internal sealed class ClassInterfaceWriter : DualInterfaceWriter
{
    public ClassInterfaceWriter(Type sourceType, LibraryWriter libraryWriter, WriterContext context) : base(sourceType, libraryWriter, context)
    {
        TypeFlags = TYPEFLAGS.TYPEFLAG_FDUAL | TYPEFLAGS.TYPEFLAG_FDISPATCHABLE | TYPEFLAGS.TYPEFLAG_FOLEAUTOMATION | TYPEFLAGS.TYPEFLAG_FHIDDEN;
    }

    protected override string Name => $"_{base.Name!}";

    public override void Create()
    {
        Context.LogTypeExported($"Class interface '{Name}' exported.");
    }

    protected override Guid GetTypeGuid()
    {
        return MarshalExtension.GetClassInterfaceGuidForType(SourceType);
    }

    /// <summary>
    /// Return 0 as major version. Even if this is questionable, tlbexp behaves like this.
    /// </summary>
    protected override ushort MajorVersion => 0;
}
