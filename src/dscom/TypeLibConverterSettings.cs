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
///  Represents the settings used by <see cref="TypeLibConverter"/>.
/// </summary>
public class TypeLibConverterSettings
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
    public string[] TLBRefpath { get; set; } = Array.Empty<string>();

    /// <summary>
    /// Gets or sets a path to a directory with assemblies.
    /// </summary>
    public string[] ASMPath { get; set; } = Array.Empty<string>();


    /// <summary>
    /// Gets or sets the type library GUID.
    /// </summary>
    public Guid OverrideTlbId { get; set; } = Guid.Empty;
}
