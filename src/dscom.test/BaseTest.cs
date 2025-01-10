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

using System.Diagnostics.CodeAnalysis;

namespace dSPACE.Runtime.InteropServices.Tests;

public class BaseTest
{
    protected static Regex ValidChars { get; } = new("[^a-zA-Z0-9_]");

    internal DynamicAssemblyBuilder CreateAssembly([CallerMemberName] string callerName = "")
    {
        return CreateAssembly(CreateAssemblyName(callerName, string.Empty, 0, 0));
    }

    internal DynamicAssemblyBuilder CreateAssembly(AssemblyName name)
    {
        return CreateAssembly(name, Array.Empty<CustomAttributeBuilder>());
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Compatibility")]
    internal DynamicAssemblyBuilder CreateAssembly(AssemblyName assemblyName,
                                                   CustomAttributeBuilder[] customAttributeBuilders)
    {
#if NETFRAMEWORK
        var assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndSave, customAttributeBuilders);
#else
        var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run, customAttributeBuilders);
#endif

        return new DynamicAssemblyBuilder(assemblyName.Name!, assemblyBuilder);
    }

    protected static AssemblyName CreateAssemblyName([CallerMemberName] string assemblyName = "", string assemblyNameSuffix = "", int major = 0, int minor = 0)
    {
        assemblyName = ValidChars.Replace(assemblyName, string.Empty);
        assemblyNameSuffix = ValidChars.Replace(assemblyNameSuffix, string.Empty);
        return new AssemblyName($"Dynamic{assemblyName.GetHashCode()}{assemblyNameSuffix}") { Version = new Version(major, minor) };
    }
}
