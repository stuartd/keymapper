using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Reflection;

namespace KeyMapper.Classes
{
    public static class ButtonImages
    {
        private const string Path = "KeyMapper.Images.";
        private static float lastScale;

        // Basic blank button, unscaled, no effects
        public static Bitmap GetButtonImage(int scanCode, int extended)
        {
            return GetButtonImage(scanCode, extended, BlankButton.Blank, 0, 0, 1F, ButtonEffect.None, string.Empty);
        }

        // Specific scaled button with custom caption
        public static Bitmap GetButtonImage(BlankButton button, float scale, string caption, ButtonEffect effect)
        {
            return GetButtonImage(-1, -1, button, 0, 0, scale, effect, caption);
        }

        // Custom ColorMatrix and font colour
        public static Bitmap GetButtonImage(BlankButton button, float scale, string caption, ColorMatrix cm, Color fontColour)
        {
            var bmp = GetBitmap(button, 0, 0, scale, ButtonEffect.None, false);
            bmp = Transform(bmp, cm);
            return WriteCaption(bmp, caption, false, false, fontColour);
        }


        // Offers full control over the button scale and stretch.
        public static Bitmap GetButtonImage
            (int scanCode, int extended, BlankButton button, int horizontalStretch, int verticalStretch, float scale, ButtonEffect effect)
        {
            return GetButtonImage
                (scanCode, extended, button, horizontalStretch, verticalStretch, scale, effect, string.Empty);
        }

        // This (private) method does the work. 
        private static Bitmap GetButtonImage(int scanCode, int extended, BlankButton button, int horizontalStretch,
                                             int verticalStretch, float scale, ButtonEffect effect, string caption)
        {
            var bmp = GetBitmap(button, horizontalStretch, verticalStretch, scale, effect, true);

            var fontColour = GetFontColour(effect);

            Bitmap bmpWithCaption;
            if (string.IsNullOrEmpty(caption))
            {
                bmpWithCaption = WriteCaption(bmp, scanCode, extended, fontColour);
            }
            else
            {
                bmpWithCaption = WriteCaption(bmp, caption, false, false, fontColour);
            }

            return bmpWithCaption;
        }

        private static Bitmap GetImage(string buttonFileName, string extension)
        {
            string filepath = Path + buttonFileName.ToLowerInvariant() + "." + extension;
            return ExtractImage(filepath);
        }

        public static Bitmap ResizeBitmap(Bitmap bmp, float scale, bool forceEven)
        {
            if (bmp == null)
            {
                return null;
            }

            // Even numbers only in case we want to stretch.
            int newWidth = (int)Math.Round((bmp.Width * scale), 0);

            if (forceEven && newWidth % 2 == 1)
            {
                newWidth += 1;
            }

            int newHeight = (int)Math.Round((bmp.Height * scale), 0);

            if (forceEven && newHeight % 2 == 1)
            {
                newHeight += 1;
            }

            return ScaleBitmap(bmp, newWidth, newHeight);
        }

        public static Color GetFontColour(ButtonEffect effect)
        {
            return GetFontColour(effect, false);
        }

        public static Color GetFontColour(ButtonEffect effect, bool ignoreUserSettings)
        {
            if (ignoreUserSettings == false)
            {
                var setting = UserColourSettingManager.GetColourSettings(effect);
                if (setting != null)
                {
                    return setting.FontColour;
                }
            }

            return Color.Black;
        }

        private static Bitmap StretchBitmap(Bitmap bmp, int horizontalStretch, int verticalStretch)
        {

            Bitmap newBitMap = null;

            if ((horizontalStretch != 0) & (verticalStretch != 0))
            {
                var tempBitMap = StretchButtonHorizontal(bmp, horizontalStretch);
                newBitMap = StretchButtonVertical(tempBitMap, verticalStretch);
                tempBitMap.Dispose();
            }
            else
            {
                if (horizontalStretch != 0)
                {
                    newBitMap = StretchButtonHorizontal(bmp, horizontalStretch);
                }

                else if (verticalStretch != 0)
                {
                    newBitMap = StretchButtonVertical(bmp, verticalStretch);
                }
            }

            bmp.Dispose();

            return newBitMap;
        }

