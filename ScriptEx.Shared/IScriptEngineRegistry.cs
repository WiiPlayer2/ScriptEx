﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ScriptEx.Shared
{
    public interface IScriptEngineRegistry
    {
        IReadOnlyList<IScriptEngine> RegisteredEngines { get; }

        Task Register(IScriptEngine engine);

        Task Unregister(IScriptEngine engine);
    }
}
