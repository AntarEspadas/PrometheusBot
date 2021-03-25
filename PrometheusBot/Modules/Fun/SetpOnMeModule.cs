using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using PrometheusBot.Extensions;

namespace PrometheusBot.Modules.Fun
{
    public class SetpOnMeModule : ModuleBase<SocketCommandContext>
    {
        private static readonly string[] _replies = {
            "Kimoi",
            "Kimochi warui",
            "Disgusting",
            "Pathetic",
            "Shine",
            "Go kill yourself"
        };

        [Command("Step on me")]
        [Alias("Abuse me")]
        public async Task StepOnMe()
        {
            await ReplyAsync(_replies.RandomElement());
        }
    }
}
