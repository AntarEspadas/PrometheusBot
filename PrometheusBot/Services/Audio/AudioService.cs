using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrometheusBot.Services.Audio
{
    public class AudioService
    {
        private Dictionary<ulong, AudioConnection> _connections = new();

        public async Task<AudioConnection> JoinAsync(IVoiceChannel voiceChannel)
        {
            ulong guildId = voiceChannel.GuildId;

            _connections.TryGetValue(guildId, out var connection);
            if (connection?.VoiceChannel?.Id == voiceChannel.Id) return null;
            if (connection is null)
            {
                connection = new();
                connection.Disconnected += OnDisconnection;
            }
            _connections[guildId] = connection;
            await connection.MoveChannelAsync(voiceChannel);
            return connection;
        }

        public async Task<bool> DisconnectAsync(ulong guildId)
        {
            if (!_connections.TryGetValue(guildId, out var connection)) return false;
            await connection.CloseAsync();
            return true;
        }

        private void OnDisconnection(object sender, EventArgs e)
        {
            var connection = (AudioConnection)sender;
            ulong guildId = connection.VoiceChannel.GuildId;
            _connections.Remove(guildId);
        }


    }
}
