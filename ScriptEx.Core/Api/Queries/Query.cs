using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HotChocolate;
using Microsoft.Extensions.Options;
using ScriptEx.Core.Api.Types;
using ScriptEx.Shared;
using Path = System.IO.Path;

namespace ScriptEx.Core.Api.Queries
{
    public class Query
    {
        public IReadOnlyCollection<IScriptEngine> GetEngines(
            [Service] IScriptEngineRegistry engineRegistry)
            => engineRegistry.RegisteredEngines;
        
        public IEnumerable<Entry> GetScripts(
            string? path,
            [Service] IOptions<AppOptions> appOptions)
            => new DirectoryEntry(string.Empty, Path.Join(appOptions.Value.ScriptsPath, path ?? ".")).GetScripts();
    }
}
