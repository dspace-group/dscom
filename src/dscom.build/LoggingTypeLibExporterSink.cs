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
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace dSPACE.Runtime.InteropServices.BuildTasks;

/// <summary>
/// Implementation of the a <see cref="ITypeLibExporterNotifySink" />
/// forwarding all messages to the MsBuild log.
/// </summary>
internal sealed class LoggingTypeLibExporterSink : ITypeLibExporterNotifySink, ITypeLibExporterNameProvider
{
    /// <summary>
    /// The logging sink.
    /// </summary>
    private readonly TaskLoggingHelper _log;

    private readonly INameResolver _nameResolver;

    /// <summary>
    /// Creates a new instance of the <see cref="LoggingTypeLibExporterSink" />
    /// using the specified <paramref name="log" /> as logging target.
    /// </summary>
    /// <param name="log">The log to write to.</param>
    internal LoggingTypeLibExporterSink(TaskLoggingHelper log, INameResolver nameResolver)
    {
        _log = log;
        _nameResolver = nameResolver;
    }

    public INameResolver GetNameResolver()
    {
        return _nameResolver;
    }

    /// <inheritdoc cref="ITypeLibExporterNotifySink.ReportEvent" />
    void ITypeLibExporterNotifySink.ReportEvent(ExporterEventKind eventKind, int eventCode, string eventMsg)
    {
        var importance = eventKind switch
        {
            ExporterEventKind.NOTIF_TYPECONVERTED => MessageImportance.Low,
            ExporterEventKind.NOTIF_CONVERTWARNING => MessageImportance.Normal,
            ExporterEventKind.ERROR_REFTOINVALIDASSEMBLY => MessageImportance.High,
            _ => MessageImportance.High,
        };

        _log.LogMessage(importance, "Received {0} event. Event Code is {1}: {2}", Enum.GetName(typeof(ExporterEventKind), eventKind), eventCode, eventMsg);
    }

    /// <inheritdoc cref="ITypeLibExporterNotifySink.ResolveRef" />
    object? ITypeLibExporterNotifySink.ResolveRef(Assembly assembly)
    {
        return default;
    }
}
