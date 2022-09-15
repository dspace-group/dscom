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

/// <summary>
/// Provides a set of services for registering and unregistering managed assemblies for use from COM.
/// /// </summary>
public class RegistrationServices
{

    /// <summary>Registers the specified type with COM using the specified GUID.</summary>
    /// <param name="type">The <see cref="T:System.Type" /> to be registered for use from COM.</param>
    /// <param name="g">The <see cref="T:System.Guid" /> used to register the specified type.</param>
    /// <exception cref="T:System.ArgumentException">The <paramref name="type" /> parameter is <see langword="null" />.</exception>
    /// <exception cref="T:System.ArgumentNullException">The <paramref name="type" /> parameter cannot be created.</exception>
    public void RegisterTypeForComClients(Type type, ref Guid g)
    {

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
        return 0;

    }
}


