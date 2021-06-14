using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrometheusBot.Services.Audio
{
    public class SpeakerChangedArgs : EventArgs
    {
        public ulong Speaker { get; }

        public SpeakerChangedArgs(ulong speaker)
        {
            Speaker = speaker;
        }
    }
}
