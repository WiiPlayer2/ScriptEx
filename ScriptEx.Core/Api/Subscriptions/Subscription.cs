using System;
using HotChocolate;
using HotChocolate.Types;
using ScriptEx.Shared;

namespace ScriptEx.Core.Api.Subscriptions
{
    public class Subscription
    {
        public const string TOPIC_SCRIPT_EXECUTED = "script_executed";

        [Subscribe]
        [Topic(TOPIC_SCRIPT_EXECUTED)]
        public ScriptExecution ScriptExecuted([EventMessage] ScriptExecution item) => item;
    }
}
