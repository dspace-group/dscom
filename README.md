# dSPACE COM tools

[![Nuget:Cli](https://img.shields.io/nuget/v/dscom?label=dotnet%20tool&style=flat)](https://www.nuget.org/packages/dscom/)
[![Nuget:Lib](https://img.shields.io/nuget/v/dSPACE.Runtime.InteropServices?label=nuget&style=flat)](https://www.nuget.org/packages/dSPACE.Runtime.InteropServices/)
[![Build](https://img.shields.io/github/workflow/status/dspace-group/dscom/Build?style=flat)](https://github.com/dspace-group/dscom/actions/workflows/build.yaml)
[![Release](https://img.shields.io/github/v/release/dspace-group/dscom?label=release)](https://github.com/dspace-group/dscom/releases)
![License](https://img.shields.io/github/license/dspace-group/dscom)
[![dSPACE](https://img.shields.io/badge/-OpenSource%20powered%20by%20dSPACE-blue)](https://www.dspace.com/)

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

> The documentation is under construction.
