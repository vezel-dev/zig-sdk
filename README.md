# Zig SDK

<div align="center">
    <img src="zig.svg"
         width="128"
         alt="Zig SDK" />
</div>

<p align="center">
    <strong>
        An MSBuild SDK for building Zig, C, and C++ projects using the Zig
        compiler.
    </strong>
</p>

<div align="center">

[![License](https://img.shields.io/badge/license-0BSD-brown)](LICENSE-0BSD)
[![Commits](https://img.shields.io/github/commit-activity/m/vezel-dev/zig-sdk/master?label=commits&color=slateblue)](https://github.com/vezel-dev/zig-sdk/commits/master)
[![Build](https://img.shields.io/github/actions/workflow/status/vezel-dev/zig-sdk/build.yml?branch=master)](https://github.com/vezel-dev/zig-sdk/actions/workflows/build.yml)
[![Discussions](https://img.shields.io/github/discussions/vezel-dev/zig-sdk?color=teal)](https://github.com/vezel-dev/zig-sdk/discussions)
[![Discord](https://img.shields.io/badge/discord-chat-7289da?logo=discord)](https://discord.gg/wtzCfaX2Nj)

</div>

--------------------------------------------------------------------------------

The **Zig SDK** is an
[MSBuild SDK](https://docs.microsoft.com/en-us/visualstudio/msbuild/how-to-use-project-sdk)
that augments the .NET SDK with the ability to build Zig, C, and C++ projects.

With support for multiple programming languages, cross-compilation, NuGet
packaging, and more, the **Zig SDK** makes it trivial to author native
components as part of your .NET solution - without all the hassle that is
usually part and parcel of building and packaging native code. These features
are powered by the [Zig](https://ziglang.org) toolchain.

## Usage

This project offers the following packages:

| Package | Description | Downloads |
| -: | - | :- |
| [![Vezel.Zig.Sdk][sdk-img]][sdk-pkg] | Provides the MSBuild SDK and associated tasks. | ![Downloads][sdk-dls] |

[sdk-pkg]: https://www.nuget.org/packages/Vezel.Zig.Sdk

[sdk-img]: https://img.shields.io/nuget/v/Vezel.Zig.Sdk?label=Vezel.Zig.Sdk

[sdk-dls]: https://img.shields.io/nuget/dt/Vezel.Zig.Sdk?label=

To install an SDK package in a project, add it to your `global.json` under the
`msbuild-sdks` property.

For more information, please visit the
[project home page](https://docs.vezel.dev/zig-sdk).

## Building

You will need the .NET SDK installed. Simply run `./cake`
(a [Bash](https://www.gnu.org/software/bash) script) to build artifacts. You can
also use `./cake pack` if you do not want to build the documentation (which
requires Node.js).

## License

This project is licensed under the terms found in
[`LICENSE-0BSD`](LICENSE-0BSD).

The Zig logo is licensed under the terms found in
[`LICENSE-CC-BY-SA-4.0`](LICENSE-CC-BY-SA-4.0).
