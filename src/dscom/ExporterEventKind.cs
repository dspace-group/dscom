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
/// Describes the callbacks that the type library exporter makes when exporting a type library.
/// </summary>
public enum ExporterEventKind
{
    /// <summary>
    /// Specifies that the event is invoked when a type has been exported.
    /// </summary>
    NOTIF_TYPECONVERTED,
    /// <summary>
    /// Specifies that the event is invoked when a warning occurs during conversion.
    /// </summary>
    NOTIF_CONVERTWARNING,
    /// <summary>
    /// This value is not supported in this version of the .NET Framework.
    /// </summary>
    ERROR_REFTOINVALIDASSEMBLY
}
