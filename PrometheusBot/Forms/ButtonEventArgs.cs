using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;

namespace PrometheusBot.Forms
{
    public class ButtonEventArgs : EventArgs
    {
        public IUser User { get; }
        public bool ReactionAdded { get; }
        public bool ReactionRemoved { get => !ReactionAdded; }
        internal ButtonEventArgs(IUser user, bool added)
        {
            User = user;
            ReactionAdded = added;
        }
    }
}
