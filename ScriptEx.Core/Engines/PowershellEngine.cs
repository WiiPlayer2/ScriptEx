using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ScriptEx.Shared;

namespace ScriptEx.Core.Engines
{
    internal class PowershellEngine : ExecutableEngine
    {
        public PowershellEngine() : base("pwsh") { }

        public override string FileExtension => ".ps1";

        public override string LanguageIdentifier => "powershell";

        public override string SingleLineCommentSymbol => "#";

        public override Task<ScriptResult> Run(string file, string arguments, IReadOnlyDictionary<string, string> environment, CancellationToken cancellationToken = default) =>
            Invoke(
                cancellationToken,
                environment,
                "-ExecutionPolicy", "Unrestricted",
                "-NonInteractive",
                "-NoLogo",
                "-OutputFormat", "Text",
                "-File", $"\"{file}\"",
                arguments);
    }
}
