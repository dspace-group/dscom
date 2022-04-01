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
using dSPACE.Runtime.InteropServices.ComTypes;

namespace dSPACE.Runtime.InteropServices;

/// <summary>
/// Provides a callback mechanism for the assembly converter to inform the caller of the status of the conversion, and involve the caller in the conversion process itself.
/// </summary>
public class TypeLibExporterNotifySink : ITypeLibExporterNotifySink, ITypeLibExporterNameProvider, ITypeLibCacheProvider
{
    public TypeLibExporterNotifySink(TypeLibConverterOptions options)
    {
        Options = options;

        CollectNames();
    }

    private List<string> Names { get; } = new();

    public TypeLibConverterOptions Options { get; }

    public ITypeLibCache? TypeLibCache { get; set; }

    /// <summary>Notifies the caller that an event occurred during the conversion of an assembly.</summary>
    /// <param name="eventKind">An <see cref="T:System.Runtime.InteropServices.ExporterEventKind" /> value indicating the type of event.</param>
    /// <param name="eventCode">Indicates extra information about the event.</param>
    /// <param name="eventMsg">A message generated by the event.</param>
    public void ReportEvent(ExporterEventKind eventKind, int eventCode, string eventMsg)
    {
        switch (eventKind)
        {
            case ExporterEventKind.NOTIF_TYPECONVERTED:
                if (Options.Verbose && !Options.Silent)
                {
                    Console.WriteLine(eventMsg);
                }
                break;
            case ExporterEventKind.NOTIF_CONVERTWARNING:
                if (Options.Silent || Options.Silence.Contains($"{eventCode}") || Options.Silence.Contains($"TX{eventCode:X8}"))
                {
                    return;
                }

                Console.Error.WriteLine($"dscom : warning TX{eventCode:X8} : {eventMsg}");
                break;
            default:
                if (!Options.Silent)
                {
                    Console.WriteLine(eventMsg);
                }
                break;
        }
    }

    /// <summary>Asks the user to resolve a reference to another assembly.</summary>
    /// <param name="assembly">The assembly to resolve.</param>
    /// <returns>The type library for <paramref name="assembly" />.</returns>
    public virtual object ResolveRef(Assembly assembly)
    {
        // Try load from cache
        if (TypeLibCache != null)
        {
            var identifier = assembly.GetLibIdentifier();
            var typelib = TypeLibCache.GetTypeLibFromIdentifier(identifier);
            if (typelib != null)
            {
                return typelib;
            }
        }

        var outputPath = Options.Out;
        outputPath = Path.GetDirectoryName(outputPath);

        if (outputPath == null)
        {
            return null!;
        }

        var name = assembly.GetName().Name;
        outputPath = Path.Combine(outputPath, $"{name!}.tlb");

        if (!Options.CreateMissingDependentTLBs ?? false)
        {
            var message = $"The referenced library {name} does not have a type library and auto generation of dependent type libs is disabled";
            ReportEvent(ExporterEventKind.NOTIF_CONVERTWARNING, 0, message);
            return null!;
        }

        var typeLibConverter = new TypeLibConverter();
        if (!assembly.GetCustomAttributes<AssemblyMetadataAttribute>().Any(z => z.Key.Equals(".NETFrameworkAssembly", StringComparison.Ordinal)))
        {
            var typeLib = typeLibConverter.ConvertAssemblyToTypeLib(assembly, outputPath, this);
            if (typeLib is ICreateTypeLib createTypeLib2)
            {
                createTypeLib2.SaveAllChanges().ThrowIfFailed($"Failed to save type library {outputPath}.");
                return typeLib;
            }
        }
        return null!;
    }

    /// <summary>
    /// Returns a list of names that can be used to specify the casing of type library elements.
    /// </summary>
    public virtual string[] GetNames()
    {
        return Names.ToArray();
    }

    /// <summary>
    /// Read all names from all files specified in the "Names" option.
    /// </summary>
    private void CollectNames()
    {
        foreach (var fileName in Options.Names)
        {
            if (!File.Exists(fileName))
            {
                throw new ArgumentException($"Given names file {fileName} does not exist.");
            }

            File.ReadLines(fileName).ToList().ForEach(n => Names.Add(n));
        }
    }
}
