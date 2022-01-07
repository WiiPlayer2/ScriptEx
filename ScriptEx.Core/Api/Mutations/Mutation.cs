using System;
using System.Threading.Tasks;
using HotChocolate;
using ScriptEx.Core.Internals;
using ScriptEx.Shared;

namespace ScriptEx.Core.Api.Mutations
{
    public class Mutation
    {
        public Task<ScriptResult> Run(
            string file,
            [Service] IScriptHandler scriptHandler)
            => scriptHandler.Run(file);
    }
}
