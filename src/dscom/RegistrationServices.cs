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

using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.InteropServices;

namespace dSPACE.Runtime.InteropServices;

/// <summary>
/// Provides a set of services for registering and unregistering managed assemblies for use from COM.
/// </summary>
[SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Justification = "Compatibility to the mscorelib TypeLibConverter class")]
[SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Compatibility to the mscorelib TypeLibConverter class")]
public class RegistrationServices
{

    /// <summary>Registers the specified type with COM using the specified GUID.</summary>
    /// <param name="type">The <see cref="T:System.Type" /> to be registered for use from COM.</param>
    /// <param name="g">The <see cref="T:System.Guid" /> used to register the specified type.</param>
    /// <exception cref="T:System.ArgumentException">The <paramref name="type" /> parameter is <see langword="null" />.</exception>
    /// <exception cref="T:System.ArgumentNullException">The <paramref name="type" /> parameter cannot be created.</exception>
    public void RegisterTypeForComClients(Type type, ref Guid g)
    {
        var genericClassFactory = typeof(ClassFactory<>);
        Type[] typeArgs = { type };
        var constructedClassFactory = genericClassFactory.MakeGenericType(typeArgs);

        var createdClassFactory = Activator.CreateInstance(constructedClassFactory);

        var hr = Ole32.CoRegisterClassObject(g, createdClassFactory!, (uint)(ComTypes.RegistrationClassContext.InProcessServer | ComTypes.RegistrationClassContext.LocalServer), (uint)ComTypes.RegistrationConnectionType.MultipleUse, out _);
        if (hr < 0)
        {
            Marshal.ThrowExceptionForHR(hr);
        }
    }

    /// <summary>Registers the specified type with COM using the specified execution context and connection type.</summary>
    /// <param name="type">The <see cref="T:System.Type" /> object to register for use from COM.</param>
    /// <param name="classContext">One of the <see cref="T:dSPACE.Runtime.InteropServices.ComTypes.RegistrationClassContext" /> values that indicates the context in which the executable code will be run.</param>
    /// <param name="flags">One of the <see cref="T:dSPACE.Runtime.InteropServices.ComTypes.RegistrationConnectionType" /> values that specifies how connections are made to the class object.</param>
    /// <returns>An integer that represents a cookie value.</returns>
    /// <exception cref="T:System.ArgumentException">The <paramref name="type" /> parameter is <see langword="null" />.</exception>
    /// <exception cref="T:System.ArgumentNullException">The <paramref name="type" /> parameter cannot be created.</exception>
    public int RegisterTypeForComClients(Type type, ComTypes.RegistrationClassContext classContext, ComTypes.RegistrationConnectionType flags)
    {
        var value = type.GetCustomAttributes<GuidAttribute>().FirstOrDefault()?.Value
                    ?? type.Assembly.GetCustomAttributes<GuidAttribute>().FirstOrDefault()?.Value;
        if (value == null)
        {
            // ToDo: We have to decide which Exception should be thrown here 
            throw new InvalidComObjectException($"The given type {type} does not have a valid GUID attribute and cannot be registered.");
        }

        var guid = new Guid(value);

        var genericClassFactory = typeof(ClassFactory<>);
        Type[] typeArgs = { type };
        var constructedClassFactory = genericClassFactory.MakeGenericType(typeArgs);

        var createdClassFactory = Activator.CreateInstance(constructedClassFactory);

        var hr = Ole32.CoRegisterClassObject(guid, createdClassFactory!, (uint)classContext, (uint)flags, out var cookie);
        if (hr < 0)
        {
            Marshal.ThrowExceptionForHR(hr);
        }
        return cookie;
    }

    /// <summary>Unregisters the specified type referenced by the cookie.</summary>
    /// <param name="cookie">The cookie to unregister for use from COM.</param>
    public void UnregisterTypeForComClients(int cookie)
    {
        var hr = Ole32.CoRevokeClassObject(cookie);
        if (hr < 0)
        {
            Marshal.ThrowExceptionForHR(hr);
        }
    }
}
