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

namespace dSPACE.Runtime.InteropServices;

internal class AssemblyResolver : IDisposable
{
    private bool _disposedValue;

    internal AssemblyResolver(TypeLibConverterOptions options)
    {
        Options = options;
        AppDomain.CurrentDomain.AssemblyResolve += ResolveEventHandler;
    }

    public TypeLibConverterOptions Options { get; }

    private Assembly? ResolveEventHandler(object? sender, ResolveEventArgs args)
    {
        var name = args.Name;
        var fileNameWithoutExtension = new AssemblyName(name).Name;

        foreach (var path in Options.ASMPath)
        {
            var dlltoLoad = Path.Combine(path, $"{fileNameWithoutExtension}.dll");
            if (File.Exists(dlltoLoad))
            {
                return Assembly.LoadFrom(dlltoLoad);
            }

            var exeToLoad = Path.Combine(path, $"{fileNameWithoutExtension}.exe");
            if (File.Exists(exeToLoad))
            {
                return Assembly.LoadFrom(exeToLoad);
            }
        }

        return null;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve -= ResolveEventHandler;
            }

            _disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
