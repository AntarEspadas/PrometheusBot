using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PrometheusBot.Services.MessageHistory
{
    public class MessageHistoryService
    {
        public Dictionary<ulong, Dictionary<ulong, Dictionary<ulong, MessageHistoryList>>> Guilds { get; private set; }
        public MessageHistoryList this[ulong guildId, ulong channelId, ulong messageId] { get => GetHistory(guildId, channelId, messageId); }

        public MessageHistoryService()
        {
            Guilds = new Dictionary<ulong, Dictionary<ulong, Dictionary<ulong, MessageHistoryList>>>();
        }

        public void Add(IMessage message)
        {
            var channel = message.Channel as SocketTextChannel;
            ulong guildId = channel.Guild.Id;
            ulong channelId = channel.Id;
            ulong messageId = message.Id;

            if (!Guilds.ContainsKey(guildId))
            {
                Guilds[guildId] = new Dictionary<ulong, Dictionary<ulong, MessageHistoryList>>();
            }
            var channels = Guilds[guildId];
            if (!channels.ContainsKey(channelId))
            {
                channels[channelId] = new Dictionary<ulong, MessageHistoryList>();
            }
            var messages = channels[channelId];
            if (!messages.ContainsKey(messageId))
            {
                messages[messageId] = new MessageHistoryList();
            }
            var messageHistory = messages[messageId];
            messageHistory.AddLast(message);
        }

        public async Task AddAsync(Cacheable<IMessage, ulong> before, SocketMessage after, ISocketMessageChannel channel)
        {
            if (before.HasValue)
            {
                Add(await before.GetOrDownloadAsync());
            }
        }

        public async Task AddDeletedAsync(Cacheable<IMessage, ulong> deleted, ISocketMessageChannel channel)
        {
            if (deleted.HasValue)
            {
                var message = await deleted.GetOrDownloadAsync();
                Add(message);
                GetHistory(message).DeletedTimestamp = DateTimeOffset.Now;
            }
        }

        public MessageHistoryList GetHistory(IMessage message)
        {
            var channel = message.Channel as SocketTextChannel;
            ulong guildId = channel.Guild.Id;
            ulong channelId = channel.Id;
            ulong messageId = message.Id;

            return this[guildId, channelId, messageId];
        }

        private MessageHistoryList GetHistory(ulong guildId, ulong channelId, ulong messageId)
        {
            Dictionary<ulong, Dictionary<ulong, MessageHistoryList>> channels;
            if (!Guilds.TryGetValue(guildId, out channels))
                return null;
            Dictionary<ulong, MessageHistoryList> messages;
            if (!channels.TryGetValue(channelId, out messages))
                return null;
            messages.TryGetValue(messageId, out MessageHistoryList messageHistory);
            return messageHistory;
        }

        public MessageHistoryList GetLastDeletedHistory(IMessageChannel channel)
        {
            var textChannel = channel as SocketTextChannel;
            ulong guildId = textChannel.Guild.Id;
            ulong channelId = channel.Id;

            if (!Guilds.ContainsKey(guildId))
            {
                Guilds[guildId] = new Dictionary<ulong, Dictionary<ulong, MessageHistoryList>>();
            }
            var channels = Guilds[guildId];
            if (!channels.ContainsKey(channelId))
            {
                channels[channelId] = new Dictionary<ulong, MessageHistoryList>();
            }
            var messages = channels[channelId];
            var latest = new MessageHistoryList();
            latest.DeletedTimestamp = DateTimeOffset.MinValue;
            foreach (var history in messages)
                if (history.Value.IsDeleted && history.Value.DeletedTimestamp > latest.DeletedTimestamp)
                    latest = history.Value;
            return latest;
        }
    }
}
