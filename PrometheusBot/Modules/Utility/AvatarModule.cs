using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace PrometheusBot.Modules.Utility
{
    public class AvatarModule : ModuleBase<SocketCommandContext>
    {

        private const string summary = "Gets an user's profile picture";

        [Command("Avatar")]
        [Alias("Profile", "pfp")]
        [Summary(summary)]
        public async Task Avatar(
            [Summary("The name of the user.")]
            [Remainder] IUser user)
        {
            string pfp = user.GetAvatarUrl(size:256);
            await ReplyAsync(pfp);
        }

        [Command("Avatar")]
        [Alias("Profile", "pfp")]
        [Summary(summary)]
        public async Task Avatar()
        {
            var user = Context.Message.Author;
            await Avatar(user);
        }
    }
}
