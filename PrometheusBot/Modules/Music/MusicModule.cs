using Discord;
using Discord.Audio;
using Discord.Commands;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using Victoria;
using Victoria.Responses.Search;
using System.Linq;
using PrometheusBot.Services.Music;

namespace PrometheusBot.Modules.Music
{
    public class MusicModule : ModuleBase<SocketCommandContext>
    {
        private readonly MusicService _musicService;

        public MusicModule(MusicService musicService)
        {
            _musicService = musicService;
        }

        [Command("Join")]
        [Summary("Joins the voice channel the user is currently in")]
        public async Task<RuntimeResult> Join()
        {
            return await _musicService.JoinAsync(Context);
        }

        [Command("Play")]
        [Summary("Play a song from a url in the current voice channel")]
        public async Task<RuntimeResult> Play(
            [Summary("The link to a video or a search query for YouTube")]
            [Remainder] string query)
        {
            return await _musicService.PlayAsync(Context, query);
        }

        [Command("Disconnect")]
        [Alias("DC")]
        [Summary("Disconnects from the current voice channel")]
        public async Task<RuntimeResult> Disconnect()
        {
            return await _musicService.DisconnectAsync(Context);
        }

        [Command("Pause")]
        [Summary("Pauses playback of the current track")]
        public async Task<RuntimeResult> Pause()
        {
            return await _musicService.PauseAsync(Context);
        }

        [Command("Resume")]
        [Summary("Resumes playback of the current track")]
        public async Task<RuntimeResult> Resume()
        {
            return await _musicService.ResumeAsync(Context);
        }

        [Command("Force skip")]
        [Alias("FS")]
        [Summary("Forces the currently playing track to skip")]
        public async Task<RuntimeResult> ForceSkip()
        {
            return await _musicService.ForceSkipAsync(Context);
        }

        [Command("Shuffle")]
        [Summary("Shuffles the items in the queue")]
        public async Task<RuntimeResult> Shuffle()
        {
            return await _musicService.ShuffleAsync(Context);
        }

        [Command("Loop")]
        [Alias("Loop track")]
        [Summary("Plays the current track on a loop")]
        public async Task<RuntimeResult> LoopTrack()
        {
            return await _musicService.ToggleTrackLoopAsync(Context);
        }

        [Command("Loop playlist")]
        [Summary("Plays the current playlist on a loop")]
        public async Task<RuntimeResult> LoopPlaylist()
        {
            return await _musicService.TogglePlaylistLoopAsync(Context);
        }
    }
}
