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

namespace dSPACE.Runtime.InteropServices.Tests;

/// <summary>
/// The tests for embeds are performed as a separate class due to the additional setup
/// required and the need for creating a TLB file as part of export prior to testing
/// the embed functionality itself. There are parallelization issues with running them,
/// even within the same class. Thus, the separate class has additional setup during the
/// constructor to perform the export once and ensure the process relating to export is
/// disposed with before attempting to test the embed functionality.
/// </summary>
[Collection("CLI Tests")]
public class CLITestEmbed : CLITestBase
{
    internal string TlbFilePath { get; }

    internal string DependentTlbPath { get; }

    public CLITestEmbed(CompileReleaseFixture compileFixture) : base(compileFixture)
    {
        var outputDirectory = Directory.GetParent(TestAssemblyPath) ?? throw new DirectoryNotFoundException("Output directory not found.");
        var tlbFileName = $"{Path.GetFileNameWithoutExtension(TestAssemblyPath)}.tlb";
        TlbFilePath = Path.Combine(outputDirectory.FullName, tlbFileName);

        var result = Execute(DSComPath, "tlbexport", TestAssemblyPath, "--out", TlbFilePath);
        result.ExitCode.Should().Be(0, $"because it should succeed. Error: ${result.StdErr}. Output: ${result.StdOut}");

        result = Execute(DSComPath, "tlbexport", TestAssemblyDependencyPath);
        result.ExitCode.Should().Be(0, $"because it should succeed. Error: ${result.StdErr}. Output: ${result.StdOut}");

        DependentTlbPath = $"{Path.GetFileNameWithoutExtension(TestAssemblyDependencyPath)}.tlb";

        File.Exists(TlbFilePath).Should().BeTrue($"File {TlbFilePath} should be available.");
        File.Exists(DependentTlbPath).Should().BeTrue($"File {DependentTlbPath} should be available.");

        // This is necessary to ensure the process from previous Execute for the export command has completely disposed before running tests.
        GC.Collect();
        GC.WaitForPendingFinalizers();
    }

    public void TlbEmbedAssembly_ExitCodeIs0AndTlbIsEmbeddedAndValid()
    {
        var embedPath = GetEmbeddedPath(TestAssemblyPath);

        var result = Execute(DSComPath, "tlbembed", TlbFilePath, embedPath);
        result.ExitCode.Should().Be(0, $"because embedding should succeed. Output: {result.StdErr} ");

        OleAut32.LoadTypeLibEx(embedPath, REGKIND.NONE, out var embeddedTypeLib);
        OleAut32.LoadTypeLibEx(TlbFilePath, REGKIND.NONE, out var sourceTypeLib);

        embeddedTypeLib.GetDocumentation(-1, out var embeddedTypeLibName, out _, out _, out _);
        sourceTypeLib.GetDocumentation(-1, out var sourceTypeLibName, out _, out _, out _);

        Assert.Equal(sourceTypeLibName, embeddedTypeLibName);
    }

    public void TlbEmbedAssemblyWithArbitraryIndex_ExitCodeIs0AndTlbIsEmbeddedAndValid()
    {
        var embedPath = GetEmbeddedPath(TestAssemblyPath);
        var result = Execute(DSComPath, "tlbembed", TlbFilePath, embedPath, "--index 2");
        result.ExitCode.Should().Be(0);

        OleAut32.LoadTypeLibEx(embedPath + "\\2", REGKIND.NONE, out var embeddedTypeLib);
        OleAut32.LoadTypeLibEx(TlbFilePath, REGKIND.NONE, out var sourceTypeLib);

        embeddedTypeLib.GetDocumentation(-1, out var embeddedTypeLibName, out _, out _, out _);
        sourceTypeLib.GetDocumentation(-1, out var sourceTypeLibName, out _, out _, out _);

        Assert.Equal(sourceTypeLibName, embeddedTypeLibName);
    }

    public void TlbEmbedAssemblyWithArbitraryTlbAndArbitraryIndex_ExitCodeIs0AndTlbIsEmbeddedAndValid()
    {
        var embedPath = GetEmbeddedPath(TestAssemblyPath);
        var result = Execute(DSComPath, "tlbembed", TlbFilePath, embedPath, "--index 3");
        result.ExitCode.Should().Be(0);

        OleAut32.LoadTypeLibEx(embedPath + "\\3", REGKIND.NONE, out var embeddedTypeLib);
        OleAut32.LoadTypeLibEx(TlbFilePath, REGKIND.NONE, out var sourceTypeLib);

        embeddedTypeLib.GetDocumentation(-1, out var embeddedTypeLibName, out _, out _, out _);
        sourceTypeLib.GetDocumentation(-1, out var sourceTypeLibName, out _, out _, out _);

        Assert.Equal(sourceTypeLibName, embeddedTypeLibName);
    }

    public void TlbEmbedAssemblyWithMultipleTypeLibraries_ExitCodeAre0AndTlbsAreEmbeddedAndValid()
    {
        var embedPath = GetEmbeddedPath(TestAssemblyPath);
        var result = Execute(DSComPath, "tlbembed", TlbFilePath, embedPath);
        result.ExitCode.Should().Be(0);

        result = Execute(DSComPath, "tlbembed", DependentTlbPath, TestAssemblyPath, "--index 2");
        result.ExitCode.Should().Be(0);

        OleAut32.LoadTypeLibEx(embedPath, REGKIND.NONE, out var embeddedTypeLib1);
        OleAut32.LoadTypeLibEx(TlbFilePath, REGKIND.NONE, out var sourceTypeLib1);

        embeddedTypeLib1.GetDocumentation(-1, out var embeddedTypeLibName1, out _, out _, out _);
        sourceTypeLib1.GetDocumentation(-1, out var sourceTypeLibName1, out _, out _, out _);

        Assert.Equal(sourceTypeLibName1, embeddedTypeLibName1);

        OleAut32.LoadTypeLibEx(TestAssemblyPath + "\\2", REGKIND.NONE, out var embeddedTypeLib2);
        OleAut32.LoadTypeLibEx(DependentTlbPath, REGKIND.NONE, out var sourceTypeLib2);

        embeddedTypeLib2.GetDocumentation(-1, out var embeddedTypeLibName2, out _, out _, out _);
        sourceTypeLib2.GetDocumentation(-1, out var sourceTypeLibName2, out _, out _, out _);

        Assert.Equal(sourceTypeLibName2, embeddedTypeLibName2);
    }
}
