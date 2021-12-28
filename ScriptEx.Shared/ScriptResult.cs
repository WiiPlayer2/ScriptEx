using System;

namespace ScriptEx.Shared
{
    public record ScriptResult(
        string StandardOutput,
        string StandardError,
        int ExitCode);
}
