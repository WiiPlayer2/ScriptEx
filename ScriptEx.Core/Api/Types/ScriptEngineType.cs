using System;
using HotChocolate.Types;
using ScriptEx.Shared;

namespace ScriptEx.Core.Api.Types
{
    public class ScriptEngineType : ObjectType<IScriptEngine>
    {
        protected override void Configure(IObjectTypeDescriptor<IScriptEngine> descriptor)
        {
            descriptor.Name("ScriptEngine");
            descriptor.Field(o => o.Run(default!, default!, default!, default))
                .Ignore();
        }
    }
}
