using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;

namespace PrometheusBot.Modules.Admin;
public class PinDumperModule : ModuleBase<SocketCommandContext>
{
    [Command("Dump pins")]
    [Alias("pins")]
    public async Task<RuntimeResult> DumpPins(SocketTextChannel source, SocketTextChannel destination, int count = -1)
    {
        IEnumerable<RestMessage> pinnedMessages = await source.GetPinnedMessagesAsync();
        if (count >= 0)
            pinnedMessages = pinnedMessages.Take(count);
        foreach (var pinnedMessage in pinnedMessages)
        {
            var embed = GetEmbed(pinnedMessage);
            await ReplyAsync(embed: embed);
        }

        return CommandResult.FromSuccess();
    }

    private Embed GetEmbed(IMessage message)
    {
        var embedBuilder = new EmbedBuilder
        {
            Author = new()
            {
                IconUrl = message.Author.GetAvatarUrl() ?? message.Author.GetDefaultAvatarUrl(),
                Name = message.Author.Username,
                Url = message.GetJumpUrl()
            },
            ImageUrl = message.Attachments.FirstOrDefault()?.Url,
            Description = message.Content,
            Timestamp = message.Timestamp,
            Footer = new EmbedFooterBuilder
            {
                Text = "Click username to vew original message"
            }
        };
        return embedBuilder.Build();
    }
}
