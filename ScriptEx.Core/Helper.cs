using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using ScriptEx.Shared;

namespace ScriptEx.Core
{
    internal static class Helper
    {
        public static T GetOrCreate<T>(this IServiceProvider services)
            => (T)services.GetOrCreate(typeof(T));

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
    }
}
