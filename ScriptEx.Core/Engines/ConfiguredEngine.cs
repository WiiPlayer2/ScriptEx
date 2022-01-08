using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ScriptEx.Shared;

namespace ScriptEx.Core.Engines
{
    public record EngineConfiguration
    {
        public string Command { get; init; } = default!;

        public string ArgumentMask { get; init; } = default!;

        public string FileExtension { get; init; } = default!;

        public string LanguageIdentifier { get; init; } = default!;

        public string SingleLineCommentSymbol { get; init; } = default!;
    }

    internal class ConfiguredEngine : ExecutableEngine
    {
        private const string PLACEHOLDER_ARGS = "{ARGS}";

        private const string PLACEHOLDER_FILE = "{FILE}";

        private readonly string argumentMask;

        public ConfiguredEngine(EngineConfiguration config) : base(config.Command, config.FileExtension, config.LanguageIdentifier, config.SingleLineCommentSymbol)
        {
            argumentMask = config.ArgumentMask;
        }

        public override Task<ScriptResult> Run(string file, string arguments, IReadOnlyDictionary<string, string> environment, CancellationToken cancellationToken = default)
        {
            var cmdArgs = argumentMask
                .Replace(PLACEHOLDER_FILE, file)
                .Replace(PLACEHOLDER_ARGS, arguments);
            return Invoke(cancellationToken, environment, cmdArgs);
        }
    }
}
