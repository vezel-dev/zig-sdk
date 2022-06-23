# MSBuild Items

The following
[MSBuild items](https://docs.microsoft.com/en-us/visualstudio/msbuild/msbuild-items)
are used by the Zig SDK:

* `Compile`: Source code files passed to the Zig compiler. By default, the Zig
  SDK will populate this item type according to the project type.
* `PreludeHeader`: C/C++ header files that will be automatically `#include`d in
  every C/C++ source file by way of Clang's `-include` flag.
* `IncludeDirectory`: Header include directories passed to the compiler with the
  `-I` flag. Note that this applies to Zig as well, not just C/C++.
* `LibraryIncludeDirectory`: Header include directories passed to the compiler
  with the `-isystem` flag. Note that this applies to Zig as well, not just
  C/C++.
* `LinkerDirectory`: Library search directories passed to the linker with the
  `-L` flag.
* `LinkerReference`: Names of native libraries that should be linked using the
  `-l` flag. These can be either static or dynamic.
* `LibraryReference`: Direct paths to native library files that should be
  linked, ignoring library search directories. These can be either static or
  dynamic.
* `CHeader`: Prepopulated by the Zig SDK with all files in the project directory
  ending in `.h`.
* `CSource`: Prepopulated by the Zig SDK with all files in the project directory
  ending in `.c`.
* `CxxHeader`: Prepopulated by the Zig SDK with all files in the project
  directory ending in `.hxx`.
* `CxxHeader`: Prepopulated by the Zig SDK with all files in the project
  directory ending in `.cxx`.
* `ZigSource`: Prepopulated by the Zig SDK with all files in the project
  directory ending in `.zig`.
* `Watch`: Files that are monitored by `dotnet watch` for code changes. The Zig
  SDK will automatically populate this with all C, C++, and Zig source and
  header files in the project directory.
