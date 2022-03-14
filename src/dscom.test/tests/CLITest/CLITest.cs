// Copyright 2022 dSPACE GmbH, Mark Lechtermann, Matthias Nissen and Contributors
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

using System.Diagnostics;
using System.Text;

namespace dSPACE.Runtime.InteropServices.Tests;

public class CLITest : IClassFixture<CompileReleaseFixture>
{
    private const string ErrorNoCommandOrOptions = "Required command was not provided.";

    internal record struct ProcessOutput(string StdOut, string StdErr, int ExitCode);

    internal string DSComPath { get; set; } = string.Empty;

    internal string Workdir { get; } = string.Empty;

    public CLITest(CompileReleaseFixture compileFixture)
    {
        DSComPath = compileFixture.DSComPath;
        Workdir = compileFixture.Workdir;
    }

    [FactNoFramework]
    public void CallWithoutCommandOrOption_ExitCodeIs1AndStdOutIsHelpStringAndStdErrIsUsed()
    {
        var restult = Execute(DSComPath);

        restult.ExitCode.Should().Be(1);
        restult.StdErr.Trim().Should().Be(ErrorNoCommandOrOptions);
        restult.StdOut.Trim().Should().Contain("Description");
    }

    [FactNoFramework]
    public void CallWithoutCommandABC_ExitCodeIs1AndStdOutIsHelpStringAndStdErrIsUsed()
    {
        var restult = Execute(DSComPath, "ABC");

        restult.ExitCode.Should().Be(1);
        restult.StdErr.Trim().Should().Contain(ErrorNoCommandOrOptions);
        restult.StdErr.Trim().Should().Contain("Unrecognized command or argument 'ABC'");
    }

    [FactNoFramework]
    public void CallWithVersionOption_IsAssemblyInformationalVersionAttributeValue()
    {
        var assemblyInformationalVersion = typeof(TypeLibConverter).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
        assemblyInformationalVersion.Should().NotBeNull("AssemblyInformationalVersionAttribute is not set");
        var versionFromLib = assemblyInformationalVersion!.InformationalVersion;

        var restult = Execute(DSComPath, "--version");
        restult.ExitCode.Should().Be(0);
        restult.StdOut.Trim().Should().Be(versionFromLib);
    }

    internal static ProcessOutput Execute(string filname, params string[] args)
    {
        var processOutput = new ProcessOutput();
        var process = new Process();
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.RedirectStandardError = true;
        var sb = new StringBuilder();
        process.ErrorDataReceived += new DataReceivedEventHandler((sender, e) => { sb.Append(e.Data); });
        process.StartInfo.FileName = filname;
        process.StartInfo.Arguments = string.Join(" ", args);
        process.Start();

        process.BeginErrorReadLine();
        processOutput.StdOut = process.StandardOutput.ReadToEnd();
        processOutput.StdErr = sb.ToString();
        process.WaitForExit();
        processOutput.ExitCode = process.ExitCode;

        return processOutput;
    }
}
