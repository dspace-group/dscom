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

using dSPACE.Runtime.InteropServices.Attributes;
using System.Reflection;

namespace dSPACE.Runtime.InteropServices;

/// <summary>
/// Static factory that can return a <see cref="INameResolver"/>, selecting appropriate name resolution strategy based on the parameters provided.
/// </summary>
public static class NameResolver
{
    /// <summary>
    /// Returns a name resolver based on names saved in text file(s) with one name per line, matching all elements.
    /// This is useful for where you want to simply modify lettercase in a global scope without regards to the type of
    /// elements being exported.
    /// </summary>
    /// <param name="fileNames">An array of full paths to text file(s) containing the names to adjust the lettercase.</param>
    /// <returns>A name resolver that will modify the lettercasing of the elements whenever its name is found within the <paramref name="fileNames"/>.</returns>
    public static INameResolver Create(IEnumerable<string> fileNames)
    {
        var names = new List<string>();
        foreach (var fileName in fileNames)
        {
            if (!File.Exists(fileName))
            {
                throw new ArgumentException($"Given names file {fileName} does not exist.");
            }

            File.ReadLines(fileName).ToList().ForEach(n => names.Add(n));
        }
        return new SimpleNameResolver(names);
    }

    /// <summary>
    /// Returns a name resolver based on names found within <paramref name="names"/>.
    /// This is useful for where you want to simply modify lettercase in a global scope without regards to the type of
    /// elements being exported.
    /// </summary>
    /// <param name="names">An array of name containing the names to adjust the lettercase.</param>
    /// <returns>A name resolver that will modify the lettercasing of the elements whenever its name is found within the <paramref name="names"/> list.</returns>
    public static INameResolver CreateFromList(IEnumerable<string> names)
    {
        return new SimpleNameResolver(names);
    }

    /// <summary>
    /// Returns a name resolver based on <see cref="ComAliasAttribute"/> decorated on the types. The name resolver will
    /// examine the assembly and find all instances of <see cref="ComAliasAttribute"/> and then use its alias during
    /// exporting the elements in the type library.
    /// </summary>
    /// <param name="assembly">Source assembly containing the elements to be exported into a type library.</param>
    /// <returns>A name resolver that will alilas the element name whenever its type is found decorated with the <see cref="ComAliasAttribute"/>.</returns>
    public static INameResolver Create(Assembly assembly)
    {
        return new ComAliasNameResolver(assembly);
    }
}
