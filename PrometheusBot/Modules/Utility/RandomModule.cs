using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using PrometheusBot.Extensions;

namespace PrometheusBot.Modules.Utility
{
    public class RandomModule : ModuleBase<SocketCommandContext>
    {
        private static readonly Random _random = new();
        [Command("Choose")]
        [Summary("Make the bot pick a random item from a list of options.")]
        public async Task<RuntimeResult> Choose(
            [Summary("The options to choose from.")]
            params string[] options)
        {
            if (options.Length < 2)
                return CommandResult.FromError(CommandError.Unsuccessful, "Not much of a choice without two or more options");
            string chosen = options.RandomElement();
            await ReplyAsync("> " + chosen);
            return CommandResult.FromSuccess();
        }
        [Command("Coin flip")]
        [Alias("Flip a coin")]
        [Summary("Make the bot flip a coin.")]
        public async Task CoinFlip()
        {
            await Choose("Heads", "Tails");
        }
        [Command("Random")]
        [Summary("Generate a random integer.")]
        public async Task<RuntimeResult> Random(
            [Summary("The inclusive minimum value for the ranodm number.")]
            int minValue,
            
            [Summary("the exclusive maximum value for the random number.")]
            int maxValue)
        {
            if (maxValue < minValue)
                return CommandResult.FromError(CommandError.Unsuccessful, "The maximum value must be greater than the minimum value");
            int randInt = _random.Next(minValue, maxValue);
            await ReplyAsync("> " + randInt);
            return CommandResult.FromSuccess();
        }
        [Command("Random decimal")]
        [Alias("Randomd")]
        public async Task RandomDouble()
        {
            double randDouble = _random.NextDouble();
            await ReplyAsync("> " + randDouble);
        }
    }
}
