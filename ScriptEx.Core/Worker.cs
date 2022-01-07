using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ScriptEx.Core.Internals;

namespace ScriptEx.Core
{
    internal class Worker : BackgroundService
    {
        private readonly ILogger<Worker> logger;

        private readonly ScriptScheduleService scriptScheduleService;

        public Worker(ILogger<Worker> logger, ScriptScheduleService scriptScheduleService)
        {
            this.logger = logger;
            this.scriptScheduleService = scriptScheduleService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await scriptScheduleService.UpdateAll();
        }
    }
}
