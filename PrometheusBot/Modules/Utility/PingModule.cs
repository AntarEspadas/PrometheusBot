using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace PrometheusBot.Modules.Utility
{
    public class PingModule : ModuleBase<SocketCommandContext>
    {
        [Command("Ping")]
        [Summary("Test the bot's latency.")]
        public async Task Ping()
        {
            EmbedBuilder embedBuilder = new()
            {
                Title = "Latency",
                Description = "Pinging..."
            };

            var startTime = DateTime.Now;
            var message = await ReplyAsync(embed: embedBuilder.Build());
            var endTime = DateTime.Now;
            int msgChannelLatency = endTime.Subtract(startTime).Milliseconds;
            int apiLatency = Context.Client.Latency;

            string msg = $"**Message channel Latency**: {msgChannelLatency}ms.\n**Discord API latency**: {apiLatency}ms.";
            embedBuilder.Description = msg;
            await message.ModifyAsync(message => message.Embed = embedBuilder.Build());
        }
    }
}
