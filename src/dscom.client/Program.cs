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
using System.CommandLine.Parsing;
using System.Reflection;
using System.Runtime.InteropServices;
using dSPACE.Runtime.InteropServices.ComTypes;

namespace dSPACE.Runtime.InteropServices;

public static class ConsoleApp
{
    public static int Main(string[] args)
    {
        var tlbexportCommand = new Command("tlbexport", "Export the assembly to the specified type library") {
                new Argument("Assembly", "File name of assembly to parse"),
                new Option<string>(new string [] {"--out", "/out"}, description: "File name of type library to be produced"),
                new Option(new string [] {"--tlbreference", "/tlbreference"}, description: "Type library used to resolve references", typeof(string[]), null, ArgumentArity.ZeroOrMore),
                new Option(new string [] {"--tlbrefpath", "/tlbrefpath"}, description: "Path used to resolve referenced type libraries", typeof(string[]), null, ArgumentArity.ZeroOrMore),
                new Option(new string [] {"--asmpath", "/asmpath"}, description: "Look for assembly references here", typeof(string[]), null, ArgumentArity.ZeroOrMore),
                new Option<bool>(new string [] {"--silent", "/silent"}, description: "Suppresses all output except for errors"),
                new Option(new string [] {"--silence", "/silence"}, description: "Suppresses output for the given warning (Can not be used with /silent)", typeof(string[]), null, ArgumentArity.ZeroOrMore),
                new Option<bool>(new string [] {"--verbose", "/verbose"}, description: "Detailed log output"),
                new Option(new string [] {"--names", "/names"}, description: "A file in which each line specifies the capitalization of a name in the type library.", typeof(string[]), null, ArgumentArity.ZeroOrMore),
                new Option<Guid>(new string [] {"--overridetlbid", "/overridetlbid"}, description: "Overwrites the library id"),
            };

        var tlbdumpCommand = new Command("tlbdump", "Dump a type library")
            {
                new Argument("TypeLibrary", "File name of type library"),
                new Option<string>(new string [] {"--out", "/out"}, description: "File name of the output"),
                new Option<string>(new string [] {"--type", "/type"}, description: "Output type (only \"yaml\" is supported)"),
                new Option(new string [] {"--tlbreference", "/tlbreference"}, description: "Type library used to resolve references", typeof(string[]), null, ArgumentArity.ZeroOrMore),
                new Option(new string [] {"--tlbrefpath", "/tlbrefpath"}, description: "Path used to resolve referenced type libraries", typeof(string[]), null, ArgumentArity.ZeroOrMore),
                new Option(new string [] {"--filterregex", "/filterregex"}, description: "Regex to filter the output", typeof(string[]), null, ArgumentArity.ZeroOrMore),
            };

        var tlbregisterCommand = new Command("tlbregister", "Register a type library")
            {
                new Argument("TypeLibrary", "File name of type library"),
                new Option<bool>(new string [] {"--foruser", "/foruser"}, description: "Registered for use only by the calling user identity."),

            };

        var tlbunregisterCommand = new Command("tlbunregister", "Unregister a type library")
            {
                new Argument("TypeLibrary", "File name of type library"),
                new Option<bool>(new string [] {"--foruser", "/foruser"}, description: "Registered for use only by the calling user identity."),
            };

        var rootCommand = new RootCommand
            {
                tlbexportCommand,
                tlbdumpCommand,
                tlbregisterCommand,
                tlbunregisterCommand
            };

        rootCommand.Description = "dSPACE COM tools";

        ConfigureTLBExportHandler(tlbexportCommand);
        ConfigureTLBDumpHandler(tlbdumpCommand);
        ConfigureTLBRegisterHandler(tlbregisterCommand);
        ConfigureTLBUnRegisterHandler(tlbunregisterCommand);

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
#if DEBUG
            catch
            {
                throw;
            }
#else
            catch (Exception e)
            {
                Console.Error.WriteLine($"Failed to register type library. {e.Message}");
                return 1;
            }
#endif
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
#if DEBUG
            catch
            {
                throw;
            }
#else
            catch (Exception e)
            {
                Console.Error.WriteLine($"Failed to register type library. {e.Message}");
                return 1;
            }
#endif
            return 0;
        });
    }

    private static void ConfigureTLBDumpHandler(Command tlbdumpCommand)
    {
        tlbdumpCommand.Handler = CommandHandler.Create<TypeLibExporterOptions>((options) =>
        {
            try
            {
                switch (options.OutPutType)
                {
                    case "yaml":
                        TypeLibExporter.ExportToYaml(options);
                        break;
                    default:
                        throw new ArgumentException($"Output type {options.OutPutType} not supported");
                }
            }
#if DEBUG
            catch
            {
                throw;
            }
#else
            catch (Exception e)
            {
                Console.Error.WriteLine($"Failed to dump type library. {e.Message} {e.InnerException?.Message}");
                return 1;
            }
#endif

            return 0;
        });
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

                using var assemblyResolver = new AssemblyResolver(options);

                var assembly = Assembly.LoadFrom(options.Assembly);
                var typeLibConverter = new TypeLibConverter();
                var typeLib = typeLibConverter.ConvertAssemblyToTypeLib(assembly, options, new TypeLibExporterNotifySink(options));

                if (typeLib is ICreateTypeLib2 createTypeLib2)
                {
                    createTypeLib2.SaveAllChanges().ThrowIfFailed($"Failed to save type library {options.Out}.");
                }

                return 0;
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Message);
                if (e.InnerException != null)
                {
                    Console.Error.WriteLine($"Exception caught: {e.InnerException.Message}");
                }

                return 1;
            }

        });

        tlbexportCommand.AddValidator(v =>
        {
            return string.Empty;
        });
    }

    private static void RegisterTypeLib(string typeLibFilePath, bool forUser = false)
    {
        // Only windows is supported
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            throw new PlatformNotSupportedException();
        }

        if (!File.Exists(typeLibFilePath))
        {
            throw new ArgumentException($"File {typeLibFilePath} not found");
        }

        var hresult = OleAut32.LoadTypeLibEx(typeLibFilePath, REGKIND.NONE, out var typeLib);
        hresult.ThrowIfFailed($"Failed to load typelib {typeLibFilePath} {hresult}");

        hresult = forUser
            ? OleAut32.RegisterTypeLibForUser(typeLib, typeLibFilePath, Path.GetDirectoryName(typeLibFilePath)!)
            : OleAut32.RegisterTypeLib(typeLib, typeLibFilePath, Path.GetDirectoryName(typeLibFilePath)!);
        hresult.ThrowIfFailed($"Failed registered type library {typeLibFilePath} {hresult}");

        Console.WriteLine("Type library was registered successfully");
    }

    private static void UnRegisterTypeLib(string typeLibFilePath, bool forUser)
    {
        // Only windows is supported
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            throw new PlatformNotSupportedException();
        }

        ITypeLib? typeLib = null;
        var ppTLibAttr = IntPtr.Zero;
        try
        {
            var hresult = OleAut32.LoadTypeLibEx(typeLibFilePath, REGKIND.NONE, out typeLib);
            hresult.ThrowIfFailed($"Failed to load typelib {typeLibFilePath} {hresult}");

            typeLib.GetLibAttr(out ppTLibAttr);
            var typeLibAttr = Marshal.PtrToStructure<TYPELIBATTR>(ppTLibAttr);
            hresult = forUser
                ? OleAut32.UnRegisterTypeLibForUser(typeLibAttr.guid, (ushort)typeLibAttr.wMajorVerNum, (ushort)typeLibAttr.wMinorVerNum, typeLibAttr.lcid, typeLibAttr.syskind)
                : OleAut32.UnRegisterTypeLib(typeLibAttr.guid, (ushort)typeLibAttr.wMajorVerNum, (ushort)typeLibAttr.wMinorVerNum, typeLibAttr.lcid, typeLibAttr.syskind);

            hresult.ThrowIfFailed($"Failed unregister type library {typeLibFilePath} {hresult}");
        }
        finally
        {
            if (ppTLibAttr != IntPtr.Zero)
            {
                if (typeLib != null)
                {
                    typeLib.ReleaseTLibAttr(ppTLibAttr);
                }
            }
        }
        Console.WriteLine($"Type library {typeLibFilePath} unregistered successfully");
    }
}
