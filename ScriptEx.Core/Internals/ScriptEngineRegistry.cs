using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using ScriptEx.Core.Engines;
using ScriptEx.Shared;

namespace ScriptEx.Core.Internals
{
    internal class ScriptEngineRegistry : IScriptEngineRegistry
    {
        private readonly HashSet<IScriptEngine> engines = new();

        public ScriptEngineRegistry(IOptions<AppOptions> appOptions)
        {
            foreach (var engineConfiguration in appOptions.Value.Engines)
                Register(new ConfiguredEngine(engineConfiguration));
        }

        public IReadOnlyCollection<IScriptEngine> RegisteredEngines => engines;

        public Task Register(IScriptEngine engine)
        {
            engines.Add(engine);
            return Task.CompletedTask;
        }

        public Task Unregister(IScriptEngine engine)
        {
            engines.Remove(engine);
            return Task.CompletedTask;
        }
    }
}
