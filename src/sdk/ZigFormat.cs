using Microsoft.Build.Framework;
using Microsoft.Build.Tasks;

namespace Zig.Tasks;

[SuppressMessage("", "CA1819")]
public sealed class ZigFormat : ZigToolTask
{
    [Required]
    public string FormatterMode
    {
        get => _formatterMode.ToString();
        set => _formatterMode = (ZigFormatterMode)Enum.Parse(typeof(ZigFormatterMode), value);
    }

    [Required]
    public ITaskItem[] Sources { get; set; } = null!;

    ZigFormatterMode _formatterMode;

    protected override string GenerateCommandLineCommands()
    {
        var builder = new CommandLineBuilderExtension();

        builder.AppendSwitch("fmt");

        if (_formatterMode == ZigFormatterMode.Check)
            builder.AppendSwitch("--check");

        builder.AppendFileNamesIfNotNull(Sources, " ");

        return builder.ToString();
    }

    protected override bool HandleTaskExecutionErrors()
    {
        if (_formatterMode == ZigFormatterMode.Execute)
            return base.HandleTaskExecutionErrors();

        // In check mode, zig fmt will just print a list of files to standard
        // output and exit with a non-zero code. This causes
        // ToolTask.HandleTaskExecutionErrors to log an error notifying the user
        // of the exit code. This can be a bit confusing to a user who is not
        // already familiar with zig fmt's behavior. So, we try to present a
        // more actionable error message to the user.
        //
        // Note that zig fmt will actually log errors if the files contain
        // syntax errors, so we should not log our message in that case.
        if (!HasLoggedErrors)
            Log.LogError("The above files have incorrect code formatting (run the 'Format' target to fix them)");

        return false;
    }
}
