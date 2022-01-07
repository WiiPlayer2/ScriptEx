﻿using System;
using System.IO;
using Microsoft.Extensions.Options;

namespace ScriptEx.Core.Internals
{
    public class PathFinder
    {
        private readonly Uri baseUri;

        public PathFinder(IOptions<AppOptions> appOptions)
        {
            baseUri = new Uri(Path.GetFullPath(appOptions.Value.ScriptsPath + Path.DirectorySeparatorChar));
        }

        public string GetFilePath(string filePath)
        {
            var filePathUri = new Uri(Path.GetFullPath(filePath));
            var relativeUri = baseUri.MakeRelativeUri(filePathUri);
            return relativeUri.ToString();
        }
    }
}