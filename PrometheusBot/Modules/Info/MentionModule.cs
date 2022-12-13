using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using PrometheusBot.Model;
using PrometheusBot.Services.Settings;

namespace PrometheusBot.Modules.Info
{
    class MentionModule : ModuleBase<SocketCommandContext>
    {
        private readonly SettingsService _settings;

        public MentionModule(SettingsService settings)
        {
            _settings = settings;
        }

        [MentionCommand]
        public async Task OnMention()
        {
            string[] prefixes = _settings.GetPrefixes(Context.User.Id, Context.Guild.Id, Context.Channel.Id);
            string naturalPrefix = prefixes[0];
            string syntheticPrefix = prefixes[1];
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
            public override bool Validate(ICommandContext context, IServiceProvider services)
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
