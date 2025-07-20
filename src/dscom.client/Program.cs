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
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

#pragma warning disable CA1861 

namespace dSPACE.Runtime.InteropServices;

public static class ConsoleApp
{
    public static int Main(string[] args)
    {
        var tlbexportCommand = new Command("tlbexport", "Export the assembly to the specified type library") {
            new Argument<string>("Assembly"){ Description = "File name of assembly to parse"},
            new Option<string>("--out", "/out") { Description = "File name of type library to be produced"},
            new Option<string[]>("--tlbreference", "/tlbreference") { Description = "Type library used to resolve references", Arity = ArgumentArity.ZeroOrMore, DefaultValueFactory = _ => Array.Empty<string>() },
            new Option<string[]>("--tlbrefpath", "/tlbrefpath") { Description = "Path used to resolve referenced type libraries", DefaultValueFactory = _ => Array.Empty<string>(), Arity = ArgumentArity.ZeroOrMore },
            new Option<string[]>("--asmpath", "/asmpath") { Description = "Look for assembly references here", DefaultValueFactory = _ => Array.Empty<string>(), Arity = ArgumentArity.ZeroOrMore },
            new Option<bool>("--silent", "/silent") { Description = "Suppresses all output except for errors" },
            new Option<string[]>("--silence", "/silence") { Description = "Suppresses output for the given warning (Can not be used with /silent)", DefaultValueFactory = _ => Array.Empty<string>(), Arity = ArgumentArity.ZeroOrMore },
            new Option<bool>("--verbose", "/verbose") { Description = "Detailed log output" },
            new Option<string[]>("--names", "/names") { Description = "A file in which each line specifies the capitalization of a name in the type library.", DefaultValueFactory = _ => Array.Empty<string>(), Arity = ArgumentArity.ZeroOrMore },
            new Option<string>("--overridename", "/overridename") { Description = "Overwrites the library name" },
            new Option<Guid>("--overridetlbid", "/overridetlbid") { Description = "Overwrites the library id" },
            new Option<bool?>("--createmissingdependenttlbs", "/createmissingdependenttlbs") { Description = "Generate missing type libraries for referenced assemblies. (default true)" },
            new Option<string?>("--embed", "/embed") { DefaultValueFactory =_ =>  TypeLibConverterOptions.NotSpecifiedViaCommandLineArgumentsDefault, Description = "Embeds type library into the assembly. (default: false)", Arity = ArgumentArity.ZeroOrOne },
            new Option<ushort>("--index", "/index") { DefaultValueFactory = _ => 1, Description = "If the switch --embed is specified, the index indicates the resource ID to be used for the embedded type library. Must be a number between 1 and 65535. Ignored if --embed not present. (default 1)" }
        };

        var tlbdumpCommand = new Command("tlbdump", "Dump a type library")
        {
            new Argument<string>("TypeLibrary") {Description = "File name of type library" },
            new Option<string>("--out", "/out") { Description =  "File name of the output" },
            new Option<string[]>("--tlbreference", "/tlbreference") { Description = "Type library used to resolve references", DefaultValueFactory = _ => Array.Empty<string>(), Arity = ArgumentArity.ZeroOrMore },
            new Option<string[]>("--tlbrefpath", "/tlbrefpath") { Description = "Path used to resolve referenced type libraries", DefaultValueFactory = _ => Array.Empty<string>(), Arity = ArgumentArity.ZeroOrMore },
            new Option<string[]>("--filterregex", "/filterregex") { Description = "Regex to filter the output", DefaultValueFactory = _ => Array.Empty<string>(), Arity = ArgumentArity.ZeroOrMore },
        };

        var tlbregisterCommand = new Command("tlbregister", "Register a type library")
        {
            new Argument<string>("TypeLibrary") { Description = "File name of type library" },
            new Option<bool>("--foruser", "/foruser") { Description = "Registered for use only by the calling user identity."}
        };

        var tlbunregisterCommand = new Command("tlbunregister", "Unregister a type library")
        {
            new Argument<string>("TypeLibrary") {  Description = "File name of type library" },
            new Option<bool> ("--foruser", "/foruser") { Description ="Registered for use only by the calling user identity." },
        };

        var tlbembedCommand = new Command("tlbembed", "Embeds a source type library into a target file")
        {
            new Argument<string>("SourceTypeLibrary") { Description ="File name of type library" },
            new Argument<string>("TargetAssembly") { Description = "File name of target assembly to receive the type library as a resource" },
            new Option<ushort>("--index", "/index") { DefaultValueFactory = _ =>  1, Description = "Index to use for resource ID for the type library. If omitted, defaults to 1. Must be a positive integer from 1 to 65535." }
        };

        var registerAssemblyCommand = new Command("regasm", "Register an assembly")
        {
            new Argument<string>("TargetAssembly") { Description = "File name of target assembly to receive the type library as a resource" },
            new Option<string[]>("--asmpath", "/asmpath") { Description= "Look for assembly references here", DefaultValueFactory = _ => Array.Empty<string>(), Arity = ArgumentArity.ZeroOrMore},
            new Option<string[]>("--tlbrefpath", "/tblrefpath") { Description= "Look for type library references here", DefaultValueFactory = _ => Array.Empty<string>(), Arity = ArgumentArity.ZeroOrMore } ,
            new Option<bool>("--tlb", "/tlb"){ Description = "Will create and register a typelibrary for the given assembly" },
            new Option<bool>("--codebase", "/codebase") { Description = "Will register the assembly with codebase" },
            new Option<bool>("--unregister", "/unregister") { Description = "Will unregister the assembly" },
        };

        var rootCommand = new RootCommand
        {
            tlbexportCommand,
            tlbdumpCommand,
            tlbregisterCommand,
            tlbunregisterCommand,
            tlbembedCommand,
            registerAssemblyCommand
        };

        rootCommand.Description = $"dSPACE COM tools ({(Environment.Is64BitProcess ? "64Bit" : "32Bit")})";

        ConfigureTLBExportHandler(tlbexportCommand);
        ConfigureTLBDumpHandler(tlbdumpCommand);
        ConfigureTLBRegisterHandler(tlbregisterCommand);
        ConfigureTLBUnRegisterHandler(tlbunregisterCommand);
        ConfigureTLBEmbedHandler(tlbembedCommand);
        ConfigureRegisterAssemblyHandler(registerAssemblyCommand);

        return rootCommand.Parse(args).Invoke();
    }

