using System;
using System.Collections.Generic;
using System.IO;
using Discord.Commands;
using SixLabors.Fonts;
using PrometheusBot.Extensions;
using SixLabors.ImageSharp;

namespace PrometheusBot.Modules.Fun.LabelMemes
{
    class LabelMeme
    {
        private static readonly string _fontsPath = Path.Combine(Program.Directory, "assets", "fonts");
        private static readonly string _imagesPath = Path.Combine(Program.Directory, "assets", "images");
        public string ImageFile { get; set; }
        public List<string> FontFiles { get; set; }
        public List<string> Triggers { get; set; }
        public List<Label> Labels { get; set; }

        public void GetImage(ICommandContext context, Stream outputStream)
        {
            using var image = Image.Load(Path.Combine(_imagesPath, ImageFile));
            var fonts = GetFonts();
            var repliedMessage = context.Message.GetReplyingMessageAsync().Result;

            foreach (var label in Labels)
                label.Draw(image, fonts, context, repliedMessage);

            image.SaveAsPng(outputStream);
        }
        private FontCollection GetFonts()
        {
            FontCollection result = new();
            foreach (string fontFile in FontFiles)
            {
                string path = Path.Combine(_fontsPath, fontFile);
                result.Install(path);
            }
            return result;
        }
    }
}
