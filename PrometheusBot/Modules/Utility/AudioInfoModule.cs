using Discord;
using Discord.Audio;
using Discord.Commands;
using PrometheusBot.Services.Audio;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PrometheusBot.Modules.Utility
{
    public class AudioInfoModule : ModuleBase<SocketCommandContext>
    {

        private readonly AudioService _audioService;
        private string _url;
        private static readonly HttpClient httpClient = new();

        public AudioInfoModule(AudioService audioService)
        {
            _audioService = audioService;
        }

        [Command("Listen")]
        public async Task Listen(string url)
        {
            _url = url;

            var voiceChannel = ((IVoiceState)Context.User).VoiceChannel;
            var connection = await _audioService.JoinAsync(voiceChannel);
            await connection.PlayAsync(@"awawawawawawawa.m4a");
            connection.UpdateSpeaker = true;
            connection.SpeakerChanged += OnSpeakerChanged;
        }

        private void OnSpeakerChanged(object sender, SpeakerChangedArgs e)
        {
            httpClient.GetAsync(_url + "?user=" + e.Speaker);
        }

        [Command("Disconnect")]
        public async Task Disconnect()
        {
            await _audioService.DisconnectAsync(Context.Guild.Id);
        }
    }
}
