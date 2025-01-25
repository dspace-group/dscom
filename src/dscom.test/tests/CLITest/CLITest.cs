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

using Xunit;

namespace dSPACE.Runtime.InteropServices.Tests;
/// <summary>
/// The basic CLI tests to run.
/// </summary>
[Collection("CLI Tests")]
public class CLITest : CLITestBase
{
    public CLITest(CompileReleaseFixture compileFixture) : base(compileFixture) { }

    public void CallWithoutCommandOrOption_ExitCodeIs1AndStdOutIsHelpStringAndStdErrIsUsed()
    {
        var result = Execute(DSComPath);

        Assert.Equal(1, result.ExitCode);
        Assert.Contains("Usage:", result.StdOut);
        Assert.Contains(ErrorNoCommandOrOptions, result.StdErr);
    }

    public void CallWithoutCommandABC_ExitCodeIs1AndStdOutIsHelpStringAndStdErrIsUsed()
    {
        var result = Execute(DSComPath, "ABC");

        Assert.Equal(1, result.ExitCode);
        Assert.Contains("Usage:", result.StdOut);
        Assert.Contains("Unrecognized command or argument 'abc'", result.StdErr);
    }

    public void CallWithVersionOption_VersionIsAssemblyInformationalVersionAttributeValue()
    {
        var assemblyInformationalVersion = typeof(TypeLibConverter).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
        Assert.NotNull(assemblyInformationalVersion);
        var versionFromLib = assemblyInformationalVersion!.InformationalVersion;

        var result = Execute(DSComPath, "--version");
        Assert.Equal(0, result.ExitCode);
        Assert.Contains(versionFromLib, result.StdOut);
    }

    public void CallWithHelpOption_StdOutIsHelpStringAndExitCodeIsZero()
    {
        var result = Execute(DSComPath, "--help");
        Assert.Equal(0, result.ExitCode);
        Assert.Contains("Usage:", result.StdOut);
    }

    public void TlbExportAndHelpOption_StdOutIsHelpStringAndExitCodeIsZero()
    {
        var result = Execute(DSComPath, "tlbexport", "--help");
        Assert.Equal(0, result.ExitCode);
        Assert.Contains("Usage:", result.StdOut);
    }

    public void TlbDumpAndHelpOption_StdOutIsHelpStringAndExitCodeIsZero()
    {
        var result = Execute(DSComPath, "tlbdump", "--help");
        Assert.Equal(0, result.ExitCode);
        Assert.Contains("Usage:", result.StdOut);
    }

    public void TlbRegisterAndHelpOption_StdOutIsHelpStringAndExitCodeIsZero()
    {
        var result = Execute(DSComPath, "tlbregister", "--help");
        Assert.Equal(0, result.ExitCode);
        Assert.Contains("Usage:", result.StdOut);
    }

    public void TlbUnRegisterAndHelpOption_StdOutIsHelpStringAndExitCodeIsZero()
    {
        var result = Execute(DSComPath, "tlbunregister", "--help");
        Assert.Equal(0, result.ExitCode);
        Assert.Contains("Usage:", result.StdOut);
    }

    public void TlbUnRegisterAndFileNotExist_StdErrIsFileNotFoundAndExitCodeIs1()
    {
        var result = Execute(DSComPath, "tlbunregister", "abc");
        Assert.Equal(1, result.ExitCode);
        Assert.Contains("File not found", result.StdErr);
    }

    public void TlbRegisterAndFileNotExist_StdErrIsFileNotFoundAndExitCodeIs1()
    {
        var result = Execute(DSComPath, "tlbregister", "abc");
        Assert.Equal(1, result.ExitCode);
        Assert.Contains("File not found", result.StdErr);
    }

    public void TlbDumpAndFileNotExist_StdErrIsFileNotFoundAndExitCodeIs1()
    {
        var result = Execute(DSComPath, "tlbdump", "abc");
        Assert.Equal(1, result.ExitCode);
        Assert.Contains("File not found", result.StdErr);
    }

