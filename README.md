# dSPACE COM tools

[![Nuget](https://img.shields.io/nuget/v/dscom?label=NuGet:CLI&style=flat)](https://www.nuget.org/packages/dscom/)
[![Nuget](https://img.shields.io/nuget/v/dSPACE.Runtime.InteropServices?label=NuGet:Lib&style=flat)](https://www.nuget.org/packages/dSPACE.Runtime.InteropServices/)
[![GitHub Workflow](https://img.shields.io/github/workflow/status/dspace-group/dscom/Build?style=flat&label=Build)](https://github.com/dspace-group/dscom/actions/workflows/build.yaml)
[![GitHub Release](https://img.shields.io/github/v/release/dspace-group/dscom?label=Latest-Release)](https://github.com/dspace-group/dscom/releases)

A replacement for `TypeLibConverter.ConvertTypeLibToAssembly`, `tlbexp.exe` and `OleView` that runs with .NET >= 6.0.

# dscom (CLI)

The command-line interface (CLI) tools `dcscom` is a replacement for `tlbexp.exe` and `OleView`.

## Installation

```bash
c:\> dotnet tool install --global dscom
```

## Usage

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
  tlbexport <Assembly>         Export the assembly to the specified type library
  tlbdump <TypeLibrary>        Dump a type library
  tlbregister <TypeLibrary>    Register a type library
  tlbunregister <TypeLibrary>  Unregister a type library
```
