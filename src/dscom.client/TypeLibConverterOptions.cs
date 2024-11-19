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

namespace dSPACE.Runtime.InteropServices;

/// <summary>
///  Represents the options used by dscom tlbexport.
/// </summary>
public class TypeLibConverterOptions : TypeLibConverterSettings
{
    /// <summary>
    /// Use a null character, which is an illegal character and therefore impossible
    /// to input via a command line arguments to make it distinct from nulls that
    /// gets passed when the option is specified but no argument is given.
    /// </summary>
    internal const string NotSpecifiedViaCommandLineArgumentsDefault = "\0";

    /// <summary>
    /// Gets or sets a value indicating whether the output is silent.
    /// </summary>
    public bool Silent { get; set; }

    /// <summary>
    /// Gets or sets an array of warnings that should ignored.
    /// </summary>
    /// <returns></returns>
    public string[] Silence { get; set; } = Array.Empty<string>();

    /// <summary>
    /// Gets or sets a value indicating whether the output should be verbose.
    /// </summary>
    public bool Verbose { get; set; }

    /// <summary>
    /// Gets or sets whether to generate missing TLB files for dependent assemblies.  
    /// </summary>
    public bool? CreateMissingDependentTLBs { get; set; } = true;

    /// <summary>
    /// Gets or sets the optional path to embed the TLB into a different file other than the assembly. Defaults to the assembly.
    /// </summary>
    public string? Embed { get; set; }

    /// <summary>
    /// Gets or sets the index to be used for type library resource when embedding 
    /// type library into assembly. Effective only if the <seealso cref="Embed"/> 
    /// is set to <c>true</c>.
    /// </summary>
    public ushort Index { get; set; }

    /// <summary>
    /// Centralized check for using a meaningful value for <see cref="Embed"/>.
    /// </summary>
    /// <returns><c>true</c>, if the tlb should be embedded into the resulting assembly; <c>false</c> otherwise.</returns>
    internal bool ShouldEmbed()
    {
        return Embed is null || !StringComparer.OrdinalIgnoreCase.Equals(Embed, NotSpecifiedViaCommandLineArgumentsDefault);
    }
}
