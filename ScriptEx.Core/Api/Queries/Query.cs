using System;
using System.Collections.Generic;
using HotChocolate;
using ScriptEx.Shared;

namespace ScriptEx.Core.Api.Queries
{
    public class Query
    {
        public IReadOnlyCollection<IScriptEngine> GetEngines(
            [Service] IScriptEngineRegistry engineRegistry)
            => engineRegistry.RegisteredEngines;
    }
}
