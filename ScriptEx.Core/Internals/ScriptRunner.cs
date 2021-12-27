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
            var engine = FindEngineByExtension(Path.GetExtension(file).ToLowerInvariant());
            if (engine is null)
                return new ScriptResult(string.Empty, $"No valid engine for \"{file}\" found.", -1);

            return await engine.Run(file, cancellationToken);
        }

        public async Task<ScriptResult> Execute(string language, string contents, CancellationToken cancellationToken = default)
        {
            var engine = FindEngineByLanguage(language);
            if (engine is null)
                return new ScriptResult(string.Empty, $"No valid engine for language \"{language}\" found.", -1);

            return await engine.Execute(contents, cancellationToken);
        }

        private IScriptEngine? FindEngineByExtension(string fileExtension)
            => engineRegistry.RegisteredEngines.FirstOrDefault(o => o.FileExtension == fileExtension);

        private IScriptEngine? FindEngineByLanguage(string language)
            => engineRegistry.RegisteredEngines.FirstOrDefault(o => o.LanguageIdentifier == language);
    }
}
