using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;

namespace PrometheusBot.Modules.Utility
{
    public class EchoModule : ModuleBase<SocketCommandContext>
    {
        [Command("Echo")]
        [Alias("Say")]
        [Summary("Send a message in chat")]
        public Task Echo(string message)
        {
            return ReplyAsync(message);
        }
    }
}
