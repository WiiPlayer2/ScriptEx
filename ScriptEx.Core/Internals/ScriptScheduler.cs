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
        private readonly List<Task> currentlyRunningTasks = new();

        private readonly CancellationTokenSource globalCancellationTokenSource = new();

        private readonly string relativePath;

        private readonly IScriptHandler scriptHandler;

        private CancellationTokenSource? currentCancellationTokenSource;

        public ScriptScheduler(string relativePath, IScriptHandler scriptHandler)
        {
            this.relativePath = relativePath;
            this.scriptHandler = scriptHandler;
        }

        public async ValueTask DisposeAsync()
        {
            globalCancellationTokenSource?.Cancel();

            var tasks = new List<Task>();
            lock (currentlyRunningTasks)
            {
                tasks.AddRange(currentlyRunningTasks);
            }

            await Task.WhenAll(tasks.Select(o => o.ContinueWith(_ => { })));
        }

        public async Task Update()
        {
            currentCancellationTokenSource?.Cancel();
            currentCancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = CancellationTokenSource.CreateLinkedTokenSource(currentCancellationTokenSource.Token, globalCancellationTokenSource.Token).Token;

            var metaData = await scriptHandler.GetMetaData(relativePath);
            if (metaData == null || metaData.CronEntries.Count == 0)
                return;

            foreach (var entry in metaData.CronEntries)
            {
                var expression = new CronExpression(entry.Expression);
                ScheduleNext(expression, entry.Arguments, cancellationToken);
            }
        }

        private void ScheduleNext(CronExpression expression, string arguments, CancellationToken cancellationToken)
        {
            var nextTime = expression.NextOccurrence(DateTime.Now);

            var task = Task.Run(() => RunScriptAt(expression, nextTime, arguments, cancellationToken), cancellationToken);

            lock (currentlyRunningTasks)
            {
                currentlyRunningTasks.Add(task);
            }

            task.ContinueWith(t =>
            {
                lock (currentlyRunningTasks)
                {
                    currentlyRunningTasks.Remove(task);
                }
            });
        }

        private async Task RunScriptAt(CronExpression expression, DateTime dateTime, string arguments, CancellationToken cancellationToken)
        {
            var now = DateTime.Now;
            var delay = dateTime - now;
            if (delay >= TimeSpan.Zero)
                await Task.Delay(delay, cancellationToken);

            await scriptHandler.Run(relativePath, arguments, cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
            ScheduleNext(expression, arguments, cancellationToken);
        }
    }
}
