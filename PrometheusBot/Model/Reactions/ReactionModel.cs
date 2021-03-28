using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrometheusBot.Model.Reactions
{
    public class ReactionModel
    {
        public ulong GuildId { get; }
        public string Id { get; set; }
        public string Trigger { get; set; }
        public string Response { get; set; }
        public bool? Anywhere { get; set; }
        public int? Weight { get; set; }

        public ReactionModel(ulong guildId, string id)
        {
            GuildId = guildId;
            Id = id;
        }

        public ReactionModel(ulong guild_id, string id, string text_trigger, string text_response, bool? anywhere, int? weight) : this(guild_id, id)
        {
            Trigger = text_trigger;
            Response = text_response;
            Anywhere = anywhere;
            Weight = weight;
        }
    }
}
