using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using ScriptEx.Shared;

namespace ScriptEx.Core.Internals
{
    internal class ScriptRunner : IScriptRunner
    {
        private readonly AppOptions appOptions;

        private readonly IScriptEngineRegistry engineRegistry;

        public ScriptRunner(IOptions<AppOptions> appOptions, IScriptEngineRegistry engineRegistry)
        {
            this.appOptions = appOptions.Value;
            this.engineRegistry = engineRegistry;
        }

        public async Task<ScriptResult> Run(string file, CancellationToken cancellationToken = default)
        {
            var engine = FindEngineByExtension(Path.GetExtension(file).ToLowerInvariant());
            if (engine is null)
                return new ScriptResult(string.Empty, $"No valid engine for \"{file}\" found.", -1);

            return await engine.Run(Path.Join(appOptions.ScriptsPath, file), cancellationToken);
        }

        public async Task<ScriptResult> Execute(string language, string script, CancellationToken cancellationToken = default)
        {
            var engine = FindEngineByLanguage(language);
            if (engine is null)
                return new ScriptResult(string.Empty, $"No valid engine for language \"{language}\" found.", -1);

            return await engine.Execute(script, cancellationToken);
        }

        private IScriptEngine? FindEngineByExtension(string fileExtension)
            => engineRegistry.RegisteredEngines.FirstOrDefault(o => o.FileExtension == fileExtension);

        private IScriptEngine? FindEngineByLanguage(string language)
            => engineRegistry.RegisteredEngines.FirstOrDefault(o => o.LanguageIdentifier == language);
    }
}
