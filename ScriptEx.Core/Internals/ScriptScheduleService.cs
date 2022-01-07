using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace ScriptEx.Core.Internals
{
    public class ScriptScheduleService : IAsyncDisposable
    {
        private readonly string basePath;

        private readonly PathFinder pathFinder;

        private readonly List<ScriptScheduler> schedulers = new();

        private readonly IScriptHandler scriptHandler;

        public ScriptScheduleService(IOptions<AppOptions> appOptions, PathFinder pathFinder, IScriptHandler scriptHandler)
        {
            this.pathFinder = pathFinder;
            this.scriptHandler = scriptHandler;
            basePath = appOptions.Value.ScriptsPath;
        }

        public async ValueTask DisposeAsync()
        {
            foreach (var scheduler in schedulers)
                await scheduler.DisposeAsync();
            schedulers.Clear();
        }

        public async Task UpdateAll()
        {
            foreach (var scheduler in schedulers)
                await scheduler.DisposeAsync();
            schedulers.Clear();

            schedulers.AddRange(Directory.EnumerateFiles(basePath, "*", SearchOption.AllDirectories)
                .Select(file => new ScriptScheduler(pathFinder.GetRelativePath(file), scriptHandler)));
            await Task.WhenAll(schedulers.Select(o => o.Update()));
        }
    }
}
