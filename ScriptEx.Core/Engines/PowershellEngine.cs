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

        public async Task<ScriptResult> Run(string file, CancellationToken cancellationToken = default)
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "powershell",
                    Arguments = string.Join(" ",
                        "-ExecutionPolicy", "Unrestricted",
                        "-NonInteractive",
                        "-NoLogo",
                        "-File", $"\"{file}\""),
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

        public Task<IReadOnlyList<string>> ReadMetaDataStrings(string contents, CancellationToken cancellationToken = default) => throw new NotImplementedException();
    }
}
