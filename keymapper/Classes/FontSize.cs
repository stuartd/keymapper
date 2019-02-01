using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Globalization;
using System.Linq;
using System.Text;

namespace KeyMapper.Classes
{
    public class FontSize
    {
        public static float BaseFontSize { get; private set; }

        public static void SetFontSizes(float scale)
        {
            // See what font size fits the scaled-down button 
            float baseFontSize = 36F;

            // Not using ButtonImages.GetButtonImage as that is where we were called from..
            using (var font = AppController.GetButtonFont(baseFontSize, false))
            using (var bmp = ButtonImages.ResizeBitmap(ButtonImages.GetBitmap(BlankButton.Blank), scale, false))
            using (var g = Graphics.FromImage(bmp))
            {
                // Helps MeasureString. Can also pass StringFormat.GenericTypographic apparently ??

                g.TextRenderingHint = TextRenderingHint.AntiAlias;
                var characterWidth = (int)g.MeasureString(((char)77).ToString(CultureInfo.InvariantCulture), font).Width;

                // Only use 90% of the bitmap's size to allow for the edges (especially at small sizes)
                float ratio = (((0.9F * bmp.Height) / 2)) / characterWidth;
                baseFontSize = (baseFontSize * ratio);
            }

            BaseFontSize = baseFontSize;
        }
    }
}
