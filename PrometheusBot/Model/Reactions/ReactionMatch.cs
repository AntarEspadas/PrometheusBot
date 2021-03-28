using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrometheusBot.Model.Reactions
{
    public class ReactionMatch
    {
        public string Trigger { get; }
        public string Response { get; }
        public long MatchIndex { get; }
        public int Weight { get; }

        public ReactionMatch(string text_trigger, string text_response, long match_index, int weight)
        {
            Trigger = text_trigger;
            Response = text_response;
            MatchIndex = match_index;
            Weight = weight;
        }
    }
}
