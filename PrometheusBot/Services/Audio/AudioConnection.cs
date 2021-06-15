using Discord;
using Discord.Audio;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace PrometheusBot.Services.Audio
{
    public class AudioConnection
    {
        public event EventHandler<SpeakerChangedArgs> SpeakerChanged;
        public event EventHandler Disconnected;

        private readonly Timer _timer;
        private readonly Dictionary<ulong, int> _frames = new();
        private readonly Dictionary<ulong, DateTime> _lastSpoken = new();

        public bool UpdateSpeaker { get => _updateSpeaker; set => _updateSpeaker = _timer.Enabled = value; }
        private bool _updateSpeaker;

        private ulong _lastSpeakingUser = 0;

        public IVoiceChannel VoiceChannel { get; private set; }
        public IAudioClient AudioClient { get; private set; }

        public AudioConnection()
        {
            _timer = new(500);
            _timer.Elapsed += CheckSpeaker;
        }

        public async Task CloseAsync()
        {
            await VoiceChannel.DisconnectAsync();
            UpdateSpeaker = false;
            Disconnected?.Invoke(this, EventArgs.Empty);
        }

        public async Task MoveChannelAsync(IVoiceChannel value)
        {
            VoiceChannel = value;
            AudioClient = await value.ConnectAsync();
            AudioClient.Disconnected += OnDisconnect;
        }

        private async Task OnDisconnect(Exception arg)
        {
            await CloseAsync();
        }

        public async Task PlayAsync(string path)
        {
            using var ffmpeg = Process.Start(new ProcessStartInfo()
            {
                FileName = "ffmpeg",
                Arguments = $"-hide_banner -loglevel panic -i {path} -ac 2 -f s16le -ar 48000 pipe:1",
                UseShellExecute = false,
                RedirectStandardOutput = true
            });
            using var ffmpegStream = ffmpeg.StandardOutput.BaseStream;
            using var outStream = AudioClient.CreatePCMStream(AudioApplication.Mixed);
            await ffmpegStream.CopyToAsync(outStream);
        }

        private void CheckSpeaker(object sender, ElapsedEventArgs e)
        {
            var streams = AudioClient.GetStreams();
            foreach (var item in streams)
            {
                UpdateLastSpoken(item.Key, item.Value);
            }

            ulong speakingUser = GetSpeakingUser();
            if (speakingUser != _lastSpeakingUser && speakingUser != 0)
            {
                _lastSpeakingUser = speakingUser;
                SpeakerChanged?.Invoke(this, new SpeakerChangedArgs(speakingUser));
            }
        }
        private void UpdateLastSpoken(ulong userId, AudioInStream stream)
        {
            if (!_frames.ContainsKey(userId))
                _frames[userId] = 0;
            int lastFrameCount = _frames[userId];
            int availableFrames = stream.AvailableFrames;
            if (availableFrames == 0 || availableFrames < lastFrameCount) return;
            Console.WriteLine(userId + " is speaking");
            _frames[userId] = availableFrames;
            _lastSpoken[userId] = DateTime.Now;
            stream.TryReadFrame(default, out _);
        }

        private ulong GetSpeakingUser()
        {
            //Return the id of a user that has spoken within the last second, only if no other user has spoken within the las few seconds

            ulong speakingUser = default;
            foreach (var item in _lastSpoken)
            {
                var timeSinceSpoken = DateTime.Now - item.Value;
                if (timeSinceSpoken > TimeSpan.FromSeconds(3)) continue;
                if (timeSinceSpoken < TimeSpan.FromSeconds(1))
                {
                    if (speakingUser != default) return default;
                    speakingUser = item.Key;
                    continue;
                }
                //User hasn't spoken within the last second, but spoke recently, meaning no user can be valid
                return default;
            }
            return speakingUser;
        }
    }
}