    private static void ConfigureTLBUnRegisterHandler(Command tlbregisterCommand)
    {
        tlbregisterCommand.Action = CommandHandler.Create<TlbUnRegisterOptions>((options) =>
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
        tlbregisterCommand.Action = CommandHandler.Create<TlbRegisterOptions>((options) =>
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
        tlbdumpCommand.Action = CommandHandler.Create<TypeLibTextConverterSettings>((options) =>
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
        tlbembedCommand.Action = CommandHandler.Create<TypeLibEmbedderSettings>((settings) => TypeLibEmbedder.EmbedTypeLib(settings));
    }

    /// <summary>
    /// Will register an assembly like regasm.exe
    /// </summary>
    /// <param name="registerCommand"></param>
    /// <exception cref="FileNotFoundException"></exception>
    private static void ConfigureRegisterAssemblyHandler(Command registerCommand)
    {
        registerCommand.Action = CommandHandler.Create<RegisterAssemblySettings>(
            (options) =>
            {
                try
                {
                    foreach (var asmPath in options.ASMPath)
                    {
                        if (!Directory.Exists(asmPath))
                        {
                            throw new DirectoryNotFoundException($"Directory {asmPath} not found.");
                        }
                    }
                    var paths = options.ASMPath.Append(Path.GetDirectoryName(options.TargetAssembly)).ToArray();

                    using var assemblyResolver = new AssemblyResolver(paths!, false);

                    if (!File.Exists(options.TargetAssembly))
                    {
                        throw new FileNotFoundException($"File {options.TargetAssembly} not found.");
                    }

                    var assembly = assemblyResolver.LoadAssembly(options.TargetAssembly);

                    var registrationService = new RegistrationServices();

                    if (options.Unregister)
                    {
                        // Unregister the assembly
                        if (!registrationService.UnregisterAssembly(assembly))
                        {
                            return -1;
                        }
                    }
                    else
                    {
                        var lypeLibConvertOptions = new TypeLibConverterOptions()
                        {
                            ASMPath = paths!,

                            TLBRefpath = options.TLBRefpath,

                            Assembly = options.TargetAssembly,

                            Out = Path.ChangeExtension(options.TargetAssembly, ".tlb"),

                            CreateMissingDependentTLBs = false
                        };

                        if (options.TLB)
                        {
                            // Export TLB
                            ExportTypeLibraryImpl(assembly, lypeLibConvertOptions);

                            // Register TLB
                            RegisterTypeLib(lypeLibConvertOptions.Out);
                        }

                        if (!registrationService.RegisterAssembly(assembly, options.Codebase))
                        {
                            return -1;
                        }
                    }

                    return 0;
                }
                catch (Exception e)
                {
                    return HandleException(e, "Failed to register assembly.");
                }
            });
    }

    private static void ConfigureTLBExportHandler(Command tlbexportCommand)
    {
        tlbexportCommand.Action = CommandHandler.Create<TypeLibConverterOptions>((options) =>
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

                if (options.ShouldEmbed())
                {
                    ExportTypeLibraryAfterExport(options, weakRef);
                }

                _ = RemoveWeakReference(weakRef);

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
        var dir = Path.GetDirectoryName(options.Assembly);

        var asmPaths = options.ASMPath;
        foreach (var asmPath in asmPaths)
        {
            if (!Directory.Exists(asmPath))
            {
                throw new DirectoryNotFoundException($"Directory {asmPath} not found.");
            }
        }

        if (Directory.Exists(dir))
        {
            asmPaths = asmPaths.Prepend(dir).ToArray();
        }

        using var assemblyResolver = new AssemblyResolver(asmPaths, options.ShouldEmbed());
        weakRef = new WeakReference(assemblyResolver, trackResurrection: true);
        if (!File.Exists(options.Assembly))
        {
            throw new FileNotFoundException($"File {options.Assembly} not found.");
        }

        var assembly = assemblyResolver.LoadAssembly(options.Assembly);

        ExportTypeLibraryImpl(assembly, options);
    }

    private static void ExportTypeLibraryImpl(
        Assembly assembly,
        TypeLibConverterOptions options)
    {
        var typeLibConverter = new TypeLibConverter();
        var nameResolver = options.Names.Length > 0 ? NameResolver.Create(options.Names) : NameResolver.Create(assembly);
        var typeLib = typeLibConverter.ConvertAssemblyToTypeLib(assembly, options, new TypeLibExporterNotifySink(options, nameResolver));

        if (typeLib is ICreateTypeLib2 createTypeLib2)
        {
            createTypeLib2.SaveAllChanges().ThrowIfFailed($"Failed to save type library {options.Out}.");
        }
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

    private static void ExportTypeLibraryAfterExport(TypeLibConverterOptions options, WeakReference weakRef)
    {
        if (!RemoveWeakReference(weakRef))
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

    private static bool RemoveWeakReference(WeakReference weakRef)
    {
        for (var i = 0; weakRef.IsAlive && (i < 10); i++)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        return !weakRef.IsAlive;
    }
}
