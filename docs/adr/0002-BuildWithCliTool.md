# Build with CLI tool

## Story

As a developer I want to export a type library during the build, even if my assembly does not target .NET 4.8 or .NET 6.0 using the x64 platform. I also would appreciate the ability to build my .NET 4.8 or .NET 6.0 assemblies with the CLI during the build using the CLI and neglecting the built-in task, but in this case I don't want to care about installation of the global tool.

## State

Accepted

## Decision

The package `dSPACE.Runtime.InteropServices.Build` will contain the DsCom Client executable. It will ship in both architectures - x64 and x86. It will not reference the runtime assembly `dSPACE.Runtime.InteropServices.dll` but contain it within its IL code using the `PublishSingleFile` parameter. The task will be called from the `.props` and `.targets` within the package. The tool itself will be published as a .NET 6.0 Framework dependent executable.

### Reasons

The MsBuild task runs in the selected target framework using the preferred target architecture, which is in most cases x64. Hence other target frameworks or architectures are not supported at build time and may cause issues with the task. Hence the call to the type-library must take place out of the MsBuild process. Therefore both must be shipped, the DsCOM executable and its runtime library. To get rid of any dependencies at runtime, like `System.CommandLine`, the assemblies must be included into the executable.

## Dropped Decisions

* Provide a recipe to install and call the DsCOM dotnet tool. This is harder to maintain and may cause problems during update scenarios.
* Provide the `dSPACE.Runtime.InteropServices.dll` as a standalone assembly. This will either add the assembly three times to the same NuGet package or may enforce a probing path.
* Provide the tool as a self contained executable. It will blow up the NuGet package.
