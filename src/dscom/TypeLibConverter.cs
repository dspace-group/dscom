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
using System.Reflection;
using System.Runtime.InteropServices;
using dSPACE.Runtime.InteropServices.ComTypes;
using dSPACE.Runtime.InteropServices.Writer;

namespace dSPACE.Runtime.InteropServices;

[SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Justification = "Compatibility to the mscorelib TypeLibConverter class")]
[SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Compatibility to the mscorelib TypeLibConverter class")]
public class TypeLibConverter
{
    /// <summary>Converts an assembly to a COM type library.</summary>
    /// <param name="assembly">The assembly to convert.</param>
    /// <param name="tlbFilePath">The file pathe of the resulting type library.</param>
    /// <param name="notifySink">The <see cref="T:dSPACE.Runtime.InteropServices.ITypeLibExporterNotifySink" /> interface implemented by the caller.</param>
    /// <returns>An object that implements the <see langword="ITypeLib" /> interface.</returns>
    public object? ConvertAssemblyToTypeLib(Assembly assembly, string tlbFilePath, ITypeLibExporterNotifySink? notifySink)
    {
        var options = new TypeLibConverterSettings() { Out = tlbFilePath };
        return ConvertAssemblyToTypeLib(assembly, options, notifySink);
    }

    /// <summary>Converts an assembly to a COM type library.</summary>
    /// <param name="assembly">The assembly to convert.</param>
    /// <param name="options">The <see cref="T:dSPACE.Runtime.InteropServices.TypeLibConverterSettings" /> to configure the converter.</param>
    /// <param name="notifySink">The <see cref="T:dSPACE.Runtime.InteropServices.ITypeLibExporterNotifySink" /> interface implemented by the caller.</param>
    /// <returns>An object that implements the <see langword="ITypeLib" /> interface.</returns>
    public object? ConvertAssemblyToTypeLib(Assembly assembly, TypeLibConverterSettings options, ITypeLibExporterNotifySink? notifySink)
    {
        CheckPlatform();

        OleAut32.CreateTypeLib2(SYSKIND.SYS_WIN64, options.Out!, out var typelib).ThrowIfFailed("Failed to create type library.");

        using var writer = new LibraryWriter(assembly, new WriterContext(options, typelib, notifySink));
        writer.Create();

        return typelib;
    }

    [ExcludeFromCodeCoverage] // UnitTest on different platforms is not supported
    private static void CheckPlatform()
    {
        // Only windows is supported
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            throw new PlatformNotSupportedException();
        }
    }
}
