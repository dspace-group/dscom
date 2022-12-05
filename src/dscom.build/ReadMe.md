# dscom.build

## Preface

The `dSPACE.Runtime.InteropServices.Build` assembly and NuGet package provide the ability to create type libraries for a certain assembly at runtime.

For details on the implementation refer to the [documentation](/docs/ReadMe.md) section of the repository.

## Usage

To create a type library at compile time, simply add a reference to the nuget package, e.g. by using the command line.

```shell
$ dotnet add package dSPACE.Runtime.InteropServices.Build
...
$ 
```

The result should be a line as follows in your `.csproj` file:

```xml
    <PackageReference Include="dSPACE.Runtime.InteropServices.Build" Version="0.17.0" NoWarn="NU1701">
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
      <PrivateAssets>all</PrivateAssets>
    </PackageReference>
```

**Note**: The extra attribute `NoWarn="NU1701"` is only required, if neither `.NET 4.8` nor `.NET 6.0` are targeted, since dotnet pack will currently not create a .NETStandard 2.0 compliant NuGet Package.

### Using the native build task

The native build task is automatically selected, if a .NET 4.8 or .NET 6.0 assembly for Windows is being build using an x64 platform.

### Using the CLI based task

The CLI task is automatically selected, if a .NET Standard 2.0 assembly is build. It is also chosen, if the target platform is set to x86.

### Enforcing the usage of the CLI

It might be necessary to select the CLI based task. To do so, add the following property to your `.csproj` file:

```XML
<_DsComForceToolUsage>true</_DsComForceToolUsage>
```

This will enforce the usage of the DsCom as a command-line tool. Please note, that verbose logging will no longer be working.

### Enforcing to stop the build, if an error occurs

The build tasks puts a warning to the build log, if the desired type library has not been created, even if the backend has reported a success.

This warning is issued with the warning code `DSCOM001`, which can be collected in the `WarningsAsErrors` array:

```XML
<WarningsAsErrors>$(WarningsAsErrors);DSCOM001</WarningsAsErrors>
```

This way the build stops, if the type library is not exported.

## Parameters

The build task can be parameterized with the following properties:

| **Name**      | **Description**      | **Default** |
| ------------- | -------------------- | ----------- |
| _DsComTlbExt  | Extension of the resulting type library. | .tlb |
| _DsComForceToolUsage | Use DsCom Exe files to create the TLB | false |
| DsComTypeLibraryUniqueId | Overwrite the library UUID | Empty Guid |
| DsComRegisterTypeLibrariesAfterBuild | Use regasm call after the build to register type library after the build | false |
| DsComTlbExportAutoAddReferences | Add referenced assemblies automatically to type libraries | true |
| DsComTlbExportIncludeReferencesWithoutHintPath | If a `Reference` assembly does not provide a `HintPath` Metadata, the item spec shall be task. | false |
| _DsComExportTypeLibraryTargetFile | Path to the resulting file. | `$(TargetDir)\$(TargetName)$(_DsComTlbExt)` * |
| _DsComExportTypeLibraryAssemblyFile | Path to the source assembly file. | `$(TargetPath)` * |

*) This value cannot be overridden.

The build task consumes the following items:

| **Name**                     | **Description**                  |
| ---------------------------- | -------------------------------- |
| DsComTlbExportTlbReferences  | Referenced type library files.   |
| DsComTlbExportReferencePaths | Directories containing type libraries to use for export. |
| DsComTlbExportAssemblyPaths  | Assemblies to add for the export. |
