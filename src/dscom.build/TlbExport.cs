﻿// Copyright 2022 dSPACE GmbH, Carsten Igel and Contributors
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

using dSPACE.Runtime.InteropServices;
using Microsoft.Build.Framework;

using Task = Microsoft.Build.Utilities.Task;

namespace dSPACE.Build.Tasks.dscom;

public sealed class TlbExport : Task
{
    private readonly IBuildContext _context;

    public TlbExport(IBuildContext context)
    {
        _context = context;
    }

    public TlbExport() : this(new DefaultBuildContext())
    {
    }

    public Guid TlbOverriddenId { get; set; } = Guid.Empty;

    [Required]
    public string TargetFile { get; set; } = string.Empty;

    [Required]
    public string SourceAssemblyFile { get; set; } = string.Empty;

    public ITaskItem[] TypeLibraryReferences { get; set; } = Array.Empty<ITaskItem>();

    public ITaskItem[] TypeLibraryReferencePaths { get; set; } = Array.Empty<ITaskItem>();

    public ITaskItem[] AssemblyPaths { get; set; } = Array.Empty<ITaskItem>();

    public override bool Execute()
    {
        var settings = new TypeLibConverterSettings()
        {
            Out = TargetFile,
            Assembly = SourceAssemblyFile,
            OverrideTlbId = TlbOverriddenId,
            TLBReference = ConvertTaskItemToFsPath(TypeLibraryReferences),
            TLBRefpath = ConvertTaskItemToFsPath(TypeLibraryReferencePaths),
            ASMPath = ConvertTaskItemToFsPath(AssemblyPaths)
        };

        if (settings.OverrideTlbId != Guid.Empty)
        {
            Log.LogWarning("The default unique id of the resulting type library will be overridden with the following value: {0}", settings.OverrideTlbId);
        }

        var checks = new FileSystemChecks(Log);

        var result = true;
        checks.VerifyFilePresent(settings.Assembly, true, ref result);
        checks.VerifyFilesPresent(settings.TLBReference, false, ref result);
        checks.VerifyDirectoriesPresent(settings.TLBRefpath, false, ref result);
        checks.VerifyDirectoriesPresent(settings.ASMPath, false, ref result);

        result = result && _context.ConvertAssemblyToTypeLib(settings, Log);

        return result && !Log.HasLoggedErrors;
    }

    private static string[] ConvertTaskItemToFsPath(IReadOnlyCollection<ITaskItem> items)
    {
        return items.Where(item => item != null).Select(item => item.ItemSpec).ToArray();
    }

}
