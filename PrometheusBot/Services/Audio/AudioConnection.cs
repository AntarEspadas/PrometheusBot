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
        private readonly Dictionary<ulong, Queue<int>> _speakPoints = new();

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
                UpdateSpokenShare(item.Key, item.Value);
            }

            ulong speakingUser = GetSpeakingUser();
            if (speakingUser != _lastSpeakingUser && speakingUser != 0)
            {
                _lastSpeakingUser = speakingUser;
                SpeakerChanged?.Invoke(this, new SpeakerChangedArgs(speakingUser));
            }
        }
        private void UpdateSpokenShare(ulong userId, AudioInStream stream)
        {
            if (!_frames.ContainsKey(userId))
                _frames[userId] = 0;
            int lastFrameCount = _frames[userId];
            int availableFrames = stream.AvailableFrames;

            if (!_speakPoints.ContainsKey(userId))
                _speakPoints[userId] = new(new int[10]);
            var userSpeakPoints = _speakPoints[userId];
            userSpeakPoints.Dequeue();

            if (availableFrames == 0 || availableFrames < lastFrameCount)
            {
                userSpeakPoints.Enqueue(0);
                return;
            }

            //If this part is reched, then the user spoke within the last half second
            _frames[userId] = availableFrames;
            userSpeakPoints.Enqueue(1);
            stream.TryReadFrame(default, out _);
        }

        private ulong GetSpeakingUser()
        {
            //Find the user who spoke the most within the last 5 seconds, so long as they spoke for at least 1.5 seconds
            //That is, the user with the most points, and with at least 3 pionts
            //If multiple users spoke the most or no one spoke for more than 1.5 seconds, no user is returned
            int maxPoints = 0;
            ulong result = 0;
            foreach (var item in _speakPoints)
            {
                int points = item.Value.Sum();
                if (points < 3) continue;
                if (points == maxPoints) return 0;
                if (points > maxPoints) maxPoints = points;
            }
            return result;
        }
    }
}
