using System;
using System.Collections.Generic;

namespace ScriptEx.Shared
{
    public record CronEntry(string Expression, string Arguments);

    public record ScriptMetaData(IReadOnlyList<CronEntry> CronEntries);
}
