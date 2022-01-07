﻿using System;
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

        public Task<ScriptResult> Run(string file, CancellationToken cancellationToken = default) =>
            Invoke(
                cancellationToken,
                "-ExecutionPolicy", "Unrestricted",
                "-NonInteractive",
                "-NoLogo",
                "-OutputFormat", "Text",
                "-File", $"\"{file}\"");

        private async Task<ScriptResult> Invoke(CancellationToken cancellationToken, params string[] arguments)
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

            process.Start();
            await process.WaitForExitAsync(cancellationToken);

            var standardOutput = await process.StandardOutput.ReadToEndAsync();
            var standardError = await process.StandardError.ReadToEndAsync();
            var exitCode = process.ExitCode;
            return new ScriptResult(standardOutput, standardError, exitCode);
        }
    }
}
