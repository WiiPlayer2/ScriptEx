using System;
using System.Threading.Tasks;
using HotChocolate;
using ScriptEx.Core.Internals;
using ScriptEx.Shared;

namespace ScriptEx.Core.Api.Mutations
{
    public class Mutation
    {
        public Task<ScriptResult> Execute(
            string language,
            string script,
            [Service] IScriptRunner scriptRunner)
            => scriptRunner.Execute(language, script);

        public Task<ScriptResult> Run(
            string file,
            [Service] IScriptRunner scriptRunner)
            => scriptRunner.Run(file);
    }
}
