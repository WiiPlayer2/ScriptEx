using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Shuttle.Core.Cron;

namespace ScriptEx.Core.Internals
{
    public class ScriptScheduler : IAsyncDisposable
    {
        private readonly List<(CronExpression Expression, string Arguments)> cronEntries = new();

        private readonly CancellationTokenSource globalCancellationTokenSource = new();

        private readonly string relativePath;

        private readonly IScriptHandler scriptHandler;

        private CancellationTokenSource? currentCancellationTokenSource;

        private Task? currentlyScheduledTask;

        public ScriptScheduler(string relativePath, IScriptHandler scriptHandler)
        {
            this.relativePath = relativePath;
            this.scriptHandler = scriptHandler;
        }

        public async ValueTask DisposeAsync()
        {
            globalCancellationTokenSource?.Cancel();
            if (currentlyScheduledTask == null)
                return;

            await currentlyScheduledTask.ContinueWith(_ => { });
            currentlyScheduledTask = null;
        }

        public async Task Update()
        {
            currentCancellationTokenSource?.Cancel();

            var metaData = await scriptHandler.GetMetaData(relativePath);
            cronEntries.Clear();
            if (metaData == null || metaData.CronEntries.Count == 0)
                return;

            cronEntries.AddRange(metaData.CronEntries.Select(o => (new CronExpression(o.Expression), o.Arguments)));
            ScheduleNext();
        }

        private void ScheduleNext()
        {
            var now = DateTime.Now;
            var nextExecutionExpression = cronEntries
                .Select(o => (Time: o.Expression.NextOccurrence(now), o.Arguments))
                .OrderBy(o => o.Item1)
                .First();

            currentCancellationTokenSource = new CancellationTokenSource();
            var currentToken = CancellationTokenSource.CreateLinkedTokenSource(globalCancellationTokenSource.Token, currentCancellationTokenSource.Token).Token;
            currentlyScheduledTask = Task.Run(() => RunScriptAt(nextExecutionExpression.Time, nextExecutionExpression.Arguments, currentToken), currentToken);
        }

        private async Task RunScriptAt(DateTime dateTime, string arguments, CancellationToken cancellationToken)
        {
            var now = DateTime.Now;
            var delay = dateTime - now;
            if (delay >= TimeSpan.Zero)
                await Task.Delay(delay, cancellationToken);

            await scriptHandler.Run(relativePath, cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
            ScheduleNext();
        }
    }
}
