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
/// Represents the task to export a type library file from an assembly.
/// </summary>
public sealed class TlbExport : Microsoft.Build.Utilities.Task
{
    /// <summary>
    /// Creates a new instance of the <see cref="TlbExport" />
    /// class using the default build context.
    /// </summary>
    public TlbExport()
    {
    }

    /// <summary>
    /// Gets or sets the COM Type Library unique id. If this value
    /// will be equal to <see cref="Guid.Empty" />, the id of the
    /// resulting COM type library will be not be changed.
    /// </summary>
    public string TlbOverriddenId { get; set; } = Guid.Empty.ToString();

    /// <summary>
    /// Gets or sets the name of the resulting COM type library file (tlb).
    /// This value must be set for this task to work and cannot be left empty.
    /// Existing files will be overridden.
    /// </summary>
    [Required]
    public string TargetFile { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the name of the incoming managed assembly file (dll or exe).
    /// This value must be set for this task to work and cannot be left empty.
    /// The file must exist and be readable.
    /// </summary>
    [Required]
    public string SourceAssemblyFile { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the overridden name of the library, which will be
    /// set in the TLB using the IDL instructions.
    /// If left empty, the name of the assembly will be used.
    /// </summary>
    public string TlbOverriddenName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the additional type library files required for conversion.
    /// </summary>
    public ITaskItem[] TypeLibraryReferences { get; set; } = Array.Empty<ITaskItem>();

    /// <summary>
    /// Gets or sets the additional type library search paths required for conversion.
    /// </summary>
    public ITaskItem[] TypeLibraryReferencePaths { get; set; } = Array.Empty<ITaskItem>();

    /// <summary>
    /// Gets or sets the additional assemblies that might be required for conversion.
    /// </summary>
    public ITaskItem[] AssemblyPaths { get; set; } = Array.Empty<ITaskItem>();

    /// <summary>
    /// Gets or sets the names to map to a new resolved name.
    /// </summary>
    public string[] Names { get; set; } = Array.Empty<string>();

    /// <inheritdoc cref="Task.Execute()" />
    public override bool Execute()
    {
        /* This task remains for historical reasons */
        throw new NotSupportedException();
    }
}
