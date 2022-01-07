using System;
using System.Threading;
using System.Threading.Tasks;
using ScriptEx.Shared;

namespace ScriptEx.Core.Internals
{
    public interface IScriptHandler
    {
        Task<ScriptMetaData?> GetMetaData(string file, CancellationToken cancellationToken = default);

        Task<ScriptResult> Run(string file, CancellationToken cancellationToken = default);
    }
}
