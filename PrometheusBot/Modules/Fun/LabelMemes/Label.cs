using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using Color = SixLabors.ImageSharp.Color;
using Image = SixLabors.ImageSharp.Image;
using Newtonsoft.Json;

namespace PrometheusBot.Modules.Fun.LabelMemes
{
    //[JsonConverter(typeof(LabelConverter))]
    class Label
    {
        [JsonConverter(typeof(RectangleConverter))]
        public Rectangle Rectangle { get; set; }
        public string FontName { get; set; }
        public float FontSize { get; set; }

        [JsonConverter(typeof(ColorConverter))]
        public Color MainColor { get; set; }

        [JsonConverter(typeof(ColorConverter))]
        public Color OutlineColor { get; set; }
        public string LabelText { get; set; }
        public bool ShowRectangle { get; set; }
        public VerticalAlignment VerticalAlignment { get; set; }

        public void Draw(Image image, FontCollection fonts, ICommandContext context, IMessage repliedMessage)
        {
            var font = fonts.CreateFont(FontName, FontSize);
            string labelText = GetLabelText(context, repliedMessage);
            image.Mutate(ctx => ctx.DrawBoxedText(font, labelText, MainColor, OutlineColor, Rectangle, VerticalAlignment, ShowRectangle));
        }

        private string GetLabelText(ICommandContext context, IMessage repliedMessage)
        {
            const StringComparison comp = StringComparison.InvariantCultureIgnoreCase;
            string result = LabelText
                 .Replace("$User", context.User.Username, comp)
                 .Replace("$Message", context.Message.Content, comp)
                 .Replace("$RepliedUser", repliedMessage.Author.Username, comp)
                 .Replace("$RepliedMessage", repliedMessage.Content, comp)
                 .Replace("$Guild", context.Guild?.Name, comp);
            return result;
        }
    }
}
