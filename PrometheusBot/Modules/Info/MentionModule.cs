using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace PrometheusBot.Modules.Info
{
    public class MentionModule : ModuleBase<SocketCommandContext>
    {
        [MentionCommand]
        public async Task OnMention()
        {
            string naturalPrefix = "navi";
            string syntheticPrefix = "n.";
            List<EmbedFieldBuilder> fields = new()
            {
                new EmbedFieldBuilder(){Value = $"{syntheticPrefix}help", Name = "Synthetic prefix:"},
                new EmbedFieldBuilder(){Value = $"{naturalPrefix} help | {naturalPrefix}, help", Name = "Natural prefix:"},
                new EmbedFieldBuilder(){Value = $"{Context.Client.CurrentUser.Mention} help", Name = "Mention:"}
            };
            var eb = new EmbedBuilder()
            {
                Title = "Use the help command to know what I can do",
                Fields = fields

            };
            await ReplyAsync(embed: eb.Build());
        }
        private class MentionCommandAttribute : NonStandardCommandAttribute
        {
            public override bool Validate(ICommandContext context)
            {
                string content = context.Message.Content;
                if (string.IsNullOrEmpty(content) || content.Length < 4 || content[0] != '<' || content[1] != '@' || content[^1] != '>')
                    return false;
                if (!MentionUtils.TryParseUser(content, out ulong userId))
                    return false;
                if (userId == context.Client.CurrentUser.Id)
                    return true;
                return false;
            }
        }
    }
}
