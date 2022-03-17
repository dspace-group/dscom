# dSPACE COM tools

[![Nuget:Cli](https://img.shields.io/nuget/v/dscom?label=dotnet%20tool&style=flat)](https://www.nuget.org/packages/dscom/)
[![Nuget:Lib](https://img.shields.io/nuget/v/dSPACE.Runtime.InteropServices?label=nuget&style=flat)](https://www.nuget.org/packages/dSPACE.Runtime.InteropServices/)
[![Build](https://img.shields.io/github/workflow/status/dspace-group/dscom/Build?style=flat)](https://github.com/dspace-group/dscom/actions/workflows/build.yaml)
[![Release](https://img.shields.io/github/v/release/dspace-group/dscom?label=release)](https://github.com/dspace-group/dscom/releases)
![License](https://img.shields.io/github/license/dspace-group/dscom)
[![dSPACE](https://img.shields.io/badge/-OpenSource%20powered%20by%20dSPACE-blue)](https://www.dspace.com/)

> This is an unstable prerelease. Anything may change at any time!

dscom is a replacement for `tlbexp.exe` and `TypeLibConverter.ConvertAssemblyToTypeLib`.  
The tool consists of a library and a command line tool. The library can be used in `net6.0` or in `net48` projects.

Fortunately, .NET still supports COM, but there is no support for generating TLBs.  
From the Microsoft documentation:

> Unlike in .NET Framework, there is no support in .NET Core or .NET 5+ for generating a COM Type Library (TLB) from a .NET assembly.

<https://docs.microsoft.com/en-us/dotnet/core/native-interop/expose-components-to-com>

One main goal is to make `dscom` behave like `tlbexp.exe`.

_Happy IUnknowing and IDispatching ;-)_

## Command Line Client

The command-line interface (CLI) tool `dscom` is a replacement for `tlbexp.exe` and `OleView` (View TypeLib).

It supports the following features:

- Convert an assembly to a type library
- Convert a type library to `YAML` file
- Register a type library
- Unregister a type library

### Installation

The installation is quite simple. You can use `dotnet tool` to install the `dscom` binary.

```bash
c:\> dotnet tool install --global dscom
```

Here you can find all available versions:  
<https://www.nuget.org/packages/dscom/>

### Usage

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

## dSPACE.Runtime.InteropServices library

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

- No imports of the `mscorelib` typelib (all types are VT_UNKNOWN)
  - \_Object not supported
  - \_EventArgs not supported
  - \_Delegate not supported
  - \_Type not supported
  - ISerializable not supported
  - ICloneable not supported
  - IDisposable not supported
  - ...
- Only 64 Bit support (x64)
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
