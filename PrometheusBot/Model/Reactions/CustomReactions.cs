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
            sql = FormatAddOrUpdateQuery(sql, reaction);
            _data.SaveData(sql, reaction);
        }
        private string FormatAddOrUpdateQuery(string sql, ReactionModel reaction)
        {
            var rowsAndParams = GetColumnsAndParams(reaction);
            var columns = rowsAndParams[0];
            var parameters = rowsAndParams[1]; 

            string columnsString = string.Join(", ", columns);
            string parametersString = string.Join(", ", parameters);
            IList<string> pairs = GetPairs(columns, parameters);
            string updateString = string.Join(", ", pairs);

            return string.Format(sql, columnsString, parametersString, updateString);
        }
        private List<string>[] GetColumnsAndParams(ReactionModel reaction)
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

            var result = new[] { columns, parameters };
            return result;

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
        public IList<ReactionModel> GetAllReactions(ReactionModel reaction)
        {
            string sql = GetAllReactionsQuery(reaction);
            return _data.LoadData<ReactionModel, object>(sql, reaction);
        }
        private string GetAllReactionsQuery(ReactionModel reaction)
        {
            string additionalConditions = "";
            var ColumnsAndParams = GetColumnsAndParams(reaction);
            var columns = ColumnsAndParams[0];
            var parameters = ColumnsAndParams[1];

            for (int i = 0; i < columns.Count; i++)
            {
                string value;
                if (columns[i] == "text_trigger" || columns[i] == "text_response")
                    value = $"instr({columns[i]}, {parameters[i]})";
                else
                    value = $"{columns[i]} = {parameters[i]}";
                additionalConditions += $"and {value} ";
            }

            string sql = Queries.getAllReactions;
            sql = string.Format(sql, additionalConditions);
            return sql;
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
