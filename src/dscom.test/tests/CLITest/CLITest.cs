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

// The CLI is not available for .NET Framework
public class CLITest : IClassFixture<CompileReleaseFixture>
{
    private const string ErrorNoCommandOrOptions = "Required command was not provided.";

    internal record struct ProcessOutput(string StdOut, string StdErr, int ExitCode);

    internal string DSComPath { get; set; } = string.Empty;

    internal string Workdir { get; } = string.Empty;

    internal string DemoProjectAssembly1Path { get; }

    public CLITest(CompileReleaseFixture compileFixture)
    {
        DSComPath = compileFixture.DSComPath;
        Workdir = compileFixture.Workdir;
        DemoProjectAssembly1Path = compileFixture.DemoProjectAssembly1Path;
    }

    [Fact]
    public void CallWithoutCommandOrOption_ExitCodeIs1AndStdOutIsHelpStringAndStdErrIsUsed()
    {
        var result = Execute(DSComPath);

        result.ExitCode.Should().Be(1);
        result.StdErr.Trim().Should().Be(ErrorNoCommandOrOptions);
        result.StdOut.Trim().Should().Contain("Description");
    }

    [Fact]
    public void CallWithoutCommandABC_ExitCodeIs1AndStdOutIsHelpStringAndStdErrIsUsed()
    {
        var result = Execute(DSComPath, "ABC");

        result.ExitCode.Should().Be(1);
        result.StdErr.Trim().Should().Contain(ErrorNoCommandOrOptions);
        result.StdErr.Trim().Should().Contain("Unrecognized command or argument 'ABC'");
    }

    [Fact]
    public void CallWithVersionOption_VersionIsAssemblyInformationalVersionAttributeValue()
    {
        var assemblyInformationalVersion = typeof(TypeLibConverter).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
        assemblyInformationalVersion.Should().NotBeNull("AssemblyInformationalVersionAttribute is not set");
        var versionFromLib = assemblyInformationalVersion!.InformationalVersion;

        var result = Execute(DSComPath, "--version");
        result.ExitCode.Should().Be(0);
        result.StdOut.Trim().Should().Be(versionFromLib);
    }

    [Fact]
    public void CallWithHelpOption_StdOutIsHelpStringAndExitCodeIsZero()
    {
        var result = Execute(DSComPath, "--help");
        result.ExitCode.Should().Be(0);
        result.StdOut.Trim().Should().Contain("Description");
    }

    [Fact]
    public void CallWithCommandTlbExportAndHelpOption_StdOutIsHelpStringAndExitCodeIsZero()
    {
        var result = Execute(DSComPath, "tlbexport", "--help");
        result.ExitCode.Should().Be(0);
        result.StdOut.Trim().Should().Contain("Description");
    }

    [Fact]
    public void CallWithCommandTlbDumpAndHelpOption_StdOutIsHelpStringAndExitCodeIsZero()
    {
        var result = Execute(DSComPath, "tlbdump", "--help");
        result.ExitCode.Should().Be(0);
        result.StdOut.Trim().Should().Contain("Description");
    }

    [Fact]
    public void CallWithCommandTlbRegisterAndHelpOption_StdOutIsHelpStringAndExitCodeIsZero()
    {
        var result = Execute(DSComPath, "tlbregister", "--help");
        result.ExitCode.Should().Be(0);
        result.StdOut.Trim().Should().Contain("Description");
    }

    [Fact]
    public void CallWithCommandTlbUnRegisterAndHelpOption_StdOutIsHelpStringAndExitCodeIsZero()
    {
        var result = Execute(DSComPath, "tlbunregister", "--help");
        result.ExitCode.Should().Be(0);
        result.StdOut.Trim().Should().Contain("Description");
    }

    [Fact]
    public void CallWithCommandTlbUnRegisterAndFileNotExist_StdErrIsFileNotFoundAndExitCodeIs1()
    {
        var result = Execute(DSComPath, "tlbunregister", "abc");
        result.ExitCode.Should().Be(1);
        result.StdErr.Trim().Should().Contain("File abc not found");
    }

    [Fact]
    public void CallWithCommandTlbRegisterAndFileNotExist_StdErrIsFileNotFoundAndExitCodeIs1()
    {
        var result = Execute(DSComPath, "tlbregister", "abc");
        result.ExitCode.Should().Be(1);
        result.StdErr.Trim().Should().Contain("File abc not found");
    }