        private static Bitmap ExtractImage(string name)
        {
            Bitmap bmp = null;

            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(name))
            {
                if (stream != null)
                {
                    bmp = new Bitmap(stream);
                    stream.Close();
                }
            }
            return bmp;
        }

        public static Bitmap GetBitmap(BlankButton button)
        {
            // Have we already extracted this bmp?
            // Buttons are stored as lower case.
            string buttonName = button.ToString().ToLowerInvariant();

            return GetImage(buttonName, "png");
        }

        private static Bitmap GetBitmap(BlankButton button, int horizontalStretch, int verticalStretch, float scale, ButtonEffect effect, bool transform)
        {
            if (Math.Abs(scale - lastScale) > float.Epsilon)
            {
                FontSize.SetFontSizes(scale);
            }

            Bitmap bmp;

            // Cache bitmap until scale or button changes (as long as no stretch is applied)

            if (horizontalStretch != 0 || verticalStretch != 0)
            {
                // IF this image needs to be stretched, force the height and width to even numbers
                // (This only seems to affect height?)
                bool forceEvenNumber = (verticalStretch != 0);
                bmp = ResizeBitmap(GetBitmap(button), scale, forceEvenNumber);
                bmp = StretchBitmap(bmp, horizontalStretch, verticalStretch);
                // _lastButton = BlankButton.None;

            }
            else
            {
                bmp = ResizeBitmap(GetBitmap(button), scale, false);
            }

            // Now all decisions have been made..
            lastScale = scale;

            if (transform)
            {
                bmp = ApplyEffect(bmp, effect);
            }

            return bmp;

        }

        private static Bitmap StretchButtonHorizontal(Bitmap bmp, int horizontalStretch)
        {
            // snap it in half, pull halves apart, slice off the inner edges and
            // clone them along the gap

            // Get as big a slice in the middle as we can (the highest common factor of the 
            // stretch and half the width) 

            int halfWidth = bmp.Width / 2;
            int sliceWidth = AppController.GetHighestCommonDenominator(horizontalStretch, halfWidth);

            var newBitMap = new Bitmap(bmp.Width + horizontalStretch, bmp.Height);
            using (var g = Graphics.FromImage(newBitMap))
            {
                g.DrawImage(bmp, 0, 0, new Rectangle(0, 0, halfWidth, bmp.Height), GraphicsUnit.Pixel);
                for (int i = 0; i < horizontalStretch; i += sliceWidth)
                {
                    g.DrawImage(bmp, halfWidth + i, 0, new Rectangle(halfWidth, 0, sliceWidth, bmp.Height), GraphicsUnit.Pixel);
                }
                g.DrawImage(bmp, halfWidth + horizontalStretch - 1, 0,
                    new Rectangle(halfWidth, 0, halfWidth, bmp.Height), GraphicsUnit.Pixel);
            }

            bmp.Dispose();
            return newBitMap;

        }

        private static Bitmap StretchButtonVertical(Bitmap bmp, int verticalStretch)
        {

            // Same as horizontal more-or-less

            int halfHeight = bmp.Height / 2;
            int sliceHeight = AppController.GetHighestCommonDenominator(verticalStretch, halfHeight);

            var newBitMap = new Bitmap(bmp.Width, bmp.Height + verticalStretch);

            using (var g = Graphics.FromImage(newBitMap))
            {
                g.DrawImage(bmp, 0, 0, new Rectangle(0, 0, bmp.Width, halfHeight), GraphicsUnit.Pixel);
                for (int i = 0; i < verticalStretch; i += sliceHeight)
                {
                    g.DrawImage(bmp, 0, halfHeight + i, new Rectangle(0, halfHeight, bmp.Width, sliceHeight), GraphicsUnit.Pixel);
                }
                g.DrawImage(bmp, 0, halfHeight + verticalStretch - 1,
                    new Rectangle(0, halfHeight, bmp.Width, halfHeight), GraphicsUnit.Pixel);
            }

            bmp.Dispose();
            return newBitMap;

        }

