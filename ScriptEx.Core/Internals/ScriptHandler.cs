using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using HotChocolate.Subscriptions;
using Microsoft.Extensions.Options;
using ScriptEx.Core.Api.Subscriptions;
using ScriptEx.Shared;

namespace ScriptEx.Core.Internals
{
    internal class ScriptHandler : IScriptHandler
    {
        private readonly AppOptions appOptions;

        private readonly IScriptEngineRegistry engineRegistry;

        private readonly IScriptHistoryRepository historyRepository;

        private readonly PathFinder pathFinder;

        private readonly ITopicEventSender topicEventSender;

        public ScriptHandler(IOptions<AppOptions> appOptions, IScriptEngineRegistry engineRegistry, IScriptHistoryRepository historyRepository, PathFinder pathFinder, ITopicEventSender topicEventSender)
        {
            this.appOptions = appOptions.Value;
            this.engineRegistry = engineRegistry;
            this.historyRepository = historyRepository;
            this.pathFinder = pathFinder;
            this.topicEventSender = topicEventSender;
        }

        public async Task<ScriptMetaData?> GetMetaData(string file, CancellationToken cancellationToken = default)
        {
            var engine = engineRegistry.GetEngineForFile(file);
            if (engine == null)
                return null;

            var contents = await File.ReadAllTextAsync(file, cancellationToken);
            return new ScriptMetaDataScanner(engine.SingleLineCommentSymbol).GetMetaData(contents);
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

            var execution = new ScriptExecution(startTime, endTime, pathFinder.GetFilePath(scriptPath), string.Empty, result);
            await historyRepository.AddHistory(execution);
            await topicEventSender.SendAsync(Subscription.TOPIC_SCRIPT_EXECUTED, execution, CancellationToken.None);

            return result;
        }
    }
}