    [Fact]
    public void CallWithCommandTlbDumpAndFileNotExist_StdErrIsFileNotFoundAndExitCodeIs1()
    {
        var result = Execute(DSComPath, "tlbdump", "abc");
        result.ExitCode.Should().Be(1);
        result.StdErr.Trim().Should().Contain("File abc not found");
    }

    [Fact]
    public void CallWithCommandTlbExportAndFileNotExist_StdErrIsFileNotFoundAndExitCodeIs1()
    {
        var result = Execute(DSComPath, "tlbexport", "abc");
        result.ExitCode.Should().Be(1);
        result.StdErr.Trim().Should().Contain("File abc not found");
    }

    [Fact]
    public void CallWithCommandTlbExportAndDemoAssemblyAndCallWithTlbDump_ExitCodeIs0AndTLBIsAvailableAndValid()
    {
        var tlbFileName = $"{Path.GetFileNameWithoutExtension(DemoProjectAssembly1Path)}.tlb";
        var yamlFileName = $"{Path.GetFileNameWithoutExtension(DemoProjectAssembly1Path)}.yaml";

        var tlbFilePath = Path.Combine(Environment.CurrentDirectory, tlbFileName);
        var yamlFilePath = Path.Combine(Environment.CurrentDirectory, yamlFileName);

        var result = Execute(DSComPath, "tlbexport", DemoProjectAssembly1Path);
        result.ExitCode.Should().Be(0);

        File.Exists(tlbFilePath).Should().BeTrue($"File {tlbFilePath} should be available.");

        var dumpResult = Execute(DSComPath, "tlbdump", tlbFilePath);
        dumpResult.ExitCode.Should().Be(0);

        File.Exists(yamlFilePath).Should().BeTrue($"File {yamlFilePath} should be available.");

        File.Delete(tlbFilePath);
        File.Delete(yamlFilePath);
    }

    [Fact]
    public void CallWithCommandTlbExportAndOptionSilent_StdOutAndStdErrIsEmpty()
    {
        var tlbFileName = $"{Guid.NewGuid()}.tlb";
        var tlbFilePath = Path.Combine(Environment.CurrentDirectory, tlbFileName);

        var result = Execute(DSComPath, "tlbexport", DemoProjectAssembly1Path, "--out", tlbFileName, "--silent");
        result.ExitCode.Should().Be(0);

        File.Exists(tlbFilePath).Should().BeTrue($"File {tlbFilePath} should be available.");

        result.StdOut.Trim().Should().BeNullOrEmpty();
        result.StdErr.Trim().Should().BeNullOrEmpty();

        File.Delete(tlbFilePath);
    }

    [Fact]
    public void CallWithCommandTlbExportAndOptionSilenceTX801311A6_StdOutAndStdErrIsEmpty()
    {
        var tlbFileName = $"{Guid.NewGuid()}.tlb";
        var tlbFilePath = Path.Combine(Environment.CurrentDirectory, tlbFileName);

        var result = Execute(DSComPath, "tlbexport", DemoProjectAssembly1Path, "--out", tlbFileName, "--silence", "TX801311A6");
        result.ExitCode.Should().Be(0);

        File.Exists(tlbFilePath).Should().BeTrue($"File {tlbFilePath} should be available.");

        result.StdOut.Trim().Should().BeNullOrEmpty();
        result.StdErr.Trim().Should().BeNullOrEmpty();

        File.Delete(tlbFilePath);
    }

    [Fact]
    public void CallWithCommandTlbExportAndOptionOverrideTLBId_TLBIdIsCorrect()
    {
        var guid = Guid.NewGuid().ToString();

        var tlbFileName = $"{guid}.tlb";
        var yamlFileName = $"{guid}.yaml";

        var tlbFilePath = Path.Combine(Environment.CurrentDirectory, tlbFileName);
        var yamlFilePath = Path.Combine(Environment.CurrentDirectory, yamlFileName);

        var result = Execute(DSComPath, "tlbexport", DemoProjectAssembly1Path, "--out", tlbFilePath, "--overridetlbid", guid);

        result.ExitCode.Should().Be(0);
        File.Exists(tlbFilePath).Should().BeTrue($"File {tlbFilePath} should be available.");

        var dumpResult = Execute(DSComPath, "tlbdump", tlbFilePath, "/out", yamlFilePath);
        dumpResult.ExitCode.Should().Be(0);

        File.Exists(yamlFilePath).Should().BeTrue($"File {yamlFilePath} should be available.");

        var yamlContent = File.ReadAllText(yamlFilePath);
        yamlContent.Should().Contain($"guid: {guid}");

        File.Delete(tlbFilePath);
        File.Delete(yamlFilePath);
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
