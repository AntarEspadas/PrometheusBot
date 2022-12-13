using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using PrometheusBot.Extensions;
using PrometheusBot.Services.MessageHistory;
using PrometheusBot.Services.Settings;

namespace PrometheusBot.Modules.Utility.History
{
    class MessageHistoryModule : ModuleBase<SocketCommandContext>
    {
        private readonly MessageHistoryService _messageHistory;
        private readonly SettingsService _settings;

        public MessageHistoryModule(MessageHistoryService messageHistory, SettingsService settings)
        {
            _messageHistory = messageHistory;
            _settings = settings;
        }

        [Command("unedit")]
        public async Task UneditAsync()
        {
            IMessage message = await Context.Message.GetReplyingMessageAsync();
            if (message.EditedTimestamp is null)
                return;
            SettingLookupInfo info = new("unedit:opt-out") { GId = Context.Guild.Id, CId = Context.Channel.Id };
            _settings.GetSetting(info, out bool optOut, true);
            if (optOut)
            {
                await ReplyAsync("Sorry, that command is disabled for this channel or server");
                return;
            }
            info.CId = null;
            info.UId = message.Author.Id;
            _settings.GetSetting(info, out optOut, true);
            if (optOut)
            {
                await ReplyAsync("Sorry, the targeted user has opted out of this feature");
                return;
            }
            var messages = _messageHistory.GetHistory(message);
            if (messages is null)
                return;
            _ = SendAllHistory(messages);
        }
        [Command("undelete")]
        public async Task UndeleteAsync()
        {
            SettingLookupInfo info = new("undelete:opt-out") { GId = Context.Guild.Id, CId = Context.Channel.Id };
            _settings.GetSetting(info, out bool optOut, true);
            if (optOut)
            {
                await ReplyAsync("Sorry, that command is disabled for this channel or server");
                return;
            }
            var messages = _messageHistory.GetLastDeletedHistory(Context.Message.Channel);
            if (messages is null)
                return;
            info.CId = null;
            info.UId = messages.First.Value.Author.Id;
            _settings.GetSetting(info, out optOut, true);
            if (optOut)
            {
                await ReplyAsync("Sorry, the latest user to have deleted a message in this channel has opted out of this feature");
                return;
            }
            _ = SendAllHistory(messages);
        }
        private async Task SendAllHistory(ICollection<IMessage> messages)
        {
            foreach (var item in messages)
            {
                var eb = item.Author.Quote(item.Content, item.Timestamp);
                await ReplyAsync(embed: eb.Build());
            }
        }

    }
}
