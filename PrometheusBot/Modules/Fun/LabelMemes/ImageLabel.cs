using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Newtonsoft.Json;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using Image = SixLabors.ImageSharp.Image;

namespace PrometheusBot.Modules.Fun.LabelMemes
{
    class ImageLabel
    {
        private static readonly HttpClient _httpClient = new();
        public string ImageSource { get; set; }

        [JsonConverter(typeof(RectangleConverter))]
        public Rectangle Rectangle { get; set; }
        public float Opacity { get; set; }
        public async Task DrawAsync(Image image, ICommandContext context, IMessage repliedMessage)
        {
            string imageUrl = GetImageUrl(context, repliedMessage);
            Image imageToDraw = await GetImageAsync(imageUrl);
            if (imageToDraw is null) return;
            imageToDraw.Mutate(ctx => ctx.Resize(Rectangle.Width, Rectangle.Height));

            Point point = new(Rectangle.X, Rectangle.Y);
            image.Mutate(ctx => ctx.DrawImage(imageToDraw, point, Opacity));
        }
        private string GetImageUrl(ICommandContext context, IMessage repliedMessage)
        {
            ushort targetSize = (ushort)Math.Max(Rectangle.Width, Rectangle.Height);
            targetSize = (ushort)NextPow2(targetSize);

            string url = ImageSource switch
            {
                "$User" => GetAvatarUrlOrDefault(context.User, targetSize),
                "$RepliedUser" => GetAvatarUrlOrDefault(repliedMessage.Author, targetSize),
                _ => ImageSource
            };

            return url;
        }
        private string GetAvatarUrlOrDefault(IUser user, ushort size)
        {
            string url = user.GetAvatarUrl(size: size) ?? user.GetDefaultAvatarUrl();
            return url;
        }
        private async Task<Image> GetImageAsync(string url)
        {
            Image result = null;
            try
            {
                var data = await _httpClient.GetByteArrayAsync(url);
                result = Image.Load(data);
            }
            catch { }
            return result;
        }
        private uint NextPow2(uint value)
        {
            value--;
            value |= value >> 1;
            value |= value >> 2;
            value |= value >> 4;
            value |= value >> 8;
            value |= value >> 16;
            value++;
            return value;
        }
    }
}
