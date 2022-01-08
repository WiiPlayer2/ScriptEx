using System;
using System.Threading;
using System.Threading.Tasks;
using ScriptEx.Shared;

namespace ScriptEx.Core.Internals
{
    public interface IScriptHandler
    {
        Task<ScriptMetaData?> GetMetaData(string relativePath, CancellationToken cancellationToken = default);

        Task<ScriptResult> Run(string relativePath, string arguments, TimeSpan? timeout = default, CancellationToken cancellationToken = default);
    }
}
