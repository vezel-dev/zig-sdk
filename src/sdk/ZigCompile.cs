using Microsoft.Build.Framework;
using Microsoft.Build.Tasks;

namespace Vezel.Zig.Tasks;

[SuppressMessage("", "CA1819")]
public sealed class ZigCompile : ZigToolTask
{
    private const StringSplitOptions SplitOptions = StringSplitOptions.RemoveEmptyEntries;

    private const StringComparison Comparison = StringComparison.InvariantCulture;

    [Required]
    public bool AccessControl { get; set; }

    [Required]
    public bool BlockExtensions { get; set; }

    public ITaskItem? CommandFragmentsDirectory { get; set; }

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
    public bool ConsumptionAnalysis { get; set; }

    [Required]
    public bool CxxExceptions { get; set; }

    [Required]
    public bool CxxReflection { get; set; }

    [Required]
    public bool DebugSymbols { get; set; }

    public string? DefineConstants { get; set; }

    [Required]
    public bool Deterministic { get; set; }

    public string? DisableWarnings { get; set; }

    [Required]
    public bool DocumentationAnalysis { get; set; }

    [Required]
    public bool EagerBinding { get; set; }

    [Required]
    public bool FastMath { get; set; }

    [Required]
    public ITaskItem[] IncludeDirectories { get; set; } = null!;

    [Required]
    public string LanguageStandard { get; set; } = null!;

    [Required]
    public ITaskItem[] LibraryIncludeDirectories { get; set; } = null!;

    [Required]
    public ITaskItem[] LibraryReferences { get; set; } = null!;

    [Required]
    public ITaskItem[] LinkerDirectories { get; set; } = null!;

    [Required]
    public ITaskItem[] LinkerReferences { get; set; } = null!;

    [Required]
    public bool LinkTimeOptimization { get; set; }

    [Required]
    public bool MicrosoftExtensions { get; set; }

    [Required]
    public bool NullabilityAnalysis { get; set; }

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

    public ITaskItem? PublicIncludeDirectory { get; set; }

    [Required]
    public string ReleaseMode
    {
        get => _releaseMode.ToString();
        set => _releaseMode = (ZigReleaseMode)Enum.Parse(typeof(ZigReleaseMode), value);
    }

    [Required]
    public bool RelocationHardening { get; set; }

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
    public bool TagAnalysis { get; set; }

    [Required]
    public string TargetFileName { get; set; } = null!;

    [Required]
    public string TargetRuntimeIdentifier { get; set; } = null!;

    [Required]
    public string TargetTriple { get; set; } = null!;

    public string? TestFilter { get; set; }

    [Required]
    public bool ThreadingAnalysis { get; set; }

    [Required]
    public bool TreatWarningsAsErrors { get; set; }

    [Required]
    public bool TrustAnalysis { get; set; }

    [Required]
    public int WarningLevel { get; set; }

    private ZigCompilerMode _compilerMode;

    private ZigConfiguration _configuration;

    private ZigOutputType _outputType;

    private ZigReleaseMode _releaseMode;

    private ZigSymbolExports _symbolExports;

    private ZigSymbolVisibility _symbolVisibility;

