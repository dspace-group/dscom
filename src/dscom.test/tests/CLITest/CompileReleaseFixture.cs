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

public class CompileReleaseFixture
{
    public string Workdir { get; private set; } = string.Empty;

    public string DSComPath { get; private set; } = string.Empty;

    public string DemoProjectAssembly1Path { get; private set; } = string.Empty;

    public string DemoProjectAssembly2Path { get; private set; } = string.Empty;

    public string DemoProjectAssembly3Path { get; private set; } = string.Empty;

    public CompileReleaseFixture()
    {
        var workdir = new DirectoryInfo(Environment.CurrentDirectory).Parent?.Parent?.Parent?.Parent?.Parent;
        if (workdir == null || !workdir.Exists)
        {
            throw new DirectoryNotFoundException("Workdir not found.");
        }

        Workdir = workdir.FullName;

#if DEBUG
        var configuration = "Debug";
#else
        var configuration = "Release";
#endif

#if NET8_0_OR_GREATER
        var frameworkVersion = "net8.0";
#elif NET6_0_OR_GREATER
        var frameworkVersion = "net6.0";
#else
        var frameworkVersion = "net48";
#endif

        // Path to descom.exe
        DSComPath = Path.Combine(Workdir, "src", "dscom.client", "bin", configuration, "net6.0", "dscom.exe");

        // Path to dscom.demo assemblies
        DemoProjectAssembly1Path = Path.Combine(Workdir, "src", "dscom.demo", "assembly1", "bin", configuration, frameworkVersion, "dSPACE.Runtime.InteropServices.DemoAssembly1.dll");
        DemoProjectAssembly2Path = Path.Combine(Workdir, "src", "dscom.demo", "assembly2", "bin", configuration, frameworkVersion, "dSPACE.Runtime.InteropServices.DemoAssembly2.dll");
        DemoProjectAssembly3Path = Path.Combine(Workdir, "src", "dscom.demo", "assembly3", "bin", configuration, frameworkVersion, "dSPACE.Runtime.InteropServices.DemoAssembly3.dll");
    }
}
