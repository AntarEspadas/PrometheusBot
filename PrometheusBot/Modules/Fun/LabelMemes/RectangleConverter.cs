using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using ISRectangle = SixLabors.ImageSharp.Rectangle;
using Newtonsoft.Json;

namespace PrometheusBot.Modules.Fun.LabelMemes
{
    class RectangleConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(ISRectangle);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var rectangle = serializer.Deserialize<Rectangle>(reader);
            return new ISRectangle(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var sourceRectangle = (ISRectangle)value;
            var rectangle = new Rectangle(sourceRectangle.X, sourceRectangle.Y, sourceRectangle.Width, sourceRectangle.Height);
            string json = JsonConvert.SerializeObject(rectangle);
            writer.WriteRawValue(json);
        }
    }
}