    public void TlbExportAndFileNotExist_StdErrIsFileNotFoundAndExitCodeIs1()
    {
        var result = Execute(DSComPath, "tlbexport", "abc");
        Assert.Equal(1, result.ExitCode);
        Assert.Contains("File not found", result.StdErr);
    }

    public void TlbExportAndDemoAssemblyAndCallWithTlbDump_ExitCodeIs0AndTlbIsAvailableAndValid()
    {
        var dependentFileName = Path.GetFileName(TestAssemblyDependencyTemporyTlbFilePath);
        var dependentTlbPath = Path.Combine(Path.GetDirectoryName(TestAssemblyTemporyTlbFilePath)!, dependentFileName);

        var result = Execute(DSComPath, "tlbexport", TestAssemblyPath, "--out", TestAssemblyTemporyTlbFilePath);
        Assert.Equal(0, result.ExitCode);

        Assert.True(File.Exists(TestAssemblyTemporyTlbFilePath));
        Assert.True(File.Exists(dependentTlbPath));

        var dumpResult = Execute(DSComPath, "tlbdump", TestAssemblyTemporyTlbFilePath, "--out", TestAssemblyTemporyYamlFilePath);
        Assert.Equal(0, dumpResult.ExitCode);

        Assert.True(File.Exists(TestAssemblyTemporyYamlFilePath));
    }

    public void TlbExportAndEmbedAssembly_ExitCodeIs0AndTlbIsEmbeddedAndValid()
    {
        var embedPath = GetEmbeddedPath(TestAssemblyPath);

        var dependentFileName = Path.GetFileName(TestAssemblyDependencyTemporyTlbFilePath);
        var dependentTlbPath = Path.Combine(Path.GetDirectoryName(TestAssemblyTemporyTlbFilePath)!, dependentFileName);

        var result = Execute(DSComPath, "tlbexport", TestAssemblyPath, $"--embed {embedPath}", "--out", TestAssemblyTemporyTlbFilePath);
        Assert.Equal(0, result.ExitCode);

        Assert.True(File.Exists(TestAssemblyTemporyTlbFilePath));
        Assert.True(File.Exists(dependentTlbPath));

        OleAut32.LoadTypeLibEx(embedPath, REGKIND.NONE, out var embeddedTypeLib);
        OleAut32.LoadTypeLibEx(TestAssemblyTemporyTlbFilePath, REGKIND.NONE, out var sourceTypeLib);

        embeddedTypeLib.GetDocumentation(-1, out var embeddedTypeLibName, out _, out _, out _);
        sourceTypeLib.GetDocumentation(-1, out var sourceTypeLibName, out _, out _, out _);

        Assert.Equal(sourceTypeLibName, embeddedTypeLibName);
    }

    public void TlbExportCreateMissingDependentTLBsFalseAndOverrideTlbId_ExitCodeIs0AndTlbIsAvailableAndDependentTlbIsNot()
    {
        var tlbFileName = $"{Path.GetFileNameWithoutExtension(TestAssemblyPath)}.tlb";
        var tlbFilePath = Path.Combine(Environment.CurrentDirectory, tlbFileName);

        var parameters = new[] { "tlbexport", TestAssemblyPath, "--createmissingdependenttlbs", "false", "--overridetlbid", "12345678-1234-1234-1234-123456789012" };

        var result = Execute(DSComPath, parameters);

        Assert.Equal(0, result.ExitCode);
        Assert.True(File.Exists(tlbFilePath));
    }

