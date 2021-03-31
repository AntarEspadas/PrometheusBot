using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Processing;

namespace PrometheusBot.Modules.Fun.LabelMemes
{
    static class ImageProcessing
    {
        public static IImageProcessingContext DrawBoxedText(this IImageProcessingContext processingContext,
               Font font,
               string text,
               Color mainColor,
               Color outlineColor,
               Rectangle rectangle,
               VerticalAlignment verticalAlignment,
               bool debugRectangle = false)
        {
            Size imgSize = processingContext.GetCurrentSize();
            float targetWidth = rectangle.Width;
            float targetHeight = rectangle.Height;

            float targetMinHeight = (int)(targetHeight * 0.95);

            var scaledFont = font;
            FontRectangle s = new(0, 0, float.MaxValue, float.MaxValue);

            float scaleFactor = (scaledFont.Size / 2);
            int trapCount = (int)scaledFont.Size * 2;
            if (trapCount < 10)
            {
                trapCount = 10;
            }

            bool isTooSmall = false;

            while ((s.Height > targetHeight || s.Height < targetMinHeight) && trapCount > 0)
            {
                if (s.Height > targetHeight)
                {
                    if (isTooSmall)
                    {
                        scaleFactor /= 2;
                    }

                    scaledFont = new Font(scaledFont, scaledFont.Size - scaleFactor);
                    isTooSmall = false;
                }

                if (s.Height < targetMinHeight)
                {
                    if (!isTooSmall)
                    {
                        scaleFactor /= 2;
                    }
                    scaledFont = new Font(scaledFont, scaledFont.Size + scaleFactor);
                    isTooSmall = true;
                }
                trapCount--;

                s = TextMeasurer.Measure(text, new RendererOptions(scaledFont)
                {
                    WrappingWidth = targetWidth
                });
            }

            var textGraphicOptions = new TextGraphicsOptions()
            {
                TextOptions = {
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = verticalAlignment,
                    WrapTextWidth = targetWidth
                }
            };
            var position = new PointF(rectangle.X, 0);
            switch (verticalAlignment)
            {
                case VerticalAlignment.Top:
                    break;
                case VerticalAlignment.Center:
                    position.Y = rectangle.Y + rectangle.Height / 2;
                    break;
                case VerticalAlignment.Bottom:
                    position.Y = rectangle.Y + rectangle.Height;
                    break;
                default:
                    break;
            }
            if (debugRectangle)
            {
                PointF[] points = { new PointF(rectangle.X, rectangle.Y), new PointF(rectangle.X, rectangle.Y + rectangle.Height), new PointF(rectangle.X + rectangle.Width, rectangle.Y + rectangle.Height), new PointF(rectangle.X + rectangle.Width, rectangle.Y) };
                processingContext = processingContext.DrawPolygon(new Pen(Color.Red, 5), points);
            }
            return processingContext.DrawText(textGraphicOptions, text, scaledFont, Brushes.Solid(mainColor), Pens.Solid(outlineColor, font.Size / 10), position);
        }
    }
}
