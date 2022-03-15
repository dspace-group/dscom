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
/// Extension methods for <see cref="Assembly"/>.
/// </summary>
[Browsable(false)]
public static class AssemblyExtensions
{
    /// <summary>
    /// Returns a assembly identifier.
    /// </summary>
    /// <param name="assembly">An assembly that is used to create an identifer.</param>
    /// <param name="overrideGuid">A guid that should be used</param>
    public static TypeLibIdentifier GetLibIdentifier(this Assembly assembly, Guid overrideGuid)
    {
        var version = assembly.GetTLBVersionForAssembly();

        // From Major, Minor, Language, Guid
        return new TypeLibIdentifier()
        {
            Name = assembly.GetName().Name ?? string.Empty,
            MajorVersion = (ushort)version.Major,
            MinorVersion = (ushort)version.Minor,
            LibID = overrideGuid == Guid.Empty ? assembly.GetTLBGuidForAssembly() : overrideGuid,
            LanguageIdentifier = assembly.GetTLBLanguageIdentifierForAssembly()
        };
    }

    /// <summary>
    /// Returns a assembly identifier.
    /// </summary>
    /// <param name="assembly">An assembly that is used to create an identifer.</param>
    public static TypeLibIdentifier GetLibIdentifier(this Assembly assembly)
    {
        return assembly.GetLibIdentifier(Guid.Empty);
    }
}
