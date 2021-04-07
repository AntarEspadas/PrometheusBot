using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using System.Net.Http;
using System.Net;
using Newtonsoft.Json;
using Discord;
using PrometheusBot.Model;

namespace PrometheusBot.Modules.Fun.RandomAnimals
{
    public class RandomAnimalModule : ModuleBase<SocketCommandContext>
    {
        private static readonly string _catApiKey;
        private static readonly string _dogApiKey;

        static RandomAnimalModule()
        {
            var settings = LocalSettings.Load();
            _catApiKey = settings.CatApiKey;
            _dogApiKey = settings.DogApiKey;
        }

        [Command("Cat")]
        public async Task<RuntimeResult> RandomCat()
        {
            return await GetAndSendAnimalAsync(Animal.Cat);
        }

        [Command("Dog")]
        public async Task<RuntimeResult> RandomDog()
        {
            return await GetAndSendAnimalAsync(Animal.Dog);
        }

        private async Task<CommandResult> GetAndSendAnimalAsync(Animal animal)
        {
            string animalName = animal.ToString().ToLower();
            string api = $"https://api.the{animalName}api.com";
            string apiKey = animal switch
            {
                Animal.Cat => _catApiKey,
                Animal.Dog => _dogApiKey,
                _ => throw new NotSupportedException()
            };
            string imageUrl;
            try { imageUrl = await GetImageUrlAsync(api, apiKey); }
            catch { return CommandResult.FromError(CommandError.Unsuccessful, "There was an error retrieving data from the api"); }
            await SendAnimalAsync($"{animal}!", $"Pwered by the{animalName}api.com - {animal}s as a service", imageUrl);
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

        private async Task<string> GetImageUrlAsync(string api, string apiKey = null)
        {
            WebClient client = new();
            if (apiKey is not null)
                client.Headers["x-api-key"] = apiKey;
            string apiRequest = "/v1/images/search";
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
