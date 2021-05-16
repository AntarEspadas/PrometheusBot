using System;
using System.Threading.Tasks;
using Discord.Commands;
using System.Net;
using Newtonsoft.Json;
using Discord;
using PrometheusBot.Services;

namespace PrometheusBot.Modules.Fun.RandomAnimals
{
    public class RandomAnimalModule : ModuleBase<SocketCommandContext>
    {
        private readonly string _catApiKey;
        private readonly string _dogApiKey;

        public RandomAnimalModule(LocalSettingsService settings)
        {
            _catApiKey = settings.CatApiKey;
            _dogApiKey = settings.DogApiKey;
        }

        [Command("Cat")]
        [Summary("Gets a random cat image.")]
        public async Task<RuntimeResult> RandomCat(
            [Summary("'gif' or 'static'")]
            string fileType = null)
        {
            return await GetAndSendAnimalAsync(Animal.Cat, fileType);
        }

        [Command("Dog")]
        [Summary("Gets a random dog image.")]
        public async Task<RuntimeResult> RandomDog(
            [Summary("'gif' or 'static'")]
            string fileType = null)
        {
            return await GetAndSendAnimalAsync(Animal.Dog, fileType);
        }

        private async Task<CommandResult> GetAndSendAnimalAsync(Animal animal, string fileType)
        {
            string mimeTypes;
            switch (fileType)
            {
                case "gif":
                    mimeTypes = "gif";
                    break;
                case "static":
                    mimeTypes = "jpg,png";
                    break;
                case null:
                    mimeTypes = "gif,jpg,png";
                    break;
                default:
                    return CommandResult.FromError(CommandError.Unsuccessful, "Type must be either `gif` or `static`");
            }

            string animalName = animal.ToString().ToLower();
            string api = $"https://api.the{animalName}api.com";
            string apiKey = animal switch
            {
                Animal.Cat => _catApiKey,
                Animal.Dog => _dogApiKey,
                _ => throw new NotSupportedException()
            };
            string imageUrl;
            try { imageUrl = await GetImageUrlAsync(api, mimeTypes, apiKey); }
            catch { return CommandResult.FromError(CommandError.Unsuccessful, "There was an error retrieving data from the api"); }
            await SendAnimalAsync($"{animal}!", $"Powered by the{animalName}api.com - {animal}s as a service", imageUrl);
            return CommandResult.FromSuccess();
        }

        private async Task SendAnimalAsync(string title, string footer, string imageUrl)
        {
            EmbedFooterBuilder embedFooterBuilder = new()
            {
                Text = footer
            };
            EmbedBuilder embedBuilder = new()
            {
                Title = title,
                Footer = embedFooterBuilder,
                ImageUrl = imageUrl
            };
            var embed = embedBuilder.Build();
            await ReplyAsync(embed: embed);
        }

        private async Task<string> GetImageUrlAsync(string api, string mimeTypes, string apiKey = null)
        {
            WebClient client = new();
            if (apiKey is not null)
                client.Headers["x-api-key"] = apiKey;
            string apiRequest = $"/v1/images/search?mime_types={mimeTypes}";
            string stringResponse = await client.DownloadStringTaskAsync(api + apiRequest);
            var response = JsonConvert.DeserializeObject<ApiResponse[]>(stringResponse)[0];
            return response.Url;
        }
    }
    enum Animal
    {
        Cat,
        Dog
    }
}
