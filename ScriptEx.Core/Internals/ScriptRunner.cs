using System;
using System.IO;
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

        private readonly IScriptHistoryRepository historyRepository;

        private readonly PathFinder pathFinder;

        public ScriptRunner(IOptions<AppOptions> appOptions, IScriptEngineRegistry engineRegistry, IScriptHistoryRepository historyRepository, PathFinder pathFinder)
        {
            this.appOptions = appOptions.Value;
            this.engineRegistry = engineRegistry;
            this.historyRepository = historyRepository;
            this.pathFinder = pathFinder;
        }

        public async Task<ScriptResult> Run(string file, CancellationToken cancellationToken = default)
        {
            var engine = engineRegistry.GetEngineForFile(file);
            if (engine is null)
                return new ScriptResult(string.Empty, $"No valid engine for \"{file}\" found.", -1);

            var scriptPath = Path.Join(appOptions.ScriptsPath, file);
            var startTime = DateTimeOffset.Now;
            var result = await engine.Run(scriptPath, cancellationToken);
            var endTime = DateTimeOffset.Now;

            await historyRepository.AddHistory(new ScriptExecution(startTime, endTime, pathFinder.GetFilePath(scriptPath), string.Empty, result));

            return result;
        }
    }
}
