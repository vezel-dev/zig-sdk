// SPDX-License-Identifier: 0BSD

using Microsoft.Build.Utilities;

namespace Vezel.Zig.Tasks;

public abstract class ZigToolTask : ToolTask
{
    protected override sealed string ToolName =>
        RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "zig.exe" : "zig";

    protected override sealed string GenerateFullPathToTool()
    {
        return ToolExe;
    }
}
