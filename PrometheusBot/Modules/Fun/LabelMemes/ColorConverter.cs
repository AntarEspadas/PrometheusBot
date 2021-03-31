using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ISColor = SixLabors.ImageSharp.Color;

namespace PrometheusBot.Modules.Fun.LabelMemes
{
    class ColorConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(ISColor);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var color = serializer.Deserialize<Color>(reader);
            return ISColor.FromRgba(color.R, color.G, color.B, color.A);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var sourceColor = (ISColor)value;
            var pixelColor = sourceColor.ToPixel<SixLabors.ImageSharp.PixelFormats.Argb32>();
            var destinationColor = Color.FromArgb(pixelColor.A, pixelColor.R, pixelColor.G, pixelColor.B);
            string json = JsonConvert.SerializeObject(destinationColor);
            writer.WriteRawValue(json);
        }
    }
}
