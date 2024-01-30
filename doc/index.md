# Home

The **Zig SDK** is an
[MSBuild SDK](https://docs.microsoft.com/en-us/visualstudio/msbuild/how-to-use-project-sdk)
that augments the .NET SDK with the ability to build Zig, C, and C++ projects.

With support for multiple programming languages, cross-compilation, NuGet
packaging, and more, the **Zig SDK** makes it trivial to author native
components as part of your .NET solution - without all the hassle that is
usually part and parcel of building and packaging native code. These features
are powered by the [Zig](https://ziglang.org) toolchain.

## Features

Here are some of the **Zig SDK** highlights:

* **Multiple programming languages:** Although Zig is a modern and pleasant
  systems programming language, you might prefer to use C or C++ instead. As it
  happens, the Zig compiler also embds a full C and C++ compiler - namely,
  [Clang](https://clang.llvm.org). So, whichever language you prefer, the
  **Zig SDK** has you covered.
* **Cross-compilation:** Thanks to the Zig compiler's excellent cross-targeting
  support, cross-compilation is a first-class citizen in the **Zig SDK**. Gone
  are the days of having to do overly complicated cross toolchain setup, or
  resorting to building on multiple platforms for releases - just type
  `dotnet build` to compile for all targets supported by your project.
* **Binary emulator support:** When cross-compiling, the **Zig SDK** will look
  at the host and target platforms and try to pick an appropriate emulator. In
  the majority of cases, this allows you to run and unit test the foreign
  binary. [Darling](https://darlinghq.org), [QEMU](https://qemu.org),
  [Wine](https://winehq.org), and
  [WSL](https://docs.microsoft.com/en-us/windows/wsl) are recognized.
* **Unit testing:** The Zig language provides built-in unit testing constructs.
  The **Zig SDK** allows you to run your project's unit tests with the familiar
  `dotnet test` command. Test name filters are supported - e.g.
  `dotnet test --filter foo`.
* **Code change monitoring:** The **Zig SDK** integrates with `dotnet watch` so
  that e.g. `dotnet watch build`, `dotnet watch run`, and `dotnet watch test`
  work as expected, enabling a rapid development loop.
* **Sensible NuGet packaging:** Out of the box, `dotnet pack` with the
  **Zig SDK** will produce NuGet packages containing cross-built binaries for
  all platforms that your project supports. Also, your public C and C++ header
  files will be bundled, as will your Zig source code. This makes the resulting
  NuGet package easy to consume both in .NET projects and in other projects
  using the **Zig SDK**.
* **Multi-project solutions:**
  [Soon™.](https://github.com/vezel-dev/zig-sdk/issues/8)
* **Editor integration:** The **Zig SDK** can generate files needed by language
  servers, resulting in an IDE-like experience when editing code. For C/C++,
  [clangd](https://clangd.llvm.org) is fully supported, while for Zig projects,
  there is limited [ZLS](https://github.com/zigtools/zls) support.

Please note that the **Zig SDK** is *not* intended to be a full replacement for
[Zig's build system](https://ziglearn.org/chapter-3). The goal of the
**Zig SDK** is specifically to make it simple to integrate Zig, C, and C++
components into the .NET ecosystem. For that reason, the **Zig SDK** has no
support for platforms that Zig supports but that .NET does not (yet) run on,
such as `linux-riscv64`. The level of configuration that is possible for C and
C++ is also somewhat limited compared to most build systems that support those
languages.
