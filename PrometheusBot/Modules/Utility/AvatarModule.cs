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
        [Command("Avatar")]
        [Alias("Profile", "pfp")]
        public async Task Avatar([Remainder] IUser user)
        {
            string pfp = user.GetAvatarUrl(size:256);
            await ReplyAsync(pfp);
        }

        [Command("Avatar")]
        [Alias("Profile", "pfp")]
        public async Task Avatar()
        {
            var user = Context.Message.Author;
            await Avatar(user);
        }
    }
}
