using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ScriptEx.Core.Engines;
using ScriptEx.Core.Internals;
using ScriptEx.Shared;

namespace ScriptEx.Core
{
    internal class Worker : BackgroundService
    {
        private readonly AppOptions appOptions;

        private readonly ILogger<Worker> logger;

        private readonly ScriptRunner scriptRunner;

        public Worker(ILogger<Worker> logger, IServiceProvider services, IScriptEngineRegistry engineRegistry, ScriptRunner scriptRunner, IOptions<AppOptions> appOptions)
        {
            this.logger = logger;
            this.scriptRunner = scriptRunner;
            this.appOptions = appOptions.Value;

            engineRegistry.Register(services.GetOrCreate<PowershellEngine>());
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var result = await scriptRunner.Run("./bin/Debug/net5.0/scripts/test.ps1", stoppingToken);
            logger.LogDebug(result.ToString());
        }
    }
}
