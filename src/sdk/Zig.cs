using System;
using System.Runtime.InteropServices;
using Microsoft.Build.Framework;
using Microsoft.Build.Tasks;
using Microsoft.Build.Utilities;

namespace Zig.Tasks
{
    public sealed class Zig : ToolTask
    {
        const StringSplitOptions SplitOptions = StringSplitOptions.RemoveEmptyEntries;

        [Required]
        public bool AccessControl { get; set; }

        [Required]
        public bool AttributeExtensions { get; set; }

        [Required]
        public bool BlockExtensions { get; set; }

        [Required]
        public string CompilerMode
        {
            get => _compilerMode.ToString();
            set => _compilerMode = (ZigCompilerMode)Enum.Parse(typeof(ZigCompilerMode), value);
        }

        [Required]
        public string Configuration
        {
            get => _configuration.ToString();
            set => _configuration = (ZigConfiguration)Enum.Parse(typeof(ZigConfiguration), value);
        }

        [Required]
        public bool CxxExceptions { get; set; }

        [Required]
        public bool CxxReflection { get; set; }

        [Required]
        public bool DebugSymbols { get; set; }

        public string? DefineConstants { get; set; }

        [Required]
        public bool Deterministic { get; set; }

        [Required]
        public bool FastMath { get; set; }

        [Required]
        public ITaskItem[] IncludeDirectories { get; set; } = null!;

        [Required]
        public string LanguageStandard { get; set; } = null!;

        [Required]
        public bool MicrosoftExtensions { get; set; }

        [Required]
        public ITaskItem OutputBinary { get; set; } = null!;

        [Required]
        public string OutputType
        {
            get => _outputType.ToString();
            set => _outputType = (ZigOutputType)Enum.Parse(typeof(ZigOutputType), value);
        }

        [Required]
        public ITaskItem[] PreludeHeaders { get; set; } = null!;

        public string? PublicIncludeDirectory { get; set; }

        [Required]
        public string ReleaseMode
        {
            get => _releaseMode.ToString();
            set => _releaseMode = (ZigReleaseMode)Enum.Parse(typeof(ZigReleaseMode), value);
        }

        [Required]
        public ITaskItem[] Sanitizers { get; set; } = null!;

        [Required]
        public ITaskItem[] Sources { get; set; } = null!;

        [Required]
        public string SymbolExports
        {
            get => _symbolExports.ToString();
            set => _symbolExports = (ZigSymbolExports)Enum.Parse(typeof(ZigSymbolExports), value);
        }

        [Required]
        public string SymbolVisibility
        {
            get => _symbolVisibility.ToString();
            set => _symbolVisibility = (ZigSymbolVisibility)Enum.Parse(typeof(ZigSymbolVisibility), value);
        }

        [Required]
        public string TargetTriple { get; set; } = null!;

        public string? TestFilter { get; set; }

        [Required]
        public bool TreatWarningsAsErrors { get; set; }

        [Required]
        public int WarningLevel { get; set; }

        protected override string ToolName =>
            RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "zig.exe" : "zig";

        ZigCompilerMode _compilerMode;

        ZigConfiguration _configuration;

        ZigOutputType _outputType;

        ZigReleaseMode _releaseMode;

        ZigSymbolExports _symbolExports;

        ZigSymbolVisibility _symbolVisibility;

        protected override string GenerateFullPathToTool()
        {
            return ToolExe;
        }

