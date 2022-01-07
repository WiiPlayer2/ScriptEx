using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using ScriptEx.Shared;

namespace ScriptEx.Core.Internals
{
    public class ScriptMetaDataScanner
    {
        private const string KEY_CRON = "cron";

        private static Regex regexCron = new Regex(@"(?<cron>[^\s]+(\ +[^\s]+){4})(\s+(?<arguments>.*))?");

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
                    var match = regexCron.Match(entry);
                    if (!match.Success)
                        return default;
                    var cleanedExpression = string.Join(' ', match.Groups["cron"].Value.Split(' ', StringSplitOptions.RemoveEmptyEntries));
                    return new CronEntry(cleanedExpression, match.Groups["arguments"].Value.Trim());
                })
                .WhereNotNull()
                .ToList();

        public ScriptMetaData GetMetaData(string contents)
        {
            var entries = GetMetaDataLines(contents);
            var cronEntries = GetCronEntries(entries);
            return new(cronEntries);
        }
    }
}
