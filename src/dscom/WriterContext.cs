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

using dSPACE.Runtime.InteropServices.ComTypes;

namespace dSPACE.Runtime.InteropServices;

internal sealed class WriterContext
{
    public WriterContext(TypeLibConverterOptions options, ICreateTypeLib2 targetTypeLib, ITypeLibExporterNotifySink notifySink)
    {
        Options = options;
        TargetTypeLib = targetTypeLib;
        NotifySink = notifySink;
        NameResolver = new NameResolver(notifySink is ITypeLibExporterNameProvider provider ?
            provider.GetNames() :
            Array.Empty<string>());
        if (NotifySink is ITypeLibCacheProvider x)
        {
            if (x.TypeLibCache is TypeInfoResolver typeInfoResolver)
            {
                TypeInfoResolver = typeInfoResolver;
            }
        }
        if (TypeInfoResolver == null)
        {
            TypeInfoResolver = new TypeInfoResolver(this);
        }

    }

    public ITypeLibExporterNotifySink? NotifySink { get; private set; }

    public ICreateTypeLib2? TargetTypeLib { get; private set; }

    public TypeInfoResolver TypeInfoResolver { get; private set; }

    public TypeLibConverterOptions Options { get; private set; }

    public NameResolver NameResolver { get; private set; }

    public void LogVerbose(string message)
    {
        if (Options.Verbose)
        {
            Console.WriteLine($"Verbose: {message}");
        }
    }

    public void LogWarning(string message, int eventCode = 0)
    {
        if (NotifySink != null)
        {
            NotifySink.ReportEvent(ExporterEventKind.NOTIF_CONVERTWARNING, eventCode, message);
        }
    }
}
