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

namespace dSPACE.Build.Tasks.dscom;

internal sealed class FileSystemChecks
{
    private readonly TaskLoggingHelper _log;

    internal FileSystemChecks(TaskLoggingHelper log)
    {
        this._log = log;
    }

    internal void VerifyFilePresent(string fileSystemReference, bool treatAsError, ref bool checkResult)
    {
        checkResult = checkResult && this.LogCheckIfFileSystemEntryIsMissing(
            File.Exists,
            fileSystemReference,
            treatAsError,
            "The following file is required, but does not exist: {0}",
            fileSystemReference);
    }

    internal void VerifyFilesPresent(IReadOnlyCollection<string> fileSystemReferences, bool treatAsError, ref bool checkResult)
    {
        foreach (var possibleFileSystemEntry in fileSystemReferences)
        {
            this.VerifyFilePresent(possibleFileSystemEntry, treatAsError, ref checkResult);
        }
    }

    internal void VerifyDirectoryPresent(string fileSystemReference, bool treatAsError, ref bool checkResult)
    {
        checkResult = checkResult && this.LogCheckIfFileSystemEntryIsMissing(
            Directory.Exists,
            fileSystemReference,
            treatAsError,
            "The following directory is required, but does not exist: {0}",
            fileSystemReference);
    }

    internal void VerifyDirectoriesPresent(IReadOnlyCollection<string> fileSystemReferences, bool treatAsError, ref bool checkResult)
    {
        foreach (var possibleFileSystemEntry in fileSystemReferences)
        {
            this.VerifyDirectoryPresent(possibleFileSystemEntry, treatAsError, ref checkResult);
        }
    }

    private bool LogCheckIfFileSystemEntryIsMissing(Func<string, bool> performCheck, string fileSystemEntry, bool treatAsError, string message, params object[] args)
    {
        var flag = performCheck(fileSystemEntry);
        if (flag)
        {
            this.WriteMessageToLog(treatAsError, message, args);
        }

        return !treatAsError || flag;
    }

    private void WriteMessageToLog(bool treatAsError, string message, params object[] args)
    {
        Action<string, object[]> logger = this._log.LogWarning;
        if (treatAsError)
        {
            logger = this._log.LogError;
        }

        logger(message, args);
    }
}