        private static Bitmap ScaleBitmap(Bitmap bmp, int newWidth, int newHeight)
        {

            if (newWidth == 0 || newHeight == 0)
            {
                Console.WriteLine("Attempting to resize a bitmap to invalid dimensions");
                return bmp;
            }

            var newBitMap = new Bitmap(newWidth, newHeight);
            using (var g = Graphics.FromImage(newBitMap))
            {
                g.DrawImage(bmp, 0, 0, newWidth, newHeight);
            }
            bmp.Dispose();
            return newBitMap;

        }

        private static Bitmap Transform(Bitmap bmp, ColorMatrix cm)
        {
            if (bmp == null)
            {
                return null;
            }

            var copy = new Bitmap(bmp.Width, bmp.Height);
            using (var ia = new ImageAttributes())
            using (var g = Graphics.FromImage(copy))
            {
                ia.SetColorMatrix(cm);
                g.DrawImage(bmp, new Rectangle(0, 0, bmp.Width, bmp.Height), 0, 0, bmp.Width, bmp.Height, GraphicsUnit.Pixel, ia);
                bmp.Dispose();
            }
            return copy;

        }

        private static float GetMultiLineFontSize(Bitmap bmp, string caption, bool localizable, float fontSize)
        {
            var words = caption.Split();

            // Ignore words in brackets as they won't be displayed.
            string longestWord = words[0];

            for (int i = 1; i < words.GetLength(0); i++)
            {
                string word = words[i];
                if (word.StartsWith("(", StringComparison.Ordinal) && word.EndsWith(")", StringComparison.Ordinal))
                {
                    continue;
                }

                if (word.Length > longestWord.Length)
                {
                    longestWord = word;
                }
            }

            using (var g = Graphics.FromImage(bmp))
            using (var font = AppController.GetButtonFont(fontSize, localizable))
            {
                g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

                SizeF stringSize;
                stringSize = g.MeasureString(longestWord, font);

                if (stringSize.Width > (bmp.Width * 0.8F))
                {
                    fontSize *= ((bmp.Width * 0.8F) / stringSize.Width);
                }
            }

            return fontSize;

        }


        private static Bitmap WriteCaption(Bitmap bmp, string caption, bool overlong, bool localizable, Color fontColour)
        {
            // Set the sizes for 2 and 3 or more letters.
            float fontSizeDouble = (FontSize.BaseFontSize * 0.75F);
            float fontSizeMulti = (FontSize.BaseFontSize * 0.575F);

            int nameLength = caption.Length;

            // Pretend overlong keys have a length of three:

            if (overlong)
            {
                nameLength = 3;
            }

            switch (nameLength)
            {
                case 1: // e.g. single letter - letters and numbers
                    bmp = DrawCaptionLine(bmp, caption, FontSize.BaseFontSize, localizable, fontColour);
                    break;

                case 2: // Two letters - mostly F keys, which need a constant font whether they are 2 or 3 chars long
                    if (caption.Substring(0, 1) == "F" && char.IsDigit(caption, 1))
                    {
                        bmp = DrawCaptionLine(bmp, caption, fontSizeDouble, localizable, fontColour);
                    }
                    else
                    {
                        bmp = DrawCaptionLine(bmp, caption, fontSizeMulti, localizable, fontColour);
                    }

                    break;

                case 3:

                    int spacePosition = caption.IndexOf(' ');

                    if (spacePosition > 0) // eg "7 &" or 5 %"
                    {

                        bmp = DrawCaptionLine(bmp, caption.Substring(0, spacePosition), fontSizeDouble, TextPosition.Bottom, localizable, fontColour);
                        // May not strictly be three characters so send the rest:
                        bmp = DrawCaptionLine(bmp, caption.Substring(spacePosition + 1), fontSizeDouble, TextPosition.SymbolTop, localizable, fontColour);
                    }
                    else // End, F12 etc...
                    {
                        if (caption.Substring(0, 1) == "F" && char.IsDigit(caption, 1) && char.IsDigit(caption, 2))
                        {
                            // F11, F12 to be the same size as F1 to F9
                            bmp = DrawCaptionLine(bmp, caption, fontSizeDouble, localizable, fontColour);
                        }
                        else
                        { // End, Tab, Esc etc
                            bmp = DrawCaptionLine(bmp, caption, fontSizeMulti, localizable, fontColour);
                        }
                    }
                    break;

                default:
                    // More than 3 letters long
                    var words = caption.Split(' ');

                    fontSizeMulti = GetMultiLineFontSize(bmp, caption, localizable, fontSizeMulti);

                    // If the last word is a word in brackets, remove it
                    // E.G. Enter (NumberPad) will just be written on the key as Enter.

                    // Assuming there are only two words.

                    switch (words.Length)
                    {
                        case 1:
                            DrawCaptionLine(bmp, words[0], fontSizeMulti, localizable, fontColour);
                            break;
                        case 2:
                            if ((words[1].StartsWith("(", StringComparison.Ordinal) && words[1].EndsWith(")", StringComparison.Ordinal)))
                            {
                                DrawCaptionLine(bmp, words[0], fontSizeMulti, TextPosition.Middle, localizable, fontColour);
                            }
                            else
                            {
                                DrawCaptionLine(bmp, words[0], fontSizeMulti, TextPosition.TextTop, localizable, fontColour);
                                DrawCaptionLine(bmp, words[1], fontSizeMulti, TextPosition.Bottom, localizable, fontColour);
                            }
                            break;
                    }
                    break;
            }

            return bmp;

        }

