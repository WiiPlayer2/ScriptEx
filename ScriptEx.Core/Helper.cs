using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ScriptEx.Shared;

namespace ScriptEx.Core
{
    internal static class Helper
    {
        public static T GetOrCreate<T>(this IServiceProvider services)
            => (T) services.GetOrCreate(typeof(T));

        public static object GetOrCreate(this IServiceProvider services, Type type)
        {
            var service = services.GetService(type);
            if (service is not null)
                return service;

            var constructor = type.GetConstructors().Single();
            var args = constructor.GetParameters().Select(o => services.GetOrCreate(o.ParameterType)).ToArray();
            service = constructor.Invoke(args);
            return service;
        }

        public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> sequence)
            => sequence.Where(o => o is not null).Select(o => o!);

        public static IScriptEngine? GetEngineForFile(this IScriptEngineRegistry engineRegistry, string path)
        {
            var fileExtension = Path.GetExtension(path);
            return engineRegistry.RegisteredEngines.SingleOrDefault(o => o.FileExtension.Equals(fileExtension, StringComparison.InvariantCultureIgnoreCase));
        }

        public static void Merge(this IDictionary<string, string> dict, IReadOnlyDictionary<string, string> mergeValues)
        {
            foreach (var (key, value) in mergeValues)
                dict[key] = value;
        }

        public static async Task<T?> IgnoreCancellation<T>(this Task<T> task)
        {
            try
            {
                return await task;
            }
            catch (OperationCanceledException)
            {
                return default;
            }
        }

        public static async Task IgnoreCancellation(this Task task)
        {
            try
            {
                await task;
            }
            catch (OperationCanceledException) { }
        }

        public static T But<T>(this T? value, Func<T> getValue)
            => value == null ? getValue() : value;

        public static async Task<T> ButAsync<T>(this Task<T?> task, Func<T> getValue)
            => (await task).But(getValue);
    }
}
