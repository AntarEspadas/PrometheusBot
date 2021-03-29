using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Timer = System.Timers.Timer;

namespace PrometheusBot.Forms
{
    public abstract class Form
    {
        public IUserMessage Message { get; private set; }
        public ICommandContext Context { get; }
        public DialogResult Result { get; set; }
        public Timer TimeoutTimer { get; }
        public ImmutableDictionary<IEmote, Button> Buttons => _buttons.ToImmutableDictionary();
        private readonly Dictionary<IEmote, Button> _buttons = new();
        private readonly List<Button> _buttonslist = new();

        public string Text { get => _text; set => SetText(value); }
        private string _text;
        public Embed Embed { get => _embed; set => SetEmbed(value); }
        private Embed _embed;

        private readonly ManualResetEvent _resetEvent = new(true);

        public Form(SocketCommandContext context, int timeoutSeconds)
        {
            Context = context;
            context.Client.ReactionAdded += ReactionAdded;
            context.Client.ReactionRemoved += ReactionRemoved;

            if (timeoutSeconds >= 0)
            {
                TimeoutTimer = new(timeoutSeconds * 1000);
                TimeoutTimer.Elapsed += async (_, _) => { Result = DialogResult.TimedOut; await CloseAsync(); };
            }
            else
                TimeoutTimer = new();
            TimeoutTimer.AutoReset = false;
        }
        public async Task<DialogResult> ShowDialogAsync()
        {
            TimeoutTimer?.Start();
            Message = await Context.Channel.SendMessageAsync(Text, embed: Embed);
            var emotes = _buttonslist.Select(button => button.Emote).ToArray();
            await Message.AddReactionsAsync(emotes);
            _resetEvent.Reset();
            return await Task.Run(GetResult);
        }
        private DialogResult GetResult()
        {
            _resetEvent.WaitOne();
            return Result;
        }

        private Task ReactionAdded(Cacheable<IUserMessage, ulong> cacheable, ISocketMessageChannel channel, SocketReaction reaction)
        {
            HandleReaction(reaction, true);
            return Task.CompletedTask;
        }

        private Task ReactionRemoved(Cacheable<IUserMessage, ulong> cacheable, ISocketMessageChannel channel, SocketReaction reaction)
        {
            HandleReaction(reaction, false);
            return Task.CompletedTask;
        }

        private void HandleReaction(SocketReaction reaction, bool added)
        {
            if (Message is null) return;
            if (reaction.UserId == Context.Client.CurrentUser.Id) return;
            if (reaction.Message.Value?.Id != Message.Id) return;
            if (!Buttons.TryGetValue(reaction.Emote, out var button)) return;
            TimeoutTimer.Interval = TimeoutTimer.Interval;
            button.InvokeClicked(reaction.User.Value, added);
        }

        private void SetText(string value)
        {
            _text = value;
            Message?.ModifyAsync(msg => msg.Content = _text);
        }
        private void SetEmbed(Embed value)
        {
            _embed = value;
            Message?.ModifyAsync(msg => msg.Embed = _embed);
        }
        public async Task CloseAsync()
        {
            try { await Message.RemoveAllReactionsAsync(); } catch { }
            Message = null;
            TimeoutTimer.Enabled = false;
            _resetEvent.Set();
        }
        public async Task<bool> AddButtonAsync(Button button)
        {
            bool result = _buttons.TryAdd(button.Emote, button);
            if (result)
            {
                _buttonslist.Add(button);
                if (Message is not null)
                    await Message.AddReactionAsync(button.Emote);
            }
            return result;
        }
        public async Task<bool> RemoveButtonAsync(IEmote buttonEmote)
        {
            bool result = _buttons.Remove(buttonEmote, out Button button);
            if (result)
            {
                _buttonslist.Remove(button);
                try { await Message?.RemoveAllReactionsForEmoteAsync(buttonEmote); } catch { };
            }
            return result;
        }
    }
}
