using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using PrometheusBot.Commands;
using PrometheusBot.Model;

namespace PrometheusBot
{
    class Program
    {
        public static string Directory { get; } = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        static async Task Main(string[] args)
        {
            LocalSettings settings = LoadSettings();

            DiscordSocketConfig config = new()
            {
                MessageCacheSize = 100
            };
            DiscordSocketClient client = new(config);
            client.Log += Log;

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

            CommandHandler commandHandler = new(client, commands, nonStandardCommands);
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

        private static LocalSettings LoadSettings()
        {
            LocalSettings settings = LocalSettings.Load();
            if (!string.IsNullOrWhiteSpace(settings.DiscordToken)) return settings;

            //Token was not found, presumably first time running program. Prompt user for token
            do
            {
                Console.Write("Bot token: ");
                settings.DiscordToken = Console.ReadLine();
            } while (string.IsNullOrWhiteSpace(settings.DiscordToken));
            Console.WriteLine($"Saving settings. They can be changed later from file '{LocalSettings.Path}'");
            if (!settings.Save())
            {
                Console.WriteLine("Failed to write new settings. You will be promted for a token again next time the program runs");
            }
            return settings;
        }
    }
}
