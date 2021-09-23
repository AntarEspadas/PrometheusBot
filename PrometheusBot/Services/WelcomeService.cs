using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrometheusBot.Services
{
    class WelcomeService
    {
        private DiscordSocketClient _client;
        public WelcomeService(DiscordSocketClient client)
        {
            _client = client;
            client.UserJoined += OnUserJoined;
        }

        private async Task OnUserJoined(SocketGuildUser user)
        {
            var application = await _client.GetApplicationInfoAsync();
            var owner = application.Owner;
            if (user.Id != owner.Id) return;
            
            var channel = user.Guild.DefaultChannel;
            await channel.SendMessageAsync($"Welcome, my master o7");

            _client.MessageReceived += OnMessageReceived;

            async Task OnMessageReceived(SocketMessage message)
            {
                if (message.Channel.Id != channel.Id) return;
                if (message.Author.Id != owner.Id) return;
                string content = message.Content.ToLower();
                if (content == "at ease" || content == "at ease, navi")
                    await channel.SendMessageAsync($"Understood, {owner.Username}-sama");
            }
        }
    }
}