        protected override string GenerateCommandLineCommands()
        {
            var builder = new CommandLineBuilderExtension();

            var isTest = _compilerMode == ZigCompilerMode.Test;
            var isZig = _compilerMode == ZigCompilerMode.Zig || isTest;

            builder.AppendTextUnquoted((_compilerMode, _outputType) switch
            {
                (ZigCompilerMode.C, _) => "cc",
                (ZigCompilerMode.Cxx, _) => "c++",
                (ZigCompilerMode.Zig, ZigOutputType.Exe) => "build-exe",
                (ZigCompilerMode.Zig, ZigOutputType.Library) => "build-lib",
                (ZigCompilerMode.Test, _) => "test",
                _ => throw new Exception(),
            });

            // This enables MSBuild to recognize diagnostics properly. Make sure
            // we pass this immediately after the cc/c++ command.
            if (!isZig)
                builder.AppendTextUnquoted(" -fdiagnostics-format=msvc");

            builder.AppendTextUnquoted($" -target {TargetTriple.ToLowerInvariant()}");

            if (_outputType == ZigOutputType.Exe)
            {
                builder.AppendTextUnquoted(" -fPIE");

                if (_symbolExports == ZigSymbolExports.All)
                    builder.AppendTextUnquoted(" -rdynamic");
            }
            else
            {
                builder.AppendTextUnquoted(isZig ? " -dynamic" : " -shared");
                builder.AppendTextUnquoted(" -fPIC");
            }

            if (isZig)
            {
                builder.AppendTextUnquoted((_configuration, _releaseMode) switch
                {
                    (ZigConfiguration.Debug, _) => " -O Debug",
                    (ZigConfiguration.Release, var m) => $" -O Release{m}",
                    _ => throw new Exception(),
                });

                if (!DebugSymbols)
                    builder.AppendTextUnquoted(" --strip");
            }
            else
            {
                if (_symbolVisibility == ZigSymbolVisibility.Hidden)
                    builder.AppendTextUnquoted(" -fvisibility=hidden");

                builder.AppendTextUnquoted($" -std={LanguageStandard.ToLowerInvariant()}");

                if (BlockExtensions)
                    builder.AppendTextUnquoted(" -fblocks");

                if (MicrosoftExtensions)
                    builder.AppendTextUnquoted(" -fms-extensions");

                if (_compilerMode == ZigCompilerMode.C)
                {
                    if (AttributeExtensions)
                        builder.AppendTextUnquoted(" -fdouble-square-bracket-attributes");

                    if (CxxExceptions)
                        builder.AppendTextUnquoted(" -fexceptions");
                }
                else if (_compilerMode == ZigCompilerMode.Cxx)
                {
                    if (!AccessControl)
                        builder.AppendTextUnquoted(" -fno-access-control");

                    if (!CxxReflection)
                        builder.AppendTextUnquoted(" -fno-rtti");

                    if (!CxxExceptions)
                        builder.AppendTextUnquoted(" -fno-exceptions");
                }

                builder.AppendTextUnquoted(" -fno-strict-aliasing");
                builder.AppendTextUnquoted(" -fno-strict-overflow");

                if (FastMath)
                    builder.AppendTextUnquoted("-ffast-math");

                if (WarningLevel > 0)
                {
                    if (WarningLevel >= 2)
                        builder.AppendTextUnquoted(" -Wall");

                    if (WarningLevel >= 3)
                        builder.AppendTextUnquoted(" -Wextra");

                    if (WarningLevel >= 4)
                        builder.AppendTextUnquoted(" -Wdocumentation");

                    if (TreatWarningsAsErrors)
                        builder.AppendTextUnquoted(" -Werror");
                }
                else
                    builder.AppendTextUnquoted(" -w");

                if (Deterministic)
                {
                    builder.AppendSwitch("-Werror=date-time");
                    builder.AppendSwitch("-no-canonical-prefixes");
                    builder.AppendSwitchIfNotNull("-fdebug-compilation-dir ", GetWorkingDirectory() ?? ".");
                }

                // These exact flags are treated specially by zig cc/c++. They
                // activate Debug, ReleaseFast, ReleaseSafe, and ReleaseSmall
                // respectively. This in turns activates a bunch of other
                // mode-specific flags that we do not have to specify here as a
                // result.
                builder.AppendTextUnquoted((_configuration, _releaseMode) switch
                {
                    (ZigConfiguration.Debug, _) => " -Og",
                    (ZigConfiguration.Release, ZigReleaseMode.Fast) => " -O3",
                    (ZigConfiguration.Release, ZigReleaseMode.Safe) => " -O2 -fsanitize=undefined",
                    (ZigConfiguration.Release, ZigReleaseMode.Small) => " -Oz",
                    _ => throw new Exception(),
                });

                if (DebugSymbols)
                    builder.AppendTextUnquoted(" -g");
            }

            foreach (var item in Sanitizers)
            {
                switch (item.ItemSpec.ToLowerInvariant())
                {
                    case "undefined":
                        Log.LogWarning("The '{0}' sanitizer is controlled by '{1}'",
                            item, nameof(ReleaseMode));
                        break;
                    case "thread" when isZig:
                        builder.AppendTextUnquoted(" -fsanitize-thread");
                        break;
                    default:
                        if (!isZig)
                            builder.AppendTextUnquoted($" -fsanitize={item.ItemSpec}");
                        else
                            Log.LogWarning("The '{0}' sanitizer is not supported with '{1}={2}'",
                                item, nameof(CompilerMode), ZigCompilerMode.Zig);
                        break;
                }
            }

            if (_configuration != ZigConfiguration.Debug)
                builder.AppendTextUnquoted(" -flto");

            foreach (var define in (DefineConstants ?? string.Empty).Split(new[] { ';' }, SplitOptions))
            {
                var trimmed = define.Trim();

                if (!string.IsNullOrEmpty(trimmed))
                    builder.AppendTextUnquoted($" -D{trimmed}");
            }

            builder.AppendSwitchIfNotNull("-I ", PublicIncludeDirectory);
            builder.AppendSwitchIfNotNull("-I ", IncludeDirectories, " ");

            if (!isZig)
                builder.AppendSwitchIfNotNull("-include ", PreludeHeaders, " ");

            // TODO: Library references?

            builder.AppendFileNamesIfNotNull(Sources, " ");
            builder.AppendSwitchIfNotNull(isZig ? "-femit-bin=" : "-o ", OutputBinary);

            if (isTest)
                builder.AppendSwitchIfNotNull("--test-filter ", TestFilter);

            return builder.ToString();
        }
    }
}
