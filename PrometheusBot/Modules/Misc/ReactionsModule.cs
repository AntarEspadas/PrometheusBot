using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using PrometheusBot.Model.Reactions;
using PrometheusBot.Extensions;
using Discord;

namespace PrometheusBot.Modules.Misc
{
    public class ReactionsModule : ModuleBase<SocketCommandContext>
    {
        internal static readonly Dictionary<ICommandContext, Tuple<IList<ReactionMatch>, bool>> pendingMatches = new();

        private static readonly CustomReactions _reactions = CustomReactions.Instance;
        [Command("Reaction")]
        public async Task<RuntimeResult> AddOrUpdateReaction(string reactionId, Arguments args)
        {
            string invalidLength = "{0} length must be at most {1} characteres";
            if (reactionId.Length > 25)
                return CommandResult.FromError(CommandError.Unsuccessful, string.Format(invalidLength, "Id", 25));
            if (args.Trigger?.Length > 100)
                return CommandResult.FromError(CommandError.Unsuccessful, string.Format(invalidLength, "Trigger", 100));
            if (args.Response?.Length > 1000)
                return CommandResult.FromError(CommandError.Unsuccessful, string.Format(invalidLength, "Response", 1000));
            if (args.Weight < 1 || args.Weight > 1000)
                return CommandResult.FromError(CommandError.Unsuccessful, $"{args.Weight} must be between 1 and 1000");
            if (_reactions.Count(Context.Guild.Id) > 100)
                return CommandResult.FromError(CommandError.Unsuccessful, "The server limit of 100 reactions has been reached");
            ReactionModel reaction = new(Context.Guild.Id, reactionId)
            {
                Anywhere = args.Anywhere,
                Response = args.Response,
                Trigger = args.Trigger,
                Weight = args.Weight
            };
            bool exists = _reactions.Exists(Context.Guild.Id, reactionId);
            _reactions.AddOrUpdate(reaction);
            string message = $"Reaction `{reactionId}` was {(exists ? "updated" : "added")}";
            await ReplyAsync(message);
            return CommandResult.FromSuccess();
        }

        [Command("Delete reaction")]
        [Alias("Remove reaction")]
        public async Task<RuntimeResult> DeleteReaction(string reactionId)
        {
            RuntimeResult notExists = CommandResult.FromError(CommandError.Unsuccessful, $"Reaction `{reactionId}` does not exist");
            if (reactionId.Length > 20)
                return notExists;
            bool exists = _reactions.Exists(Context.Guild.Id, reactionId);
            if (!exists)
                return notExists;
            _reactions.Delete(Context.Guild.Id, reactionId);
            await ReplyAsync($"Reaction `{reactionId}` was succesfully deleted");
            return CommandResult.FromSuccess();
        }

        [Command("List reactions")]
        public async Task<RuntimeResult> ListReactions()
        {
            IList<ReactionModel> reactions = _reactions.GetAllReactions(Context.Guild.Id);
            ReactionListForm form = new(Context, 90, reactions);
            await form.ShowDialogAsync();
            return CommandResult.FromSuccess();
        }
        [Command("View reaction")]
        [Alias("Get reaction")]
        public async Task<RuntimeResult> GetReaction(string id)
        {
            var reaction = _reactions.GetReaction(Context.Guild.Id, id);
            if (reaction is null)
                return CommandResult.FromError(CommandError.Unsuccessful, $"No reaction with id `{id}` could be found");
            EmbedBuilder embedBuilder = new()
            {
                Title = reaction.Id,
                Fields = GetFields(reaction)
            };
            await ReplyAsync(embed: embedBuilder.Build());
            return CommandResult.FromSuccess();
        }
        private List<EmbedFieldBuilder> GetFields(ReactionModel reaction)
        {
            List<EmbedFieldBuilder> fields = new()
            {
                GetField("Trigger", reaction.Trigger),
                GetField("Response", reaction.Response),
                GetField("Anywhere", reaction.Anywhere),
                GetField("Weight", reaction.Weight)
            };
            return fields;
        }
        private EmbedFieldBuilder GetField(string field, object valule)
        {
            string strValue = Newtonsoft.Json.JsonConvert.SerializeObject(valule);
            strValue += "\n\u200B";
            EmbedFieldBuilder fieldBuilder = new()
            {
                Name = field,
                Value = strValue
            };
            return fieldBuilder;
        }
        [ReactCommand]
        [Priority(-50)]
        public async Task React()
        {
            var info = pendingMatches[Context];
            pendingMatches.Remove(Context);
            IList<ReactionMatch> matches = info.Item1;
            bool isStrict = info.Item2;
            if (!isStrict)
                matches = SelectMatches(matches);
            string response = matches
                .RandomElementByWeight(element => element.Weight)
                .Response;
            await ReplyAsync(response);
        }

        private IList<ReactionMatch> SelectMatches(IList<ReactionMatch> matches)
        {
            List < ReactionMatch > result = new();
            long minIndex = long.MaxValue;
            int maxLength = -1;
            foreach (ReactionMatch match in matches)
            {
                long index = match.MatchIndex;
                if (match.MatchIndex == minIndex)
                {
                    int length = match.Trigger.Length;
                    if (length == maxLength)
                    {
                        result.Add(match);
                        continue;
                    }
                    if (length > maxLength)
                    {
                        result.Clear();
                        result.Add(match);
                        maxLength = length;
                    }
                    continue;
                }
                if (index < minIndex)
                {
                    result.Clear();
                    result.Add(match);
                    minIndex = index;
                    maxLength = match.Trigger.Length;
                }
            }
            return result;
        }

        [NamedArgumentType]
        public class Arguments
        {
            public string Trigger { get; set; }
            public string Response { get; set; }
            public bool? Anywhere { get; set; }
            public int? Weight { get; set; }
        }
    }
    class ReactCommandAttribute : NonStandardCommandAttribute
    {
        private static readonly CustomReactions _reactions = CustomReactions.Instance;
        public override bool Validate(ICommandContext context)
        {
            string message = context.Message.Content;
            IList<ReactionMatch> matches = _reactions.GetStrictMatchingReactions(context.Guild.Id, message);
            if (matches.Count > 0)
            {
                ReactionsModule.pendingMatches[context] = new(matches, true);
                return true;
            }
            matches = _reactions.GetMatchingReactions(context.Guild.Id, message);
            bool valid = matches.Count > 0;
            if (valid)
                ReactionsModule.pendingMatches[context] = new(matches, false);
            return valid;
        }
    }
}
