using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using PrometheusBot.Forms;
using PrometheusBot.Model.Reactions;

namespace PrometheusBot.Modules.Misc
{
    class ReactionListForm : Form
    {
        private readonly List<List<ReactionModel>> _pages;
        private readonly Button _previousButton;
        private readonly Button _nextButton;
        private readonly EmbedBuilder _embedBuilder;
        private int _currentPage;
        public ReactionListForm(SocketCommandContext context, int timeoutSeconds, IList<ReactionModel> reactions) : base(context, timeoutSeconds)
        {
            _pages = new();
            List<ReactionModel> currentPage = null;
            for (int i = 0; i < reactions.Count; i++)
            {
                if (i % 10 == 0)
                {
                    currentPage = new();
                    _pages.Add(currentPage);
                }
                currentPage.Add(reactions[i]);
            }

            _embedBuilder = GetEmbedBuilder();

            if (_pages.Count > 1)
            {
                _previousButton = new(new Emoji("\u2B05"));
                _nextButton = new(new Emoji("\u27A1"));

                _previousButton.Clicked += PreviousButton_Clicked;
                _nextButton.Clicked += NextButton_Clicked;

                AddButtonAsync(_previousButton).Wait();
                AddButtonAsync(_nextButton).Wait();
            }

            if (_pages.Count > 0)
            {
                Embed = GetPageEmbed();
                return;
            }

            EmbedFieldBuilder emptyField = new()
            {
                Name = "No reactions found",
                Value = "\u200B"
            };

            _embedBuilder.AddField(emptyField);

            var embed = _embedBuilder.Build();
            Embed = embed;
        }
        private EmbedBuilder GetEmbedBuilder()
        {
            EmbedFooterBuilder footerBuilder = new();
            EmbedBuilder builder = new()
            {
                Title = "Custom reactions",
                Description =
                "Results can be filtered with commands such as\n`list reactions trigger:UwU`\n`list reactions response:\"What's this?\"`\n" +
                "You can get full information on a reaction with the command\n" +
                "`view reaction <id>`",
                Footer = footerBuilder
            };
            return builder;
        }

        private void PreviousButton_Clicked(object sender, ButtonEventArgs e)
        {
            if (e.User.Id != Context.User.Id) return;
            if (_currentPage == 0) return;
            _currentPage--;
            Embed = GetPageEmbed();
        }

        private void NextButton_Clicked(object sender, ButtonEventArgs e)
        {
            if (e.User.Id != Context.User.Id) return;
            if (_currentPage == _pages.Count - 1) return;
            _currentPage++;
            Embed = GetPageEmbed();
        }

        private Embed GetPageEmbed()
        {
            var page = _pages[_currentPage];
            var fields = page
                .Select(reaction => GetField(reaction))
                .ToList();

            lock (_embedBuilder)
            {
                _embedBuilder.Footer.Text = $"Page {_currentPage + 1} of {_pages.Count}";
                _embedBuilder.Fields = fields;
                return _embedBuilder.Build();
            }
        }
        private EmbedFieldBuilder GetField(ReactionModel reaction)
        {
            string trigger = Format(reaction.Trigger);
            string response = Format(reaction.Response);

            string value =
                $"**Trigger**:{trigger} **Response**:{response} **Anywhere**:{reaction.Anywhere} **Weight**:{reaction.Weight}\n\u200B";

            EmbedFieldBuilder builder = new()
            {
                IsInline = false,
                Name = $"**{reaction.Id}**",
                Value = value
            };

            return builder;
        }
        private string Format(string value)
        {
            value = Newtonsoft.Json.JsonConvert.SerializeObject(value);
            const int max = 20;
            if (value.Length > max)
                value = value[..max] + "...\"";
            return value;
        }
    }
}
