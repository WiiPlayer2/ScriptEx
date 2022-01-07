using System;
using System.Collections.Generic;
using System.Linq;
using ScriptEx.Shared;

namespace ScriptEx.Core.Internals
{
    public class ScriptMetaDataScanner
    {
        private const string KEY_CRON = "cron";

        private readonly string metaDataIndicator;

        public ScriptMetaDataScanner(string singleLineComment)
        {
            this.metaDataIndicator = $"{singleLineComment}:";
        }

        public IReadOnlyList<(string Key, string Value)> GetMetaDataLines(string contents) =>
            contents.Split('\n')
                .TakeWhile(o => o.StartsWith(metaDataIndicator))
                .Select(o =>
                {
                    var firstSpace = o.IndexOf(' ');
                    if (firstSpace == -1)
                        return default;
                    var key = o[metaDataIndicator.Length..firstSpace];
                    var value = o[firstSpace..].Trim();
                    return (key, value);
                })
                .Where(o => o != default)
                .ToList();

        private IEnumerable<string> GetValues(IEnumerable<(string Key, string Value)> metaData, string key)
            => metaData
                .Where(o => o.Key.Equals(key, StringComparison.InvariantCultureIgnoreCase))
                .Select(o => o.Value);

        public IReadOnlyList<CronEntry> GetCronEntries(IReadOnlyList<(string Key, string Value)> metaData)
            => GetValues(metaData, KEY_CRON)
                .Select(entry =>
                {
                    var splitBySpaces = entry.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    if (splitBySpaces.Length < 5)
                        return default;

                    var currentIndex = 0;
                    for (var i = 0; i < 5; i++)
                    {
                        var spaceIndex = entry.IndexOf(' ', currentIndex);
                        if (spaceIndex == -1)
                        {
                            currentIndex = entry.Length - 1;
                            break;
                        }

                        currentIndex = entry.LastIndexOf(' ', spaceIndex);
                    }

                    var expression = string.Join(' ', entry[..currentIndex].Split(' ', StringSplitOptions.RemoveEmptyEntries));
                    var arguments = entry[currentIndex..].Trim();
                    return new CronEntry(expression, arguments);
                })
                .WhereNotNull()
                .ToList();
    }
}
