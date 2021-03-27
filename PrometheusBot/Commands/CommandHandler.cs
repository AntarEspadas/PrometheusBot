using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using PrometheusBot.Model;

namespace PrometheusBot.Commands
{
    class CommandHandler
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;
        private readonly NonStandardCommandService _nonStandardCommands;

        public CommandHandler(DiscordSocketClient client, CommandService commands, NonStandardCommandService nonStandardCommands)
        {
            _client = client;
            _commands = commands;
            _nonStandardCommands = nonStandardCommands;
        }
        public async Task InstallCommandsAsync()
        {
            // Hook the MessageReceived event into our command handler
            _client.MessageReceived += HandleCommandAsync;
            _commands.CommandExecuted += OnCommandExecutedAsync;
            _nonStandardCommands.CommandExecuted += OnCommandExecutedAsync;

            Assembly assembly = Assembly.GetEntryAssembly();
            await _commands.AddModulesAsync(assembly, null);
            await _nonStandardCommands.AddModulesAsync(assembly);
        }

        private async Task HandleCommandAsync(SocketMessage messageParam)
        {
            // Don't process the command if it was a system message
            if (messageParam is not SocketUserMessage message) return;

            // Create a number to track where the prefix ends and the command begins
            int argPos = 0;

            // Create a WebSocket-based command context based on the message
            SocketCommandContext context = new(_client, message);

            //Get the prefixes
            ulong UserId = context.User.Id;
            ulong GuildId = context.Guild.Id;
            ulong ChannelId = context.Channel.Id;
            string[] prefixes = PrometheusModel.Instance.GetPrefixes(UserId, GuildId, ChannelId);
            string naturalPrefix = prefixes[0];
            string syntheticPrefix = prefixes[1];

            // Determine if the message is a command based on the prefix and make sure no bots trigger commands
            if (!(message.HasStringPrefix(syntheticPrefix, ref argPos) ||
                message.HasStringPrefix(naturalPrefix + " ", ref argPos) ||
                message.HasStringPrefix(naturalPrefix + ", ", ref argPos) ||
                message.HasMentionPrefix(_client.CurrentUser, ref argPos)) ||
                message.Author.IsBot)
            {
                await _nonStandardCommands.ExecuteAsync(context);
                return;
            }


            // Execute the command with the command context we just
            // created, along with the service provider for precondition checks.
            await _commands.ExecuteAsync(
                context: context,
                argPos: argPos,
                services: null);
        }

        private Task OnCommandExecutedAsync(Optional<CommandInfo> command, ICommandContext context, IResult result)
        {
            //If the command was succesful, simply log that it was run. If not, log the error and the reason
            string source = "CommaneExecution";
            string commandName = command.IsSpecified ? command.Value.Name : "A command";
            if (string.IsNullOrEmpty(result?.ErrorReason))
            {
                LogCommandExecution(source, commandName);
                return Task.CompletedTask;
            }
            LogCommandError(source, commandName, result);
            return Task.CompletedTask;
        }
        private void LogCommandExecution(string source, string commandName)
        {
            Program.Log(new LogMessage(LogSeverity.Info, source, $"{commandName} executed succesfully."));
        }

        private void LogCommandError(string source, string commandName, IResult result)
        {
            Program.Log(new LogMessage(LogSeverity.Error, source, $"{commandName} caused an error: {result.Error}. Reason: {result.ErrorReason}"));
        }
    }
}
