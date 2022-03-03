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
using System.Text;
using System.Text.RegularExpressions;
using dSPACE.Runtime.InteropServices.ComTypes;
using dSPACE.Runtime.InteropServices.Exporter;

namespace dSPACE.Runtime.InteropServices;

/// <summary>
/// Export a type library to YAML file
/// </summary>
public static class TypeLibExporter
{
    /// <summary>
    /// Export a type library to a text fileas yaml.
    /// Creates a new file, writes the specified string to the file, and then closes the file. If the target file already exists, it is overwritten.
    /// </summary>
    /// <param name="options">The options.</param>
    public static void ExportToYaml(TypeLibExporterOptions options)
    {
        foreach (var tlbFile in options.TLBReference)
        {
            if (!File.Exists(tlbFile))
            {
                throw new ArgumentException($"File {tlbFile} not exist.");
            }

            OleAut32.LoadTypeLibEx(tlbFile, REGKIND.NONE, out _).ThrowIfFailed();
        }

        foreach (var tlbdirectory in options.TLBRefpath)
        {
            if (!Directory.Exists(tlbdirectory))
            {
                throw new ArgumentException($"Directory {tlbdirectory} not exist.");
            }

            foreach (var tlbFile in Directory.GetFiles(tlbdirectory, "*.tlb"))
            {
                // Try to load any tlb found in the folder and ignore any fails.
                OleAut32.LoadTypeLibEx(tlbFile, REGKIND.NONE, out _);
            }
        }

        File.WriteAllText(options.Out, GetYamlTextFromTlb(options.TypeLibrary, options.FilterRegex));
    }

    private static string GetYamlTextFromTlb(string inputTlb, string[]? filters)
    {
        var typeLibInfo = new TypelLibInfo(inputTlb);

        StringBuilder builder = new();
        var regExs = filters != null ? filters.ToList().Select(f => new Regex(f)) : Array.Empty<Regex>();
        CreateYaml(builder, typeLibInfo, regExs);
        return builder.ToString();
    }

    private static void CreateYaml(StringBuilder builder, BaseInfo data, IEnumerable<Regex>? filters, int indentLevel = 0, int collectionIndex = -1)
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

            if (filters != null)
            {
                var isMatch = false;
                foreach (var filter in filters)
                {
                    var path = $"{data.GetPath(false)}.{name}={value}";

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
                builder.Append("  ");
            }

            if (isFirstPropertyInFirstCollectionItem)
            {
                builder.Append("- ");
            }

            if (value is BaseInfo baseInfo)
            {

                builder.AppendLine($"{name}:");
                CreateYaml(builder, baseInfo, filters, indentLevel + 1);
            }
            else if (value is IEnumerable items and not string)
            {
                builder.AppendLine($"{name}:");
                var index = 0;
                foreach (var item in items)
                {
                    if (item is BaseInfo childBaseItem)
                    {
                        CreateYaml(builder, childBaseItem, filters, indentLevel + 1, index);
                    }

                    index++;
                }
            }
            else if (value != null)
            {
                builder.AppendLine($"{name}: {value}");
            }

            propertyIndex++;
        }
    }
}
