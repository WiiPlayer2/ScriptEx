using System;
using System.Collections.Generic;

namespace ScriptEx.Shared
{
    public record CronEntry(string Expression, string Arguments);

    public record ScriptMetaData(IReadOnlyList<CronEntry> CronEntries);

    public record ScriptResult(
        string StandardOutput,
        string StandardError,
        int ExitCode);

    public record ScriptExecution(
        DateTimeOffset StartTime,
        DateTimeOffset EndTime,
        string File,
        string Arguments,
        ScriptResult Result)
    {
        public TimeSpan Duration { get; } = EndTime - StartTime;
    }
}
