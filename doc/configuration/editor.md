# Editor

The Zig SDK is capable of integrating with
[ZLS](https://github.com/zigtools/zls) and [clangd](https://clangd.llvm.org).
This means that any editor with support for those language servers (e.g.
[Visual Studio Code](https://code.visualstudio.com)) can be used to edit a
project using the Zig SDK.

## ZLS

Start by
[installing ZLS](https://github.com/zigtools/zls/blob/master/README.md#installation).
The
[Visual Studio Code extension](https://marketplace.visualstudio.com/items?itemName=AugusteRame.zls-vscode)
is highly recommended.

At the moment, no further configuration is necessary.

## clangd

Start by
[installing clangd](https://clangd.llvm.org/installation). The
[Visual Studio Code extension](https://marketplace.visualstudio.com/items?itemName=llvm-vs-code-extensions.vscode-clangd)
is highly recommended.

You have to tell clangd where to find the `compile_commands.json` compilation
database. The Zig SDK creates these files in `IntermediateOutputPath`, i.e.
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
