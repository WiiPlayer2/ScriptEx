using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using HotChocolate.Subscriptions;
using Microsoft.Extensions.Logging;
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

        private readonly ILogger logger;

        private readonly PathFinder pathFinder;

        private readonly ITopicEventSender topicEventSender;

        public ScriptHandler(
            ILogger<ScriptHandler> logger,
            IOptions<AppOptions> appOptions,
            IScriptEngineRegistry engineRegistry,
            IScriptHistoryRepository historyRepository,
            PathFinder pathFinder,
            ITopicEventSender topicEventSender)
        {
            this.appOptions = appOptions.Value;
            this.logger = logger;
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
            try
            {
                return new ScriptMetaDataScanner(engine.SingleLineCommentSymbol).GetMetaData(contents);
            }
            catch
            {
                return null;
            }
        }

        public async Task<ScriptResult> Run(string relativePath, string arguments, TimeSpan? timeout = default, CancellationToken cancellationToken = default)
        {
            var cmd = $"{relativePath} {arguments}".Trim();
            logger.LogDebug($"Running {cmd}...");

            var engine = engineRegistry.GetEngineForFile(relativePath);
            if (engine is null)
                return new ScriptResult(string.Empty, $"No valid engine for \"{relativePath}\" found.", -1);

            var metaData = await GetMetaData(relativePath, cancellationToken);
            if (metaData is null)
                return new ScriptResult(string.Empty, $"Failed to get meta data for \"{relativePath}\".", -1);

            var scriptPath = pathFinder.GetAbsolutePath(relativePath);
            var timedCancellationTokenSource = new CancellationTokenSource(timeout ?? metaData.DefaultTimeout ?? appOptions.DefaultTimeout);
            var linkedCancellationToken = CancellationTokenSource.CreateLinkedTokenSource(timedCancellationTokenSource.Token, cancellationToken).Token;
            var startTime = DateTimeOffset.Now;
            var result = await engine.Run(scriptPath, arguments, linkedCancellationToken)
                .IgnoreCancellation()
                .ButAsync(() => new ScriptResult(string.Empty, "Engine failed to return cooperatively.", -1));
            var endTime = DateTimeOffset.Now;

            var execution = new ScriptExecution(startTime, endTime, pathFinder.GetRelativePath(scriptPath), arguments, result);
            await historyRepository.AddHistory(execution);
            await topicEventSender.SendAsync(Subscription.TOPIC_SCRIPT_EXECUTED, execution, CancellationToken.None);

            logger.LogTrace(execution.ToString());
            logger.LogDebug($"Ran {cmd} ({execution.Duration}) with exit code {result.ExitCode}.");
            return result;
        }
    }
}
