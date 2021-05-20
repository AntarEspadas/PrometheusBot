using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using PrometheusBot.Extensions;
using PrometheusBot.Services.Settings;
using Microsoft.Extensions.DependencyInjection;

namespace PrometheusBot.Modules.Fun
{
    public class FakeQuoteModule : ModuleBase<SocketCommandContext>
    {

        private const string summary = "Turns text into a fake quote by a famous person.";
        private readonly Random _random;

        public FakeQuoteModule(Random random)
        {
            _random = random;
        }

        [Priority(-100)]
        [RandomFakeQuoteCommand]
        public async Task RandomFakeQuote()
        {
            await FakeQuote(Context.Message.Content);
        }
        [Command("FakeQuote")]
        [Alias("Quote", "Fake", "Gandhi")]
        [Summary(summary)]
        public async Task FakeQuote()
        {
            var message = await Context.Message.GetReplyingMessageAsync();
            await FakeQuote(message.Content);
        }
        [Command("FakeQuote")]
        [Alias("Quote", "Fake", "Gandhi")]
        [Summary(summary)]
        public async Task FakeQuote(
            [Summary("The text to quote.")]
            [Remainder] string text)
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
            var index = _random.Next(0, people.GetLength(0));
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
        public override bool Validate(ICommandContext context, IServiceProvider services)
        {
            if (string.IsNullOrWhiteSpace(context.Message.Content)) return false;
            if (context.Message.Content.Length > 255) return false;

            var settings = services.GetService<SettingsService>();
            SettingLookupInfo lookupInfo = new("random-quote:enabled") { CId = context.Channel.Id, GId = context.Guild.Id };
            settings.GetSetting(lookupInfo, out bool enabled, true);

            if (!enabled) return false;

            lookupInfo.SettingName = "random-quote:probability";
            settings.GetSetting(lookupInfo, out double probability, true);
            var random = services.GetService<Random>();

            return random.NextDouble() < probability;
        }
    }
}
