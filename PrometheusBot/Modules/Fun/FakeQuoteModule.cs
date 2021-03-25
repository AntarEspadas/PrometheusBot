using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using PrometheusBot.Extensions;

namespace PrometheusBot.Modules.Fun
{
    public class FakeQuoteModule : ModuleBase<SocketCommandContext>
    {
        [Priority(-100)]
        [RandomFakeQuoteCommand]
        public async Task RandomFakeQuote()
        {
            await FakeQuote(Context.Message.Content);
        }
        [Command("FakeQuote")]
        [Alias("Quote", "Fake", "Gandhi")]
        public async Task FakeQuote()
        {
            var message = await Context.Message.GetReplyingMessageAsync();
            await FakeQuote(message.Content);
        }
        [Command("FakeQuote")]
        [Alias("Quote", "Fake", "Gandhi")]
        public async Task FakeQuote([Remainder] string text)
        {
            string[,] people = {
                {"Gandhi","https://upload.wikimedia.org/wikipedia/commons/d/d1/Portrait_Gandhi.jpg" },
                {"Abraham Lincoln","https://upload.wikimedia.org/wikipedia/commons/a/ab/Abraham_Lincoln_O-77_matte_collodion_print.jpg" },
                {"Dalai Lama","https://upload.wikimedia.org/wikipedia/commons/3/3a/HH_The_Dalai_Lama_%288098007806%29.jpg" },
                {"Albert Einstein", "https://upload.wikimedia.org/wikipedia/commons/d/d3/Albert_Einstein_Head.jpg" },
                {"Hayao Miyazaki", "https://upload.wikimedia.org/wikipedia/commons/6/65/Hayao_Miyazaki_cropped_3_Hayao_Miyazaki_201211.jpg" },
                {"God", "https://upload.wikimedia.org/wikipedia/commons/1/13/Michelangelo%2C_Creation_of_Adam_06.jpg" },
                {"Confucius","https://upload.wikimedia.org/wikipedia/commons/9/9a/Confucius_the_scholar.jpg" }
            };
            var index = new Random().Next(0, people.GetLength(0));
            var eb = new EmbedBuilder()
            {
                Title = text,
                Description = $"*-{people[index, 0]}*",
                ThumbnailUrl = people[index, 1],
            };
            await ReplyAsync(embed: eb.Build());
        }
    }

    class RandomFakeQuoteCommandAttribute : NonStandardCommandAttribute
    {
        public override bool Validate(ICommandContext context)
        {
            return new Random().Next(0, 500) == 69;
        }
    }
}
