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

using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

#pragma warning disable CA1861 

namespace dSPACE.Runtime.InteropServices;

public static class ConsoleApp
{
    /// <summary>
    /// Use a null character, which is an illegal character and therefore impossible
    /// to input via a command line arguments to make it distinct from nulls that
    /// gets passed when the option is specified but no argument is given.
    /// </summary>
    private const string NotSpecifiedViaCommandLineArgumentsDefault = "\0";

    public static int Main(string[] args)
    {
        var tlbexportCommand = new Command("tlbexport", "Export the assembly to the specified type library") {
                new Argument<string>("Assembly", "File name of assembly to parse"),
                new Option<string>(new[] {"--out", "/out"}, description: "File name of type library to be produced"),
                new Option<string[]>(new[] {"--tlbreference", "/tlbreference"}, description: "Type library used to resolve references", getDefaultValue: () =>  Array.Empty<string>()) { Arity =  ArgumentArity.ZeroOrMore},
                new Option<string[]>(new[] {"--tlbrefpath", "/tlbrefpath"}, description: "Path used to resolve referenced type libraries", getDefaultValue: () =>  Array.Empty<string>()) { Arity =  ArgumentArity.ZeroOrMore},
                new Option<string[]>(new[] {"--asmpath", "/asmpath"}, description: "Look for assembly references here", getDefaultValue: () =>  Array.Empty<string>()) { Arity =  ArgumentArity.ZeroOrMore},
                new Option<bool>(new[] {"--silent", "/silent"}, description: "Suppresses all output except for errors"),
                new Option<string[]>(new[] {"--silence", "/silence"}, description: "Suppresses output for the given warning (Can not be used with /silent)", getDefaultValue: () =>  Array.Empty<string>()) { Arity =  ArgumentArity.ZeroOrMore},
                new Option<bool>(new[] {"--verbose", "/verbose"}, description: "Detailed log output"),
                new Option<string[]>(new[] {"--names", "/names"}, description: "A file in which each line specifies the capitalization of a name in the type library.", getDefaultValue: () =>  Array.Empty<string>()) { Arity =  ArgumentArity.ZeroOrMore},
                new Option<string>(new[] { "--overridename", "/overridename"}, description: "Overwrites the library name"),
                new Option<Guid>(new[] {"--overridetlbid", "/overridetlbid"}, description: "Overwrites the library id"),
                new Option<bool?>(new[] {"--createmissingdependenttlbs", "/createmissingdependenttlbs"}, description: "Generate missing type libraries for referenced assemblies. (default true)"),
                new Option<string?>(new[] { "--embed", "/embed"}, () => NotSpecifiedViaCommandLineArgumentsDefault, description: "Embeds type library into the assembly. (default: false)") { Arity = ArgumentArity.ZeroOrOne },
                new Option<ushort>(new[] {"--index", "/index"}, () => 1, description: "If the switch --embed is specified, the index indicates the resource ID to be used for the embedded type library. Must be a number between 1 and 65535. Ignored if --embed not present. (default 1)")
            };

        var tlbdumpCommand = new Command("tlbdump", "Dump a type library")
            {
                new Argument<string>("TypeLibrary", "File name of type library"),
                new Option<string>(new[] {"--out", "/out"}, description: "File name of the output"),
                new Option<string[]>(new[] {"--tlbreference", "/tlbreference"}, description: "Type library used to resolve references", getDefaultValue: () =>  Array.Empty<string>()) { Arity =  ArgumentArity.ZeroOrMore},
                new Option<string[]>(new[] {"--tlbrefpath", "/tlbrefpath"}, description: "Path used to resolve referenced type libraries", getDefaultValue: () =>  Array.Empty<string>()) { Arity =  ArgumentArity.ZeroOrMore},
                new Option<string[]>(new[] {"--filterregex", "/filterregex"}, description: "Regex to filter the output", getDefaultValue: () =>  Array.Empty<string>()) { Arity =  ArgumentArity.ZeroOrMore},
            };

        var tlbregisterCommand = new Command("tlbregister", "Register a type library")
            {
                new Argument<string>("TypeLibrary", "File name of type library"),
                new Option<bool>(new[] {"--foruser", "/foruser"}, description: "Registered for use only by the calling user identity."),
            };

        var tlbunregisterCommand = new Command("tlbunregister", "Unregister a type library")
            {
                new Argument<string>("TypeLibrary", "File name of type library"),
                new Option<bool>(new[] {"--foruser", "/foruser"}, description: "Registered for use only by the calling user identity."),
            };

        var tlbembedCommand = new Command("tlbembed", "Embeds a source type library into a target file")
            {
                new Argument<string>("SourceTypeLibrary","File name of type library"),
                new Argument<string>("TargetAssembly", "File name of target assembly to receive the type library as a resource"),
                new Option<ushort>(new[] {"--index", "/index"}, () => 1, description:"Index to use for resource ID for the type library. If omitted, defaults to 1. Must be a positive integer from 1 to 65535.")
            };

        var rootCommand = new RootCommand
            {
                tlbexportCommand,
                tlbdumpCommand,
                tlbregisterCommand,
                tlbunregisterCommand,
                tlbembedCommand
            };

        rootCommand.Description = $"dSPACE COM tools ({(Environment.Is64BitProcess ? "64Bit" : "32Bit")})";

        ConfigureTLBExportHandler(tlbexportCommand);
        ConfigureTLBDumpHandler(tlbdumpCommand);
        ConfigureTLBRegisterHandler(tlbregisterCommand);
        ConfigureTLBUnRegisterHandler(tlbunregisterCommand);
        ConfigureTLBEmbedHandler(tlbembedCommand);

        return rootCommand.Invoke(args);
    }

