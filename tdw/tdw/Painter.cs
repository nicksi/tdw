/*
 * Based on Painter class by Rob Chartier (http://weblogs.asp.net/rchartier)
 */

using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Presentation.Media;

namespace tdw
{
    public class Painter
    {
        private Bitmap bitmap;

        public Painter(Bitmap Canvas)
        {
            bitmap = Canvas;
        }

        public int MeasureString(string text, Font font)
        {
            if (text == null || text.Trim() == "") return 0;
            int size = 0;
            for (int i = 0; i < text.Length; i++)
            {
                size += font.CharWidth(text[i]);
            }
            return size;
        }

        public void PaintBottomCenter(string text, Font font, bool inverse = false)
        {
            int x, y = 0;
            Color color = inverse ? Color.Black : Color.White;
            FindCenter(text, font, out x, out y);
            bitmap.DrawText(text, font, color, x, bitmap.Height - font.Height);
        }

        public void PaintCentered(string text, Font Font, bool inverse, int y)
        {
            int x, y1 = 0;
            Color color = inverse ? Color.Black : Color.White;
            FindCenter(text, Font, out x, out y1);

            bitmap.DrawText(text, Font, color, x, y);
        }

        public void PaintCentered(string text, Font Font, bool inverse = false)
        {

            int x, y = 0;
            Color color = inverse ? Color.Black : Color.White;
            FindCenter(text, Font, out x, out y);

            bitmap.DrawText(text, Font, color, x, y);
        }

        public void FindCenter(string text, Font Font, out int x, out int y)
        {
            int size = MeasureString(text, Font);
            int center = bitmap.Height / 2;
            int centerText = size / 2 - 2;
            x = center - centerText;

            y = center - (Font.Height / 2);
        }

        public void PaintImage(byte[] imageData, Bitmap.BitmapImageType imageType, Point point)
        {
            var img = new Bitmap(imageData, imageType);
            bitmap.DrawImage(point.X, point.Y, img, 0, 0, img.Height, img.Width);

        }

        public void PaintParagraph(byte[] imageData, Bitmap.BitmapImageType imageType, Point point)
        {
            var img = new Bitmap(imageData, imageType);
            bitmap.DrawImage(point.X, point.Y, img, 0, 0, img.Height, img.Width);

        }


        public void PaintCentered(byte[] ImageData, Bitmap.BitmapImageType ImageType)
        {
            var img = new Bitmap(ImageData, ImageType);
            int x = (bitmap.Width / 2) - (img.Width / 2);
            int y = (bitmap.Height / 2) - (img.Height / 2);
            bitmap.DrawImage(x, y, img, 0, 0, img.Width, img.Height);
        }

    }
}
