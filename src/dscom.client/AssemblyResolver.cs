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
using System.Runtime.Loader;

namespace dSPACE.Runtime.InteropServices;

/// <summary>
/// Uses the "ASMPath" option to handle the AppDomain.CurrentDomain.AssemblyResolve event and try to load the specified assemblies.
/// </summary>
internal sealed class AssemblyResolver : AssemblyLoadContext, IDisposable
{
    private bool _disposedValue;

    internal AssemblyResolver(TypeLibConverterOptions options) : base("dscom", isCollectible: false)
    {
        Options = options;
        Resolving += Context_Resolving;
    }

    private Assembly? Context_Resolving(AssemblyLoadContext context, AssemblyName name)
    {
        var dir = Path.GetDirectoryName(Options.Assembly);

        var asmPaths = Options.ASMPath;
        if (Directory.Exists(dir))
        {
            asmPaths = asmPaths.Prepend(dir).ToArray();
        }

        foreach (var path in asmPaths)
        {
            var dllToLoad = Path.Combine(path, $"{name.Name}.dll");
            if (File.Exists(dllToLoad))
            {
                return LoadFromAssemblyPath(dllToLoad);
            }

            var exeToLoad = Path.Combine(path, $"{name.Name}.exe");
            if (File.Exists(exeToLoad))
            {
                return LoadFromAssemblyPath(exeToLoad);
            }
        }

        return null;
    }

    public Assembly LoadAssembly(string path)
    {
        return LoadFromAssemblyPath(path);
    }

    public TypeLibConverterOptions Options { get; }

    private void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                Resolving -= Context_Resolving;
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
