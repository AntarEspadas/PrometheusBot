using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrometheusBot.Model.Reactions
{
    public class CustomReactions
    {
        public static CustomReactions Instance { get; } = new();
        private DataAccess _data;
        private CustomReactions()
        {
            _data = PrometheusModel.Instance.Data;
        }
        public void AddOrUpdate(ReactionModel reaction)
        {
            string sql = Queries.updateReaction;
            sql = FormatQuery(sql, reaction);
            _data.SaveData(sql, reaction);
        }
        private string FormatQuery(string sql, ReactionModel reaction)
        {
            List<string> columns = new();
            List<string> parameters = new();

            AddIfNotNull(columns, reaction.Trigger, "text_trigger");
            AddIfNotNull(parameters, reaction.Trigger, "@Trigger");
            AddIfNotNull(columns, reaction.Response, "text_response");
            AddIfNotNull(parameters, reaction.Response, "@Response");
            AddIfNotNull(columns, reaction.Anywhere, "anywhere");
            AddIfNotNull(parameters, reaction.Anywhere, "@Anywhere");
            AddIfNotNull(columns, reaction.Weight, "weight");
            AddIfNotNull(parameters, reaction.Weight, "@Weight");

            string columnsString = string.Join(", ", columns);
            string parametersString = string.Join(", ", parameters);
            IList<string> pairs = GetPairs(columns, parameters);
            string updateString = string.Join(", ", pairs);

            return string.Format(sql, columnsString, parametersString, updateString);
        }
        private IList<string> GetPairs(IList<string> columns, IList<string> parameters)
        {
            List<string> result = new(columns.Count);
            for (int i = 0; i < columns.Count; i++)
            {
                result.Add(columns[i] + "=" + parameters[i]);
            }
            return result;
        }
        private void AddIfNotNull(IList<string> list, object obj, string value)
        {
            if (obj is not null)
                list.Add(value);
        }
        public void Delete(ulong guildId, string id)
        {
            string sql = Queries.deleteReaction;
            _data.SaveData(sql, new { GuildId = guildId, Id = id });
        }
        public bool Exists(ulong guildId, string id)
        {
            string sql = "select guild_id from reactions where guild_id = @GuildId and id = @Id";
            List<ulong> result = _data.LoadData<ulong, object>(sql, new { GuildId = guildId, Id = id });
            return result.Count > 0;
        }
        public IList<ReactionModel> GetAllReactions(ulong guildId)
        {
            string sql = Queries.selectAllReactions;
            return _data.LoadData<ReactionModel, object>(sql, new { GuildId = guildId });
        }
        public IList<ReactionMatch> GetMatchingReactions(ulong guildId, string message)
        {
            string sql = Queries.selectAnywhereMatchingReactions;
            return _data.LoadData<ReactionMatch, object>(sql, new { GuildId = guildId, Message = message });
        }
        public IList<ReactionMatch> GetStrictMatchingReactions(ulong guildId, string message)
        {
            string sql = Queries.selectStrictMatchingReactions;
            return _data.LoadData<ReactionMatch, object>(sql, new { GuildId = guildId, Message = message });
        }
        public int Count(ulong guildId)
        {
            string sql = Queries.countReactions;
            return _data.LoadData<int, object>(sql, new { GuildId = guildId }).FirstOrDefault();
        }
        public ReactionModel GetReaction(ulong guildId, string id)
        {
            string sql = Queries.getReaction;
            return _data.LoadData<ReactionModel, object>(sql, new { GuildId = guildId, Id = id }).FirstOrDefault();
        }
    }
}
