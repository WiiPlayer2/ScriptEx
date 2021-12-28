using System;
using System.Threading;
using System.Threading.Tasks;

namespace ScriptEx.Shared
{
    public interface IScriptEngine
    {
        string FileExtension { get; }

        string LanguageIdentifier { get; }

        Task<ScriptResult> Run(string file, CancellationToken cancellationToken = default);

        Task<ScriptResult> Execute(string script, CancellationToken cancellationToken = default);
    }
}
