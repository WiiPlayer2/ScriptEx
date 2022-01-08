using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using ScriptEx.Shared;

namespace ScriptEx.Core.Engines
{
    internal class PowershellEngine : IScriptEngine
    {
        public string FileExtension => ".ps1";

        public string LanguageIdentifier => "powershell";

        public string SingleLineCommentSymbol => "#";

        public Task<ScriptResult> Run(string file, string arguments, IReadOnlyDictionary<string, string> environment, CancellationToken cancellationToken = default) =>
            Invoke(
                cancellationToken,
                environment,
                "-ExecutionPolicy", "Unrestricted",
                "-NonInteractive",
                "-NoLogo",
                "-OutputFormat", "Text",
                "-File", $"\"{file}\"",
                arguments);

        private async Task<ScriptResult> Invoke(CancellationToken cancellationToken, IReadOnlyDictionary<string, string> environment, params string[] arguments)
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "pwsh",
                    Arguments = string.Join(" ", arguments),
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    WorkingDirectory = Environment.CurrentDirectory,
                },
                EnableRaisingEvents = true,
            };
            foreach (var (key, value) in environment)
                process.StartInfo.Environment[key] = value;

            process.Start();
            cancellationToken.Register(() => process.Kill(true));
            await process.WaitForExitAsync(cancellationToken).IgnoreCancellation();

            var standardOutput = await process.StandardOutput.ReadToEndAsync();
            var standardError = await process.StandardError.ReadToEndAsync();
            var exitCode = process.ExitCode;
            return new ScriptResult(standardOutput, standardError, exitCode);
        }
    }
}
