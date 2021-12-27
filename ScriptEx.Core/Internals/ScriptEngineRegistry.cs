using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ScriptEx.Shared;

namespace ScriptEx.Core.Internals
{
    internal class ScriptEngineRegistry : IScriptEngineRegistry
    {
        private readonly HashSet<IScriptEngine> engines = new();

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
