// Copyright 2022 dSPACE GmbH, Carsten Igel and Contributors
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

using Microsoft.Build.Framework;

namespace dSPACE.Runtime.InteropServices.BuildTasks;

/// <summary>
/// Represents the task to embed a type library file into an assembly.
/// </summary>
public sealed class TlbEmbed : Microsoft.Build.Utilities.Task
{
    /// <summary>
    /// Creates a new instance of the <see cref="TlbEmbed" />
    /// class using the default build context.
    /// </summary>
    public TlbEmbed()
    {
    }

    /// <summary>
    /// Gets or sets the name of the resulting COM type library file (tlb).
    /// This value must be set for this task to work and cannot be left empty.
    /// Existing files will be overridden.
    /// </summary>
    [Required]
    public string SourceTlbFile { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the name of the incoming managed assembly file (dll or exe).
    /// This value must be set for this task to work and cannot be left empty.
    /// The file must exist and be readable.
    /// </summary>
    [Required]
    public string TargetAssemblyFile { get; set; } = string.Empty;

    /// <inheritdoc cref="Task.Execute()" />
    public override bool Execute()
    {
        /* This task remains for historical reasons */
        throw new NotSupportedException();
    }
}
