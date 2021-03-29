using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;

namespace PrometheusBot.Forms
{
    public class Button
    {
        public event EventHandler<ButtonEventArgs> Clicked;
        public IEmote Emote { get; }
        public Button(IEmote emote)
        {
            Emote = emote;
        }
        internal void InvokeClicked(IUser user, bool added)
        {
            Clicked?.Invoke(this, new ButtonEventArgs(user, added));
        }
    }
}
