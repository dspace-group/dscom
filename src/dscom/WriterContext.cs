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

namespace dSPACE.Runtime.InteropServices;

internal sealed class WriterContext
{
    public WriterContext(TypeLibConverterSettings options, ICreateTypeLib2 targetTypeLib, ITypeLibExporterNotifySink? notifySink)
    {
        Options = options;
        TargetTypeLib = targetTypeLib;
        NotifySink = notifySink;
        NameResolver = notifySink is ITypeLibExporterNameProvider provider
            ? provider.GetNameResolver()
            : InteropServices.NameResolver.CreateFromList(Array.Empty<string>());

        if (NotifySink is ITypeLibCacheProvider typeLibCacheProvider)
        {
            if (typeLibCacheProvider.TypeLibCache is TypeInfoResolver typeInfoResolver)
            {
                TypeInfoResolver = typeInfoResolver;
            }
        }
        TypeInfoResolver ??= new TypeInfoResolver(this);
    }

    public ITypeLibExporterNotifySink? NotifySink { get; private set; }

    public ICreateTypeLib2? TargetTypeLib { get; private set; }

    public TypeInfoResolver TypeInfoResolver { get; private set; }

    public TypeLibConverterSettings Options { get; private set; }

    public INameResolver NameResolver { get; private set; }

    public void LogTypeExported(string message)
    {
        NotifySink?.ReportEvent(ExporterEventKind.NOTIF_TYPECONVERTED, 0, message);
    }

    public void LogWarning(string message, int eventCode = 0)
    {
        NotifySink?.ReportEvent(ExporterEventKind.NOTIF_CONVERTWARNING, eventCode, message);
    }
}
