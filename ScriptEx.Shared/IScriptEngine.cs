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

        string SingleLineCommentSymbol { get; }

        Task<ScriptResult> Run(string file, string arguments, IReadOnlyDictionary<string, string> environment, CancellationToken cancellationToken = default);
    }
}
