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

using Moq;

namespace dSPACE.Runtime.InteropServices.Tests;

public class BuildTaskTest : BaseTest
{
    private sealed class BuildContextStub : IBuildContext
    {
        public ISet<string> ExistingFiles = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);

        public bool ShouldSucceed { get; set; } = true;

        public bool IsRunningOnWindows { get; set; } = true;

        public string RuntimeDescription { get; set; } = "x-unit runner";

        public bool ConvertAssemblyToTypeLib(TypeLibConverterSettings settings, TaskLoggingHelper log)
        {
            return ShouldSucceed;
        }

        public bool EnsureFileExists(string? fileNameAndPath)
        {
            return !ExistingFiles.Any() || ExistingFiles.Contains(fileNameAndPath ?? string.Empty);
        }
    }

    private sealed class BuildEngineStub : IBuildEngine
    {
        public bool ContinueOnError => true;

        public int LineNumberOfTaskNode => 1;

        public int ColumnNumberOfTaskNode => 1;

        public string ProjectFileOfTaskNode => "demo.test.csproj";

        public int NumberOfCustomLogEvents { get; private set; }

        public int NumberOfMessageLogEvents { get; private set; }

        public int NumberOfWarningLogEvents { get; private set; }

        public int NumberOfErrorLogEvents { get; private set; }

        public bool BuildProjectFile(string projectFileName, string[] targetNames, IDictionary globalProperties, IDictionary targetOutputs)
        {
            return true;
        }

        public void LogCustomEvent(CustomBuildEventArgs e)
        {
            NumberOfCustomLogEvents++;
        }

        public void LogErrorEvent(BuildErrorEventArgs e)
        {
            NumberOfErrorLogEvents++;
        }

        public void LogMessageEvent(BuildMessageEventArgs e)
        {
            NumberOfMessageLogEvents++;
        }

        public void LogWarningEvent(BuildWarningEventArgs e)
        {
            NumberOfWarningLogEvents++;
        }
    }

    private static ITaskItem GetTaskItem(string itemSpec, string? path = null, bool useHintPath = false)
    {
        var taskItemMock = new Mock<ITaskItem>(MockBehavior.Loose);

        taskItemMock.SetupGet(inst => inst.ItemSpec).Returns(itemSpec);

        if (useHintPath)
        {
            const string HintPath = nameof(HintPath);

            var hintPath = path ?? itemSpec;

            taskItemMock.Setup(inst => inst.GetMetadata(It.Is(HintPath, StringComparer.InvariantCulture))).Returns(hintPath);

            taskItemMock.SetupGet(inst => inst.MetadataCount).Returns(1);
            taskItemMock.SetupGet(inst => inst.MetadataNames).Returns(new string[] { HintPath });
        }
        else
        {
            taskItemMock.SetupGet(inst => inst.MetadataCount).Returns(0);
            taskItemMock.SetupGet(inst => inst.MetadataNames).Returns(Array.Empty<string>());
        }

        taskItemMock.Setup(inst => inst.GetMetadata(It.IsAny<string>())).Returns(string.Empty);

        return taskItemMock.Object;
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

    [Fact]
    public void TestAssemblyFileCheckSuccess()
    {
        var task = GetBuildTask(out var context);
        var assemblyFileName = "MyAssemblyFile.dll";
        var assemblyFilePath = Path.Combine(Path.GetTempPath(), assemblyFileName);
        task.SourceAssemblyFile = assemblyFilePath;
        context.ExistingFiles.Add(assemblyFilePath);

        task.Execute().Should().BeTrue();
        task.Log.HasLoggedErrors.Should().BeFalse();
    }

    [Fact]
    public void TestAssemblyFileCheckFail()
    {
        var task = GetBuildTask(out var context);
        var assemblyFileName = "MyAssemblyFile.dll";
        var assemblyFilePath = Path.Combine(Path.GetTempPath(), assemblyFileName);
        task.SourceAssemblyFile = assemblyFilePath;
        context.ExistingFiles.Add(assemblyFilePath + ".notExisting");

        task.Execute().Should().BeFalse();
        task.Log.HasLoggedErrors.Should().BeTrue();
    }

    [Fact]
    public void TestAssemblyFileReferencesItemSpecCheckSuccess()
    {
        var task = GetBuildTask(out var context);
        var assemblyFileName = "MyAssemblyFile.dll";
        var assemblyFilePath = Path.Combine(Path.GetTempPath(), assemblyFileName);
        task.SourceAssemblyFile = assemblyFilePath;
        context.ExistingFiles.Add(assemblyFilePath);

        var taskItems = new List<ITaskItem>();
        for (var i = 1; i <= 5; i++)
        {
            var fileName = $"ReferencedAssembly{i}.dll";
            var filePath = Path.Combine(Path.GetTempPath(), fileName);
            var taskItem = GetTaskItem(filePath);
            taskItems.Add(taskItem);
            context.ExistingFiles.Add(filePath);
        }

        task.Execute().Should().BeTrue();
        task.Log.HasLoggedErrors.Should().BeFalse();
        task.BuildEngine.As<BuildEngineStub>().NumberOfWarningLogEvents.Should().Be(0, "No warning should be present");
    }

    [Fact]
    public void TestAssemblyFileReferencesItemSpecCheckFail()
    {
        var task = GetBuildTask(out var context);
        var assemblyFileName = "MyAssemblyFile.dll";
        var assemblyFilePath = Path.Combine(Path.GetTempPath(), assemblyFileName);
        task.SourceAssemblyFile = assemblyFilePath;
        context.ExistingFiles.Add(assemblyFilePath);

        var notFoundByRandom = new Random().Next(5) + 1;

        var taskItems = new List<ITaskItem>();
        for (var i = 1; i <= 5; i++)
        {
            var fileName = $"ReferencedAssembly{i}.dll";
            var filePath = Path.Combine(Path.GetTempPath(), fileName);
            var taskItem = GetTaskItem(filePath);
            taskItems.Add(taskItem);
            if (i != notFoundByRandom)
            {
                context.ExistingFiles.Add(filePath);
            }

        }

        task.Execute().Should().BeTrue();
        task.Log.HasLoggedErrors.Should().BeFalse();
        task.BuildEngine.As<BuildEngineStub>().Should().NotBe(0, "At least one warning should be present");
    }

    [Fact]
    public void TestAssemblyFileReferencesHintPathCheckSuccess()
    {
        var task = GetBuildTask(out var context);
        var assemblyFileName = "MyAssemblyFile.dll";
        var assemblyFilePath = Path.Combine(Path.GetTempPath(), assemblyFileName);
        task.SourceAssemblyFile = assemblyFilePath;
        context.ExistingFiles.Add(assemblyFilePath);

        var taskItems = new List<ITaskItem>();
        for (var i = 1; i <= 5; i++)
        {
            var fileName = $"ReferencedAssembly{i}.dll";
            var filePath = Path.Combine(Path.GetTempPath(), fileName);
            var taskItem = GetTaskItem(fileName, filePath, true);
            taskItems.Add(taskItem);
            context.ExistingFiles.Add(filePath);
        }

        task.Execute().Should().BeTrue();
        task.Log.HasLoggedErrors.Should().BeFalse();
        task.BuildEngine.As<BuildEngineStub>().NumberOfWarningLogEvents.Should().Be(0, "No warning should be present");
    }

    [Fact]
    public void TestAssemblyFileReferencesHintPathCheckFail()
    {
        var task = GetBuildTask(out var context);
        var assemblyFileName = "MyAssemblyFile.dll";
        var assemblyFilePath = Path.Combine(Path.GetTempPath(), assemblyFileName);
        task.SourceAssemblyFile = assemblyFilePath;
        context.ExistingFiles.Add(assemblyFilePath);

        var notFoundByRandom = new Random().Next(5) + 1;

        var taskItems = new List<ITaskItem>();
        for (var i = 1; i <= 5; i++)
        {
            var fileName = $"ReferencedAssembly{i}.dll";
            var filePath = Path.Combine(Path.GetTempPath(), fileName);
            var taskItem = GetTaskItem(fileName, filePath, true);
            taskItems.Add(taskItem);
            if (i != notFoundByRandom)
            {
                context.ExistingFiles.Add(filePath);
            }
        }

        task.Execute().Should().BeTrue();
        task.Log.HasLoggedErrors.Should().BeFalse();
        task.BuildEngine.As<BuildEngineStub>().Should().NotBe(0, "At least one warning should be present");
    }

    [Fact]
    public void TestAssemblyFileReferencesHybridCheckSuccess()
    {
        var task = GetBuildTask(out var context);
        var assemblyFileName = "MyAssemblyFile.dll";
        var assemblyFilePath = Path.Combine(Path.GetTempPath(), assemblyFileName);
        task.SourceAssemblyFile = assemblyFilePath;
        context.ExistingFiles.Add(assemblyFilePath);

        var taskItems = new List<ITaskItem>();
        for (var i = 1; i <= 5; i++)
        {
            var fileName = $"ReferencedAssembly{i}.dll";
            var filePath = Path.Combine(Path.GetTempPath(), fileName);
            var arg = (i % 2 == 0) ? ((ValueTuple<string, string?>)(fileName, filePath)) : ((ValueTuple<string, string?>)(filePath, null));
            var (fn, fp) = arg;
            var taskItem = GetTaskItem(fn, fp!, i % 2 == 0);
            taskItems.Add(taskItem);
            context.ExistingFiles.Add(filePath);
        }

        task.Execute().Should().BeTrue();
        task.Log.HasLoggedErrors.Should().BeFalse();
        task.BuildEngine.As<BuildEngineStub>().NumberOfWarningLogEvents.Should().Be(0, "No warning should be present");
    }

    [Fact]
    public void TestAssemblyFileReferencesHybridCheckFail()
    {
        var task = GetBuildTask(out var context);
        var assemblyFileName = "MyAssemblyFile.dll";
        var assemblyFilePath = Path.Combine(Path.GetTempPath(), assemblyFileName);
        task.SourceAssemblyFile = assemblyFilePath;
        context.ExistingFiles.Add(assemblyFilePath);

        var notFoundByRandom = new Random().Next(5) + 1;

        var taskItems = new List<ITaskItem>();
        for (var i = 1; i <= 5; i++)
        {
            var fileName = $"ReferencedAssembly{i}.dll";
            var filePath = Path.Combine(Path.GetTempPath(), fileName);
            var arg = (i % 2 == 0) ? ((ValueTuple<string, string?>)(fileName, filePath)) : ((ValueTuple<string, string?>)(filePath, null));
            var (fn, fp) = arg;
            var taskItem = GetTaskItem(fn, fp!, i % 2 == 0);
            taskItems.Add(taskItem);
            if (i != notFoundByRandom)
            {
                context.ExistingFiles.Add(filePath);
            }
        }

        task.Execute().Should().BeTrue();
        task.Log.HasLoggedErrors.Should().BeFalse();
        task.BuildEngine.As<BuildEngineStub>().Should().NotBe(0, "At least one warning should be present");
    }

    [Fact]
    public void TestBuildIsSuccessful()
    {
        var task = GetBuildTask(out var context);
        var assemblyFileName = "MyAssemblyFile.dll";
        var assemblyFilePath = Path.Combine(Path.GetTempPath(), assemblyFileName);
        task.SourceAssemblyFile = assemblyFilePath;
        context.ExistingFiles.Add(assemblyFilePath);

        context.ShouldSucceed = true;

        task.Execute().Should().BeTrue();
        task.Log.HasLoggedErrors.Should().BeFalse();
    }

    [Fact]
    public void TestBuildIsSuccessFail()
    {
        var task = GetBuildTask(out var context);
        var assemblyFileName = "MyAssemblyFile.dll";
        var assemblyFilePath = Path.Combine(Path.GetTempPath(), assemblyFileName);
        task.SourceAssemblyFile = assemblyFilePath;
        context.ExistingFiles.Add(assemblyFilePath);

        context.ShouldSucceed = false;

        task.Execute().Should().BeFalse();
        task.Log.HasLoggedErrors.Should().BeFalse();
    }
}

