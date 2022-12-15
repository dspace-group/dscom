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
/// Implementation of file system contracts and the corresponding checks using the logging mechanisms of MsBuild.
/// </summary>
internal sealed class FileSystemChecks
{
    /// <summary>
    /// The logging mechanism from the task.
    /// </summary>
    private readonly TaskLoggingHelper _log;

    /// <summary>
    /// The build context.
    /// </summary>
    private readonly IBuildContext _context;

    /// <summary>
    /// Creates a new instance of the <see cref="FileSystemChecks" /> class
    /// using the supplied <paramref name="log" /> to write messages to.
    /// </summary>
    /// <param name="log">The log to apply.</param>
    /// <param name="context">The context to apply.</param>
    internal FileSystemChecks(TaskLoggingHelper log, IBuildContext context)
    {
        _log = log;
        _context = context;
    }

    /// <summary>
    /// Verifies, that the specified <paramref name="fileSystemReference" /> is a valid file.
    /// If a non-existing file shall be treated as an error, the corresponding parameter <paramref name="treatAsError" />
    /// must be set to <c>true</c> and the check will fail. The result of the check will be stored in the <paramref name="checkResult" />
    /// parameter.
    /// </summary>
    /// <param name="fileSystemReference">The files to check.</param>
    /// <param name="treatAsError">If set to <c>true</c>, the check will fail, if the file does not exist.</param>
    /// <param name="checkResult">The result of any previous check. If <c>true</c> is supplied,
    /// the result can remain <c>true</c>. Otherwise the result will always be <c>false</c>, even, if the check succeeds.</param>
    internal void VerifyFilePresent(string fileSystemReference, bool treatAsError, ref bool checkResult)
    {
        checkResult = checkResult && LogCheckIfFileSystemEntryIsMissing(
            _context.EnsureFileExists,
            fileSystemReference,
            treatAsError,
            "The following file is required, but does not exist: {0}",
            fileSystemReference);
    }

    /// <summary>
    /// Verifies, that the specified <paramref name="fileSystemReferences" /> are valid files.
    /// If a non-existing file shall be treated as an error, the corresponding parameter <paramref name="treatAsError" />
    /// must be set to <c>true</c> and the check will fail. The result of the check will be stored in the <paramref name="checkResult" />
    /// parameter.
    /// </summary>
    /// <param name="fileSystemReferences">The files to check.</param>
    /// <param name="treatAsError">If set to <c>true</c>, the check will fail, if at least one file does not exist.</param>
    /// <param name="checkResult">The result of any previous check. If <c>true</c> is supplied,
    /// the result can remain <c>true</c>. Otherwise the result will always be <c>false</c>, even, if the check succeeds.</param>
    internal void VerifyFilesPresent(IReadOnlyCollection<string> fileSystemReferences, bool treatAsError, ref bool checkResult)
    {
        foreach (var possibleFileSystemEntry in fileSystemReferences)
        {
            VerifyFilePresent(possibleFileSystemEntry, treatAsError, ref checkResult);
        }
    }

    /// <summary>
    /// Verifies, that the specified <paramref name="fileSystemReference" /> is a valid directory.
    /// If a non-existing file shall be treated as an error, the corresponding parameter <paramref name="treatAsError" />
    /// must be set to <c>true</c> and the check will fail. The result of the check will be stored in the <paramref name="checkResult" />
    /// parameter.
    /// </summary>
    /// <param name="fileSystemReference">The directory to check.</param>
    /// <param name="treatAsError">If set to <c>true</c>, the check will fail, if the directory does not exist.</param>
    /// <param name="checkResult">The result of any previous check. If <c>true</c> is supplied,
    /// the result can remain <c>true</c>. Otherwise the result will always be <c>false</c>, even, if the check succeeds.</param>
    internal void VerifyDirectoryPresent(string fileSystemReference, bool treatAsError, ref bool checkResult)
    {
        checkResult = checkResult && LogCheckIfFileSystemEntryIsMissing(
            _context.EnsureDirectoryExists,
            fileSystemReference,
            treatAsError,
            "The following file is required, but does not exist: {0}",
            fileSystemReference);
    }

    /// <summary>
    /// Verifies, that the specified <paramref name="fileSystemReferences" /> are valid directories.
    /// If a non-existing file shall be treated as an error, the corresponding parameter <paramref name="treatAsError" />
    /// must be set to <c>true</c> and the check will fail. The result of the check will be stored in the <paramref name="checkResult" />
    /// parameter.
    /// </summary>
    /// <param name="fileSystemReferences">The directories to check.</param>
    /// <param name="treatAsError">If set to <c>true</c>, the check will fail, if at least one directory does not exist.</param>
    /// <param name="checkResult">The result of any previous check. If <c>true</c> is supplied,
    /// the result can remain <c>true</c>. Otherwise the result will always be <c>false</c>, even, if the check succeeds.</param>
    internal void VerifyDirectoriesPresent(IReadOnlyCollection<string> fileSystemReferences, bool treatAsError, ref bool checkResult)
    {
        foreach (var possibleFileSystemEntry in fileSystemReferences)
        {
            VerifyDirectoryPresent(possibleFileSystemEntry, treatAsError, ref checkResult);
        }
    }


    /// <summary>
    /// Performs the specified <paramref name="performCheck" /> method using the <paramref name="fileSystemEntry" />.
    /// If the check fails, the specified <paramref name="message" /> will be issued to the log.
    /// If no error is issued, the method will return <c>true</c>.
    /// </summary>
    /// <param name="performCheck">The check to apply.</param>
    /// <param name="fileSystemEntry">The file system entry to check.</param>
    /// <param name="treatAsError">If set to <c>true</c>, the <paramref name="message" /> will be issued as error; else a warning shall be submitted.</param>
    /// <param name="message">The message to log.</param>
    /// <param name="args">Formatting arguments for the <paramref name="message" />.</param>
    /// <returns><c>true</c>, if the check is issued no error; <c>false</c> otherwise.</returns>
    private bool LogCheckIfFileSystemEntryIsMissing(Func<string, bool> performCheck, string fileSystemEntry, bool treatAsError, string message, params object[] args)
    {
        var flag = performCheck(fileSystemEntry);
        if (!flag)
        {
            WriteMessageToLog(treatAsError, message, args);
        }

        return !treatAsError || flag;
    }

    /// <summary>
    /// Writes a message to the log referenced by this instance.
    /// </summary>
    /// <param name="treatAsError">If set to <c>true</c>, the message will be treated as an error. A warning will be issued otherwise.</param>
    /// <param name="message">The message to supply.</param>
    /// <param name="args">The arguments to format the message.</param>
    private void WriteMessageToLog(bool treatAsError, string message, params object[] args)
    {
        Action<string, object[]> logger = _log.LogWarning;
        if (treatAsError)
        {
            logger = _log.LogError;
        }

        logger(message, args);
    }
}
