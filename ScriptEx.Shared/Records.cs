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
        DateTime StartTime,
        DateTime EndTime,
        TimeSpan Duration,
        string Arguments,
        ScriptResult Result);
}
