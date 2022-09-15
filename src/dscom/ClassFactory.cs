// The MIT License (MIT)
// Copyright (c) Microsoft Corporation

// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and 
// associated documentation files (the "Software"), to deal in the Software without restriction, 
// including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, 
// and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, 
// subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial 
// portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT 
// NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. 
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE 
// SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System.Runtime.InteropServices;

namespace dSPACE.Runtime.InteropServices;

[ComVisible(true)]
internal class ClassFactory<T> : ComTypes.IClassFactory where T : new()
{
    public void CreateInstance(
        [MarshalAs(UnmanagedType.Interface)] object instancePointer,
        ref Guid riid,
        out IntPtr outInterfacePointFromClass)
    {
        var interfaceType = GetInterfaceFromClassType(typeof(T), ref riid, instancePointer);

        object aggregatedObject = new T();
        if (instancePointer != null)
        {
            aggregatedObject = CreateAggregatedObject(instancePointer, aggregatedObject);
        }

        outInterfacePointFromClass = GetObjectAsInterface(aggregatedObject, interfaceType);
    }

    public void LockServer([MarshalAs(UnmanagedType.Bool)] bool fLock) { }

    private static Type GetInterfaceFromClassType(Type classType, ref Guid riid, object outer)
    {
        if (riid == new Guid(Guids.IID_IUnknown))
        {
            return typeof(object);
        }

        if (outer != null)
        {
            throw new COMException(string.Empty, ComTypes.HRESULT.CLASS_E_NOAGGREGATION);
        }

        foreach (var i in classType.GetInterfaces())
        {
            if (i.GUID == riid)
            {
                return i;
            }
        }

        throw new InvalidCastException();
    }

    private static IntPtr GetObjectAsInterface(object obj, Type interfaceType)
    {
        if (interfaceType == typeof(object))
        {
            return Marshal.GetIUnknownForObject(obj);
        }

        var interfaceMaybe = Marshal.GetComInterfaceForObject(obj, interfaceType, CustomQueryInterfaceMode.Ignore);
        if (interfaceMaybe == IntPtr.Zero)
        {
            throw new InvalidCastException();
        }

        return interfaceMaybe;
    }

    private static object CreateAggregatedObject(object pUnkOuter, object comObject)
    {
        var outerPtr = Marshal.GetIUnknownForObject(pUnkOuter);

        try
        {
            var innerPtr = Marshal.CreateAggregatedObject(outerPtr, comObject);
            return Marshal.GetObjectForIUnknown(innerPtr);
        }
        finally
        {
            Marshal.Release(outerPtr);
        }
    }
}
