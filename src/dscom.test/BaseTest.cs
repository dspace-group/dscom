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
    static BaseTest()
    {
        Dir = Path.Combine(Directory.GetCurrentDirectory(), "dynamic");
        if (!Directory.Exists(Dir))
        {
            Directory.CreateDirectory(Dir);
        }
    }

    private static string Dir { get; }
    protected static Regex ValidChars { get; } = new("[^a-zA-Z0-9_]");

    internal DynamicAssemblyBuilder CreateAssembly([CallerMemberName] string callerName = "",
                                                         [CallerFilePath] string filepath = "")
    {
        return CreateAssembly(CreateAssemblyName(callerName, string.Empty, 0, 0), callerName, filepath);
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Compatibility")]
    internal DynamicAssemblyBuilder CreateAssembly(AssemblyName assemblyName,
                                                         CustomAttributeBuilder[] customAttributeBuilders,
                                                         [CallerMemberName] string callerName = "",
                                                         [CallerFilePath] string filepath = "")
    {


#if NETFRAMEWORK
        var assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndSave, Dir, true, customAttributeBuilders);
#else
        var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run, customAttributeBuilders);
#endif

        var name = GenerateAssemblyName(assemblyName, filepath, callerName);

        return new DynamicAssemblyBuilder(name, assemblyBuilder);
    }

    internal DynamicAssemblyBuilder CreateAssembly(AssemblyName name,
                                                         [CallerMemberName] string callerName = "",
                                                         [CallerFilePath] string filepath = "")
    {
        return CreateAssembly(name, Array.Empty<CustomAttributeBuilder>(), callerName, filepath);
    }

    protected static AssemblyName CreateAssemblyName([CallerMemberName] string assemblyName = "", string assemblyNameSuffix = "", int major = 0, int minor = 0)
    {
        assemblyName = ValidChars.Replace(assemblyName, string.Empty);
        assemblyNameSuffix = ValidChars.Replace(assemblyNameSuffix, string.Empty);
        return new AssemblyName($"Dynamic{assemblyName.GetHashCode()}{assemblyNameSuffix}") { Version = new Version(major, minor) };
    }

    private static string GenerateAssemblyName(AssemblyName assemblyName, string filePath, string testMethodName)
    {
        var name = $"{assemblyName.Name}_{assemblyName.Version?.Major}_{assemblyName.Version?.Minor}_{ValidChars.Replace(Path.GetFileNameWithoutExtension(filePath), "")}_{ValidChars.Replace(testMethodName, "")}";

        return name.Substring(0, Math.Min(150, name.Length));
    }
}
