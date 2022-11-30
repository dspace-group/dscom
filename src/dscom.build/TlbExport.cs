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
        if (!_context.IsRunningOnWindows)
        {
            var verbatimDescription = _context.RuntimeDescription;
            Log.LogError("This task can only be executed on Microsoft Windows (TM) based operating systems. This platform does not support the creation of this task: {0}", verbatimDescription);
            return false;
        }

        if (!Guid.TryParse(TlbOverriddenId, out var tlbOverriddenId))
        {
            Log.LogError("Cannot convert {0} to a valid Guid", TlbOverriddenId);
            return false;
        }

        // Create type library converter settings from task parameters
        var settings = new TypeLibConverterSettings()
        {
            Out = TargetFile,
            Assembly = SourceAssemblyFile,
            OverrideTlbId = tlbOverriddenId,
            TLBReference = ConvertTaskItemToFsPath(TypeLibraryReferences, false),
            TLBRefpath = ConvertTaskItemToFsPath(TypeLibraryReferencePaths, false),
            ASMPath = ConvertTaskItemToFsPath(AssemblyPaths, true)
        };

        // Issue a warning, if the type library is about to be overridden.
        if (settings.OverrideTlbId != Guid.Empty)
        {
            Log.LogMessage(MessageImportance.High, "The default unique id of the resulting type library will be overridden with the following value: {0}", settings.OverrideTlbId);
        }

        // Perform file system checks.
        var checks = new FileSystemChecks(Log, _context);

        var result = true;
        checks.VerifyFilePresent(settings.Assembly, true, ref result);
        checks.VerifyFilesPresent(settings.TLBReference, false, ref result);
        checks.VerifyDirectoriesPresent(settings.TLBRefpath, false, ref result);
        checks.VerifyFilesPresent(settings.ASMPath, false, ref result);

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
    /// <param name="canContainHintPaths">If set to <c>true</c>, the <paramref name="items"/> can 
    /// contain the Metadata value 'HintPath'. Hence the data will be scanned for this.
    /// If not, it is assumed, that the <paramref name="items"/> property 
    /// <see cref="ITaskItem.ItemSpec"/> contains the file system path.</param>
    /// <returns>The converted item-spec values.</returns>
    private static string[] ConvertTaskItemToFsPath(IReadOnlyCollection<ITaskItem> items, bool canContainHintPaths)
    {
        const string HintPath = nameof(HintPath);

        var itemsWithHintPath = Enumerable.Empty<ITaskItem>();
        if (canContainHintPaths)
        {
            itemsWithHintPath = items
                .Where(item => item != null)
                .Where(item => !string.IsNullOrWhiteSpace(item.GetMetadata(HintPath)));
        }

        var remainingItems = items.Except(itemsWithHintPath);

        return itemsWithHintPath
            .Select(item => item.GetMetadata(HintPath))
            .Union(remainingItems.Where(item => item != null)
                .Select(item => item.ItemSpec)).ToArray();
    }
}
