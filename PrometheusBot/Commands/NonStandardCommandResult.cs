using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;

namespace PrometheusBot.Commands
{
    class NonStandardCommandResult : RuntimeResult
    {
        public NonStandardCommandResult(CommandError? error, string reason) : base(error, reason)
        {

        }

        public NonStandardCommandResult() : base(null, null)
        {

        }
    }
}
