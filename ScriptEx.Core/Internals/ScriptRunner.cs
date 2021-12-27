using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ScriptEx.Shared;

namespace ScriptEx.Core.Internals
{
    internal class ScriptRunner
    {
        private readonly IScriptEngineRegistry engineRegistry;

        public ScriptRunner(IScriptEngineRegistry engineRegistry)
        {
            this.engineRegistry = engineRegistry;
        }

        public async Task<ScriptResult> Run(string file, CancellationToken cancellationToken = default)
        {
            var engine = FindEngine(file);
            if (engine is null)
                return new ScriptResult(string.Empty, $"No valid engine for \"{file}\" found.", -1);

            return await engine.Run(file, cancellationToken);
        }

        private IScriptEngine? FindEngine(string file)
            => engineRegistry.RegisteredEngines.FirstOrDefault(o => Path.GetExtension(file).ToLowerInvariant() == o.FileExtension);
    }
}
