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
///  Represents the settings used by <see cref="TypeLibEmbedder"/>.
/// </summary>
public class TypeLibEmbedderSettings
{
    /// <summary>
    /// Gets or sets source TLB file path.
    /// </summary>
    public string SourceTypeLibrary { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets target assembly path.
    /// </summary>
    public string TargetAssembly { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the resource index. Defaults to 1.
    /// </summary>
    public ushort Index { get; set; } = 1;
}
