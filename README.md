# Zig MSBuild SDK

[![Zig.Sdk](https://img.shields.io/nuget/v/Zig.Sdk.svg?label=Zig.Sdk)](https://www.nuget.org/packages/Zig.Sdk)
[![Build Status](https://github.com/alexrp/zig-msbuild-sdk/actions/workflows/build.yml/badge.svg)](https://github.com/alexrp/zig-msbuild-sdk/actions/workflows/build.yml)

`Zig.Sdk` is an
[MSBuild SDK](https://docs.microsoft.com/en-us/visualstudio/msbuild/how-to-use-project-sdk)
that makes it trivial to author native components as part of your .NET project
through the power of [Zig](https://ziglang.org).

## Features

Here are some of the `Zig.Sdk` highlights:

* **Multiple programming languages:** Although Zig is a modern and pleasant
  systems programming language, you might prefer to use C or C++ instead.
  As it happens, the Zig compiler also embds a full C and C++ compiler - namely,
  [Clang](https://clang.llvm.org). So, whichever language you prefer, `Zig.Sdk`
  has you covered.
* **Cross-compilation:** Thanks to the Zig compiler's excellent cross-targeting
  support, cross-compilation is a first-class citizen in `Zig.Sdk`. Gone are the
  days of having to do overly complicated cross toolchain setup, or resorting to
  building on multiple platforms for releases - just type `dotnet build` to
  compile for all targets supported by your project.
* **Binary emulator support:** When cross-compiling, `Zig.Sdk` will look at the
  host and target platforms and try to pick an appropriate emulator. In the
  majority of cases, this allows you to run and unit test the foreign binary.
  [Darling](https://darlinghq.org), [QEMU](https://qemu.org),
  [Wine](https://winehq.org), and
  [WSL](https://docs.microsoft.com/en-us/windows/wsl) are recognized.
* **Unit testing:** The Zig language provides built-in unit testing constructs.
  `Zig.Sdk` allows you to run your project's unit tests with the familiar
  `dotnet test` command. Test name filters are supported - e.g.
  `dotnet test --filter foo`.
* **Code change monitoring:** `Zig.Sdk` integrates with `dotnet watch` so that
  e.g. `dotnet watch build`, `dotnet watch run`, and `dotnet watch test` work as
  expected, enabling a rapid development loop.
* **Sensible NuGet packaging:** Out of the box, `dotnet pack` with `Zig.Sdk`
  will produce NuGet packages containing cross-built binaries for all platforms
  that your project supports. Also, your public C and C++ header files will be
  bundled, as will your Zig source code. This makes the resulting NuGet package
  easy to consume both in `Microsoft.NET.Sdk` projects and in other `Zig.Sdk`
  projects.
* **Multi-project solutions:**
  [Soon™.](https://github.com/alexrp/zig-msbuild-sdk/issues/8)
* **Editor integration:** `Zig.Sdk` can generate files needed by language
  servers, resulting in an IDE-like experience when editing code. For C/C++,
  [clangd](https://clangd.llvm.org) is fully supported, while for Zig projects,
  there is limited [ZLS](https://github.com/zigtools/zls) support. See the
  [Editor](#editor) section below for more information.
* **Easy to use:** Just add an entry to your `global.json`, and create a project
  file ending in `.cproj`, `.cxxproj`, or `.zigproj`, with an `Sdk="Zig.Sdk"`
  attribute. Then start writing code and use `dotnet build`, `dotnet run`,
  `dotnet test`, etc as you normally would. See the [Usage](#usage) section
  below to get started.

Please note that `Zig.Sdk` is *not* intended to be a full replacement for
[Zig's build system](https://ziglearn.org/chapter-3). The goal of `Zig.Sdk` is
specifically to make it simple to integrate Zig, C, and C++ components into the
.NET ecosystem. For that reason, `Zig.Sdk` has no support for platforms that
Zig supports but that .NET does not (yet) run on, such as `linux-riscv64`. The
level of configuration that is possible for C and C++ is also somewhat limited
compared to most build systems that support those languages.

## Usage

First, make sure you have the
[.NET 6 SDK](https://dotnet.microsoft.com/download/dotnet/6.0) (or later)
installed.

Next, create a
[`global.json`](https://docs.microsoft.com/en-us/dotnet/core/tools/global-json)
in the root of your repository and add the `Zig.Sdk` entry under `msbuild-sdks`:

```json
{
  "msbuild-sdks": {
    "Zig.Sdk": "x.y.z"
  }
}
```

(Replace `x.y.z` with the actual NuGet package version.)

Next, create a project file. A library project is as simple as:

```xml
<Project Sdk="Zig.Sdk" />
```

An executable project looks like:

```xml
<Project Sdk="Zig.Sdk">
    <PropertyGroup>
        <OutputType>Exe</OutputType>
    </PropertyGroup>
</Project>
```

The project file extension determines the language your code will be compiled
as - `.cproj` for C, `.cxxproj` for C++, and `.zigproj` for Zig.

The convention used by `Zig.Sdk` is that C projects should use a `.c` extension
for source files and `.h` for header files, while C++ projects should use `.cxx`
and `.hxx`. Zig projects use `.zig`.

For C/C++ projects, it does not matter what you name your source and header
files. For Zig executable projects, your root source file should be named
`main.zig`. Zig library projects should use the project name for the root source
file; that is, if your project file is `mylib.zigproj`, your root source file
should be `mylib.zig`.

Once you have written some code, you can use `dotnet build`, `dotnet run`, etc.

## Configuration

### Editor

#### ZLS

Start by
[installing ZLS](https://github.com/zigtools/zls/blob/master/README.md#installation).
The
[Visual Studio Code extension](https://marketplace.visualstudio.com/items?itemName=AugusteRame.zls-vscode)
is highly recommended.

At the moment, no further configuration is necessary.

#### clangd

Start by
[installing clangd](https://clangd.llvm.org/installation). The
[Visual Studio Code extension](https://marketplace.visualstudio.com/items?itemName=llvm-vs-code-extensions.vscode-clangd)
is highly recommended.

You have to tell clangd where to find the `compile_commands.json` compilation
database. `Zig.Sdk` creates these files in `IntermediateOutputPath`, i.e.
`obj/Debug/linux-x64` if you build with `Configuration=Debug` and
`RuntimeIdentifier=linux-x64`. Due to the nature of C/C++ compilation, these
compilation databases have to be dependent on build flags. So, you will have to
pick one of them to use for your editing experience. The good news is that you
can change which compilation database you use at any point if you need to.

To tell clangd where to find the compilation database, create a file called
`.clangd` in your project directory with the following contents:

```yaml
CompileFlags:
    CompilationDatabase: obj/Debug/linux-x64
```

(You may want to add this file to `.gitignore`.)

You can now restart the clangd language server. You should start to see rich
editor features like code completion, hover widgets, navigation, etc.

### Properties

These properties can all be set in `PropertyGroup`s in your project file. Most
of them should have sensible defaults for new projects; a few (such as
`TreatWarningsAsErrors` and `SymbolVisibility`) have defaults that are not quite
as sensible for historical reasons.

#### Project Setup

* `CompilerMode` (`C`, `Cxx`, `Zig`): The language to compile the project as.
  This is inferred from the project file extension by default.
* `AssemblyName`: Name of the project. By default, this is set to the file name
  of the project file. Used to compute the final binary name (e.g. `foo` becomes
  `libfoo.so`).
* `OutputType` (`Exe`, `Library`): Output binary type. Defaults to `Library`.
* `IsTestable` (`true`, `false`): Enable/disable `dotnet test` for Zig projects.
  Defaults to `true`.
* `IsPackable` (`true`, `false`): Enable/disable `dotnet pack`. Defaults to
  `true`.
* `IsPublishable` (`true`, `false`): Enable/disable `dotnet publish`. Defaults
  to `true`.
* `DefaultSources` (`true`, `false`): Enable/disable default `Compile` item
  includes. Defaults to `true`.
* `Deterministic` (`true`, `false`): Enable/disable deterministic builds. Among
  other things, this will try to prevent the compiler from using absolute paths
  and will prevent usage of certain problematic language features like
  `__TIME__`. Defaults to `true`.
* `EditorSupport` (`true`, `false`): Enable/disable editor support. For C/C++
  projects, this means generating a `compile_commands.json` compilation database
  in `IntermediateOutputPath`. Defaults to `true`.

#### Package Information

* `Product`: Human-friendly product name for the package. Defaults to the value
  of `AssemblyName`.
* `Authors`: A list of package authors. Defaults to the value of `AssemblyName`.
* `Description`: Brief description of the package. Defaults to
  `Package Description`.
* `Version`: Package version in [Semantic Versioning 2.0.0](https://semver.org)
  form. Defaults to `1.0.0`.
* `Copyright`: Copyright notice for the package. Unset by default.
* `PackageLicenseExpression`: [SPDX](https://spdx.org/licenses) license
  identifier for the package. Unset by default.
* `PackageProjectUrl`: Website URL associated with the package. Unset by
  default.
* `RepositoryUrl`: Source code repository URL for the package. Unset by default.

#### Preprocessor

* `PublicHeadersPath`: Can be set to a directory containing public C/C++
  headers. These headers will be included in the NuGet package and will flow to
  dependent projects. This directory will also be treated as an
  `IncludeDirectory`. Unset by default.
* `DefineConstants`: A comma-separated list of preprocessor macros to define.
  Each entry can be a simple name or an assignment of the form `NAME=VALUE`.
  These macros are passed to the compiler with the `-D` flag. Note that this
  applies to Zig as well, not just C/C++.
* `CompilerDefines` (`true`, `false`): Enable/disable adding some implicit
  `DefineConstants` macros that describe the Zig compiler version.
* `PlatformDefines` (`true`, `false`): Enable/disable adding some implicit
  `DefineConstants` macros that describe the target platform characteristics.
* `PackageDefines` (`true`, `false`): Enable/disable adding some implicit
  `DefineConstants` macros that describe the project being built.

#### Language Features

* `ZigVersion` (`x.y.z`): The version of the Zig compiler toolset to use.
  Defaults to the latest version known to the `Zig.Sdk` package that is in use.
* `LanguageStandard`: The language standard used for C/C++ projects. Passed to
  Clang's `-std` flag. Defaults to the latest standards known to the compiler
  version that `ZigVersion` defaults to.
* `AccessControl` (`true`, `false`): Enable/disable access control in C++
  projects. Defaults to `true`.
* `BlockExtensions` (`true`, `false`): Enables/disables Clang's block language
  extensions. Defaults to `false`.
* `CxxExceptions` (`true`, `false`): Enables/disables C++ exceptions. In C
  projects, this controls whether the C code will be unwindable by C++
  exceptions. Defaults to `true`.
* `CxxReflection` (`true`, `false`): Enables/disables generating C++ run-time
  type information. This feature is required for some uses of `dynamic_cast`.
  Defaults to `true`.
* `MicrosoftExtensions` (`true`, `false`): Enables/disables a variety of
  Microsoft C/C++ extensions. Defaults to `false`, but note that the compiler
  itself always enables this when targeting Windows as Windows headers require
  it.

#### Static Analysis

* `ConsumptionAnalysis` (`true`, `false`): Enable/disable static analysis with
  [consumption and type state annotations](https://clang.llvm.org/docs/AttributeReference.html#consumed-annotation-checking)
  in C/C++ projects. Defaults to `true`.
* `DocumentationAnalysis` (`true`, `false`): Enable/disable
  [Doxygen](https://doxygen.nl) documentation comment checking in C/C++
  projects. Defaults to `false`.
* `NullabilityAnalysis` (`true`, `false`): Enable/disable static analysis with
  [nullability annotations](https://clang.llvm.org/docs/analyzer/developer-docs/nullability.html)
  in C/C++ projects. Defaults to `true`.
* `TagAnalysis` (`true`, `false`): Enable/disable static analysis with
  [type tag annotations](https://clang.llvm.org/docs/AttributeReference.html#type-safety-checking)
  in C/C++ projects. Defaults to `true`.
* `ThreadingAnalysis` (`true`, `false`): Enable/disable static analysis with
  [thread safety annotations](https://clang.llvm.org/docs/ThreadSafetyAnalysis.html)
  in C/C++ projects. Defaults to `true`.
* `WarningLevel` (`0`-`4`): How aggressively the compiler should analyze C/C++
  projects for potentially problematic code. `0` disables warnings completely;
  `4` enables all warnings, including a few controversial ones. Defaults to `3`.
* `DisableWarnings`: A comma-separated list of
  [warning names](https://clang.llvm.org/docs/DiagnosticsReference.html) (e.g.
  `cast-align`) to disable in C/C++ projects. Unset by default.
* `TreatWarningsAsErrors` (`true`, `false`): Enable/disable reporting warnings
  as errors in C/C++ projects. Defaults to `false`.

#### Code Generation

* `Configuration` (`Debug`, `Release`): Specifies the overarching configuration.
  When `Release` is specified, `ReleaseMode` comes into effect. Defaults to
  `Debug`. Usually specified by the user as e.g. `dotnet build -c Release`.
* `FastMath` (`true`, `false`): Enables/disables certain lossy floating point
  optimizations that may not be standards-compliant. Defaults to `false`.
* `DebugSymbols` (`true`, `false`): Enables/disables emitting debug symbols.
  Defaults to `true` if `Configuration` is `Debug`; otherwise, `false`.
* `ReleaseMode` (`Fast`, `Safe`, `Small`): The
  [build mode](https://ziglang.org/documentation/master/#Build-Mode) to use when
  `Configuration` is set to `Release`. Defaults to `Fast`.
* `SymbolExports` (`Used`, `All`): Specifies whether to export all public
  symbols or only those that are needed to link successfully. This only applies
  when building executables. Defaults to `Used`.
* `SymbolVisibility` (`Default`, `Hidden`): Specifies the symbol visibility in
  C/C++ projects when `__attribute__((visibility(...)))` is not specified.
  `Default` (the default 😉) means public, while `Hidden` means private.
* `Sanitizers`: A semicolon-separated list of
  [sanitizers](https://github.com/google/sanitizers) to instrument code with.
  For Zig projects, only `thread` is supported. Unset by default.

#### Cross-Compilation

* `RuntimeIdentifier`: Specifies the runtime identifier (i.e. platform) to
  target. When unset, `Build` and `Clean` will run for all runtime identifiers
  specified in `RuntimeIdentifiers`. Usually specified by the user as e.g.
  `dotnet build -r linux-x64`. Unset by default.
* `RuntimeIdentifiers`: A semicolon-separated list of runtime identifiers that
  the project supports. All targets on this list will be cross-compiled as
  necessary. Defaults to all targets that the Zig compiler has known-good
  support for.
* `UseEmulator` (`true`, `false`): Enable/disable usage of an appropriate binary
  emulator when cross-compiling. Defaults to `true`.

### Items

* `Compile`: Source code files passed to the Zig compiler. By default, `Zig.Sdk`
  will populate this item type according to the project type.
* `IncludeDirectory`: Header include directories passed to the compiler with the
  `-I` flag. Note that this applies to Zig as well, not just C/C++.
* `PreludeHeader`: C/C++ header files that will be automatically `#include`d in
  every C/C++ source file by way of Clang's `-include` flag.
* `CHeader`: Prepopulated by `Zig.Sdk` with all files in the project directory
  ending in `.h`.
* `CSource`: Prepopulated by `Zig.Sdk` with all files in the project directory
  ending in `.c`.
* `CxxHeader`: Prepopulated by `Zig.Sdk` with all files in the project directory
  ending in `.hxx`.
* `CxxHeader`: Prepopulated by `Zig.Sdk` with all files in the project directory
  ending in `.cxx`.
* `ZigSource`: Prepopulated by `Zig.Sdk` with all files in the project directory
  ending in `.zig`.
* `Watch`: Files that are monitored by `dotnet watch` for code changes.
  `Zig.Sdk` will automatically populate this with all C, C++, and Zig source and
  header files in the project directory.

### References

[Soon™.](https://github.com/alexrp/zig-msbuild-sdk/issues/8)
