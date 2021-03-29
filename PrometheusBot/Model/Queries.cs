using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrometheusBot.Model
{
    static class Queries
    {
        public const string getSetting =
            @"select value from settings
              where setting_name = @SettingName
                    and ids = @Ids;";

        public const string updateSetting =
            @"insert into settings (setting_name, ids, value) Values(@SettingName, @Ids, @Value)
            on duplicate key update value=@Value";

        public const string removeSetting =
            @"delete from settings
            where setting_name = @SettingName
            and ids = @Ids";

        public const string updateReaction =
@"insert into reactions (guild_id, id, {0}) Values(@GuildId, @Id, {1})
on duplicate key update {2};";

        public const string deleteReaction =
@"delete from reactions
where guild_id = @GuildId
and id = @Id;";

        public const string selectAllReactions =
@"select * from reactions
where guild_id = @GuildId;";

        public const string selectAnywhereMatchingReactions =
@"select text_trigger, text_response, match_index, weight from
(
select *, instr(@Message, text_trigger) as match_index from anywhere_reactions
where 	guild_id = @GuildId
) as matches
where match_index;";

        public const string selectStrictMatchingReactions =
@"select text_trigger, text_response, 1 as match_index, weight from reactions
where guild_id = @GuildId
and text_trigger = @Message;";

        public const string countReactions =
@"select count(guild_id)
from reactions
where guild_id = @GuildId;";

        public const string getReaction =
@"select * from reactions
where guild_id = @GuildId
and id = @Id;";
    }
}