    private static void ConfigureTLBUnRegisterHandler(Command tlbregisterCommand)
    {
        tlbregisterCommand.Handler = CommandHandler.Create<TlbUnRegisterOptions>((options) =>
        {
            try
            {
                UnRegisterTypeLib(options.TypeLibrary, options.ForUser);
            }
            catch (Exception e)
            {
                return HandleException(e, "Failed to unregister type library.");
            }

            return 0;
        });
    }

    private static void ConfigureTLBRegisterHandler(Command tlbregisterCommand)
    {
        tlbregisterCommand.Handler = CommandHandler.Create<TlbRegisterOptions>((options) =>
        {
            try
            {
                RegisterTypeLib(options.TypeLibrary, options.ForUser);
            }
            catch (Exception e)
            {
                return HandleException(e, "Failed to register type library.");
            }

            return 0;
        });
    }

    private static void ConfigureTLBDumpHandler(Command tlbdumpCommand)
    {
        tlbdumpCommand.Handler = CommandHandler.Create<TypeLibTextConverterSettings>((options) =>
        {
            try
            {
                if (string.IsNullOrEmpty(options.Out))
                {
                    var yamlfilename = Path.GetFileNameWithoutExtension(options.TypeLibrary) + ".yaml";
                    options.Out = Path.Combine(Directory.GetCurrentDirectory(), yamlfilename);
                }
                var typeLibConvert = new TypeLibConverter();
                typeLibConvert.ConvertTypeLibToText(options);
            }
            catch (Exception e)
            {
                return HandleException(e, "Failed to dump type library.");
            }

            return 0;
        });
    }

    private static void ConfigureTLBEmbedHandler(Command tlbembedCommand)
    {
        tlbembedCommand.Handler = CommandHandler.Create<TypeLibEmbedderSettings>((settings) => TypeLibEmbedder.EmbedTypeLib(settings));
    }

