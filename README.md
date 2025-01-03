# dSPACE COM tools

[![Nuget:Cli](https://img.shields.io/nuget/v/dscom?label=dscom&style=flat)](https://www.nuget.org/packages/dscom/)  
[![Nuget:Lib](https://img.shields.io/nuget/v/dSPACE.Runtime.InteropServices?label=dSPACE.Runtime.InteropServices&style=flat)](https://www.nuget.org/packages/dSPACE.Runtime.InteropServices/)  
[![Nuget:LibBuildTask](https://img.shields.io/nuget/v/dSPACE.Runtime.InteropServices?label=dSPACE.Runtime.InteropServices.BuildTasks&style=flat)](https://www.nuget.org/packages/dSPACE.Runtime.InteropServices.BuildTasks/)  

[![Release](https://img.shields.io/github/v/release/dspace-group/dscom?label=release)](https://github.com/dspace-group/dscom/releases)
![License](https://img.shields.io/github/license/dspace-group/dscom)
[![dSPACE](https://img.shields.io/badge/-OpenSource%20powered%20by%20dSPACE-blue)](https://www.dspace.com/)

[![Unit Tests](https://github.com/dspace-group/dscom/actions/workflows/unit-test.yaml/badge.svg)](https://github.com/dspace-group/dscom/actions/workflows/unit-test.yaml)
[![Example Tests](https://github.com/dspace-group/dscom/actions/workflows/example-test.yaml/badge.svg)](https://github.com/dspace-group/dscom/actions/workflows/example-test.yaml)
[![Code Style Check](https://github.com/dspace-group/dscom/actions/workflows/code-style.yaml/badge.svg)](https://github.com/dspace-group/dscom/actions/workflows/code-style.yaml)

The command line client `dscom` is a replacement for `tlbexp.exe` and `RegAsm.exe`.  
Among other things, you can create or register TLBs from .NET assemblies.  

The `dSPACE.Runtime.InteropServices` library contains various classes and methods for COM.  
It can be used in `net5+` or in `net48` projects.  
With the library you can register assemblies and classes for COM and programmatically generate TLBs at runtime.  

The `dSPACE.Runtime.InteropServices.BuildTasks` library provides build tasks which can be used to automatically generate TLBs at compile time.

- [dSPACE COM tools](#dspace-com-tools)
  - [Introducing](#introducing)
  - [Command Line Client](#command-line-client)
    - [Installation](#installation)
    - [Usage](#usage)
  - [Library](#library)
    - [TypeLibConverter.ConvertAssemblyToTypeLib](#typelibconverterconvertassemblytotypelib)
    - [RegistrationServices.RegisterTypeForComClients](#registrationservicesregistertypeforcomclients)
    - [RegistrationServices.RegisterAssembly](#registrationservicesregisterassembly)
  - [Build Tasks](#build-tasks)
    - [Build task usage](#build-task-usage)
      - [Using the native build task](#using-the-native-build-task)
      - [Using the CLI based task](#using-the-cli-based-task)
      - [Enforcing the usage of the CLI](#enforcing-the-usage-of-the-cli)
      - [Enforcing to stop the build, if an error occurs](#enforcing-to-stop-the-build-if-an-error-occurs)
    - [Parameters](#parameters)
    - [Example](#example)
    - [Hints](#hints)
  - [32Bit support](#32bit-support)
  - [Migration notes (mscorelib vs System.Private.CoreLib)](#migration-notes-mscorelib-vs-systemprivatecorelib)
    - [Why can I load a .NET Framework library into a .NET application?](#why-can-i-load-a-net-framework-library-into-a-net-application)
  - [Limitations](#limitations)
    - [RegisterAssembly](#registerassembly)

## Introduction

Fortunately, .NET still supports COM, but there is no support for generating TLBs.  
From the Microsoft documentation:

> Unlike in .NET Framework, there is no support in .NET Core or .NET 5+ for generating a COM Type Library (TLB) from a .NET assembly.

<https://docs.microsoft.com/en-us/dotnet/core/native-interop/expose-components-to-com>

One main goal is to make `dscom` behave like `tlbexp.exe`.

Also, some classes are missing in .NET 5+ that were available in the full framework.
This is where `dSPACE.Runtime.InteropServices` may be able to help.

## Command Line Client

The command-line interface (CLI) tool `dscom` is a replacement for `tlbexp.exe`, `OleView` (View TypeLib) and `RegAsm.exe`.

It supports the following features:

- Convert an assembly to a type library
  - Optionally embed the generated type library into the converted assembly
- Convert a type library to `YAML` file
- Register a type library
- Unregister a type library
- Embeds a type library into an existing assembly
- Register an assembly (regasm.exe)
- Unregister an assembly (regasm.exe)

### Installation

The installation is quite simple. You can use `dotnet tool` to install the `dscom` binary if you want to create a 64Bit TLB.

```bash
dotnet tool install --global dscom
```

Here you can find all available versions:  
<https://www.nuget.org/packages/dscom/>

Alternatively you can download dscom.exe from the relase page.  
<https://github.com/dspace-group/dscom/releases>

### Usage

Use `dscom --help` to get further information.  

```bash
c:\> dscom --help
Description:
  dSPACE COM tools

Usage:
  dscom [command] [options]

Options:
  --version       Show version information
  -?, -h, --help  Show help and usage information

Commands:
  tlbexport <Assembly>                           Export the assembly to the specified type library
  tlbdump <TypeLibrary>                          Dump a type library
  tlbregister <TypeLibrary>                      Register a type library
  tlbunregister <TypeLibrary>                    Unregister a type library
  tlbembed <SourceTypeLibrary> <TargetAssembly>  Embeds a source type library into a target file
  regasm <TargetAssembly>                        Register an assembly
```

## Library

Usage:  

```bash
dotnet add package dSPACE.Runtime.InteropServices
```

dSPACE.Runtime.InteropServices supports the following methods and classes:  

- TypeLibConverter
  - ConvertAssemblyToTypeLib
- TypeLibEmbedder
  - EmbedTypeLib
- RegistrationServices
  - RegisterTypeForComClients
  - UnregisterTypeForComClients
  - RegisterAssembly
  - UnregisterAssembly

### TypeLibConverter.ConvertAssemblyToTypeLib

If you miss the `TypeLibConverter` class and the `ConvertAssemblyToTypeLib` method in `.NET`, then the `dSPACE.Runtime.InteropServices` might help you.
This method should behave compatible to the `.NET Framework` method.

```csharp
public object? ConvertAssemblyToTypeLib(
  Assembly assembly,
  string tlbFilePath,
  ITypeLibExporterNotifySink? notifySink)
```

<https://www.nuget.org/packages/dSPACE.Runtime.InteropServices/>

Example:

```csharp
using dSPACE.Runtime.InteropServices;

// The assembly to convert
var assembly = typeof(Program).Assembly;

// Convert to assembly
var typeLibConverter = new TypeLibConverter();
var callback = new TypeLibConverterCallback();
var result = typeLibConverter.ConvertAssemblyToTypeLib(assembly, "MyTypeLib.tlb", callback);

// Get the name of the type library
var typeLib2 = result as System.Runtime.InteropServices.ComTypes.ITypeLib2;
if (typeLib2 != null)
{
    typeLib2.GetDocumentation(-1, out string name, out _, out _, out _);
    Console.WriteLine($"TypeLib name: {name}");
}

// The callback to load additional type libraries, if necessary
public class TypeLibConverterCallback : ITypeLibExporterNotifySink
{
    public void ReportEvent(ExporterEventKind eventKind, int eventCode, string eventMsg)
    {
        Console.WriteLine($"{eventCode}: {eventMsg}");
    }

    public object? ResolveRef(System.Reflection.Assembly assembly)
    {
        // Returns additional type libraries
        return null;
    }
}
```

### TypeLibEmbedder.EmbedTypeLib

.NET +6 introduced ability to embed type library into assemblies with the ComHostTypeLibrary property. However, using this is not fully compatible with the dscom build tools as it requires a type library to be already generated prior to the build. This class provides the implementation for embedding a type library into an assembly via Win32 API p/invoke calls. 

The class and method are static, so you only need to create a settings to provide parameter for the source type library and the target assembly for where the type library will be embedded.

It is important to note that type libraries are _not_ bit-agnostic and therefore, it will not make sense to embed them in an AnyCPU assemblies. For .NET 5.0 and greater, that is not an issue since the generated *.comhost.dll are tied to a specific bitness. For .NET 4.8, it is strongly recommended that the assembly be built with either x64 or x86 rather than AnyCPU. 

```csharp
public static bool EmbedTypeLib(
    TypeLibEmbedderSettings settings
)
```

Example:

```csharp
using dSPACE.Runtime.InteropServices;

var settings = new TypeLibEmbedderSettings
{
    SourceTlbPath = "C:\\path\\to\\type\\library.tlb",
    TargetAssembly = "C:\\path\\to\\assembly.dll"
};
TypeLibEmbedder.EmbedTypeLib(settings);
```

IMPORTANT: Embedding the type library will alter the assembly, which may cause issues with signing the assembly. Therefore, the scenario of signing the assembly with a certificate or a strong name is not tested. If it is required that the assembly be signed, it is recommended that a build script be used to ensure proper sequence of steps is executed.

IMPORTANT: In order to embed the type library into the built assembly, the process must unload the assembly. As dotnet restricts the usage of unloadable assemblies via AssemblyLoadContext to pure CLR assemblies, embedding TLBs into an assembly may only work, if none of the assemblies loaded via `--asmpath` may be a mixed mode C++/CLI assembly. In this case, please use to different calls to `dscom.exe` / `dscom32.exe` or the separate dscom build tasks. For more information refer to issue [#292](https://github.com/dspace-group/dscom/issues/292).

### RegistrationServices.RegisterTypeForComClients

The `dSPACE.Runtime.InteropServices.RegistrationServices` provides a set of services for registering and unregistering managed assemblies for use from COM.

This method is equivalent to calling CoRegisterClassObject in COM.  
You can register a .NET class so that other applications can connect to it (For example as INPROC_SERVER or as a LOCAL_SERVER).  

A outproc demo application is available here: [examples\outproc](https://github.com/dspace-group/dscom/tree/main/examples/outproc)

Example:  

```csharp
using dSPACE.Runtime.InteropServices;

var registration = new RegistrationServices();
var cookie = registration.RegisterTypeForComClients(typeof(Server.Common.Greeter), 
  RegistrationClassContext.LocalServer, 
  RegistrationConnectionType.MultipleUse);

Console.WriteLine($"Press enter to stop the server");
Console.ReadLine();

registration.UnregisterTypeForComClients(cookie);
```

### RegistrationServices.RegisterAssembly

Registers the classes in a managed assembly to enable creation from COM.  

```csharp
using dSPACE.Runtime.InteropServices;

var registration = new RegistrationServices();

// Register MyAssembly
registration.RegisterAssembly(typeof(MyAssembly), true);

// Unregister MyAssembly
registration.UnregisterAssembly(typeof(MyAssembly), true);
```

## Build Tasks

The `dSPACE.Runtime.InteropServices.BuildTasks` assembly and NuGet package provide the ability to create type libraries for a certain assembly at compile time.

### Build task usage

To create a type library at compile time, simply add a reference to the nuget package, e.g. by using the command line.

```shell
dotnet add package dSPACE.Runtime.InteropServices.BuildTasks
```

The result should be a line as follows in your `.csproj` file:

```xml
    <PackageReference Include="dSPACE.Runtime.InteropServices.BuildTasks" Version="0.17.0" NoWarn="NU1701">
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
      <PrivateAssets>all</PrivateAssets>
    </PackageReference>
```

**Note**: The extra attribute `NoWarn="NU1701"` is only required, if neither `.NET 4.8` nor `.NET 6.0` are targeted, since dotnet pack will currently not create a .NETStandard 2.0 compliant NuGet Package.

#### Enforcing to stop the build, if an error occurs

The build tasks puts a warning to the build log, if the desired type library has not been created, even if the backend has reported a success.

This warning is issued with the warning code `DSCOM001`, which can be collected in the `WarningsAsErrors` array:

```XML
<WarningsAsErrors>$(WarningsAsErrors);DSCOM001</WarningsAsErrors>
```

This way the build stops, if the type library is not exported.

### Parameters

The build task can be parameterized with the following [properties](https://learn.microsoft.com/en-us/visualstudio/msbuild/msbuild-properties?view=vs-2022):

| **Name**                                       | **Description**                                                                                |
| ---------------------------------------------- | ---------------------------------------------------------------------------------------------- |
| _DsComTlbExt                                   | Extension of the resulting type library. <br /> Default Value: `.tlb`                          |
| DsComTypeLibraryUniqueId                       | Overwrite the library UUID <br/> Default Value: Empty Guid                                     |
| DsComOverideLibraryName                        | Overwrite the IDL name of the library. <br/> Default Value: Empty string                       | 
| DsComRegisterTypeLibrariesAfterBuild           | Use regasm call after the build to register type library after the build <br/> Default value: `false` |
| DsComTlbExportAutoAddReferences                | Add referenced assemblies automatically to type libraries <br/> Default value: `true`          |
| DsComTlbExportIncludeReferencesWithoutHintPath | If a `Reference` assembly does not provide a `HintPath` Metadata, the item spec shall be task. <br/> Default value: `false` |
| DsComExportTypeLibraryTargetFile               | Path to the resulting file. <br/> Default value: `$(TargetDir)\$(TargetName)$(_DsComTlbExt)` * |
| DsComExportTypeLibraryAssemblyFile             | Path to the source assembly file. <br/> Default value: `$(TargetPath)` *                       |
| DsComTypeLibraryEmbedAfterBuild                | Embeds the generated type library into the source assembly file. <br /> Default value: `false` |

The build task consumes the following [items](https://learn.microsoft.com/en-us/visualstudio/msbuild/msbuild-items?view=vs-2022):

| **Name**                     | **Description**                                          |
| ---------------------------- | -------------------------------------------------------- |
| DsComTlbExportTlbReferences  | Referenced type library files.                           |
| DsComTlbExportReferencePaths | Directories containing type libraries to use for export. |
| DsComTlbExportAssemblyPaths  | Assemblies to add for the export.                        |
| DsComTlbAliasNames           | Names to use as aliases for types with aliases.          |

The CLI based task consumes these additional [properties](https://learn.microsoft.com/en-us/visualstudio/msbuild/msbuild-properties?view=vs-2022):

| **Name**                     | **Description**                                          |
| ---------------------------- | -------------------------------------------------------- |
| DsComToolVerbose             | Enables `verbose` option.                                |
| DsComToolSilent              | Enables `silent` option.                                 |

The CLI based task consumes these additional [items](https://learn.microsoft.com/en-us/visualstudio/msbuild/msbuild-items?view=vs-2022):

| **Name**                     | **Description**                                          |
| ---------------------------- | -------------------------------------------------------- |
| DsComToolSilence             | Enables `silence` option for the given warnings          |

### Example

```xml
<Project Sdk="Microsoft.NET.Sdk">
   <PropertyGroup>
      <DsComTypeLibraryUniqueId>f74a0889-2959-4e7b-9a1b-f41c54f31d74</DsComTypeLibraryUniqueId>
   </PropertyGroup>
   <ItemGroup>
    <DsComTlbExportTlbReferences Include="C:\Path\To\My.tlb" />
    <DsComTlbExportAssemblyPaths Include="C:\Path\To\Assemblies\Dependency.dll" />
    <DsComTlbExportReferencePaths Include="C:\Path\To\Additional\TypeLibraries\" />
   </ItemGroup>
</Project>
```

### Hints

When using the build tasks to create an 32 bit version for your type libraries / projects: The tasks require the .NET runtime in their specific CPU architecture to be installed.

So, for building 32 bit TLBs, dscom32 will be executed, which **requires** .NET runtime in x86 flavor to be available.

## 32Bit support

`dscom` installed by `dotnet tool install` can only handle AnyCPU or 64Bit assemblies and can only generate a 64bit TLB.
Depending on whether you want to process 32bit or 64bit assemblies, you need to download different executables from the [release page](https://github.com/dspace-group/dscom/releases).

- **dscom.exe** to create a 64Bit TLB from a AnyCPU or a 64Bit assembly
- **dscom32.exe** to create a 32Bit TLB from a AnyCPU or a 32Bit assembly

> Warning!  
> If your assembly is an AnyCPU assembly, then an yourassemblyname.comhost.dll is created as a 64 bit dll.  
> Therefore after calling regserv32.exe a 64 bit dll is registred.  
> To prevent this it is **recommended that the assembly is compiled as a 32 bit assembly** and not as an AnyCPU assembly.  
> see: <https://github.com/dotnet/runtime/issues/32493>

> Warning!
> In order to use 32 bit support with the build targets, the x86 version of .NET must be installed, as dscom requests the hostfxr.dll to be loaded.

## Migration notes (mscorelib vs System.Private.CoreLib)

Both assemblies are **ComVisible=false** but lot of .NET Framework types are **ComVisible=true**.
But this is not the case for .NET (.NET Core and .NET >= 5).  
Unlike mscorelib (the good old .NET Framework), no tlb is shipped for .NET.

As example the `System.Exception` class:

In case of mscorelib the `System.Exception` class is **ComVisible=true**:

```csharp
[Serializable]
[ClassInterface(ClassInterfaceType.None)]
[ComDefaultInterface(typeof(_Exception))]
[ComVisible(true)]
public class Exception : ISerializable, _Exception
```

In case of `System.Private.CoreLib` (.NET Core and .NET >=5), the `Exception` class is **ComVisible=false**

```csharp
[Serializable]
[TypeForwardedFrom("mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
public class Exception : ISerializable
{
```

The `_Exception` class interface (default interface in this case) is not available in .NET (.NET Core and .NET >=5).

### Why can I load a .NET Framework library into a .NET application?

The magic is in `TypeForwardedFromAttribute`.  
If you try to load an .NET Framework assembly inside a .NET (.NET Core and .NET >=5) application, the runtime will forward the original type to
a type defined in the `System.Private.CoreLib` assembly.

```il
classextern forwarder System.Exception
{
    .assemblyextern System.Private.CoreLib
}
```

**Therefore you should make sure that you do not use any types from the `mscorelib` typelib in your .NET Framework project if you plan to migrate to .NET 5+**

## Limitations

- No imports of the `mscorelib` typelib (all types are **VT_UNKNOWN**)
  - \_Object not supported
  - \_EventArgs not supported
  - \_Delegate not supported
  - \_Type not supported
  - System.Runtime.Serialization.ISerializable not supported
  - System.ICloneable not supported
  - System.IDisposable not supported
  - System.Array not supported
  - ...
- `TypeLibExporterFlags` is not supported
- `ITypeLibExporterNotifySink` is not COM visible
- `TypeLibConverter` is not COM visible
- `AutoDual` is not supported
  - see: <https://docs.microsoft.com/en-us/previous-versions/visualstudio/visual-studio-2015/code-quality/ca1408-do-not-use-autodual-classinterfacetype?view=vs-2015&redirectedfrom=MSDN>
- LCID only NEUTRAL is supported
- No GAC support
  - see: <https://docs.microsoft.com/en-us/dotnet/core/compatibility/core-libraries/5.0/global-assembly-cache-apis-obsolete>
- IEnumerator is converted to `IEnumVARIANT` (stdole)
- Guid is converted to `GUID` (stdole)
- Color is converted to `OLE_COLOR`(stdole)
- No support for `UnmanagedType.CustomMarshaler`
- No support for .NET Framework assemblies with `AssemblyMetadataAttribute` value ".NETFrameworkAssembly"
- Using DsComTypeLibraryEmbedAfterBuild=true in combination with the Default ComHost due to a cyclic dependency in .NET SDK [#286](https://github.com/dspace-group/dscom/issues/286).

### RegisterAssembly

- InProc Registration only will take place, if a comhost assembly is present.
- No CustomRegisterFunction Or CustomUnRegisterFunction Attribute support
- No PrimaryInteropAssembly Support
