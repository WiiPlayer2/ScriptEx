using System;
using System.Collections.Generic;
using System.IO;
using DotNetEnv;

namespace ScriptEx.Core.Internals
{
    public class EnvironmentResolver
    {
        private const string ENV_FILE = ".env";

        private readonly PathFinder pathFinder;

        public EnvironmentResolver(PathFinder pathFinder)
        {
            this.pathFinder = pathFinder;
        }

        public IReadOnlyDictionary<string, string> ResolveEnvironmentVariablesFor(string relativePath)
        {
            var directoryName = Path.GetDirectoryName(relativePath);
            var isRoot = string.IsNullOrEmpty(directoryName);
            var envFile = isRoot ? ENV_FILE : Path.Combine(directoryName!, ENV_FILE);
            var environmentVariables = ReadEnvironmentVariables(envFile);
            if (isRoot)
                return environmentVariables;

            var baseEnvironmentVariables = new Dictionary<string, string>(ResolveEnvironmentVariablesFor(directoryName));
            foreach (var (key, value) in environmentVariables)
                baseEnvironmentVariables[key] = value;
            return baseEnvironmentVariables;
        }

        private IReadOnlyDictionary<string, string> ReadEnvironmentVariables(string relativePath)
            => Env.NoEnvVars().Load(pathFinder.GetAbsolutePath(relativePath)).ToDictionary();
    }
}
