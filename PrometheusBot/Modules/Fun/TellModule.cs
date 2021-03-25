using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;

namespace PrometheusBot.Modules.Fun
{
    public class TellModule : ModuleBase<SocketCommandContext>
    {
        [Command("Tell")]
        public async Task Tell(SocketUser user, [Remainder] string sentence)
        {
            Regex regex = new("to go fuck (himself)|(herself)|(themselves)|(itself)");
            if (regex.IsMatch(sentence))
            {
                var application = await Context.Client.GetApplicationInfoAsync().ConfigureAwait(false);
                if (Context.User.Id == application.Owner.Id)
                {
                    await ReplyAsync($"I love you {user.Mention}");
                }
                else
                {
                    await ReplyAsync($"{user.Mention}, go fuck yourself");
                }
            }
        }
    }
}
