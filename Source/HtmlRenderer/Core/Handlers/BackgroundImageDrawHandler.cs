// "Therefore those skilled at the unorthodox
// are infinite as heaven and earth,
// inexhaustible as the great rivers.
// When they come to an end,
// they begin again,
// like the days and months;
// they die and are reborn,
// like the four seasons."
// 
// - Sun Tsu,
// "The Art of War"

using System;
using TheArtOfDev.HtmlRenderer.Adapters;
using TheArtOfDev.HtmlRenderer.Adapters.Entities;
using TheArtOfDev.HtmlRenderer.Core.Dom;

namespace TheArtOfDev.HtmlRenderer.Core.Handlers
{
    /// <summary>
    /// Contains all the paint code to paint different background images.
    /// </summary>
    internal static class BackgroundImageDrawHandler
    {
        /// <summary>
        /// Draw the background image of the given box in the given rectangle.<br/>
        /// Handle background-repeat and background-position values.
        /// </summary>
        /// <param name="g">the device to draw into</param>
        /// <param name="box">the box to draw its background image</param>
        /// <param name="imageLoadHandler">the handler that loads image to draw</param>
        /// <param name="rectangle">the rectangle to draw image in</param>
        public static void DrawBackgroundImage(RGraphics g, CssBox box, ImageLoadHandler imageLoadHandler, RRect rectangle, RGraphicsPath roundrect = null)
        {
            // image size depends if specific rectangle given in image loader
            var imgSize = new RSize(imageLoadHandler.Rectangle == RRect.Empty ? imageLoadHandler.Image.Width : imageLoadHandler.Rectangle.Width,
                imageLoadHandler.Rectangle == RRect.Empty ? imageLoadHandler.Image.Height : imageLoadHandler.Rectangle.Height);

            // get the location by BackgroundPosition value
            var location = GetLocation(box.BackgroundPosition, rectangle, imgSize);

            var srcRect = imageLoadHandler.Rectangle == RRect.Empty
                ? new RRect(0, 0, imgSize.Width, imgSize.Height)
                : new RRect(imageLoadHandler.Rectangle.Left, imageLoadHandler.Rectangle.Top, imgSize.Width, imgSize.Height);

            // initial image destination rectangle
            var destRect = new RRect(location, imgSize);

            // need to clip so repeated image will be cut on rectangle
            var lRectangle = rectangle;
            lRectangle.Intersect(g.GetClip());
            g.PushClip(lRectangle);



            //rectangle - box to draw in
            //imgSize - image size
            //localion - inital location of image wo repeat
            //destRect - RRect(location, imgSize)
            //srcRect (0,0,imgSize)

            //brushRect - This is rectangle which needs to be filled with Image from brushRect.Location with brushRect.Size multiple to Image size.  
            RRect brushRect = new RRect(location, imgSize);

            switch (box.BackgroundRepeat)
            {
                case "no-repeat":
                    //brushRect = destRect;
                    break;
                case "repeat-x":
                    if (brushRect.X > rectangle.X) brushRect.X -= imgSize.Width * ((int)((brushRect.X - rectangle.X) / imgSize.Width) + 1);
                    if (brushRect.X + brushRect.Width < rectangle.X + rectangle.Width) brushRect.Width = imgSize.Width * ((int)((rectangle.X + rectangle.Width - brushRect.X) / imgSize.Width) + 1);
                    break;
                case "repeat-y":
                    if (brushRect.Y > rectangle.Y) brushRect.Y -= imgSize.Height * ((int)((brushRect.Y - rectangle.Y) / imgSize.Height) + 1);
                    if (brushRect.Y + brushRect.Height < rectangle.Y + rectangle.Height) brushRect.Height = imgSize.Height * ((int)((rectangle.Y + rectangle.Height - brushRect.Y) / imgSize.Height) + 1);
                    break;
                default:
                    if (brushRect.X > rectangle.X) brushRect.X -= imgSize.Width * ((int)((brushRect.X - rectangle.X) / imgSize.Width) + 1);
                    if (brushRect.X + brushRect.Width < rectangle.X + rectangle.Width) brushRect.Width = imgSize.Width * ((int)((rectangle.X + rectangle.Width - brushRect.X) / imgSize.Width) + 1);
                    if (brushRect.Y > rectangle.Y) brushRect.Y -= imgSize.Height * ((int)((brushRect.Y - rectangle.Y) / imgSize.Height) + 1);
                    if (brushRect.Y + brushRect.Height < rectangle.Y + rectangle.Height) brushRect.Height = imgSize.Height * ((int)((rectangle.Y + rectangle.Height - brushRect.Y) / imgSize.Height) + 1);
                    break;
            }
            using (var brush = g.GetTextureBrush(imageLoadHandler.Image, brushRect, brushRect.Location))
            {
                if (roundrect == null)
                    g.DrawRectangle(brush, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
                else
                    g.DrawPath(brush, roundrect);
            }

            g.PopClip();
        }


        #region Private methods

        /// <summary>
        /// Get top-left location to start drawing the image at depending on background-position value.
        /// </summary>
        /// <param name="backgroundPosition">the background-position value</param>
        /// <param name="rectangle">the rectangle to position image in</param>
        /// <param name="imgSize">the size of the image</param>
        /// <returns>the top-left location</returns>
        private static RPoint GetLocation(string backgroundPosition, RRect rectangle, RSize imgSize)
        {
            double left = rectangle.Left;
            if (backgroundPosition.IndexOf("left", StringComparison.OrdinalIgnoreCase) > -1)
            {
                left = (rectangle.Left);
            }
            else if (backgroundPosition.IndexOf("right", StringComparison.OrdinalIgnoreCase) > -1)
            {
                left = rectangle.Right - imgSize.Width;
            }
            else if (backgroundPosition.IndexOf("0", StringComparison.OrdinalIgnoreCase) < 0)
            {
                left = (rectangle.Left + (rectangle.Width - imgSize.Width) / 2);
            }

            double top = rectangle.Top;
            if (backgroundPosition.IndexOf("top", StringComparison.OrdinalIgnoreCase) > -1)
            {
                top = rectangle.Top;
            }
            else if (backgroundPosition.IndexOf("bottom", StringComparison.OrdinalIgnoreCase) > -1)
            {
                top = rectangle.Bottom - imgSize.Height;
            }
            else if (backgroundPosition.IndexOf("0", StringComparison.OrdinalIgnoreCase) < 0)
            {
                top = (rectangle.Top + (rectangle.Height - imgSize.Height) / 2);
            }

            return new RPoint(left, top);
        }

        #endregion
    }
}
