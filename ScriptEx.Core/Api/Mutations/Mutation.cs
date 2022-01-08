using System;
using System.Threading.Tasks;
using HotChocolate;
using ScriptEx.Core.Api.Types;
using ScriptEx.Core.Internals;
using ScriptEx.Shared;

namespace ScriptEx.Core.Api.Mutations
{
    public class Mutation
    {
        public Task<ScriptResult> Run(
            string file,
            string arguments,
            [Service] IScriptHandler scriptHandler)
            => scriptHandler.Run(file, arguments);

        public async Task<Unit> UpdateAll(
            [Service] ScriptScheduleService scriptScheduleService)
        {
            await scriptScheduleService.UpdateAll();
            return default;
        }
    }
}
