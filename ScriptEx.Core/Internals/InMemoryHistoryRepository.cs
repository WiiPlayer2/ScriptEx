using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ScriptEx.Shared;

namespace ScriptEx.Core.Internals
{
    public class InMemoryScriptHistoryRepository : IScriptHistoryRepository
    {
        private readonly ConcurrentDictionary<string, List<ScriptExecution>> data = new();

        public IQueryable<ScriptExecution> GetHistory(string file)
        {
            if (!data.TryGetValue(file, out var entries))
                return Enumerable.Empty<ScriptExecution>().AsQueryable();

            lock (entries)
            {
                return new List<ScriptExecution>(entries).AsQueryable();
            }
        }

        public Task PruneHistory(string file, DateTime beforeStartDate)
        {
            if (!data.TryGetValue(file, out var entries))
                return Task.CompletedTask;

            lock (entries)
            {
                var itemsToRemove = entries.Where(o => o.StartTime < beforeStartDate).ToList();
                foreach (var item in itemsToRemove)
                    entries.Remove(item);
            }

            return Task.CompletedTask;
        }

        public Task AddHistory(string file, ScriptExecution item)
        {
            var entries = data.GetOrAdd(file, _ => new List<ScriptExecution>());
            lock (entries)
            {
                entries.Add(item);
            }

            return Task.CompletedTask;
        }

        public Task DeleteHistory(string file)
        {
            data.TryRemove(file, out _);
            return Task.CompletedTask;
        }
    }
}
