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

namespace dSPACE.Runtime.InteropServices.Tests;

internal sealed class TypeLibExporterNotifySink : ITypeLibExporterNotifySink, ITypeLibExporterNameProvider
{
    private readonly Assembly? _assembly;

    private readonly IReadOnlyCollection<DynamicAssemblyBuilderResult> _dependencies;

    public TypeLibExporterNotifySink(Assembly? assembly, IReadOnlyCollection<DynamicAssemblyBuilderResult> dependencies)
    {
        _assembly = assembly;
        _dependencies = dependencies;
    }

    public List<ReportedEvent> ReportedEvents { get; } = new List<ReportedEvent>();

    public void ReportEvent(ExporterEventKind eventKind, int eventCode, string eventMsg)
    {
        ReportedEvents.Add(new ReportedEvent() { EventKind = eventKind, EventCode = eventCode, EventMsg = eventMsg });
    }

    public object? ResolveRef(Assembly assembly)
    {
        // resolve the dynamic assemblies, if necessary, because they could
        // not be find in the filesystem, see TypeInfoResolver.ResolveTypeInfo(Type, Guid)
        return _dependencies
            .FirstOrDefault(result => result.Assembly.FullName == assembly.FullName)?
            .TypeLib;
    }

    public INameResolver GetNameResolver()
    {
        return _assembly == null
            ? NameResolver.CreateFromList(new string[] { "TOUPPERCASE", "tolowercase" })
            : NameResolver.Create(_assembly);
    }
}

public struct ReportedEvent
{
    public ExporterEventKind EventKind { get; set; }

    public int EventCode { get; set; }

    public string EventMsg { get; set; }
}
