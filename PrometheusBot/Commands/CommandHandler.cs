﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using PrometheusBot.Model;
using PrometheusBot.Services.NonStandardCommands;
using PrometheusBot.Services.Settings;
using Microsoft.Extensions.DependencyInjection;

namespace PrometheusBot.Commands
{
    class CommandHandler
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;
        private readonly NonStandardCommandService _nonStandardCommands;
        private readonly SettingsService _settings;
        private readonly PrometheusBot _bot;
        private readonly IServiceProvider _services;

        public CommandHandler(IServiceProvider services)
        {
            _client = services.GetService<DiscordSocketClient>();
            _commands = services.GetService<CommandService>();
            _nonStandardCommands = services.GetService<NonStandardCommandService>();
            _settings = services.GetService<SettingsService>();
            _bot = services.GetService<PrometheusBot>();
            _services = services;
        }
        public async Task InstallCommandsAsync()
        {
            // Hook the MessageReceived event into our command handler
            _client.MessageReceived += HandleCommandAsync;
            _commands.CommandExecuted += OnCommandExecutedAsync;
            _nonStandardCommands.CommandExecuted += OnCommandExecutedAsync;

            Assembly assembly = Assembly.GetEntryAssembly();
            await _commands.AddModulesAsync(assembly, _services);
            await _nonStandardCommands.AddModulesAsync(assembly);
        }

        private async Task HandleCommandAsync(SocketMessage messageParam)
        {
            // Don't process the command if it was a system message
            if (messageParam is not SocketUserMessage message) return;

            if (message.Author.IsBot) return;

            // Create a number to track where the prefix ends and the command begins
            int argPos = 0;

            // Create a WebSocket-based command context based on the message
            SocketCommandContext context = new(_client, message);

            //Get the prefixes
            ulong UserId = context.User.Id;
            ulong? GuildId = context.Guild?.Id;
            ulong ChannelId = context.Channel.Id;
            // string[] prefixes = _settings.GetPrefixes(UserId, GuildId, ChannelId);
            string[] prefixes = new[] { "navi", "n." };
            string naturalPrefix = prefixes[0];
            string syntheticPrefix = prefixes[1];


            if (!(message.HasStringPrefix(syntheticPrefix, ref argPos) ||
                message.HasStringPrefix(naturalPrefix + " ", ref argPos) ||
                message.HasStringPrefix(naturalPrefix + ", ", ref argPos) ||
                message.HasMentionPrefix(_client.CurrentUser, ref argPos)))
            {
                await _nonStandardCommands.ExecuteAsync(context, _services);
                return;
            }


            // Execute the command with the command context we just
            // created, along with the service provider for precondition checks.
            await _commands.ExecuteAsync(context, argPos, _services);
        }

        private Task OnCommandExecutedAsync(Optional<CommandInfo> command, ICommandContext context, IResult result)
        {
            //If the command was succesful, simply log that it was run. If not, log the error and the reason
            string source = "CommandExecution";
            string commandName = command.IsSpecified ? command.Value.Name : "A command";
            if (string.IsNullOrEmpty(result?.ErrorReason))
            {
                LogCommandExecution(source, commandName);
                return Task.CompletedTask;
            }
            LogCommandError(source, commandName, result);
            if (result.Error == CommandError.UnknownCommand || result.Error == CommandError.Unsuccessful || result.Error == CommandError.UnmetPrecondition)
                context.Channel.SendMessageAsync(result.ErrorReason);
            return Task.CompletedTask;
        }
        private void LogCommandExecution(string source, string commandName)
        {
            _bot.Log(new LogMessage(LogSeverity.Info, source, $"{commandName} executed succesfully."));
        }

        private void LogCommandError(string source, string commandName, IResult result)
        {
            _bot.Log(new LogMessage(LogSeverity.Error, source, $"{commandName} caused an error: {result.Error}. Reason: {result.ErrorReason}"));
        }
    }
}
