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
        private ISocketMessageChannel _currentChannel;

        public InteractiveConsole(PrometheusBot bot)
        {
            _bot = bot;
        }

        public void Run()
        {
            while (HandleCommand()) ;
            _bot.StopAsync().Wait();
        }

        private bool HandleCommand()
        {
            var currentGuild = ((SocketGuildChannel)_currentChannel)?.Guild;
            Console.Write($"{currentGuild?.Name ?? "No guild"}, {_currentChannel?.Name ?? "No channel"}> ");
            string command = Console.ReadLine();

            if (string.IsNullOrEmpty(command))
                return true;

            if (!command.StartsWith('/'))
                return SendMessage(command);

            string[] splitCommand = command.Split(' ');
            string baseCommand = splitCommand[0];

            if (baseCommand == "/stop" || baseCommand == "/exit" || baseCommand == "/quit")
                return false;

            if (baseCommand == "/ls")
            {
                if (splitCommand.Length == 1)
                    return ListGuilds();
                if (splitCommand.Length == 2)
                    return ListChannels(splitCommand[1]);
            }

            if (baseCommand == "/channel")
                return ChangeChannel(splitCommand[1]);

            Console.WriteLine("Invalid syntax");
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

            var channel = _bot.Client.GetChannel(channelId);

            //if (channel is null)
            //{
            //    Console.WriteLine("Channel not found");
            //    return true;
            //}

            _currentChannel = (ISocketMessageChannel)channel;
            return true;
        }

        private bool ListGuilds()
        {
            var guilds = _bot.Client.Guilds
                .Select( guild => $"{guild.Id, -25}{guild.Name}");
            Console.WriteLine(string.Join('\n', guilds));
            return true;
        }

        private bool ListChannels(string guild)
        {
            bool isValid = ulong.TryParse(guild, out ulong guildId);

            if (!isValid)
            {
                Console.WriteLine("Invalid guild id");
                return true;
            }

            var channels = _bot
                .Client
                .GetGuild(guildId)
                .Channels
                .Where(channel => channel is ISocketMessageChannel)
                .Select(channel => $"{channel.Id,-25}{channel.Name}");
            Console.WriteLine(string.Join('\n', channels));
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
