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
/// <summary>
/// Provide support for aliasing the elements exported to a type library, allowing one to change the lettercase or constant names as needed.
/// </summary>
public interface INameResolver
{
    /// <summary>
    /// Provide the alias if present, otherwise return <see cref="MemberInfo.Name"/>.
    /// </summary>
    /// <param name="method">The <see cref="MethodInfo"/> of the type.</param>
    /// <param name="name">The name to be used for the exported type.</param>
    /// <returns>Either the alias or the <paramref name="name"/>.</returns>
    string GetMappedName(MethodInfo method, string name);

    /// <summary>
    /// Provide the alias if present, otherwise return <see cref="MemberInfo.Name"/>.
    /// </summary>
    /// <param name="field">The <see cref="FieldInfo"/> of the type.</param>
    /// <param name="name">The name to be used for the exported type.</param>
    /// <returns>Either the alias or the <paramref name="name"/>.</returns>
    string GetMappedName(FieldInfo field, string name);

    /// <summary>
    /// Provide the alias if present, otherwise return <see cref="ParameterInfo.Name"/>.
    /// </summary>
    /// <param name="parameter">The <see cref="ParameterInfo"/> of the type.</param>
    /// <param name="name">The name to be used for the exported type.</param>
    /// <returns>Either the alias or the <paramref name="name"/>.</returns>
    string GetMappedName(ParameterInfo parameter, string name);

    /// <summary>
    /// Provide the alias if present, otherwise return <see cref="MemberInfo.Name"/>.
    /// </summary>
    /// <param name="prop">The <see cref="PropertyInfo"/> of the type.</param>
    /// <param name="name">The name to be used for the exported type.</param>
    /// <returns>Either the alias or the <paramref name="name"/>.</returns>
    string GetMappedName(PropertyInfo prop, string name);

    /// <summary>
    /// Provide the alias if present, otherwise return <see cref="MemberInfo.Name"/>.
    /// </summary>
    /// <param name="type">The <see cref="Type"/> of the type being exported.</param>
    /// <param name="name">The name to be used for the exported type.</param>
    /// <returns>Either the alias or the <paramref name="name"/>.</returns>
    string GetMappedName(Type type, string name);
}
