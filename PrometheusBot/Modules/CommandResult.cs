using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;

namespace PrometheusBot.Modules
{
    public class CommandResult : RuntimeResult
    {
        private CommandResult(CommandError? error, string reason) : base(error, reason)
        {
        }
        public static CommandResult FromError(CommandError error, string reason)
        {
            return new CommandResult(error, reason);
        }
        public static CommandResult FromError(Exception ex)
        {
            return new CommandResult(CommandError.Exception, ex.Message);
        }
        public static CommandResult FromSuccess()
        {
            return new CommandResult(null, null);
        }
    }
}
