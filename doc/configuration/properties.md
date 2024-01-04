# Properties

The
[MSBuild properties](https://docs.microsoft.com/en-us/visualstudio/msbuild/msbuild-properties)
described on this page are used by the Zig SDK. These properties can all be set
in `PropertyGroup`s in your project file. Most of them should have sensible
defaults for new projects; a few (such as `TreatWarningsAsErrors` and
`SymbolVisibility`) have defaults that are not quite as sensible for unfortunate
historical reasons.

## Project Setup

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
* `FormatOnBuild` (`true`, `false`): Enable/disable formatting source code into
  canonical style on build in Zig projects. Defaults to `false`.

## Package Information

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

## Preprocessor

* `PublicHeadersPath`: Can be set to a directory containing public C/C++
  headers. These headers will be included in the NuGet package and will flow to
  dependent projects. This directory will also be treated as an
  `IncludeDirectory`. Unset by default.
* `DefineConstants`: A comma-separated list of preprocessor macros to define.
  Each entry can be a simple name or an assignment of the form `NAME=VALUE`.
  These macros are passed to the compiler with the `-D` flag. Note that this
  applies to Zig as well, not just C/C++.
* `CompilerDefines` (`true`, `false`): Enable/disable adding some implicit
  `DefineConstants` macros that describe the Zig compiler version. Defaults to
  `true`.
* `PlatformDefines` (`true`, `false`): Enable/disable adding some implicit
  `DefineConstants` macros that describe the target platform characteristics.
  Defaults to `true`.
* `ConfigurationDefines` (`true`, `false`): Enable/disable adding some implicit
  `DefineConstants` macros that describe the build configuration (`Debug`,
  `ReleaseFast`, etc). Defaults to `true`.
* `PackageDefines` (`true`, `false`): Enable/disable adding some implicit
  `DefineConstants` macros that describe the project being built. Defaults to
  `true`.

## Language Features

* `ZigVersion` (`major.minor.patch`): The version of the Zig compiler toolset to
  use. Defaults to the latest version known to the Zig SDK package that is in
  use.
* `LanguageStandard`: The language standard used for C/C++ projects. Passed to
  Clang's `-std` flag. Defaults to the latest standards known to the compiler
  version that `ZigVersion` defaults to.
* `AccessControl` (`true`, `false`): Enable/disable access control in C++
  projects. Defaults to `true`.
* `BlockExtensions` (`true`, `false`): Enable/disable Clang's block language
  extensions. Defaults to `false`.
* `CxxExceptions` (`true`, `false`): Enable/disable C++ exceptions. In C
  projects, this controls whether the C code will be unwindable by C++
  exceptions. Defaults to `true`.
* `AsyncExceptions` (`true`, `false`): Enable/disable the ability to catch
  [SEH](https://learn.microsoft.com/en-us/cpp/cpp/structured-exception-handling-c-cpp)
  exceptions with standard `try`/`catch` statements. Only affects Windows
  binaries. Defaults to `false`.
* `CxxReflection` (`true`, `false`): Enable/disable generating C++ run-time type
  information. This feature is required for some uses of `dynamic_cast`.
  Defaults to `true`.
* `MicrosoftExtensions` (`true`, `false`): Enable/disable a variety of
  Microsoft C/C++ extensions. Defaults to `false`, but note that the compiler
  itself always enables some parts of this when targeting Windows as Windows
  headers require it.

## Static Analysis

* `EnforceCodeStyleInBuild` (`true`, `false`): Enable/disable checking that
  source code is in the canonical style during build in Zig projects. Defaults
  to `false`.
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
* `TrustAnalysis` (`true`, `false`): Enable/disable static analysis with
  [trusted computing base annotations](https://clang.llvm.org/docs/AttributeReference.html#enforce-tcb)
  in C/C++ projects. Defaults to `true`.
* `WarningLevel` (`0`-`4`): How aggressively the compiler should analyze C/C++
  projects for potentially problematic code. `0` disables warnings completely;
  `4` enables all warnings, including a few controversial ones. Defaults to `3`.
* `DisableWarnings`: A comma-separated list of
  [warning names](https://clang.llvm.org/docs/DiagnosticsReference.html) (e.g.
  `cast-align`) to disable in C/C++ projects. Unset by default.
* `TreatWarningsAsErrors` (`true`, `false`): Enable/disable reporting warnings
  as errors in C/C++ projects. Defaults to `false`.

## Code Generation

* `Configuration` (`Debug`, `Release`): Specifies the overarching configuration.
  When `Release` is specified, `ReleaseMode` comes into effect. Defaults to
  `Debug`. Usually specified by the user as e.g. `dotnet build -c Release`.
* `DebugSymbols` (`true`, `false`): Enable/disable emitting debug symbols.
  Defaults to `true` if `Configuration` is `Debug`; otherwise, `false`.
* `ReleaseMode` (`Fast`, `Safe`, `Small`): The
  [build mode](https://ziglang.org/documentation/master/#Build-Mode) to use when
  `Configuration` is set to `Release`. Defaults to `Fast`.
* `FastMath` (`true`, `false`): Enable/disable certain lossy floating point
  optimizations that may not be standards-compliant. Defaults to `false`.
* `LinkTimeOptimization` (`true`, `false`): Enable/disable link-time
  optimization. Note that link-time optimization is known not to work well on
  some targets and so should be used selectively. Defaults to `false`.
* `SymbolExports` (`Used`, `All`): Specifies whether to export all public
  symbols or only those that are needed to link successfully. This only applies
  when building executables. Defaults to `Used`.
* `SymbolVisibility` (`Default`, `Hidden`): Specifies the symbol visibility in
  C/C++ projects when `__attribute__((visibility(...)))` is not specified.
  `Default` (the default 😉) means public, while `Hidden` means private.
* `AllowUndefinedSymbols` (`true`, `false`): Enable/disable permitting undefined
  symbols when linking. This usually only applies when building libraries.
  Defaults to `false`.
* `EagerBinding` (`true`, `false`): Enable/disable eager binding of symbols when
  performing dynamic linking at run time. Eager binding has security benefits,
  especially in combination with `RelocationHardening`. It is also more reliable
  if calling external functions from signal handlers. Defaults to `true`.
* `RelocationHardening` (`true`, `false`): Enable/disable marking relocations as
  read-only. This has security benefits, especially in combination with
  `EagerBinding`. Defaults to `true`.
* `ImageBase`: The location in memory that the binary should be loaded at. Only
  takes effect at run time if `DynamicImageBase` is `false`. Unset by default.
* `DynamicImageBase` (`true`, `false`): Enable/disable
  [ASLR](https://en.wikipedia.org/wiki/Address_space_layout_randomization), i.e.
  randomization of the image base at run time. Only affects Windows binaries.
  Defaults to `true`.
* `StackSize`: Sets the stack size for the main thread. This only applies when
  building executables. Unset by default.
* `Sanitizers`: A semicolon-separated list of
  [sanitizers](https://github.com/google/sanitizers) to instrument code with.
  Currently, only `thread` is supported. Unset by default.

## Cross-Compilation

* `RuntimeIdentifier`: Specifies the runtime identifier (i.e. platform) to
  target. When unset, `Build` and `Clean` will run for all runtime identifiers
  specified in `RuntimeIdentifiers`. Usually specified by the user as e.g.
  `dotnet build -r linux-x64`. Unset by default.
* `RuntimeIdentifiers`: A semicolon-separated list of runtime identifiers that
  the project supports. All targets in this list will be cross-compiled as
  necessary. Defaults to all targets that the Zig compiler has known-good
  support for.
* `UseMicrosoftAbi` (`true`, `false`): Enable/disable using the Microsoft ABI
  when targeting Windows. This may be necessary when linking to static libraries
  containing C++ code that was compiled for the Microsoft ABI. Note that it is
  currently not possible to cross-compile from non-Windows platforms when using
  the Microsoft ABI. Unset by default.
* `UseEmulator` (`true`, `false`): Enable/disable usage of an appropriate binary
  emulator when cross-compiling. Defaults to `true`.
