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

using System.Collections;
using dSPACE.Runtime.InteropServices.BuildTasks;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace dSPACE.Runtime.InteropServices.Tests;

public class BuildTaskTest : BaseTest
{
    private class BuildContextStub : IBuildContext
    {
        public ISet<string> ExistingFiles = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);

        public ISet<string> ExistingDirectories = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);

        public bool ShouldSucceed { get; set; } = true;

        public bool IsRunningOnWindows { get; set; } = true;

        public string RuntimeDescription { get; set; } = "x-unit runner";

        public bool ConvertAssemblyToTypeLib(TypeLibConverterSettings settings, TaskLoggingHelper log)
        {
            return ShouldSucceed;
        }

        public bool EnsureDirectoryExists(string? directoryPath)
        {
            return !ExistingDirectories.Any() || ExistingDirectories.Contains(directoryPath ?? string.Empty);
        }

        public bool EnsureFileExists(string? fileNameAndPath)
        {
            return !ExistingFiles.Any() || ExistingFiles.Contains(fileNameAndPath ?? string.Empty);
        }
    }

    private class BuildEngineStub : IBuildEngine
    {
        public bool ContinueOnError => true;

        public int LineNumberOfTaskNode => 1;

        public int ColumnNumberOfTaskNode => 1;

        public string ProjectFileOfTaskNode => "demo.test.csproj";

        public bool BuildProjectFile(string projectFileName, string[] targetNames, IDictionary globalProperties, IDictionary targetOutputs)
        {
            return true;
        }

        public void LogCustomEvent(CustomBuildEventArgs e)
        {
        }

        public void LogErrorEvent(BuildErrorEventArgs e)
        {
        }

        public void LogMessageEvent(BuildMessageEventArgs e)
        {
        }

        public void LogWarningEvent(BuildWarningEventArgs e)
        {
        }
    }

    private static BuildContextStub GetBuildContext()
    {
        return new BuildContextStub();
    }

    private static TlbExport GetBuildTask(out BuildContextStub context, bool shouldSucceed = true)
    {
        context = GetBuildContext();
        context.ShouldSucceed = shouldSucceed;
        var classUnderTest = new TlbExport(context)
        {
            BuildEngine = new BuildEngineStub()
        };
        return classUnderTest;
    }

    [Fact]
    public void TestDefaultSettingsSuccessful()
    {
        var task = GetBuildTask(out _);
        task.Execute().Should().BeTrue();
        task.Log.HasLoggedErrors.Should().BeFalse();
    }

    [Fact]
    public void TestDefaultSettingsFailingOnNonWindows()
    {
        var task = GetBuildTask(out var context);
        context.IsRunningOnWindows = false;
        task.Execute().Should().BeFalse();
        task.Log.HasLoggedErrors.Should().BeTrue();
    }

    [Fact]
    public void TestDefaultSettingsFailsOnTransformationError()
    {
        var task = GetBuildTask(out _, false);
        task.Execute().Should().BeFalse();
        task.Log.HasLoggedErrors.Should().BeFalse();
    }

    [Fact]
    public void TestGuidIsEmpty()
    {
        var task = GetBuildTask(out _);
        task.TlbOverriddenId = string.Empty;
        task.Execute().Should().BeFalse();
        task.Log.HasLoggedErrors.Should().BeTrue();
    }

    [Theory]
    [InlineData("03bc128f-0e54-44f0-9c3e-3fa1ed1.ceeb1")]
    [InlineData("03bc128g0e5444f09c3e3fa1ed1ceeb1")]
    [InlineData("03bc128f-0e54-44f0-9c3e-3fa1ed1ceeb1-abcd")]
    [InlineData("dcba-03bc128f-0e54-44f0-9c3e-3fa1ed1ceeb1")]
    public void TestGuidIsMalformed(string malformedGuid)
    {
        var task = GetBuildTask(out _);
        task.TlbOverriddenId = malformedGuid;
        task.Execute().Should().BeFalse();
        task.Log.HasLoggedErrors.Should().BeTrue();
    }

    [Theory]
    [InlineData("03bc128f-0e54-44f0-9c3e-3fa1ed1ceeb1")]
    [InlineData("03bc128f0e5444f09c3e3fa1ed1ceeb1")]
    [InlineData("00000000-0000-0000-0000-000000000000")]
    public void TestGuidIsCorrect(string guid)
    {
        var task = GetBuildTask(out _);
        task.TlbOverriddenId = guid;
        task.Execute().Should().BeTrue();
        task.Log.HasLoggedErrors.Should().BeFalse();
    }
}

