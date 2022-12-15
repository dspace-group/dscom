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

using Microsoft.Build.Utilities;

namespace dSPACE.Runtime.InteropServices.BuildTasks;

/// <summary>
/// Interface representing the execution and build context.
/// This interface is used to separate task execution from
/// tlb conversion.
/// Hence implementations will perform any TLB conversion.
/// </summary>
public interface IBuildContext
{
    /// <summary>
    /// Gets a value indicating whether the current run-time is windows-based or not.
    /// </summary>
    bool IsRunningOnWindows { get; }

    /// <summary>
    /// Gets a verbatim description of the current run-time.
    /// </summary>
    string RuntimeDescription { get; }

    /// <summary>
    /// When implemented it will return, whether the specified <paramref name="fileNameAndPath"/>
    /// points to a valid file or not.
    /// </summary>
    /// <param name="fileNameAndPath">The name and path of the file.</param>
    /// <returns><c>true</c>, if the file exists; <c>false</c> otherwise.</returns>
    bool EnsureFileExists(string? fileNameAndPath);

    /// <summary>
    /// When implemented it will return, whether the specified <paramref name="directoryPath"/>
    /// points to a valid directory or not.
    /// </summary>
    /// <param name="directoryPath">The name and path of the directory.</param>
    /// <returns><c>true</c>, if the directory exists; <c>false</c> otherwise.</returns>
    bool EnsureDirectoryExists(string? directoryPath);

    /// <summary>
    /// When implemented in a derived class, the conversion will take place
    /// trying to load the assembly specified in the <see cref="TypeLibConverterSettings.Assembly" />
    /// of the specified <paramref name="settings" /> object and convert it to the type library specified in
    /// the <see cref="TypeLibConverterSettings.TypeLibrary" /> of the same parameter.
    /// Errors, warnings and conversion messages will be written to the build <paramref name="log" />.
    /// </summary>
    /// <param name="settings">The conversion settings to apply to the built-in converter.</param>
    /// <param name="log">The log to write error messages to.</param>
    /// <returns><c>true</c>, if the conversion has taken place successfully; <c>false</c> otherwise.</returns>
    bool ConvertAssemblyToTypeLib(TypeLibConverterSettings settings, TaskLoggingHelper log);
}
