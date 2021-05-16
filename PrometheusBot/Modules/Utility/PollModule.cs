using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace PrometheusBot.Modules.Utility
{
    public class PollModule : ModuleBase<SocketCommandContext>
    {
        private static readonly IEmote[] emotes;

        static PollModule()
        {
            emotes = new IEmote[11];
            for (int i = 0; i < emotes.Length - 1; i++)
            {
                string unicode = (char)('0' + i) + "\u20E3";
                emotes[i] = new Emoji(unicode);
            }
            emotes[10] = new Emoji("\uD83D\uDD1F");
        }

        [Command("Poll")]
        public async Task<RuntimeResult> Poll(
            [Summary("The poll's title.")]
            string title,

            [Summary("The options for the poll. Eleven at most.")]
            params string[] options)
        {
            if (string.IsNullOrWhiteSpace(title))
                return CommandResult.FromError(CommandError.Unsuccessful, "Title cannot be empty");
            if (title.Length > 256)
                return CommandResult.FromError(CommandError.Unsuccessful, "Title may not be longer than 256 characters");
            if (options.Length > 11)
                return CommandResult.FromError(CommandError.Unsuccessful, "Cannot poll with more than 11 options");
            if (options.Length < 2)
                return CommandResult.FromError(CommandError.Unsuccessful, "A poll must have at least 2 option");
            int charSum = options.Sum(option => option.Length);
            if (charSum > 1500)
                return CommandResult.FromError(CommandError.Unsuccessful, "Total amount of characters in options may not exceed 1500");
            var embed = GetEmbed(options, title);
            var message = await ReplyAsync(embed: embed);
            await message.AddReactionsAsync(emotes.Take(options.Length).ToArray());
            return CommandResult.FromSuccess();
        }

        private Embed GetEmbed(string[] options, string title)
        {
            var fields = options.Select((option, index) => GetField(index, option)).ToList();
            EmbedBuilder embedBuilder = new()
            {
                Title = title,
                Fields = fields
            };
            return embedBuilder.Build();
        }
        private EmbedFieldBuilder GetField(int index, string option)
        {
            EmbedFieldBuilder embedFieldBuilder = new()
            {
                Name = $"Option {emotes[index]}:",
                Value = $"```{option}```"
            };
            return embedFieldBuilder;
        }
    }
}
