// Copyright 2022 dSPACE GmbH, Carsten Igel and Contributors
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

using System.Reflection;
using System.Security;
using dSPACE.Runtime.InteropServices;
using Microsoft.Build.Utilities;

using COMException = System.Runtime.InteropServices.COMException;

namespace dSPACE.Build.Tasks.dscom;

internal sealed class DefaultBuildContext : IBuildContext
{
    public bool ConvertAssemblyToTypeLib(TypeLibConverterSettings settings, TaskLoggingHelper log)
    {
        Assembly assembly;
        try
        {
            assembly = Assembly.LoadFrom(settings.Assembly);
        }
        catch (Exception e) when
            (e is ArgumentNullException
               or FileNotFoundException
               or FileLoadException
               or BadImageFormatException
               or SecurityException
               or ArgumentException
               or PathTooLongException)
        {
            log.LogErrorFromException(e, true, true, settings.Assembly);
            return false;
        }

        try
        {
            var converter = new TypeLibConverter();

            var sink = new LoggingTypeLibExporterSink(log);
            var tlb = converter.ConvertAssemblyToTypeLib(assembly, settings, sink);
            if (tlb == null)
            {
                log.LogError("The following type library could not be created successfully: {0}. Reason: Operation was not successful.", settings.Out);
            }

            return tlb != null;
        }
        catch (COMException e)
        {
            log.LogErrorFromException(e, false, true, settings.Assembly);
            return false;
        }
    }
}