        private static Bitmap WriteCaption(Bitmap bmp, int scanCode, int extended, Color fontColour)
        {
            string caption = AppController.GetKeyName(scanCode, extended) ?? $"SC: {scanCode} EX: {extended}";

            // Blank keys.
            if (string.IsNullOrEmpty(caption))
            {
                return bmp;
            }

            bool overlong = false;
            bool localizable = false;

            int hash = KeyHasher.GetHashFromKeyData(scanCode, extended);

            if (AppController.IsOverlongKey(hash))
            {
                overlong = true;
            }

            if (AppController.IsLocalizableKey(hash))
            {
                localizable = true;
            }

            return WriteCaption(bmp, caption, overlong, localizable, fontColour);
        }

        private static Bitmap DrawCaptionLine(Bitmap button, string caption, float fontSize, bool localizable, Color fontColour)
        {
            return DrawCaptionLine(button, caption, fontSize, TextPosition.Middle, localizable, fontColour);
        }

        private static Bitmap DrawCaptionLine(Bitmap bmp, string caption, float fontSize, TextPosition where, bool localizable, Color fontColour)
        {

            // The upper 10/64 and lower 14/64 of the button are not considered drawing area 
            // so discount them when calculating the row position. This also knocks the centre down by 4/64.

            using (var font = AppController.GetButtonFont(fontSize, localizable))
            using (var g = Graphics.FromImage(bmp))
            {
                g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

                // Use width of actual string for left placement:
                // This only takes tiny amount of time - 14ms for 10000 iterations..
                var stringSize = g.MeasureString(caption, font);

                int left = (bmp.Width / 2) - (int)(stringSize.Width / 2);
                int top = 0;

                // Vertical centre justify within button and row:
                switch (@where)
                {
                    case TextPosition.Middle:
                        // Remove cast to int, get 'possible loss of fraction'. Oh well..
                        top = (int)((bmp.Height - stringSize.Height) / 2 + Convert.ToInt32(bmp.Height / 28));
                        break;
                    case TextPosition.TextTop:
                        top = (int)(bmp.Height * 14F / 64F);
                        break;
                    case TextPosition.Bottom:
                        top = bmp.Height / 2;
                        break;
                    case TextPosition.SymbolTop:
                        top = (int)(bmp.Height * 8F / 64F);
                        break;
                }

                using (var b = new SolidBrush(fontColour))
                {
                    g.DrawString(caption, font, b, new Point(left, top));
                }
            }

            return bmp;
        }

