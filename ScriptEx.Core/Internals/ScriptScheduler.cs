using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ScriptEx.Shared;
using Shuttle.Core.Cron;

namespace ScriptEx.Core.Internals
{
    public class ScriptScheduler : IAsyncDisposable
    {
        private readonly List<CronEntry> cronEntries = new();

        private readonly CancellationTokenSource globalCancellationTokenSource = new();

        private readonly IScriptHandler scriptHandler;

        private readonly string scriptPath;

        private CancellationTokenSource? currentCancellationTokenSource;

        private Task? currentlyScheduledTask;

        public ScriptScheduler(string scriptPath, IScriptHandler scriptHandler)
        {
            this.scriptPath = scriptPath;
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

            if (!File.Exists(scriptPath))
                return;

            var metaData = await scriptHandler.GetMetaData(scriptPath);
            cronEntries.Clear();
            if (metaData == null || metaData.CronEntries.Count == 0)
                return;

            cronEntries.AddRange(metaData.CronEntries);
            ScheduleNext();
        }

        private void ScheduleNext()
        {
            var now = DateTime.Now;
            var nextExecutionExpression = cronEntries.Select(o => (Time: new CronExpression(o.Expression).GetNextOccurrence(now), o.Arguments)).OrderBy(o => o.Item1).First();
            currentCancellationTokenSource = new CancellationTokenSource();
            var currentToken = CancellationTokenSource.CreateLinkedTokenSource(globalCancellationTokenSource.Token, currentCancellationTokenSource.Token).Token;
            currentlyScheduledTask = Task.Run(() => RunScriptAt(nextExecutionExpression.Time, nextExecutionExpression.Arguments, currentToken), currentToken);
        }

        private async Task RunScriptAt(DateTime dateTime, string arguments, CancellationToken cancellationToken)
        {
            await Task.Delay(dateTime - DateTime.Now, cancellationToken);
            await scriptHandler.Run(scriptPath, cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
            ScheduleNext();
        }
    }
}
