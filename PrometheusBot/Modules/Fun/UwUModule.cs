using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Discord.Commands;
using PrometheusBot.Extensions;

namespace PrometheusBot.Modules.Fun
{
    public class UwUModule : ModuleBase<SocketCommandContext>
    {

        [Command("UwU")]
        [Alias("OwO", "UwUtize", "OwOtize")]
        public async Task UwutizeAsync()
        {
            var message = await Context.Message.GetReplyingMessageAsync();
            var embed = message.Author.Quote(UwUtize(message.Content), null);
            await ReplyAsync(embed: embed.Build());
        }

        [Command("UwU")]
        [Alias("OwO", "UwUtize", "OwOtize")]
        public async Task UwutizeAsync([Remainder] string text)
        {
            var embed = Context.Message.Author.Quote(UwUtize(text));
            await ReplyAsync(embed: embed.Build());
        }

        private static string UwUtize(string text)
        {
            string[] faces = { "(・\\`ω´・)", ";;w;;", "owo", "UwU", ">w<", "^w^" };
            var regex = new Regex("(?:r|l)");
            text = regex.Replace(text, "w");
            regex = new Regex("(?:R|L)");
            text = regex.Replace(text, "W");
            regex = new Regex("n(?=[aeiouáéíóú])");
            text = regex.Replace(text, "ny");
            regex = new Regex("N(?=[aeiouáéíóú])");
            text = regex.Replace(text, "ny");
            regex = new Regex("N(?=[AEIOUÁÉÍÓÚ])");
            text = regex.Replace(text, "ny");
            regex = new Regex("(?<=[pt])o");
            text = regex.Replace(text, "wo");
            regex = new Regex("!+");
            var random = new Random();
            text = regex.Replace(text, $" {faces[random.Next(0, faces.Length)]} ");
            return text;
        }
    }
}
