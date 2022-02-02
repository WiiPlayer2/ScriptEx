using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using HotChocolate;
using HotChocolate.Types;
using ScriptEx.Core.Internals;
using ScriptEx.Shared;

namespace ScriptEx.Core.Api.Types;

[InterfaceType(nameof(Entry))]
public abstract record Entry(string Name, string FullName);

public record DirectoryEntry(string Name, string FullName) : Entry(Name, FullName)
{
    public IEnumerable<Entry> GetScripts()
        => new DirectoryInfo(FullName).EnumerateFileSystemInfos()
            .Select<FileSystemInfo, Entry>(o => o switch
            {
                DirectoryInfo directoryInfo => new DirectoryEntry(directoryInfo.Name, directoryInfo.FullName),
                FileInfo fileInfo => new ScriptEntry(fileInfo.Name, fileInfo.FullName),
                _ => throw new NotSupportedException(),
            });
}

public record ScriptEntry(
    string Name,
    string FullName) : Entry(Name, FullName)
{
    public string? GetLanguage([Service] IScriptEngineRegistry engineRegistry)
        => engineRegistry.GetEngineForFile(Name)?.LanguageIdentifier;

    public Task<ScriptMetaData?> GetMetaData(
        [Service] IScriptHandler scriptHandler,
        [Service] PathFinder pathFinder)
        => scriptHandler.GetMetaData(pathFinder.GetRelativePath(FullName));

    public IQueryable<ScriptExecution> GetHistory(
        [Service] IScriptHistoryRepository historyRepository,
        [Service] PathFinder pathFinder)
        => historyRepository.GetHistory(pathFinder.GetRelativePath(FullName));
}
