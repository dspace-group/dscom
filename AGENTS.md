# Project Guidelines

## Overview

dscom is a .NET library and CLI tool that replaces deprecated Microsoft COM tools (`tlbexp.exe`, `OleView`, `RegAsm.exe`) for .NET 5+. It exports .NET assemblies to COM Type Libraries (TLBs), registers/unregisters COM types, embeds TLBs, and converts TLBs to YAML.

Three NuGet packages are published: the core library (`dSPACE.Runtime.InteropServices`), the CLI (`dscom`), and MSBuild tasks (`dSPACE.Runtime.InteropServices.BuildTasks`).

## Build and Test

```shell
dotnet build dscom.sln                          # Build everything
dotnet test src/dscom.test/dscom.test.csproj    # Run tests (xUnit, targets net8.0 + net48)
dotnet format --verify-no-changes               # Validate code style (CI enforces this)
```

After every code change, run `dotnet format --verify-no-changes` to check for linter/style violations. CI will reject PRs that fail this check.

Coverage: `scripts/test.coverage.bat` and `scripts/test.coverage.report.bat`.

## Architecture

The core library (`src/dscom/`) uses a two-phase pipeline:

1. **Exporter** (`src/dscom/exporter/`) — Reflects .NET types into `*Info` DTOs (TypeInfo, FunctionInfo, ParameterInfo, etc.)
2. **Writer** (`src/dscom/writer/`) — Consumes Info objects and writes COM TypeLib via `ICreateTypeLib2` P/Invoke calls

Other layers:

- `src/dscom/internal/` — Low-level COM interop: P/Invoke signatures for Ole32.dll, OleAut32.dll, COM interface definitions
- `src/dscom/attributes/` — Custom .NET attributes controlling COM visibility/behavior (`@Hidden`, `@Restricted`, `@ComAlias`, `@TypeFlags`)
- `src/dscom/names/` — COM identifier name management
- `src/dscom.client/` — CLI using `System.CommandLine` (subcommands: tlbexport, tlbdump, tlbregister, tlbunregister, regasm)
- `src/dscom.build/` — MSBuild task integration; see [src/dscom.build/doc/ReadMe.md](src/dscom.build/doc/ReadMe.md)

**Writer hierarchy**: `BaseWriter` → specialized writers (ClassWriter, InterfaceWriter, EnumWriter, StructWriter, MethodWriter, etc.). Each writer has a `Create()` method that builds and registers COM objects.

## Code Style

- Formatting enforced via [.editorconfig](.editorconfig) and `dotnet format` in CI
- C# 10, nullable reference types enabled, implicit usings enabled
- File-scoped namespaces preferred
- Private fields: `_camelCase` prefix
- Root namespace: `dSPACE.Runtime.InteropServices`
- `var` preferred when type is apparent
- Global COM type aliases defined in [src/dscom/GlobalUsing.cs](src/dscom/GlobalUsing.cs) (50+ aliases like FUNCDESC, ITypeLib, etc.)

## Testing

- **Framework**: xUnit with Moq, targeting both `net8.0` and `net48`
- **Test files**: `src/dscom.test/tests/` (35+ files covering classes, interfaces, enums, marshaling, CLI, etc.)
- **Pattern**: `BaseTest` provides a `DynamicAssemblyBuilder` for runtime type/assembly generation — tests create .NET types dynamically, export to TLB, then verify COM metadata
- **Test assemblies**: `src/dscom.test.assembly/` and `src/dscom.test.assembly.dependency/` provide static test targets

## Breaking Changes

After every implementation or change, check whether the modification introduces a breaking change. Breaking changes include but are not limited to:

- Changes to public API signatures (method names, parameters, return types)
- Removal or renaming of public types, members, or namespaces
- Changes to COM interface definitions, GUIDs, or dispatch IDs
- Behavioral changes to existing public methods
- Changes to NuGet package structure or MSBuild task/target names
- Changes to CLI command names, options, or default behavior

**If a breaking change is detected, the agent MUST stop and warn the user before proceeding.** The implementation may only continue after the user explicitly confirms that the breaking change is acceptable. Never implement a breaking change without user approval.

## Conventions

- All projects share settings from [src/Directory.Build.props](src/Directory.Build.props) — do not duplicate those settings in individual .csproj files
- The exporter/writer split must be maintained: never write COM objects directly from reflection — always go through Info DTOs
- COM interop types in `src/dscom/internal/` wrap raw P/Invoke; prefer using the existing wrappers over adding new raw P/Invoke calls

## CI

GitHub Actions workflows in `.github/workflows/`:

- `unit-test.yaml` — Build + test on PRs
- `code-style.yaml` — `dotnet format --verify-no-changes`
- `release.yaml` — Publish to NuGet and GitHub Releases on version tags (`v*.*.*`)
- `example-test.yaml` — Validates example projects
