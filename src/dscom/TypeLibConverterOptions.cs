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
///  Represents the options for a type library converter.
/// </summary>
public class TypeLibConverterOptions
{
    /// <summary>
    /// Gets or sets output TLB file name.
    /// </summary>
    public string Out { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the assembly.
    /// </summary>
    public string Assembly { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a type libaray reference.
    /// </summary>
    public string[] TLBReference { get; set; } = Array.Empty<string>();

    /// <summary>
    /// Gets or sets a path to directory of type libraries.
    /// </summary>
    /// <typeparam name="string"></typeparam>
    /// <returns></returns>
    public string[] TLBRefpath { get; set; } = Array.Empty<string>();

    /// <summary>
    /// Gets or sets a path to a directory with assemblies.
    /// </summary>
    public string[] ASMPath { get; set; } = Array.Empty<string>();

    /// <summary>
    /// Gets or sets a value indicating whether the output is silent.
    /// </summary>
    public bool Silent { get; set; }

    /// <summary>
    /// Gets or sets an array if names that should be users.
    /// </summary>
    public string[] Names { get; set; } = Array.Empty<string>();

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
    /// Gets or sets the type library GUID.
    /// </summary>
    public Guid OverrideTlbId { get; set; } = Guid.Empty;

    /// <summary>
    /// Returns a string that reprence this object.
    /// </summary>
    /// <returns>A string.</returns>
    public override string ToString()
    {
        return
$@"
AssemblyName:  {Assembly}
Out:           {Out}
TLBReference:  {string.Join(", ", TLBReference ?? Array.Empty<string>())}  
TLBRefpath:    {string.Join(", ", TLBRefpath ?? Array.Empty<string>())}
ASMPath:       {string.Join(", ", ASMPath ?? Array.Empty<string>())}
Silent:        {Silent}
Silence:       {string.Join(", ", Silence ?? Array.Empty<string>())}
Verbose:       {Verbose}
Names:         {string.Join(", ", Names ?? Array.Empty<string>())}
OverrideTlbId: {OverrideTlbId}
";
    }
}