    private static void ConfigureTLBExportHandler(Command tlbexportCommand)
    {
        tlbexportCommand.Handler = CommandHandler.Create<TypeLibConverterOptions>((options) =>
        {
            try
            {
                // Use ; for path separation for compatibility to tlbexp.exe
                options.ASMPath = options.ASMPath.SelectMany(p => !string.IsNullOrEmpty(p) ? p.Split(";") : Array.Empty<string>()).ToArray();
                options.TLBReference = options.TLBReference.SelectMany(p => !string.IsNullOrEmpty(p) ? p.Split(";") : Array.Empty<string>()).ToArray();
                options.TLBRefpath = options.TLBRefpath.SelectMany(p => !string.IsNullOrEmpty(p) ? p.Split(";") : Array.Empty<string>()).ToArray();

                if (string.IsNullOrEmpty(options.Out))
                {
                    var tlbfilename = Path.GetFileNameWithoutExtension(options.Assembly) + ".tlb";
                    options.Out = Path.Combine(Directory.GetCurrentDirectory(), tlbfilename);
                }

                ExportTypeLibraryImpl(options, out var weakRef);

                if (options.Embed != NotSpecifiedViaCommandLineArgumentsDefault)
                {
                    for (var i = 0; weakRef.IsAlive && (i < 10); i++)
                    {
                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                    }

                    if (weakRef.IsAlive)
                    {
                        throw new ApplicationException("Unable to embed type library as the assembly is still locked by other processes.");
                    }

                    var assemblyPath = string.IsNullOrWhiteSpace(options.Embed) ? options.Assembly : options.Embed;
                    Console.WriteLine($"Embedding type library '{options.Out}' into assembly '{assemblyPath}'...");
                    var settings = new TypeLibEmbedderSettings
                    {
                        SourceTypeLibrary = options.Out,
                        TargetAssembly = assemblyPath,
                        Index = options.Index
                    };
                    TypeLibEmbedder.EmbedTypeLib(settings);
                }

                return 0;
            }
            catch (Exception e)
            {
                return HandleException(e, "Failed to export type library.");
            }
        });
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void ExportTypeLibraryImpl(TypeLibConverterOptions options, out WeakReference weakRef)
    {
        using var assemblyResolver = new AssemblyResolver(options);
        weakRef = new WeakReference(assemblyResolver, trackResurrection: true);
        if (!File.Exists(options.Assembly))
        {
            throw new FileNotFoundException($"File {options.Assembly} not found.");
        }

        var assembly = assemblyResolver.LoadAssembly(options.Assembly);
        var typeLibConverter = new TypeLibConverter();
        var nameResolver = options.Names.Length > 0 ? NameResolver.Create(options.Names) : NameResolver.Create(assembly);
        var typeLib = typeLibConverter.ConvertAssemblyToTypeLib(assembly, options, new TypeLibExporterNotifySink(options, nameResolver));

        if (typeLib is ICreateTypeLib2 createTypeLib2)
        {
            createTypeLib2.SaveAllChanges().ThrowIfFailed($"Failed to save type library {options.Out}.");
        }

        assemblyResolver.Dispose();
    }

    private static void RegisterTypeLib(string typeLibFilePath, bool forUser = false)
    {
        // Only windows is supported
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            throw new PlatformNotSupportedException();
        }

        // Use absolute path
        typeLibFilePath = Path.GetFullPath(typeLibFilePath);

        if (!File.Exists(typeLibFilePath))
        {
            throw new ArgumentException($"File {typeLibFilePath} not found");
        }

        var hresult = OleAut32.LoadTypeLibEx(typeLibFilePath, REGKIND.NONE, out var typeLib);
        hresult.ThrowIfFailed($"Failed to load typelib {typeLibFilePath}");

        hresult = forUser
            ? OleAut32.RegisterTypeLibForUser(typeLib, typeLibFilePath, Path.GetDirectoryName(typeLibFilePath)!)
            : OleAut32.RegisterTypeLib(typeLib, typeLibFilePath, Path.GetDirectoryName(typeLibFilePath)!);
        hresult.ThrowIfFailed($"Failed register {typeLibFilePath} (Please make sure you're running the application as administrator)");

        Console.WriteLine("Type library was registered successfully");
    }

    private static void UnRegisterTypeLib(string typeLibFilePath, bool forUser)
    {
        // Only windows is supported
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            throw new PlatformNotSupportedException();
        }

        // Use absolute path
        typeLibFilePath = Path.GetFullPath(typeLibFilePath);

        if (!File.Exists(typeLibFilePath))
        {
            throw new ArgumentException($"File {typeLibFilePath} not found");
        }

        ITypeLib? typeLib = null;
        var ppTLibAttr = IntPtr.Zero;
        try
        {
            var hresult = OleAut32.LoadTypeLibEx(typeLibFilePath, REGKIND.NONE, out typeLib);
            hresult.ThrowIfFailed($"Failed to load typelib {typeLibFilePath}");

            typeLib.GetLibAttr(out ppTLibAttr);
            var typeLibAttr = Marshal.PtrToStructure<TYPELIBATTR>(ppTLibAttr);
            hresult = forUser
                ? OleAut32.UnRegisterTypeLibForUser(typeLibAttr.guid, (ushort)typeLibAttr.wMajorVerNum, (ushort)typeLibAttr.wMinorVerNum, typeLibAttr.lcid, typeLibAttr.syskind)
                : OleAut32.UnRegisterTypeLib(typeLibAttr.guid, (ushort)typeLibAttr.wMajorVerNum, (ushort)typeLibAttr.wMinorVerNum, typeLibAttr.lcid, typeLibAttr.syskind);

            hresult.ThrowIfFailed($"Failed unregister {typeLibFilePath} (Please make sure you're running the application as administrator)");
        }
        finally
        {
            if (ppTLibAttr != IntPtr.Zero)
            {
                typeLib?.ReleaseTLibAttr(ppTLibAttr);
            }
        }
        Console.WriteLine($"Type library {typeLibFilePath} unregistered successfully");
    }

    private static int HandleException(Exception e, string errorText)
    {
        Console.Error.WriteLine($"{errorText} {e.Message} {e.InnerException?.Message}");
        if (Debugger.IsAttached)
        {
            throw e;
        }

        return 1;
    }
}
