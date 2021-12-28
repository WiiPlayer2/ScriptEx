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

        public Worker(ILogger<Worker> logger)
        {
            this.logger = logger;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogDebug("Worker not implemented.");
            return Task.CompletedTask;
        }
    }
}
