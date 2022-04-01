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

using System.Reflection;
using System.Runtime.Loader;
using System.Security;
using dSPACE.Runtime.InteropServices;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using COMException = System.Runtime.InteropServices.COMException;

namespace dSPACE.Build.Tasks.dscom;

/// <summary>
/// Default implementation of the <see cref="IBuildContext" /> interface 
/// using <see cref="TypeLibConverter" /> as implementation for conversion 
/// and <see cref="LoggingTypeLibExporterSink" /> as implementation for 
/// event handling.
/// </summary>
internal sealed class DefaultBuildContext : IBuildContext
{
    /// <inheritdoc cref="IBuildContext.ConvertAssemblyToTypeLib" />
    public bool ConvertAssemblyToTypeLib(TypeLibConverterSettings settings, TaskLoggingHelper log)
    {
        // Load assembly from file.
        var loadContext = CreateLoadContext(settings);
        var assembly = LoadAssembly(settings, loadContext, log);

        if (assembly is null)
        {
            return false;
        }

        try
        {
            // Create type library converter.
            var converter = new TypeLibConverter();

            // Create event handler.
            var sink = new LoggingTypeLibExporterSink(log);
            // create conversion.
            var tlb = converter.ConvertAssemblyToTypeLib(assembly, settings, sink);
            if (tlb == null)
            {
                log.LogError("The following type library could not be created successfully: {0}. Reason: Operation was not successful.", settings.Out);
            }
            else
            {
                log.LogMessage(MessageImportance.High, "Finished generation of the following type library: {0}", settings.Out);
            }

            return tlb != null;
        }
        catch (COMException e)
        {
            log.LogErrorFromException(e, false, true, settings.Assembly);
            return false;
        }
        finally
        {
            try
            {
                loadContext.Unload();
            }
            catch (InvalidOperationException)
            {
                log.LogWarning("Failed to unload the assembly load context.");
            }
        }
    }

    /// <summary>
    /// Creates unloadable <see cref="AssemblyLoadContext"/> that will
    /// take care of the loading and unloading the target assemblies.
    /// </summary>
    /// <param name="settings">The type library settings.</param>
    /// <returns>An unloadable context.</returns>
    private static AssemblyLoadContext CreateLoadContext(TypeLibConverterSettings settings)
    {
        var loadContext = new AssemblyLoadContext($"msbuild-load-ctx-{Guid.NewGuid()}", true);
        loadContext.Resolving += (ctx, name) =>
        {
            var validAssemblyExtensions = new string[] { ".dll", ".exe" };
            var fileNameWithoutExtension = name.Name ?? string.Empty;
            foreach (var path in settings.ASMPath)
            {
                foreach (var extension in validAssemblyExtensions)
                {
                    var possibleFileName = Path.Combine(path, $"{fileNameWithoutExtension}{extension}");
                    if (File.Exists(possibleFileName))
                    {
                        return ctx.LoadFromAssemblyPath(possibleFileName);
                    }
                }
            }

            return default;
        };

        return loadContext;
    }

    /// <summary>
    /// Tries to load the assembly specified in the <paramref name="settings"/> using the specified
    /// <paramref name="loadContext"/>. If the assembly cannot be loaded, the result will be <c>null</c>.
    /// </summary>
    /// <param name="settings">The settings.</param>
    /// <param name="loadContext">The assembly load context.</param>
    /// <param name="log">The log to write messages to.</param>
    /// <returns>The assembly loaded.</returns>
    private static Assembly? LoadAssembly(TypeLibConverterSettings settings, AssemblyLoadContext loadContext, TaskLoggingHelper log)
    {
        Assembly assembly;
        try
        {
            assembly = loadContext.LoadFromAssemblyPath(settings.Assembly);
        }
        catch (Exception e) when
            (e is ArgumentNullException
               or FileNotFoundException
               or FileLoadException
               or BadImageFormatException
               or SecurityException
               or ArgumentException
               or PathTooLongException)
        {
            log.LogErrorFromException(e, true, true, settings.Assembly);
            try
            {
                loadContext.Unload();
            }
            catch (InvalidOperationException)
            {
                log.LogWarning("Failed to unload the following assembly: {0}.", settings.Assembly);
            }

            return default;
        }

        return assembly;
    }
}