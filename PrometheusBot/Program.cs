using System;
using System.IO;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using PrometheusBot.Commands;
using PrometheusBot.Model;
using PrometheusBot.Services;
using PrometheusBot.Services.MessageHistory;

namespace PrometheusBot
{
    class Program
    {
        public static string Directory { get; } = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        static async Task Main(string[] args)
        {
            LocalSettingsService settings = LoadSettings();

            string settingsPath = Path.Combine(Directory, "settings.json");
            PrometheusModel.Instance.Initialize(settingsPath, settings.ConnectionString);

            DiscordSocketConfig config = new()
            {
                MessageCacheSize = 100
            };
            DiscordSocketClient client = new(config);

            MessageHistoryService messageHistory = new();

            client.Log += Log;
            client.MessageUpdated += messageHistory.AddAsync;
            client.MessageDeleted += messageHistory.AddDeletedAsync;

            await client.LoginAsync(TokenType.Bot, settings.DiscordToken);
            await client.StartAsync();


            CommandServiceConfig commandsConfig = new()
            {
                CaseSensitiveCommands = false,
                DefaultRunMode = RunMode.Async,
                IgnoreExtraArgs = false
            };
            CommandService commands = new(commandsConfig);
            NonStandardCommandService nonStandardCommands = new(RunMode.Async);

            var services = GetServiceProvider(client, commands, nonStandardCommands, settings, messageHistory);
            CommandHandler commandHandler = new(services);
            await commandHandler.InstallCommandsAsync();

            try
            {
                await Task.Delay(-1);
            }
            finally
            {
                await client.LogoutAsync();
            }
        }
        
        public static Task Log(LogMessage message)
        {
            Console.WriteLine(message);
            return Task.CompletedTask;
        }

        private static LocalSettingsService LoadSettings()
        {
            string path = Path.Join(Directory, "LocalSettings.json");
            LocalSettingsService settings = new(path);
            settings.Load();
            if (!string.IsNullOrWhiteSpace(settings.DiscordToken)) return settings;

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
        private static IServiceProvider GetServiceProvider(
            DiscordSocketClient client,
            CommandService commands,
            NonStandardCommandService nonStandardCommands,
            LocalSettingsService localSettings,
            MessageHistoryService messageHistory)
        {
            var services = new ServiceCollection()
                .AddSingleton(client)
                .AddSingleton(commands)
                .AddSingleton(nonStandardCommands)
                .AddSingleton(localSettings)
                .AddSingleton(messageHistory);

            return services.BuildServiceProvider();
        }
    }
}