    [SuppressMessage("", "CA2201")]
    [SuppressMessage("", "CA1308")]
    protected override string GenerateCommandLineCommands()
    {
        var builder = new CommandLineBuilderExtension();

        var isTest = _compilerMode == ZigCompilerMode.Test;
        var isZig = _compilerMode == ZigCompilerMode.Zig || isTest;
        var isCxx = _compilerMode == ZigCompilerMode.Cxx;

        builder.AppendSwitch((_compilerMode, _outputType) switch
        {
            (ZigCompilerMode.C, _) => "cc",
            (ZigCompilerMode.Cxx, _) => "c++",
            (ZigCompilerMode.Zig, ZigOutputType.Exe) => "build-exe",
            (ZigCompilerMode.Zig, ZigOutputType.Library) => "build-lib",
            (ZigCompilerMode.Test, _) => "test",
            _ => throw new Exception(),
        });

        // This enables MSBuild to recognize diagnostics properly. Make sure we
        // pass this immediately after the cc/c++ command.
        if (!isZig)
            builder.AppendSwitch("-fdiagnostics-format=msvc");

        builder.AppendSwitch($"-target {TargetTriple.ToLowerInvariant()}");

        if (_outputType == ZigOutputType.Exe)
        {
            builder.AppendSwitch("-fPIE");

            if (_symbolExports == ZigSymbolExports.All)
                builder.AppendSwitch("-rdynamic");
        }
        else
        {
            builder.AppendSwitch("-fPIC");

            if (!isZig)
            {
                builder.AppendSwitch("-shared");
                builder.AppendSwitchIfNotNull("-Wl,-soname,", TargetFileName);
            }
        }

        if (isZig)
        {
            // The compiler uses static linking by default when building Zig
            // code. We want dynamic linking in all cases.
            builder.AppendSwitch("-dynamic");

            // When building Zig code, by default, the compiler links statically
            // to a platform-appropriate libc. We absolutely do not want that
            // behavior when building code that might be loaded in a .NET
            // process.
            builder.AppendSwitch("-lc");

            if (_configuration == ZigConfiguration.Release)
                builder.AppendSwitch($"-O Release{_releaseMode}");

            builder.AppendSwitch(DebugSymbols ? "-fno-strip" : "-fstrip");
        }
        else
        {
            // These exact flags are treated specially by zig cc/c++. They
            // activate Debug, ReleaseFast, ReleaseSafe, and ReleaseSmall
            // respectively. This in turns activates a bunch of other
            // mode-specific flags that we do not have to specify here as a
            // result.
            builder.AppendSwitch((_configuration, _releaseMode) switch
            {
                (ZigConfiguration.Debug, _) => "-O0",
                (ZigConfiguration.Release, ZigReleaseMode.Fast) => "-O2",
                (ZigConfiguration.Release, ZigReleaseMode.Safe) => "-O2 -fsanitize=undefined",
                (ZigConfiguration.Release, ZigReleaseMode.Small) => "-Os",
                _ => throw new Exception(),
            });

            if (DebugSymbols)
                builder.AppendSwitch("-g");

            if (_symbolVisibility == ZigSymbolVisibility.Hidden)
                builder.AppendSwitch("-fvisibility=hidden");

            builder.AppendSwitch($"-std={LanguageStandard.ToLowerInvariant()}");

            if (BlockExtensions)
                builder.AppendSwitch("-fblocks");

            if (MicrosoftExtensions)
            {
                builder.AppendSwitch("-fms-extensions");

                builder.AppendSwitch("-Wno-microsoft-abstract");
                builder.AppendSwitch("-Wno-microsoft-anon-tag");
                builder.AppendSwitch("-Wno-microsoft-union-member-reference");
            }

            if (isCxx)
            {
                if (!AccessControl)
                    builder.AppendSwitch("-fno-access-control");

                if (!CxxReflection)
                    builder.AppendSwitch("-fno-rtti");

                if (!CxxExceptions)
                    builder.AppendSwitch("-fno-exceptions");
            }
            else if (CxxExceptions)
                builder.AppendSwitch("-fexceptions");

            builder.AppendSwitch("-fno-strict-aliasing");

            if (FastMath)
                builder.AppendSwitch("-ffast-math");

            if (TreatWarningsAsErrors)
                builder.AppendSwitch("-Werror");

            var disabledWarnings = new HashSet<string>(
                (DisableWarnings ?? string.Empty)
                    .Split([';'], SplitOptions)
                    .Select(w => w.Trim())
                    .Where(w =>
                    {
                        if (string.IsNullOrEmpty(w))
                            return false;

                        if (w.StartsWith("no-", Comparison))
                        {
                            Log.LogWarning("The 'no-' prefix on warning '{0}' is invalid", w[3..]);
                            return false;
                        }

                        if (w.StartsWith("error=", Comparison))
                        {
                            Log.LogWarning("Changing specific warning '{0}' to error is not supported", w[6..]);
                            return false;
                        }

                        if (w == "error")
                        {
                            Log.LogWarning(
                                "Changing all warnings to errors should be done with '{0}'",
                                nameof(TreatWarningsAsErrors));
                            return false;
                        }

                        string? property = null;

                        if (w.StartsWith("consumed", Comparison))
                            property = nameof(ConsumptionAnalysis);
                        else if (w.StartsWith("documentation", Comparison))
                            property = nameof(DocumentationAnalysis);
                        else if (w.StartsWith("microsoft", Comparison))
                            property = nameof(MicrosoftExtensions);
                        else if (w.StartsWith("nullability", Comparison) ||
                            w.StartsWith("nullable", Comparison))
                            property = nameof(NullabilityAnalysis);
                        else if (w.StartsWith("tcb-enforcement", Comparison))
                            property = nameof(TrustAnalysis);
                        else if (w.StartsWith("type-safety", Comparison))
                            property = nameof(TagAnalysis);
                        else if (w.StartsWith("thread-safety", Comparison))
                            property = nameof(ThreadingAnalysis);

                        if (property != null)
                        {
                            Log.LogWarning("The '{0}' warning is controlled by '{1}'", w, property);
                            return false;
                        }

                        return true;
                    }));

            void TryAppendWarningSwitch(string name)
            {
                // Try to avoid adding a warning flag if the user explicitly
                // disabled it. This will not cover every possible case due to
                // aggregate flags, but it will at least prevent some amount of
                // command line length explosion.
                if (!disabledWarnings.Contains(name))
                    builder.AppendSwitch($"-W{name}");
            }

            // Unfortunately, a lot of good warnings that really should be on by
            // default are not. So, we have to keep a manual list of extra
            // warnings to enable and make sure to keep it in sync with whatever
            // LLVM/Clang version Zig is shipping with.
            switch (WarningLevel)
            {
                case <= 0:
                    builder.AppendSwitch("-Wno-everything");
                    break;
                case 1:
                    TryAppendWarningSwitch("alloca");
                    TryAppendWarningSwitch("non-gcc");
                    TryAppendWarningSwitch("reserved-identifier");
                    TryAppendWarningSwitch("signed-enum-bitfield");

                    if (isCxx)
                    {
                        TryAppendWarningSwitch("class-varargs");
                        TryAppendWarningSwitch("non-virtual-dtor");
                        TryAppendWarningSwitch("undefined-reinterpret-cast");
                    }

                    foreach (var warning in disabledWarnings)
                        builder.AppendSwitch($"-Wno-{warning}");

                    break;
                case 2:
                    TryAppendWarningSwitch("all");
                    TryAppendWarningSwitch("array-bounds-pointer-arithmetic");
                    TryAppendWarningSwitch("c++-compat");
                    TryAppendWarningSwitch("cast-align");
                    TryAppendWarningSwitch("cast-qual");
                    TryAppendWarningSwitch("comma");
                    TryAppendWarningSwitch("float-equal");
                    TryAppendWarningSwitch("pointer-arith");
                    TryAppendWarningSwitch("shift-sign-overflow");

                    goto case 1;
                case 3:
                    TryAppendWarningSwitch("anon-enum-enum-conversion");
                    TryAppendWarningSwitch("assign-enum");
                    TryAppendWarningSwitch("completion-handler");
                    TryAppendWarningSwitch("conditional-uninitialized");
                    TryAppendWarningSwitch("deprecated");
                    TryAppendWarningSwitch("extra");
                    TryAppendWarningSwitch("format-pedantic");
                    TryAppendWarningSwitch("format-type-confusion");
                    TryAppendWarningSwitch("implicit-fallthrough");
                    TryAppendWarningSwitch("keyword-macro");
                    TryAppendWarningSwitch("loop-analysis");
                    TryAppendWarningSwitch("over-aligned");
                    TryAppendWarningSwitch("shadow-all");
                    TryAppendWarningSwitch("switch-enum");

                    if (isCxx)
                    {
                        TryAppendWarningSwitch("inconsistent-missing-destructor-override");
                        TryAppendWarningSwitch("suggest-destructor-override");
                        TryAppendWarningSwitch("suggest-override");
                    }

                    goto case 2;
                case 4:
                default:
                    TryAppendWarningSwitch("bad-function-cast");
                    TryAppendWarningSwitch("compound-token-split");
                    TryAppendWarningSwitch("covered-switch-default");
                    TryAppendWarningSwitch("duplicate-decl-specifier");
                    TryAppendWarningSwitch("duplicate-enum");
                    TryAppendWarningSwitch("embedded-directive");
                    TryAppendWarningSwitch("expansion-to-defined");
                    TryAppendWarningSwitch("extra-semi");
                    TryAppendWarningSwitch("format=2");
                    TryAppendWarningSwitch("four-char-constants");
                    TryAppendWarningSwitch("missing-noreturn");
                    TryAppendWarningSwitch("redundant-parens");
                    TryAppendWarningSwitch("undef");
                    TryAppendWarningSwitch("unreachable-code-aggressive");

                    if (isCxx)
                    {
                        TryAppendWarningSwitch("atomic-implicit-seq-cst");
                        TryAppendWarningSwitch("ctad-maybe-unsupported");
                        TryAppendWarningSwitch("dtor-name");
                        TryAppendWarningSwitch("header-hygiene");
                        TryAppendWarningSwitch("old-style-cast");
                        TryAppendWarningSwitch("undefined-func-template");
                        TryAppendWarningSwitch("unsupported-dll-base-class-template");
                        TryAppendWarningSwitch("unused-exception-parameter");
                        TryAppendWarningSwitch("unused-member-function");
                        TryAppendWarningSwitch("unused-template");
                        TryAppendWarningSwitch("zero-as-null-pointer-constant");
                    }
                    else
                    {
                        TryAppendWarningSwitch("missing-prototypes");
                        TryAppendWarningSwitch("missing-variable-declarations");
                        TryAppendWarningSwitch("strict-prototypes");
                    }

                    goto case 3;
            }

            // The following -W flags need to be here because they have to be
            // enabled regardless of WarningLevel. If they came before
            // -Wno-everything (when WarningLevel is set to 0), they would have
            // no effect.
            builder.AppendSwitch("-Werror=newline-eof");

            if (ConsumptionAnalysis)
                builder.AppendSwitch("-Wconsumed");

            if (DocumentationAnalysis)
            {
                builder.AppendSwitch("-Wdocumentation");
                builder.AppendSwitch("-Wdocumentation-pedantic");
            }

            if (!NullabilityAnalysis)
            {
                builder.AppendSwitch("-Wno-nullability");
                builder.AppendSwitch("-Wno-nullability-completeness");
                builder.AppendSwitch("-Wno-nullability-inferred-on-nested-type");
            }
            else
                builder.AppendSwitch("-Wnullable-to-nonnull-conversion");

            if (!TagAnalysis)
                builder.AppendSwitch("-Wno-type-safety");

            if (ThreadingAnalysis)
                builder.AppendSwitch("-Wthread-safety");

            if (!TrustAnalysis)
                builder.AppendSwitch("-Wno-tcb-enforcement");

            // TODO: https://github.com/vezel-dev/zig-sdk/issues/38
            if (Deterministic)
            {
                builder.AppendSwitch("-Werror=date-time");
                builder.AppendSwitch("-no-canonical-prefixes");
                builder.AppendSwitchIfNotNull("-fdebug-compilation-dir ", GetWorkingDirectory() ?? ".");
            }
        }

        foreach (var item in Sanitizers)
        {
            switch (item.ItemSpec.ToLowerInvariant())
            {
                case "undefined":
                    Log.LogWarning("The '{0}' sanitizer is controlled by '{1}'", item, nameof(ReleaseMode));
                    break;
                case "thread" when isZig:
                    builder.AppendSwitch("-fsanitize-thread");
                    break;
                default:
                    if (!isZig)
                        builder.AppendSwitch($"-fsanitize={item.ItemSpec}");
                    else
                        Log.LogWarning(
                            "The '{0}' sanitizer is not supported with '{1}={2}'",
                            item,
                            nameof(CompilerMode),
                            ZigCompilerMode.Zig);
                    break;
            }
        }

        if (!LinkTimeOptimization)
            builder.AppendSwitch("-fno-lto");

        foreach (var define in (DefineConstants ?? string.Empty).Split([';'], SplitOptions))
        {
            var trimmed = define.Trim();

            if (string.IsNullOrEmpty(trimmed))
                continue;

            builder.AppendSwitchIfNotNull("-D ", trimmed);
        }

        builder.AppendSwitchIfNotNull("-I ", GetWorkingDirectory() ?? ".");
        builder.AppendSwitchIfNotNull("-I ", PublicIncludeDirectory);

        foreach (var directory in LibraryIncludeDirectories)
            builder.AppendSwitchIfNotNull("-isystem ", directory);

        foreach (var directory in IncludeDirectories)
            builder.AppendSwitchIfNotNull("-I ", directory);

        if (!isZig)
            foreach (var header in PreludeHeaders)
                builder.AppendSwitchIfNotNull("-include ", header);

        if (!EagerBinding)
            builder.AppendSwitch(isZig ? "-z lazy" : "-Wl,-z,lazy");

        if (!RelocationHardening)
            builder.AppendSwitch(isZig ? "-z norelro" : "-Wl,-z,norelro");

        builder.AppendSwitch(isZig ? "-z origin" : "-Wl,-z,origin");
        builder.AppendSwitchIfNotNull(isZig ? "-rpath " : "-Wl,-rpath,", "$ORIGIN");

        // TODO: https://github.com/vezel-dev/zig-sdk/issues/8

        builder.AppendFileNamesIfNotNull(Sources, " ");
        builder.AppendFileNamesIfNotNull(LibraryReferences, " ");

        foreach (var directory in LinkerDirectories)
            builder.AppendSwitchIfNotNull("-L ", directory);

        foreach (var library in LinkerReferences)
            builder.AppendSwitchIfNotNull("-l ", library);

        builder.AppendSwitchIfNotNull(isZig ? "-femit-bin=" : "-o ", OutputBinary);

        if (!isZig)
            builder.AppendSwitchIfNotNull("-gen-cdb-fragment-path ", CommandFragmentsDirectory);

        if (isTest)
        {
            builder.AppendSwitch("--test-no-exec");
            builder.AppendSwitchIfNotNull("--test-filter ", TestFilter);
        }

        return builder.ToString();
    }
}
