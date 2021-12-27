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

        private readonly ScriptRunner scriptRunner;

        public Worker(ILogger<Worker> logger, IScriptEngineRegistry engineRegistry, ScriptRunner scriptRunner)
        {
            this.logger = logger;
            this.scriptRunner = scriptRunner;

            engineRegistry.Register(new PowershellEngine());
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var result = await scriptRunner.Run("./bin/Debug/net5.0/scripts/test.ps1", stoppingToken);
            Console.WriteLine(result);
        }
    }
}
