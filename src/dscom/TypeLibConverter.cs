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

using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using dSPACE.Runtime.InteropServices.ComTypes;
using dSPACE.Runtime.InteropServices.Exporter;
using dSPACE.Runtime.InteropServices.Writer;

namespace dSPACE.Runtime.InteropServices;

/// <summary>
/// Provides a set of services that convert a managed assembly to a COM type library ot to convert a type library to a text file.
/// </summary>
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
        var options = new TypeLibConverterSettings
        {
            Out = tlbFilePath,
        };

        return ConvertAssemblyToTypeLib(assembly, options, notifySink);
    }

    /// <summary>Converts an assembly to a COM type library.</summary>
    /// <param name="assembly">The assembly to convert.</param>
    /// <param name="settings">The <see cref="T:dSPACE.Runtime.InteropServices.TypeLibConverterSettings" /> to configure the converter.</param>
    /// <param name="notifySink">The <see cref="T:dSPACE.Runtime.InteropServices.ITypeLibExporterNotifySink" /> interface implemented by the caller.</param>
    /// <returns>An object that implements the <see langword="ITypeLib" /> interface.</returns>
    public object? ConvertAssemblyToTypeLib(Assembly assembly, TypeLibConverterSettings settings, ITypeLibExporterNotifySink? notifySink)
    {
        CheckPlatform();

        OleAut32.CreateTypeLib2(Environment.Is64BitProcess ? SYSKIND.SYS_WIN64 : SYSKIND.SYS_WIN32, settings.Out!, out var typelib).ThrowIfFailed("Failed to create type library.");

        using var writer = new LibraryWriter(assembly, new WriterContext(settings, typelib, notifySink));
        writer.Create();

        return typelib;
    }

    /// <summary>
    /// Export a type library to a text file.
    /// Creates a new file, writes the specified string to the file, and then closes the file. If the target file already exists, it is overwritten.
    /// </summary>
    /// <param name="settings">The <see cref="TypeLibTextConverterSettings"/> object.</param>
    public void ConvertTypeLibToText(TypeLibTextConverterSettings settings)
    {
        CheckPlatform();

        LoadTypeLibrariesFromOptions(settings);

        File.WriteAllText(settings.Out, GetYamlTextFromTlb(settings.TypeLibrary, settings.FilterRegex));
    }

    [ExcludeFromCodeCoverage] // UnitTest with dependent type libraries is not supported
    private static void LoadTypeLibrariesFromOptions(TypeLibTextConverterSettings options)
    {
        foreach (var tlbFile in options.TLBReference)
        {
            if (!File.Exists(tlbFile))
            {
                throw new ArgumentException($"File {tlbFile} not exist.");
            }

            OleAut32.LoadTypeLibEx(tlbFile, REGKIND.NONE, out _).ThrowIfFailed();
        }

        foreach (var tlbDirectory in options.TLBRefpath)
        {
            if (!Directory.Exists(tlbDirectory))
            {
                throw new ArgumentException($"Directory {tlbDirectory} not exist.");
            }

            foreach (var tlbFile in Directory.GetFiles(tlbDirectory, "*.tlb"))
            {
                // Try to load any tlb found in the folder and ignore any fails.
                OleAut32.LoadTypeLibEx(tlbFile, REGKIND.NONE, out _);
            }
        }
    }

    // UnitTest on different platforms is not supported
    [ExcludeFromCodeCoverage]
    private static void CheckPlatform()
    {
        // Only Windows is supported, because we need COM
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            throw new PlatformNotSupportedException();
        }
    }

    /// <summary>
    /// Load a type library and convert the library to YAML text.
    /// </summary>
    /// <param name="inputTlb">The path to a type library</param>
    /// <param name="filters">An array of regular expressions that can be used to filter the output.</param>
    /// <returns>The YAML output.</returns>
    private static string GetYamlTextFromTlb(string inputTlb, string[]? filters)
    {
        var typeLibInfo = new TypelLibInfo(inputTlb);

        StringBuilder builder = new();
        var regExs = filters != null ? filters.ToList().Select(f => new Regex(f)) : Array.Empty<Regex>();
        CreateYaml(builder, typeLibInfo, regExs);
        return builder.ToString();
    }

    /// <summary>
    /// Create a YAML string from <see cref="BaseInfo"/>,
    /// </summary>
    /// <param name="stringBuilder">The <see cref="StringBuilder"/> to use.</param>
    /// <param name="data">The <see cref="BaseInfo"/> to use.</param>
    /// <param name="filters">An array of regex filter.</param>
    /// <param name="indentLevel">The indentation level</param>
    /// <param name="collectionIndex">The collection index, if the item is collection item; otherwise -1</param>
    private static void CreateYaml(StringBuilder stringBuilder, BaseInfo data, IEnumerable<Regex>? filters, int indentLevel = 0, int collectionIndex = -1)
    {
        var type = data.GetType();

        var propertyIndex = 0;
        foreach (var property in type.GetProperties())
        {
            var name = property.Name;
            name = char.ToLowerInvariant(name[0]) + name.Substring(1);
            var value = property.GetValue(data);

            var isFirstPropertyInFirstCollectionItem = propertyIndex == 0 && collectionIndex != -1;
            if (property.GetCustomAttributes(true).Any(c => c is IgnoreAttribute) || value == null)
            {
                continue;
            }

            if (value is IEnumerable elements and not string)
            {
                if (!elements.OfType<object>().Any())
                {
                    continue;
                }
            }

            // Use all Regex strings to filter the output..
            if (filters != null)
            {
                var isMatch = false;
                foreach (var filter in filters)
                {
                    var path = $"{data.GetPath()}.{name}={value}";

                    if (filter.IsMatch(path))
                    {
                        isMatch = true;
                        continue;
                    }
                }

                if (isMatch)
                {
                    continue;
                }
            }

            for (var i = 0; i < (indentLevel - (isFirstPropertyInFirstCollectionItem ? 1 : 0)); i++)
            {
                stringBuilder.Append("  ");
            }

            if (isFirstPropertyInFirstCollectionItem)
            {
                stringBuilder.Append("- ");
            }

            if (value is BaseInfo baseInfo)
            {

                stringBuilder.AppendLine($"{name}:");
                CreateYaml(stringBuilder, baseInfo, filters, indentLevel + 1);
            }
            else if (value is IEnumerable items and not string)
            {
                stringBuilder.AppendLine($"{name}:");
                var index = 0;
                foreach (var item in items)
                {
                    if (item is BaseInfo childBaseItem)
                    {
                        CreateYaml(stringBuilder, childBaseItem, filters, indentLevel + 1, index);
                    }

                    index++;
                }
            }
            else if (value != null)
            {
                stringBuilder.AppendLine($"{name}: {value}");
            }

            propertyIndex++;
        }
    }
}
