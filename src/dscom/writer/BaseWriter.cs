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
using dSPACE.Runtime.InteropServices.ComTypes;

namespace dSPACE.Runtime.InteropServices.Writer;

internal abstract class BaseWriter : IDisposable
{
    public BaseWriter(WriterContext context)
    {
        Context = context;
    }

    ~BaseWriter()
    {
        Dispose(disposing: false);
    }

    protected bool IsDisposed { get; private set; }

    private List<IntPtr> PtrToRelease { get; } = new();

    public WriterContext Context { get; }

    public abstract void Create();

    public IntPtr StructuresToPtr<T>(IEnumerable<T> instance) where T : struct
    {
        var count = instance.Count();
        var elementSize = Marshal.SizeOf(typeof(T));
        var ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(T)) * count);
        PtrToRelease.Add(ptr);

        var index = 0;
        foreach (var element in instance)
        {
            Marshal.StructureToPtr(element, ptr + (index * elementSize), false);
            index++;
        }

        return ptr;
    }

    public IntPtr StructureToPtr<T>(T instance) where T : struct
    {
        return StructuresToPtr(new T[] { instance });
    }

    public IntPtr ObjectToVariantPtr(object? instance)
    {
        IntPtr ptrVariant;
        ptrVariant = Marshal.AllocHGlobal(Marshal.SizeOf<VARIANT>());
        PtrToRelease.Add(ptrVariant);
        Marshal.GetNativeVariantForObject(instance, ptrVariant);

        return ptrVariant;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!IsDisposed)
        {
            PtrToRelease.ForEach(ptr => Marshal.FreeHGlobal(ptr));
            PtrToRelease.Clear();

            IsDisposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
