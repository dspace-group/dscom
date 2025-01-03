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

namespace dSPACE.Runtime.InteropServices;

/// <summary>
///  Represents the settings used for registration of an assembly (like RegAsm.exe)
/// </summary>
public class RegisterAssemblySettings
{
    /// <summary>
    /// Gets or sets target assembly path.
    /// </summary>
    public string TargetAssembly { get; set; } = string.Empty;

    /// <summary>
    /// Specifies a directory containing assembly references.
    /// </summary>
    public string[] ASMPath { get; set; } = Array.Empty<string>();

    /// <summary>
    /// Gets or sets a path to a directory with type libraries.
    /// </summary>
    public string[] TLBRefpath { get; set; } = Array.Empty<string>();

    /// <summary>
    /// Get or sets the flag for creating a type library
    /// </summary>
    public bool TLB { get; set; }

    /// <summary>
    /// Gets or sets the flag for creating the codebase entry in the registry.
    /// </summary>
    public bool Codebase { get; set; }

    /// <summary>
    /// Gets or sets the flag for deregistration of the assembly
    /// </summary>
    public bool Unregister { get; set; }

}
