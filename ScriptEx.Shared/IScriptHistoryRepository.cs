using System;
using System.Linq;
using System.Threading.Tasks;

namespace ScriptEx.Shared
{
    public interface IScriptHistoryRepository
    {
        IQueryable<ScriptExecution> GetHistory();

        IQueryable<ScriptExecution> GetHistory(string file);

        Task PruneHistory(string file, DateTime beforeStartDate);

        Task AddHistory(ScriptExecution execution);

        Task DeleteHistory(string file);
    }
}
