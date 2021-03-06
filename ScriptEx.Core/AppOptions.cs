using System;
using ScriptEx.Core.Engines;

namespace ScriptEx.Core
{
    public class AppOptions
    {
        public const string SECTION = "App";

        public string ScriptsPath { get; init; } = default!;

        public TimeSpan DefaultTimeout { get; init; } = TimeSpan.FromHours(1);

        public EngineConfiguration[] Engines { get; init; } = Array.Empty<EngineConfiguration>();
    }
}
