using System;
using System.Threading;
using System.Threading.Tasks;
using ScriptEx.Shared;

namespace ScriptEx.Core.Internals
{
    public interface IScriptRunner
    {
        Task<ScriptResult> Execute(string language, string script, CancellationToken cancellationToken = default);

        Task<ScriptResult> Run(string file, CancellationToken cancellationToken = default);
    }
}
