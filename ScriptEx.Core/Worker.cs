using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ScriptEx.Core.Engines;
using ScriptEx.Core.Internals;
using ScriptEx.Shared;

namespace ScriptEx.Core
{
    internal class Worker : BackgroundService
    {
        private readonly ILogger<Worker> logger;

        private readonly IScriptRunner scriptRunner;

        public Worker(ILogger<Worker> logger, IServiceProvider services, IScriptEngineRegistry engineRegistry, IScriptRunner scriptRunner)
        {
            this.logger = logger;
            this.scriptRunner = scriptRunner;

            engineRegistry.Register(services.GetOrCreate<PowershellEngine>());
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var result = await scriptRunner.Run("./test.ps1", stoppingToken);
            logger.LogDebug(result.ToString());
        }
    }
}