        public static ColorMatrix GetMatrix(ButtonEffect effect, bool ignoreUserSettings = false)
        {
            if (ignoreUserSettings == false)
            {
                var setting = UserColourSettingManager.GetColourSettings(effect);
                if (setting != null)
                {
                    return setting.Matrix;
                }
            }

            ColorMatrix cm = null;

            switch (effect)
            {
                case ButtonEffect.None:
                    cm = new ColorMatrix();
                    break;

                case ButtonEffect.Mapped:
                    cm = Blue();
                    break;

                case ButtonEffect.Disabled:
                    cm = Darken(-0.3F);
                    break;

                case ButtonEffect.MappedPending:
                    cm = GreenyBlue();
                    break;

                case ButtonEffect.UnmappedPending:
                    cm = Golden();
                    break;

                case ButtonEffect.DisabledPending:
                    cm = DarkGold();
                    break;

                case ButtonEffect.EnabledPending:
                    cm = GoldenDarken();
                    break;
            }

            return cm;
        }

        private static Bitmap ApplyEffect(Bitmap bmp, ButtonEffect effect)
        {
            var cm = GetMatrix(effect);

            if (cm == null)
            {
                return bmp;
            }

            return Transform(bmp, cm);
        }

        private static ColorMatrix Darken(float darkFactor)
        {
            return new ColorMatrix(
                new[]
                {
                    new float[] {1, 0, 0, 0, 0},
                    new float[] {0, 1, 0, 0, 0},
                    new float[] {0, 0, 1, 0, 0},
                    new float[] {0, 0, 0, 1, 0},
                    new[] {darkFactor, darkFactor, darkFactor, 0, 1}
                });

        }

        private static ColorMatrix DarkGold()
        {
            return new ColorMatrix(
                new[]
                {
                    new float[] {1, 0, 0, 0, 0},
                    new float[] {0, 1, 0, 0, 0},
                    new float[] {0, 0, 1, 0, 0},
                    new float[] {0, 0, 0, 1, 0},
                    new[] {-0.2F, -0.2F, -0.3F, 0, 1}
                });

        }

        private static ColorMatrix GoldenDarken()
        {
            return new ColorMatrix(
                new[]
                {
                    new[] {1F, 0, 0, 0, 0},
                    new[] {0, 1F, 0, 0, 0},
                    new[] {0, 0, 1F, 0, 0},
                    new[] {0, 0, 0, 1F, 0},
                    new[] {-0.1F, -0.2F, -0.3F, 0, 1}
                });
        }

        private static ColorMatrix GreenyBlue()
        {
            return new ColorMatrix(new[]
            {
                new[] {0.5F, 0, 0, 0, 0},
                new[] {0, 1F, 0, 0, 0},
                new[] {0, 0.1F, 1, 0, 0},
                new[] {0, 0, 0, 1F, 0},
                new[] {0, 0, 0, 0, 1F}
            });
        }

        private static ColorMatrix Blue()
        {
            return new ColorMatrix(
                new[]
                {
                    new[] {1F, 0, 0, 0, 0},
                    new[] {0, 0.9F, 0, 0, 0},
                    new[] {0, 0, 1F, 0, 0},
                    new[] {0, 0, 0, 1F, 0},
                    new[] {-0.6F, -0.2F, 0.5F, 0, 1}
                });

        }

        private static ColorMatrix Golden()
        {
            // ColorMatrix cm = new ColorMatrix();
            return new ColorMatrix(new[]
            {
                new[] {1F, 0, 0, 0, 0},
                new[] {0, 1F, 0, 0, 0},
                new[] {0, 0, 1F, 0, 0},
                new[] {0.5F, 0, 0, 1F, 0},
                new[] {-0.1F, -0.1F, -1F, 0, 1}
            });
        }
    }

    public enum ButtonEffect
    {
        None, Mapped, MappedPending, Disabled, UnmappedPending, DisabledPending, EnabledPending
    }

    public enum TextPosition
    {
        Middle, TextTop, Bottom, SymbolTop
    }

    public enum BlankButton
    {
        None = -1,
        Blank = 0,
        TallBlank = 1,
        MediumWideBlank = 2,
        DoubleWideBlank = 3,
        TripleWideBlank = 4,
        QuadrupleWideBlank = 5
    };
}
