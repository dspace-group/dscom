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

public class CLITest
{
    public record struct ProcessOutput(string StdOut, string StdErr, int ExitCode);

    public CLITest()
    {
        var workdir = new DirectoryInfo(Environment.CurrentDirectory).Parent?.Parent?.Parent?.Parent?.Parent;
        if (workdir == null || !workdir.Exists)
        {
            throw new DirectoryNotFoundException("Workdir not found.");
        }

        Workdir = workdir.FullName;

        // Compile Release
        var restult = Execute("dotnet", "build", Workdir, "-c", "Release");
        restult.ExitCode.Should().Be(0);

        // Path to descom.exe
        DSComPath = Path.Combine(Workdir, "src", "dscom.client", "bin", "Release", "net6.0", "dscom.exe");
    }

    private string Workdir { get; }

    private string DSComPath { get; }

    [Fact]
    public void CLIVersion_IsAssemblyInformationalVersionAttributeValue()
    {
        var assemblyInformationalVersion = typeof(TypeLibConverter).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
        assemblyInformationalVersion.Should().NotBeNull("AssemblyInformationalVersionAttribute is not set");
        var versionFromLib = assemblyInformationalVersion!.InformationalVersion;

        var restult = Execute(DSComPath, "--version");
        restult.ExitCode.Should().Be(0);
        restult.StdOut.Trim().Should().Be(versionFromLib);
    }

    private static ProcessOutput Execute(string filname, params string[] args)
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
