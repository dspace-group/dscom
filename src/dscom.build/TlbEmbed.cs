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

#if NET5_0_OR_GREATER
using System.Runtime.Loader;
#else
using System.Reflection;
#endif

using Microsoft.Build.Framework;

namespace dSPACE.Runtime.InteropServices.BuildTasks;

/// <summary>
/// Represents the task to embed a type library file into an assembly.
/// </summary>
public sealed class TlbEmbed : Microsoft.Build.Utilities.Task
{
    /// <summary>
    /// The build context applied to this instance.
    /// </summary>
    private readonly IBuildContext _context;

    /// <summary>
    /// Creates a new instance of the <see cref="TlbEmbed" />
    /// embedder class using the specified build context.
    /// </summary>
    /// <param name="context">The build context to apply.</param>
    public TlbEmbed(IBuildContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Creates a new instance of the <see cref="TlbEmbed" />
    /// class using the default build context.
    /// </summary>
    public TlbEmbed() : this(new DefaultBuildContext())
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
        var targetAssemblyFile = GetTargetRuntimeAssembly();

        // if the assembly is found next to this assembly
        if (File.Exists(targetAssemblyFile))
        {
#if NET5_0_OR_GREATER
            // Load the assembly from a context
            var loadContext = new AssemblyLoadContext($"msbuild-preload-ctx-{Guid.NewGuid()}", true);

            try
            {
                _ = loadContext.LoadFromAssemblyPath(targetAssemblyFile);

                // Execute the task with the resolved assembly.
                return ExecuteTask();
            }
            finally
            {
                // unload the assembly
                loadContext.Unload();
            }
#else
            _ = Assembly.LoadFrom(targetAssemblyFile);

            return ExecuteTask();
#endif        
        }
        else
        {
            // Make .NET Runtime resolve the file itself
            return ExecuteTask();
        }
    }

    /// <summary>
    /// Performs the real task execution with the maybe temporarily loaded
    /// Interop Assembly.
    /// </summary>
    /// <returns>The result of the task.</returns>
    private bool ExecuteTask()
    {
        if (!_context.IsRunningOnWindows)
        {
            var verbatimDescription = _context.RuntimeDescription;
            Log.LogError("This task can only be executed on Microsoft Windows (TM) based operating systems. This platform does not support the creation of this task: {0}", verbatimDescription);
            return false;
        }

        // Perform file system checks.
        var checks = new FileSystemChecks(Log, _context);

        var result = true;
        checks.VerifyFilePresent(TargetAssemblyFile, true, ref result);
        checks.VerifyFilePresent(SourceTlbFile, true, ref result);

        var settings = new TypeLibEmbedderSettings
        {
            SourceTypeLibrary = SourceTlbFile,
            TargetAssembly = TargetAssemblyFile
        };

        // run conversion, if result has been successful.
        result = result && _context.EmbedTypeLib(settings, Log);

        // report success or failure.
        return result && !Log.HasLoggedErrors;
    }

    /// <summary>
    /// Gets the path to the path to the <code>dSPACE.Runtime.InteropServices.dll</code>
    /// next to this assembly.
    /// </summary>
    /// <returns>The assembly file path.</returns>
    private static string GetTargetRuntimeAssembly()
    {
        var assemblyPath = typeof(TlbExport).Assembly.Location;
        var extension = Path.GetExtension(assemblyPath);
        var fileBaseName = typeof(TlbExport).Namespace;
        fileBaseName = Path.GetFileNameWithoutExtension(fileBaseName);
        var fileName = fileBaseName + extension;
        var assemblyDir = Path.GetDirectoryName(assemblyPath);

        var targetAssemblyFile = fileName;

        if (!string.IsNullOrWhiteSpace(assemblyDir))
        {
            targetAssemblyFile = Path.Combine(assemblyDir, fileName);
        }

        return targetAssemblyFile;
    }
}