    public void TlbExportCreateMissingDependentTLBsFalse_ExitCodeIs0AndTlbIsAvailableAndDependentTlbIsNot()
    {
        var result = Execute(DSComPath, "tlbexport", TestAssemblyPath, "--createmissingdependenttlbs", "false", "--out", TestAssemblyTemporyTlbFilePath);
        Assert.Equal(0, result.ExitCode);

        Assert.True(File.Exists(TestAssemblyTemporyTlbFilePath));
        Assert.False(File.Exists(TestAssemblyDependencyTemporyTlbFilePath));

        Assert.Contains("auto generation of dependent type libs is disabled", result.StdErr);
        Assert.Contains(Path.GetFileNameWithoutExtension(TestAssemblyDependencyPath), result.StdErr);
    }

    public void TlbExportCreateMissingDependentTLBsTrue_ExitCodeIs0AndTlbIsAvailableAndDependentTlbIsNot()
    {
        var result = Execute(DSComPath, "tlbexport", TestAssemblyPath, "--createmissingdependenttlbs", "false", "--out", TestAssemblyTemporyTlbFilePath);
        Assert.Equal(0, result.ExitCode);

        Assert.True(File.Exists(TestAssemblyTemporyTlbFilePath));
        Assert.False(File.Exists(TestAssemblyDependencyTemporyTlbFilePath));
    }

    public void TlbExportCreateMissingDependentTLBsNoValue_ExitCodeIs0()
    {
        var dependentFileName = Path.GetFileName(TestAssemblyDependencyTemporyTlbFilePath);
        var dependentTlbPath = Path.Combine(Path.GetDirectoryName(TestAssemblyTemporyTlbFilePath)!, dependentFileName);

        var result = Execute(DSComPath, "tlbexport", TestAssemblyPath, "--createmissingdependenttlbs", "--out", TestAssemblyTemporyTlbFilePath);
        Assert.Equal(0, result.ExitCode);

        Assert.True(File.Exists(TestAssemblyTemporyTlbFilePath));
        Assert.True(File.Exists(dependentTlbPath));
    }

    public void TlbExportAndOptionSilent_StdOutAndStdErrIsEmpty()
    {
        var result = Execute(DSComPath, "tlbexport", TestAssemblyPath, "--silent", "--out", TestAssemblyTemporyTlbFilePath);
        Assert.Equal(0, result.ExitCode);

        Assert.True(File.Exists(TestAssemblyTemporyTlbFilePath));

        Assert.True(string.IsNullOrEmpty(result.StdOut.Trim()));
        Assert.True(string.IsNullOrEmpty(result.StdErr.Trim()));
    }

    public void TlbExportAndOptionSilenceTX801311A6andTX0013116F_StdOutAndStdErrIsEmpty()
    {
        var result = Execute(DSComPath, "tlbexport", TestAssemblyPath, "--silence", "TX801311A6", "--silence", "TX0013116F", "--out", TestAssemblyTemporyTlbFilePath);
        Assert.Equal(0, result.ExitCode);

        Assert.True(File.Exists(TestAssemblyTemporyTlbFilePath));

        Assert.True(string.IsNullOrEmpty(result.StdOut.Trim()));
        Assert.True(string.IsNullOrEmpty(result.StdErr.Trim()));
    }

    public void TlbExportAndOptionOverrideTLBId_TLBIdIsCorrect()
    {
        var guid = Guid.NewGuid();

        var result = Execute(DSComPath, "tlbexport", TestAssemblyPath, "--out", TestAssemblyTemporyTlbFilePath, "--overridetlbid", guid.ToString());

        Assert.Equal(0, result.ExitCode);
        Assert.True(File.Exists(TestAssemblyTemporyTlbFilePath));

        var dumpResult = Execute(DSComPath, "tlbdump", TestAssemblyTemporyTlbFilePath, "/out", TestAssemblyTemporyYamlFilePath);
        Assert.Equal(0, dumpResult.ExitCode);

        Assert.True(File.Exists(TestAssemblyTemporyYamlFilePath));

        var yamlContent = File.ReadAllText(TestAssemblyTemporyYamlFilePath);
        Assert.Contains($"guid: {guid}", yamlContent);
    }
}
