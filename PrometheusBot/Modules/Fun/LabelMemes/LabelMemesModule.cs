using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using PrometheusBot.Extensions;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.Fonts;
using SixLabors.ImageSharp.Formats;
using System.IO;
using Image = SixLabors.ImageSharp.Image;
using SixLabors.ImageSharp.Drawing;
using Color = SixLabors.ImageSharp.Color;
using Path = System.IO.Path;
using Newtonsoft.Json;

namespace PrometheusBot.Modules.Fun.LabelMemes
{
    public class LabelMemesModule : ModuleBase<SocketCommandContext>
    {
        internal static readonly Dictionary<string, LabelMeme> memes = new();
        private static readonly string _assetsFolder = Path.Combine(Program.Directory, "assets");
        public static readonly string memesPath = Path.Combine(Program.Directory, "assets", "memes.json");
        public static void LoadMemes()
        {
            string json = File.ReadAllText(memesPath);
            var memeList = JsonConvert.DeserializeObject<List<LabelMeme>>(json);
            memes.Clear();
            memeList?.ForEach(
                meme => meme?.Triggers?.ForEach
                (
                    trigger => { if (trigger is not null) memes[trigger.ToLower()] = meme; }
                )
            );
        }
        [LabelMemeCommand]
        public async Task LabelMeme()
        {
            string messageContent = Context.Message.Content.ToLower();
            var meme = memes[messageContent];

            using MemoryStream image = new();
            await meme.GetImageAsync(Context, image);
            image.Position = 0;

            await Context.Channel.SendFileAsync(image, meme.ImageFile);
        }

        //public static Stream GetImage(string astronaut1, string astronaut2, string messageContent, bool debugRectangle = false)
        //{
        //    MemoryStream stream = new();
        //    FontCollection fonts = new();
        //    fonts.Install(Path.Combine(_assetsFolder, "fonts", "arial.ttf"));
        //    Font captionFont = fonts.CreateFont("arial", 8);
        //    Rectangle messageContentRectangle = new(150, 50, 450, 165);
        //    Font astronaut1Font = fonts.CreateFont("arial", 7);
        //    Rectangle astronaut1Rectangle = new(350, 225, 150, 75);
        //    Font astronaut2Font = fonts.CreateFont("arial", 8);
        //    Rectangle astronaut2Rectangle = new(625, 50, 250, 125);
        //    Font alwaysHasBeenFont = fonts.CreateFont("arial", 9);
        //    Rectangle alwaysHasBeenRectangle = new(400, 0, 475, 50);
        //    using (Image img = Image.Load(Path.Combine(_assetsFolder, "images", "wait-its-all-ohio.png")))
        //    {
        //        img.Mutate(ctx => ctx.DrawBoxedText(astronaut1Font, astronaut1, Color.White, Color.Black, astronaut1Rectangle, VerticalAlignment.Center, debugRectangle));
        //        img.Mutate(ctx => ctx.DrawBoxedText(astronaut2Font, astronaut2, Color.White, Color.Black, astronaut2Rectangle, VerticalAlignment.Center, debugRectangle));
        //        img.Mutate(ctx => ctx.DrawBoxedText(captionFont, messageContent, Color.White, Color.Black, messageContentRectangle, VerticalAlignment.Bottom, debugRectangle));
        //        img.Mutate(ctx => ctx.DrawBoxedText(alwaysHasBeenFont, "Always has been", Color.White, Color.Black, alwaysHasBeenRectangle, VerticalAlignment.Center, debugRectangle));
        //        img.SaveAsPng(stream);
        //    }
        //    stream.Position = 0;
        //    return stream;
        //}
        
    }
    class LabelMemeCommandAttribute : NonStandardCommandAttribute
    {
        private static DateTime lastRead;
        public override bool Validate(ICommandContext context, IServiceProvider services)
        {
            try
            {
                var lastModified = File.GetLastAccessTimeUtc(LabelMemesModule.memesPath);
                if (lastRead.Subtract(lastModified).Seconds < 0)
                {
                    lastRead = DateTime.UtcNow;
                    LabelMemesModule.LoadMemes();
                }
            }
            catch { }
            return LabelMemesModule.memes.ContainsKey(context.Message.Content.ToLower());
        }
    }
}
