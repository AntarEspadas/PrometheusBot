using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;

namespace PrometheusBot.Commands.Preconditions
{
    [System.AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    sealed class ContextAttribute : RequireContextAttribute
    {
        // See the attribute guidelines at 
        //  http://go.microsoft.com/fwlink/?LinkId=85236
        public ContextAttribute(ContextType contexts) : base(contexts)
        {
        }

        public override string ErrorMessage { get => GetErrorMessage(); set => base.ErrorMessage = value; }

        private string GetErrorMessage()
        {
            if (base.ErrorMessage is not null) return base.ErrorMessage;
            List<string> contexts = new();
            AddIfEnumPresent(contexts, ContextType.DM, "a DM channel");
            AddIfEnumPresent(contexts, ContextType.Group, "a grouop chat");
            AddIfEnumPresent(contexts, ContextType.Guild, "a server");
            string message = string.Join(", ", contexts.Take(contexts.Count - 1));
            message += contexts.Count > 1 ? " or " : "";
            message += contexts.Last();
            message = $"This command may only be run in {message}";
            return message;
        }
        private void AddIfEnumPresent(List<string> list, ContextType type, string message)
        {
            if (Contexts.HasFlag(type))
                list.Add(message);
        }
    }
}
