using System;
using System.Collections.Generic;
using System.IO;
using Discord.Commands;
using SixLabors.Fonts;
using PrometheusBot.Extensions;
using SixLabors.ImageSharp;
using System.Threading.Tasks;
using System.Linq;

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
        public List<ImageLabel> ImageLabels { get; set; }

        public async Task GetImageAsync(ICommandContext context, Stream outputStream)
        {
            using var image = Image.Load(Path.Combine(_imagesPath, ImageFile));
            var fonts = GetFonts();
            var repliedMessage = await context.Message.GetReplyingMessageAsync();

            var imgTasks = ImageLabels?
                .Select(img => img?.DrawAsync(image, context, repliedMessage))
                .Where(task => task is not null);

            if (imgTasks is not null)
                await Task.WhenAll(imgTasks);

            Labels?.ForEach(label =>
               label?.Draw(image, fonts, context, repliedMessage));

            await image.SaveAsPngAsync(outputStream);
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
