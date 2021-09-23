using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using PrometheusBot.Commands;
using PrometheusBot.Services;
using PrometheusBot.Services.Audio;
using PrometheusBot.Services.MessageHistory;
using PrometheusBot.Services.Music;
using PrometheusBot.Services.NonStandardCommands;
using PrometheusBot.Services.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Victoria;

namespace PrometheusBot
{
    public class PrometheusBot
    {
        DiscordSocketClient _client;
        LocalSettingsService _localSettings;
        CommandHandler _commandHandler;
        IServiceProvider _services;

        public PrometheusBot()
        {
            _localSettings = LoadSettings();

            string settingsPath = Path.Combine(Program.Directory, "settings.json");
            SettingsService settings = new(settingsPath, _localSettings.ConnectionString);

            DiscordSocketConfig config = new()
            {
                MessageCacheSize = 100
            };
            _client = new(config);

            _client.Log += Log;
            _client.Ready += OnReady;


            CommandServiceConfig commandsConfig = new()
            {
                CaseSensitiveCommands = false,
                DefaultRunMode = RunMode.Async,
                IgnoreExtraArgs = false
            };
            CommandService commands = new(commandsConfig);
            NonStandardCommandService nonStandardCommands = new(RunMode.Async);

            _services = GetServiceProvider(_client, commands, nonStandardCommands, _localSettings, settings);

            _commandHandler = new(_services);
        }

        async Task OnReady()
        {
            var lavaNode = _services.GetService<LavaNode>();
            if (!lavaNode.IsConnected)
            {
                await lavaNode.ConnectAsync();
            }
        }

        private IServiceProvider GetServiceProvider(
            DiscordSocketClient client,
            CommandService commands,
            NonStandardCommandService nonStandardCommands,
            LocalSettingsService localSettings,
            SettingsService settings)
        {

            var services = new ServiceCollection()
                .AddSingleton(client)
                .AddSingleton(commands)
                .AddSingleton(nonStandardCommands)
                .AddSingleton(localSettings)
                .AddSingleton(settings)
                .AddSingleton(this)
                .AddSingleton<MessageHistoryService>()
                .AddSingleton<AudioService>()
                .AddSingleton<Random>()
                .AddSingleton<MusicService>()
                .AddSingleton<ReactionsService>()
                .AddSingleton<WelcomeService>()
                .AddLavaNode(x =>
                {
                    x.Hostname = "localhost";
                    x.Port = 2333;
                    x.Authorization = "youshallnotpass";
                });

            return services.BuildServiceProvider();
        }

        public async Task StartAsync()
        {
            await _client.LoginAsync(TokenType.Bot, _localSettings.DiscordToken);
            await _commandHandler.InstallCommandsAsync();
            await _client.StartAsync();
        }

        public async Task StopAsync()
        {
            await _client.StopAsync();
        }

        public Task Log(LogMessage message)
        {
            Console.WriteLine(message);
            return Task.CompletedTask;
        }


        private static LocalSettingsService LoadSettings()
        {
            string path = Path.Join(Program.Directory, "LocalSettings.json");
            LocalSettingsService settings = new(path);
            settings.Load();
            if (!string.IsNullOrWhiteSpace(settings.DiscordToken))
            {
                settings.Save();
                return settings;
            }

            //Token was not found, presumably first time running program. Prompt user for token
            do
            {
                Console.Write("Bot token: ");
                settings.DiscordToken = Console.ReadLine();
            } while (string.IsNullOrWhiteSpace(settings.DiscordToken));
            Console.WriteLine($"Saving settings. They can be changed later from file '{settings.Path}'");
            if (!settings.Save())
            {
                Console.WriteLine("Failed to write new settings. You will be promted for a token again next time the program runs");
            }
            return settings;
        }
    }
}
