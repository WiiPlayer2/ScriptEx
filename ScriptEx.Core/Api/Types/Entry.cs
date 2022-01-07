using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HotChocolate;
using HotChocolate.Types;
using Microsoft.Extensions.Options;
using ScriptEx.Core.Internals;
using ScriptEx.Shared;
using Path = System.IO.Path;

namespace ScriptEx.Core.Api.Types
{
    [InterfaceType(nameof(Entry))]
    public abstract record Entry(string Name, string FullName);

    public record DirectoryEntry(string Name, string FullName) : Entry(Name, FullName)
    {
        public IEnumerable<Entry> GetScripts([Service] IScriptEngineRegistry engineRegistry)
            => new DirectoryInfo(FullName).EnumerateFileSystemInfos().Select<FileSystemInfo, Entry>(o => o switch
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

        public ScriptMetaData? GetMetaData([Service] IScriptEngineRegistry engineRegistry)
        {
            var singleLineCommentSymbol = engineRegistry.GetEngineForFile(Name)?.SingleLineCommentSymbol;
            return singleLineCommentSymbol == null ? null : new ScriptMetaDataScanner(singleLineCommentSymbol).GetMetaData(File.ReadAllText(FullName));
        }
    }
}
