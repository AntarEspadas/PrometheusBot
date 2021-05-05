using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Newtonsoft.Json;
using PrometheusBot.Extensions;

namespace PrometheusBot.Modules.Fun.Danbooru
{
    public class DanbooruModule : ModuleBase<SocketCommandContext>
    {
        private static readonly HttpClient _httpClient = new();
        private const string POSTS_API = "https://danbooru.donmai.us/post/index.json";
        private const string POSTS_COUNTS_API = "https://danbooru.donmai.us/counts/posts.json";
        private static readonly Random _random = new();

        [Command("Feet")]
        public async Task<RuntimeResult> Feet(string character = null)
        {
            List<string> tags = new() { "feet", "rating:safe" };

            if (character is not null)
                tags.Add(character);

            string file = await RandomPost(tags);

            if (file is null)
                return CommandResult.FromError(CommandError.Unsuccessful, "Something went wrong. Maybe there are no available feet for that character?");

            var embed = GetEmbed(file);
            await ReplyAsync(embed: embed.Build());
            return CommandResult.FromSuccess();
        }

        private static async Task<string> RandomPost(IList<string> tags = null)
        {
            tags ??= Array.Empty<string>();
            string formattedTags = string.Join("%20", tags);
            string arguments = $"?tags={formattedTags}";
            int? count = await Count(POSTS_COUNTS_API + arguments);
            count ??= 200;
            int pages = (int)Math.Ceiling(count.Value / 200f);
            arguments += $"&limit=200&page={_random.Next(1, pages + 1)}";
            List<ApiResponse> response = null;
            try
            {
                string jsonResponse = await _httpClient.GetStringAsync(POSTS_API + arguments);
                response = JsonConvert.DeserializeObject<List<ApiResponse>>(jsonResponse);
            }
            catch (Exception ex) { }


            var files = response
                ?.Select(response => response.file_url)
                ?.Where(file => file is not null)
                .ToList();

            if (files is null || files.Count < 1) return null;

            string file = files?.RandomElement();

            return file;
        }

        private static EmbedBuilder GetEmbed(string file)
        {
            EmbedFooterBuilder footerBuilder = new()
            {
                Text = "From danbooru.donmai.us"
            };
            EmbedBuilder embedBuilder = new()
            {
                Title = "**Feet!**",
                ImageUrl = file,
                Footer = footerBuilder
            };

            return embedBuilder;
        }

        private static async Task<int?> Count(string url)
        {
            try
            {
                string jsonResponse = await _httpClient.GetStringAsync(url);
                var counts = JsonConvert.DeserializeObject<Counts>(jsonResponse);
                return counts.counts["posts"];
            }
            catch
            {
                return null;
            }
        }
    }
}
