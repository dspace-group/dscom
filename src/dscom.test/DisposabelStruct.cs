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

public class DisposableStruct<T> : IDisposable where T : struct
{
    private bool _disposedValue;

    public DisposableStruct(T value, Action disposeFunc)
    {
        DisposeDelegate = disposeFunc;

        Value = value;
    }

    public T Value { get; set; }

    private Action DisposeDelegate { get; }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            DisposeDelegate?.Invoke();
            _disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
