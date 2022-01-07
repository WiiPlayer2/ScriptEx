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

        public ScriptHandler(
            IOptions<AppOptions> appOptions,
            IScriptEngineRegistry engineRegistry,
            IScriptHistoryRepository historyRepository,
            PathFinder pathFinder,
            ITopicEventSender topicEventSender)
        {
            this.appOptions = appOptions.Value;
            this.engineRegistry = engineRegistry;
            this.historyRepository = historyRepository;
            this.pathFinder = pathFinder;
            this.topicEventSender = topicEventSender;
        }

        public async Task<ScriptMetaData?> GetMetaData(string relativePath, CancellationToken cancellationToken = default)
        {
            var engine = engineRegistry.GetEngineForFile(relativePath);
            if (engine == null)
                return null;

            var absolutePath = pathFinder.GetAbsolutePath(relativePath);
            if (!File.Exists(absolutePath))
                return null;

            var contents = await File.ReadAllTextAsync(absolutePath, cancellationToken);
            return new ScriptMetaDataScanner(engine.SingleLineCommentSymbol).GetMetaData(contents);
        }

        public async Task<ScriptResult> Run(string relativePath, CancellationToken cancellationToken = default)
        {
            var engine = engineRegistry.GetEngineForFile(relativePath);
            if (engine is null)
                return new ScriptResult(string.Empty, $"No valid engine for \"{relativePath}\" found.", -1);

            var scriptPath = pathFinder.GetAbsolutePath(relativePath);
            var startTime = DateTimeOffset.Now;
            var result = await engine.Run(scriptPath, cancellationToken);
            var endTime = DateTimeOffset.Now;

            var execution = new ScriptExecution(startTime, endTime, pathFinder.GetRelativePath(scriptPath), string.Empty, result);
            await historyRepository.AddHistory(execution);
            await topicEventSender.SendAsync(Subscription.TOPIC_SCRIPT_EXECUTED, execution, CancellationToken.None);

            return result;
        }
    }
}
