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
        [RequireNsfw]
        public async Task<RuntimeResult> Feet(string character = null)
        {
            List<string> tags = new() { "feet", "rating:safe" };

            if (character is not null)
                tags.Add(character);

            Post post = await RandomPost(tags);

            if (post is null)
                return CommandResult.FromError(CommandError.Unsuccessful, "Something went wrong. Maybe there are no available feet for that character?");

            var embed = GetEmbed(post);
            await ReplyAsync(embed: embed.Build());
            return CommandResult.FromSuccess();
        }

        private static async Task<Post> RandomPost(IList<string> tags = null)
        {
            tags ??= Array.Empty<string>();
            string formattedTags = string.Join("%20", tags);
            string arguments = $"?tags={formattedTags}";
            int? count = await Count(POSTS_COUNTS_API + arguments);
            count ??= 200;
            int pages = (int)Math.Ceiling(count.Value / 200f);
            arguments += $"&limit=200&page={_random.Next(1, pages + 1)}";
            List<Post> response = null;
            try
            {
                string jsonResponse = await _httpClient.GetStringAsync(POSTS_API + arguments);
                response = JsonConvert.DeserializeObject<List<Post>>(jsonResponse);
            }
            catch { }


            var files = response
                ?.Where(file => file is not null)
                ?.ToList();

            if (files?.Count < 1) return null;

            var post = files?.RandomElement();

            return post;
        }

        private static EmbedBuilder GetEmbed(Post post)
        {
            string source = "https://danbooru.donmai.us/posts/" + post.id;
            EmbedFooterBuilder footerBuilder = new()
            {
                Text = "From danbooru.donmai.us"
            };
            EmbedBuilder embedBuilder = new()
            {
                Title = "**Feet!**",
                Description = $"[Source]({source})",
                ImageUrl = post.file_url,
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
