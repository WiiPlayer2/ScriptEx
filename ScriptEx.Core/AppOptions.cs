using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptEx.Core
{
    public class AppOptions
    {
        public const string SECTION = "App";

        public string ScriptsPath { get; init; } = default!;
    }
}
