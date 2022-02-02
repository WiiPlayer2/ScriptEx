using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ScriptEx.Core.Internals;
using ScriptEx.Shared;

namespace ScriptEx.Core;

internal class Worker : BackgroundService
{
    private readonly ILogger<Worker> logger;

    private readonly AppOptions appOptions;

    private readonly ScriptScheduleService scriptScheduleService;

    private readonly IScriptHandler scriptHandler;

    private readonly PathFinder pathFinder;

    public Worker(
        ILogger<Worker> logger,
        IOptions<AppOptions> appOptions,
        ScriptScheduleService scriptScheduleService,
        IScriptHandler scriptHandler,
        PathFinder pathFinder)
    {
        this.logger = logger;
        this.appOptions = appOptions.Value;
        this.scriptScheduleService = scriptScheduleService;
        this.scriptHandler = scriptHandler;
        this.pathFinder = pathFinder;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await scriptScheduleService.UpdateAll();

        var scriptsOnStart = (await Task.WhenAll(new DirectoryInfo(appOptions.ScriptsPath).EnumerateFiles("*", SearchOption.AllDirectories)
                .Select(async o =>
                {
                    var file = pathFinder.GetRelativePath(o.FullName);
                    return (File: file, MetaData: await scriptHandler.GetMetaData(file, stoppingToken));
                })))
            .Where(o => o.MetaData is not null && o.MetaData.Hooks.Contains(HookType.OnStart))
            .Select(o => o.File);

        await Task.WhenAll(scriptsOnStart.Select(o => scriptHandler.Run(o, string.Empty, cancellationToken: stoppingToken)));
    }
}
