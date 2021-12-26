using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ScriptEx.Shared
{
    public interface IScriptEngine
    {
        string FileExtension { get; }

        string LanguageIdentifier { get; }

        Task<ScriptResult> Run(string file, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<string>> ReadMetaDataStrings(string contents, CancellationToken cancellationToken = default);
    }
}
