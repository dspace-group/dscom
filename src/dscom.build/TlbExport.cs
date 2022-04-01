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

namespace dSPACE.Build.Tasks.dscom;

public sealed class TlbExport : Task
{
    /// <summary>
    /// The build context applied to this instance.
    /// </summary>
    private readonly IBuildContext _context;

    /// <summary>
    /// Creates a new instance of the <see cref="TlbExport" />
    /// export class using the specified build context.
    /// </summary>
    /// <param name="context">The build context to apply.</param>
    public TlbExport(IBuildContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Creates a new instance of the <see cref="TlbExport" />
    /// class using the default build context.
    /// </summary>
    public TlbExport() : this(new DefaultBuildContext())
    {
    }

    /// <summary>
    /// Gets or sets the COM Type Library unique id. If this value 
    /// will be equal to <see cref="Guid.Empty" />, the id of the 
    /// resulting COM type library will be not be changed.
    /// </summary>
    public Guid TlbOverriddenId { get; set; } = Guid.Empty;

    /// <summary>
    /// Gets or sets the name of the resulting COM type library file (tlb).
    /// This value must be set for this task to work and cannot be left emptry.
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

    /// <inheritdoc cref="Task.Execute()" />
    public override bool Execute()
    {
        // Create type library converter settings from task paramters
        var settings = new TypeLibConverterSettings()
        {
            Out = TargetFile,
            Assembly = SourceAssemblyFile,
            OverrideTlbId = TlbOverriddenId,
            TLBReference = ConvertTaskItemToFsPath(TypeLibraryReferences),
            TLBRefpath = ConvertTaskItemToFsPath(TypeLibraryReferencePaths),
            ASMPath = ConvertTaskItemToFsPath(AssemblyPaths)
        };

        // Issue a warning, if the type library is about to be overridden.
        if (settings.OverrideTlbId != Guid.Empty)
        {
            Log.LogMessage(MessageImportance.High, "The default unique id of the resulting type library will be overridden with the following value: {0}", settings.OverrideTlbId);
        }

        // Perform file system checks.
        var checks = new FileSystemChecks(Log);

        var result = true;
        checks.VerifyFilePresent(settings.Assembly, true, ref result);
        checks.VerifyFilesPresent(settings.TLBReference, false, ref result);
        checks.VerifyDirectoriesPresent(settings.TLBRefpath, false, ref result);
        checks.VerifyDirectoriesPresent(settings.ASMPath, false, ref result);

        // run conversion, if result has been successful.
        result = result && _context.ConvertAssemblyToTypeLib(settings, Log);

        // report success or failure.
        return result && !Log.HasLoggedErrors;
    }

    /// <summary>
    /// Takes the <see cref="ITaskItem.ItemSpec" /> of the specified <paramref name="items" />
    /// and interprets them as file system entry and hence a file or directory path.
    /// </summary>
    /// <param name="items">The items to interpret a file system entries.</param>
    /// <returns>The converted item-spec values.</returns>
    private static string[] ConvertTaskItemToFsPath(IReadOnlyCollection<ITaskItem> items)
    {
        return items.Where(item => item != null).Select(item => item.ItemSpec).ToArray();
    }
}
