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
/// Extension methods for <see cref="MethodBase"/>.
/// </summary>
[Browsable(false)]
internal static class MethodBaseExtensions
{
    /// <summary>
    /// Return the parameters defined in a method, or an empty array if the
    /// parameter types are unloadable
    /// </summary>
    /// <param name="method">The method to enumerate</param>
    /// <returns>The loadable parameters</returns>
    internal static IEnumerable<ParameterInfo> GetLoadableParameters(this MethodInfo method)
    {
        try
        {
            return method.GetParameters();
        }
        catch
        {
            return Array.Empty<ParameterInfo>();
        }
    }
}
