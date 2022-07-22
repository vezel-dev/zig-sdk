# Usage

To use the Zig SDK, first make sure that you have the
[.NET 6 SDK](https://dotnet.microsoft.com/download/dotnet/6.0) (or later)
installed.

Next, create a
[`global.json`](https://docs.microsoft.com/en-us/dotnet/core/tools/global-json)
file in the root of your repository and add the `Vezel.Zig.Sdk` entry under the
`msbuild-sdks` property:

```json
{
  "msbuild-sdks": {
    "Vezel.Zig.Sdk": "x.y.z"
  }
}
```

(Replace `x.y.z` with the actual NuGet package version.)

Next, create a project file. A library project is as simple as:

```xml
<Project Sdk="Vezel.Zig.Sdk" />
```

An executable project looks like:

```xml
<Project Sdk="Vezel.Zig.Sdk">
    <PropertyGroup>
        <OutputType>Exe</OutputType>
    </PropertyGroup>
</Project>
```

The project file extension determines the language your code will be compiled
as - `.cproj` for C, `.cxxproj` for C++, and `.zigproj` for Zig.

The convention used by the **Zig SDK** is that C projects should use a `.c`
extension for source files and `.h` for header files, while C++ projects should
use `.cxx` and `.hxx`. Zig projects use `.zig`.

For C/C++ projects, it does not matter what you name your source and header
files. For Zig executable projects, your root source file should be named
`main.zig`. Zig library projects should use the project name for the root source
file; that is, if your project file is `mylib.zigproj`, your root source file
should be `mylib.zig`.

Once you have written some code, you can use `dotnet build`, `dotnet run`, etc.
