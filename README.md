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

COM (Component Object Model) remains a critical part of the Windows ecosystem, enabling interoperability between software components. Despite its age, COM is still widely used in many applications and systems. However, with the advent of .NET 5 and later versions, Microsoft has discontinued several tools that were essential for working with COM, such as `tlbexp.exe` and `RegAsm.exe`. This has left developers without built-in support for generating or registering COM Type Libraries (TLBs) from .NET assemblies.

There is no support in .NET Core or .NET 5+ for generating a COM Type Library (TLB) from a .NET assembly.  
<https://docs.microsoft.com/en-us/dotnet/core/native-interop/expose-components-to-com>

To address this gap, `dscom` provides a modern replacement for these tools. It offers a command-line interface (CLI) and libraries that allow developers to:

- Generate and register TLBs from .NET assemblies.
- Embed TLBs into assemblies.
- Register and unregister assemblies for COM.
- Convert TLBs to YAML files for inspection.

`dscom` aims to replicate the functionality of the discontinued Microsoft tools while introducing additional features to simplify COM-related workflows in .NET 5+ and .NET Framework projects. Whether you are working with legacy systems or building new applications that require COM interoperability, `dscom` provides the tools you need to bridge the gap.

The `dSPACE.Runtime.InteropServices.BuildTasks` library provides build tasks which can be used to automatically generate TLBs at compile time.

