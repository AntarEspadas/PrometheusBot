using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrometheusBot.InteractiveConsole
{
    class InteractiveConsole
    {

        private readonly PrometheusBot _bot;
        private SocketGuild _currentGuild;
        private ISocketMessageChannel _currentChannel;

        public InteractiveConsole(PrometheusBot bot)
        {
            _bot = bot;
        }

        public void Run()
        {
            while (handleCommand()) ;
            _bot.StopAsync().Wait();
        }

        private bool handleCommand()
        {
            Console.Write($"{_currentGuild?.Name ?? "No guild"}, {_currentChannel?.Name ?? "No channel"}> ");
            string command = Console.ReadLine();

            if (!command.StartsWith('/'))
                return SendMessage(command);

            string[] splitCommand = command.Split(' ');
            string baseCommand = splitCommand[0];

            if (baseCommand == "/stop" || baseCommand == "/exit" || baseCommand == "/quit")
                return false;

            if (splitCommand.Length < 2)
            {
                Console.WriteLine("Invalid syntax");
                return true;
            }

            if (baseCommand == "/ls" || baseCommand == "/list")
                return List(splitCommand[1]);

            if (baseCommand == "/guild" || baseCommand == "/server")
                return ChangeGuild(splitCommand[1]);

            if (baseCommand == "/channel")
                return ChangeChannel(splitCommand[1]);

            return true;

        }

        private bool ChangeGuild(string id)
        {
            bool isValid = ulong.TryParse(id, out ulong guildId);
            if (!isValid)
            {
                Console.WriteLine("Invalid guild id");
                return true;
            }

            var guild = _bot.GetGuilds()
                .Where(guild => guild.Id == guildId)
                .FirstOrDefault();

            if (guild is null)
            {
                Console.WriteLine("Guild not found");
                return true;
            }

            _currentGuild = guild;
            return true;
        }

        private bool ChangeChannel(string id)
        {
            bool isValid = ulong.TryParse(id, out ulong channelId);
            if (!isValid)
            {
                Console.WriteLine("Invalid channel id");
                return true;
            }

            var channel = _currentGuild
                ?.Channels
                ?.Where(guild => guild.Id == channelId)
                ?.FirstOrDefault();

            if (channel is null)
            {
                Console.WriteLine("Channel not found");
                return true;
            }

            _currentChannel = (ISocketMessageChannel)channel;
            return true;
        }

        private bool List(string type)
        {
            if (type == "server" || type == "servers" || type == "guild" || type == "guilds")
            {
                var guilds = _bot.GetGuilds()
                    .Select( guild => $"{guild.Id, -25}{guild.Name}");
                Console.WriteLine(string.Join('\n', guilds));
                return true;
            }

            if(type == "channel" || type == "channels")
            {
                if (_currentGuild is null)
                {
                    Console.WriteLine("Unable to process command");
                    return true;
                }
                var channels = _currentGuild
                    .Channels
                    .Where(channel => channel is ISocketMessageChannel)
                    .Select(channel => $"{channel.Id, -25}{channel.Name}");
                Console.WriteLine(string.Join('\n', channels));
                return true;
            }

            Console.WriteLine("Invalid syntax");
            return true;
        }

        private bool SendMessage(string message)
        {
            try
            {
                _currentChannel.SendMessageAsync(message);
            }
            catch
            {
                Console.WriteLine("Unable to send message");
            }

            return true;
        }
    }
}
