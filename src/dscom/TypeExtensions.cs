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

using System.ComponentModel;
using System.Reflection;

namespace dSPACE.Runtime.InteropServices;

/// <summary>
/// Extension methods for <see cref="Type"/>.
/// </summary>
[Browsable(false)]
internal static class TypeExtensions
{
    /// <summary>
    /// Return the members defined in a type, or an empty array if the
    /// members cannot be enumerated.
    /// </summary>
    /// <param name="type">The type to enumerate</param>
    /// <returns>The members which were able to be enumerated</returns>
    internal static IEnumerable<MemberInfo> GetLoadableMembers(this Type type)
    {
        try
        {
            return type.GetMembers();
        }
        catch
        {
            return Array.Empty<MemberInfo>();
        }
    }

    /// <summary>
    /// Return the methods defined in a type, or an empty array if the
    /// methods cannot be enumerated.
    /// </summary>
    /// <param name="type">The type to enumerate</param>
    /// <returns>The Methods which were able to be enumerated</returns>
    internal static IEnumerable<MethodInfo> GetLoadableMethods(this Type type)
    {
        try
        {
            return type.GetMethods();
        }
        catch
        {
            return Array.Empty<MethodInfo>();
        }
    }
}