- [dSPACE COM tools](#dspace-com-tools)
  - [Command Line Client](#command-line-client)
    - [Key Features of `dscom.exe`](#key-features-of-dscomexe)
    - [Installation](#installation)
    - [Usage](#usage)
  - [Library](#library)
    - [TypeLibConverter.ConvertAssemblyToTypeLib](#typelibconverterconvertassemblytotypelib)
    - [TypeLibEmbedder.EmbedTypeLib](#typelibembedderembedtypelib)
    - [RegistrationServices.RegisterTypeForComClients](#registrationservicesregistertypeforcomclients)
    - [RegistrationServices.RegisterAssembly](#registrationservicesregisterassembly)
  - [Build Tasks](#build-tasks)
    - [Build task usage](#build-task-usage)
      - [Enforcing to stop the build, if an error occurs](#enforcing-to-stop-the-build-if-an-error-occurs)
    - [Parameters](#parameters)
    - [Example](#example)
    - [Hints](#hints)
  - [32Bit support](#32bit-support)
  - [Migration notes (mscorelib vs System.Private.CoreLib)](#migration-notes-mscorelib-vs-systemprivatecorelib)
    - [Why can I load a .NET Framework library into a .NET application?](#why-can-i-load-a-net-framework-library-into-a-net-application)
  - [Limitations](#limitations)
    - [.NET 6 support](#net-6-support)
    - [RegisterAssembly](#registerassembly)
    - [RegAsm](#regasm)
  - [Contributing](#contributing)

## Command Line Client

The command-line interface (CLI) tool `dscom` serves as a modern replacement for several legacy tools, including `tlbexp.exe`, `OleView` (for viewing Type Libraries), and `RegAsm.exe`. It is designed to simplify and enhance workflows related to COM (Component Object Model) interoperability in .NET applications.

### Key Features of `dscom.exe`

1. **Convert an Assembly to a Type Library**  
    `dscom` allows you to generate a COM Type Library (TLB) from a .NET assembly. This is particularly useful for enabling COM interoperability with .NET assemblies in environments where a TLB is required.  
    - **Optional Embedding**: The generated Type Library can optionally be embedded directly into the converted assembly, ensuring that the TLB is always available alongside the assembly.

2. **Convert a Type Library to a YAML File**  
    With `dscom`, you can inspect the contents of a Type Library by converting it into a human-readable YAML file. This feature is helpful for debugging, documentation, or understanding the structure of a TLB.

3. **Register a Type Library**  
    The tool provides functionality to register a Type Library with the system, making it available for use by COM clients.

4. **Unregister a Type Library**  
    If a Type Library is no longer needed, `dscom` can unregister it, removing its association with the system.

5. **Embed a Type Library into an Existing Assembly**  
    `dscom` supports embedding an existing Type Library into a specified assembly. This feature is useful for scenarios where you want to bundle the TLB with the assembly for easier distribution and deployment.

6. **Register an Assembly (Equivalent to `RegAsm.exe`)**  
    The tool can register a .NET assembly for use with COM clients, similar to the functionality provided by `RegAsm.exe`. This includes creating the necessary registry entries to expose the assembly's classes to COM.

7. **Unregister an Assembly (Equivalent to `RegAsm.exe`)**  
    If an assembly is no longer required for COM interoperability, `dscom` can unregister it, cleaning up the associated registry entries.

These features make `dscom` a versatile and powerful tool for developers working with COM in modern .NET environments, addressing the gaps left by the deprecation of older tools while introducing additional capabilities to streamline the development process.

### Installation

The installation is quite simple. You can use `dotnet tool` to install the `dscom` binary if you want to create a 64Bit TLB.

```bash
dotnet tool install --global dscom
```

Here you can find all available versions:  
<https://www.nuget.org/packages/dscom/>

Alternatively you can download dscom.exe from the release page.  
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

Example to create a TLB from an assembly:  

```bash
c:\> dscom tlbexport MyAssembly.dll --out C:\path\to\output\MyAssembly.tlb
```

For more information about the command line options to create a TLB, use `dscom tlbexport --help`.


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

**Note**: The extra attribute `NoWarn="NU1701"` is only required, if neither `.NET 4.8` nor `.NET 8.0` are targeted, since dotnet pack will currently not create a .NETStandard 2.0 compliant NuGet Package.

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
| _DsComTlbExt                                   | Extension of the resulting type library. <br/> Default Value: `.tlb`                           |
| DsComTypeLibraryUniqueId                       | Overwrite the library UUID <br/> Default Value: Empty Guid                                     |
| DsComOverideLibraryName                        | Overwrite the IDL name of the library. <br/> Default Value: Empty string                       |
| DsComRegisterTypeLibrariesAfterBuild           | Use dscom call after the build to register type library after the build <br/> Default value: `false` |
| DsComRegisterTypeLibrariesAfterBuildGlobally   | Use dscom call and register the type library on global system level. <br/> Default value: `false` |
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

### .NET 6 support

.NET 6 is no longer supported by this project. Please use .NET 8 or later for full compatibility and support. If you require .NET 6 support, consider using an older release of dscom, but note that new features and bug fixes will not be backported.

Latest .NET compatible version is `1.15.2`.

- [https://github.com/dspace-group/dscom/releases/tag/v1.15.2](https://github.com/dspace-group/dscom/releases/tag/v1.15.2)
- [https://www.nuget.org/packages/dscom/1.15.2](https://www.nuget.org/packages/dscom/1.15.2)

### RegisterAssembly

- InProc Registration only will take place, if a comhost assembly is present.
- No CustomRegisterFunction Or CustomUnRegisterFunction Attribute support
- No PrimaryInteropAssembly Support

### RegAsm

| feature | dscom |
| -- | -- |
| assemblyFile | **supported** |
| codebase | **supported** |
| registered | not supported |
| asmpath | **supported** |
| nologo | not supported |
| regfile | not supported |
| silent | not supported |
| tlb | **supported** |
| unregister | **supported** |
| verbose | not supported |
| help | **supported** |

## Contributing

We would be happy if you would like to contribute to this project.  

We use VSCode as IDE, but feel free to use your preferred IDE.  
You need >= .NET 8.0 SDK and .NET Full Framework >= 4.8 SDK installed on your machine.  
Before submitting a pull request, please note the following points:  

1. **Code Formatting**  
    Ensure the code is properly formatted using `dotnet format --verify-no-changes`.

2. **Unit Tests**  
    Run all tests with `dotnet test` and make sure all tests pass successfully.
    Use `dotnet build` before running the tests to ensure that the project is built correctly and the `dscom.exe` is available.

3. **Writing Tests**  
    We like to have unit tests 😊  
    Write your own tests for any new features or bug fixes.

4. **Verifying the tlb generation**  
    To generate a TLB with dscom is the most important feature of this project.  
    Compare the output of `dscom` with that of `tlbexp`.  
    Use `scripts\test.assembly.bat` to generate a TLB with both `dscom` and `tlbexp`, and compare the outputs.  
    This script will use `src\dscom.test.assembly\dscom.test.assembly.csproj` to generate a TLB with both tools.
  
    The script will attempt to open VSCode to facilitate the file comparison.

    Ensure tlbexp is installed on your machine.  
    If you have Visual Studio installed, find tlbexp.exe in the `C:\Program Files (x86)\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.8 Tools` folder, or use the `Developer Command Prompt for VS` to run `tlbexp`.

If you have any questions, feel free to ask in the issues section.

Thank you for your contribution ❤️
