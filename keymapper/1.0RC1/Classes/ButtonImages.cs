using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace KeyMapper
{

	public static class ButtonImages
	{
		#region Fields

		private static string _path = "KeyMapper.Images.";
		private static float _lastScale ;
		private static BlankButton _lastButton = BlankButton.None;
		private static Bitmap _lastImage;

		#endregion

		#region Button Image methods

		// Basic blank button, unscaled, no effects
		public static Bitmap GetButtonImage(int scancode, int extended)
		{
			return GetButtonImage(scancode, extended, BlankButton.Blank, 0, 0, 1F, ButtonEffect.None, String.Empty);
		}

		// Specific scaled button with custom caption
		public static Bitmap GetButtonImage(BlankButton button, float scale, string caption, ButtonEffect effect)
		{
			return GetButtonImage(-1, -1, button, 0, 0, scale, effect, caption);
		}

		// Custom ColorMatrix and font colour
		public static Bitmap GetButtonImage(BlankButton button, float scale, string caption, ColorMatrix cm, Color fontColour)
		{
			Bitmap bmp = GetBitmap(button, 0, 0, scale, ButtonEffect.None, false);
			bmp = Transform(bmp, cm);
			return WriteCaption(bmp, caption, false, false, fontColour);
		}


		// Offers full control over the button scale and stretch.
		public static Bitmap GetButtonImage
			(int scancode, int extended, BlankButton button, int horizontalStretch, int verticalStretch, float scale, ButtonEffect effect)
		{
			return GetButtonImage
				(scancode, extended, button, horizontalStretch, verticalStretch, scale, effect, String.Empty);
		}

		// This (private) method does the work. 
		private static Bitmap GetButtonImage(int scancode, int extended, BlankButton button, int horizontalStretch,
															int verticalStretch, float scale, ButtonEffect effect, string caption)
		{

			Bitmap bmp = GetBitmap(button, horizontalStretch, verticalStretch, scale, effect, true);

			Color fontColour = GetFontColour(effect);

			if (String.IsNullOrEmpty(caption))
				bmp = WriteCaption(bmp, scancode, extended, fontColour);
			else
				bmp = WriteCaption(bmp, caption, false, false, fontColour);

			return bmp;
		}

		#endregion

		#region Other public methods

		public static Bitmap GetImage(string buttonFileName, string extension)
		{
			string filepath = _path + buttonFileName.ToLowerInvariant() + "." + extension;
			return ExtractImage(filepath);
		}

		public static Bitmap ResizeBitmap(Bitmap bmp, float scale, bool forceEven)
		{
			if (bmp == null)
				return bmp;

			// Even numbers only in case we want to stretch.
			int newWidth = (int)Math.Round((bmp.Width * scale), 0);

			if (forceEven && newWidth % 2 == 1)
				newWidth += 1;

			int newHeight = (int)Math.Round((bmp.Height * scale), 0);

			if (forceEven && newHeight % 2 == 1)
				newHeight += 1;

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
				UserColourSetting setting = UserColourSettingManager.GetColourSettings(effect);
				if (setting != null)
					return setting.FontColour;
			}

			switch (effect)
			{
				case ButtonEffect.NoMappingAllowed:
					return Color.DarkRed;
			
				default:
					return Color.Black;

			}
		}

		#endregion

		#region Private methods

		public static Bitmap StretchBitmap(Bitmap bmp, int horizontalStretch, int verticalStretch)
		{

			Bitmap newbitmap = null;

			if (horizontalStretch != 0 & verticalStretch != 0)
			{
				Bitmap tempbitmap = null;
				tempbitmap = StretchButtonHorizontal(bmp, horizontalStretch);
				newbitmap = StretchButtonVertical(tempbitmap, verticalStretch);
			}
			else
			{
				if (horizontalStretch != 0)
				{
					newbitmap = StretchButtonHorizontal(bmp, horizontalStretch);
				}

				else
					if (verticalStretch != 0)
					{
						newbitmap = StretchButtonVertical(bmp, verticalStretch);
					}
			}

			return newbitmap;

		}

		private static Bitmap ExtractImage(string name)
		{
			Bitmap bmp = null;

			System.IO.Stream stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(name);

			if (stream != null)
				bmp = new System.Drawing.Bitmap(stream);

			return bmp;

		}

		private static Bitmap GetBitmap(BlankButton button, int horizontalStretch, int verticalStretch, float scale, ButtonEffect effect, bool transform)
		{
			if (scale != _lastScale)
				AppController.SetFontSizes(scale);

			Bitmap bmp = null;

			// Cache bitmap until scale or button changes (as long as no stretch is applied)

			if (horizontalStretch != 0 || verticalStretch != 0)
			{
				// IF this image needs to be stretched, force the height and width to even numbers
				// (This only seems to affect height?)
				bool forceEvenNumber = (verticalStretch != 0);
				bmp = ResizeBitmap(AppController.GetBitmap(button), scale, forceEvenNumber);
				bmp = StretchBitmap(bmp, horizontalStretch, verticalStretch);
				_lastButton = BlankButton.None;

			}
			else
			{
				// If no stretch is applied, can we reuse the last bitmap we created?
				if (scale == _lastScale && button == _lastButton && _lastImage != null)
				{
					bmp = (Bitmap)_lastImage.Clone();
					// Console.WriteLine("Reusing bitmap");
				}
				else
				{
					bmp = ResizeBitmap(AppController.GetBitmap(button), scale, false);
					if (_lastImage != null)
						_lastImage.Dispose();

					_lastImage = (Bitmap)bmp.Clone();
				}


				_lastButton = button;

			}

			// Now all decisions have been made..
			_lastScale = scale;

			if (transform)
				bmp = ApplyEffect(bmp, effect);

			return bmp;

		}

		private static Bitmap StretchButtonHorizontal(Bitmap bmp, int horizontalstretch)
		{
			// snap it in half, pull halves apart, slice off the inner edges and
			// clone them along the gap

			// Get as big a slice in the middle as we can (the highest common factor of the 
			// stretch and half the width) 

			int halfwidth = bmp.Width / 2;
			int slicewidth = AppController.GetHighestCommonDenominator(horizontalstretch, halfwidth);

			Bitmap newbitmap = new Bitmap(bmp.Width + horizontalstretch, bmp.Height);
			using (Graphics g = Graphics.FromImage((Image)newbitmap))
			{
				g.DrawImage(bmp, 0, 0, new Rectangle(0, 0, halfwidth, bmp.Height), GraphicsUnit.Pixel);
				for (int i = 0; i < horizontalstretch; i += slicewidth)
				{
					g.DrawImage(bmp, halfwidth + i, 0, new Rectangle(halfwidth, 0, slicewidth, bmp.Height), GraphicsUnit.Pixel);
				}
				g.DrawImage(bmp, halfwidth + horizontalstretch - 1, 0,
						new Rectangle(halfwidth, 0, halfwidth, bmp.Height), GraphicsUnit.Pixel);
			}

			bmp.Dispose();
			return newbitmap;

		}

		private static Bitmap StretchButtonVertical(Bitmap bmp, int verticalstretch)
		{

			// Same as horizontal more-or-less

			int halfheight = bmp.Height / 2;
			int sliceheight = AppController.GetHighestCommonDenominator(verticalstretch, halfheight);

			Bitmap newbitmap = new Bitmap(bmp.Width, bmp.Height + verticalstretch);

			using (Graphics g = Graphics.FromImage((Image)newbitmap))
			{
				g.DrawImage(bmp, 0, 0, new Rectangle(0, 0, bmp.Width, halfheight), GraphicsUnit.Pixel);
				for (int i = 0; i < verticalstretch; i += sliceheight)
				{
					g.DrawImage(bmp, 0, halfheight + i, new Rectangle(0, halfheight, bmp.Width, sliceheight), GraphicsUnit.Pixel);
				}
				g.DrawImage(bmp, 0, halfheight + verticalstretch - 1,
						new Rectangle(0, halfheight, bmp.Width, halfheight), GraphicsUnit.Pixel);
			}

			bmp.Dispose();
			return newbitmap;

		}

		private static Bitmap ScaleBitmap(Bitmap bmp, int newWidth, int newHeight)
		{

			if (newWidth == 0 || newHeight == 0)
			{
				Console.WriteLine("Attempting to resize a bitmap to invalid dimensions");
				return bmp;
			}

			Bitmap newbitmap = new Bitmap(newWidth, newHeight);
			using (Graphics g = Graphics.FromImage((Image)newbitmap))
			{
				g.DrawImage(bmp, 0, 0, newWidth, newHeight);
			}
			bmp.Dispose();
			return newbitmap;

		}

		private static Bitmap Transform(Bitmap bmp, ColorMatrix cm)
		{
			if (bmp == null)
				return bmp;

			Bitmap copy = new Bitmap(bmp.Width, bmp.Height);
			using (ImageAttributes ia = new ImageAttributes())
			using (Graphics g = Graphics.FromImage(copy))
			{
				ia.SetColorMatrix(cm);
				g.DrawImage(bmp, new Rectangle(0, 0, bmp.Width, bmp.Height), 0, 0, bmp.Width, bmp.Height, GraphicsUnit.Pixel, ia);
				bmp.Dispose();
			}
			return copy;

		}

		#endregion

		#region Caption

		private static Bitmap WriteCaption(Bitmap bmp, string caption, bool overlong, bool localizable, Color fontColour)
		{

   			// Set the sizes for 2 and 3 or more letters.
			float FontSizeDouble = (AppController.BaseFontSize * 0.75F);
			float FontSizeMulti = (AppController.BaseFontSize * 0.575F);

			int namelength = caption.Length;

			// Pretend overlong keys have a length of three:

			if (overlong)
				namelength = 3;

			switch (namelength)
			{
				case 1: // e.g. single letter - letters and numbers
					bmp = DrawCaptionLine(bmp, caption, AppController.BaseFontSize, localizable, fontColour);
					break;

				case 2: // Two letters - mostly F keys, which need a constant font whether they are 2 or 3 chars long
					if (caption.Substring(0, 1) == "F" && Char.IsDigit(caption, 1))
						bmp = DrawCaptionLine(bmp, caption, FontSizeDouble, localizable, fontColour);
					else
						bmp = DrawCaptionLine(bmp, caption, FontSizeMulti, localizable, fontColour);

					break;

				case 3:

					int spacepos = caption.IndexOf(' ');

					if (spacepos > 0) // eg "7 &" or 5 %"
					{

						bmp = DrawCaptionLine(bmp, caption.Substring(0, spacepos), FontSizeDouble, TextPosition.Bottom, localizable, fontColour);
						// May not strictly be three characters so send the rest:
						bmp = DrawCaptionLine(bmp, caption.Substring(spacepos + 1), FontSizeDouble, TextPosition.SymbolTop, localizable, fontColour);
					}
					else // End, F12 etc...
					{
						if (caption.Substring(0, 1) == "F" && Char.IsDigit(caption, 1) && Char.IsDigit(caption, 2))
						{
							// F11, F12 to be the same size as F1 to F9
							bmp = DrawCaptionLine(bmp, caption, FontSizeDouble, localizable, fontColour);
						}
						else
						{ // End, Tab, Esc etc
							bmp = DrawCaptionLine(bmp, caption, FontSizeMulti, localizable, fontColour);
						}
					}
					break;

				default:
					// More than 3 letters long
					string[] words = caption.Split(' ');
                    // If the last word is a word in brackets, remove it
                    // E.G. Enter (Numberpad) will just be written on the key as Enter.
					switch (words.Length)
					{
						case 1:
							DrawCaptionLine(bmp, words[0], FontSizeMulti, localizable, fontColour);
							break;
                        case 2: if ((words[1].Trim().Substring(0, 1) == "("
                            && words[1].Trim().Substring(words[1].Length - 1, 1) == ")"))
                            {
                                DrawCaptionLine(bmp, words[0], FontSizeMulti, TextPosition.Middle, localizable, fontColour);
                            }
                            else
                            {
                                DrawCaptionLine(bmp, words[0], FontSizeMulti, TextPosition.TextTop, localizable, fontColour);
                                DrawCaptionLine(bmp, words[1], FontSizeMulti, TextPosition.Bottom, localizable, fontColour);
                            }
							break;
						default:
							break;
					}
					break;
			}

			return bmp;

		}

		private static Bitmap WriteCaption(Bitmap bmp, int scancode, int extended, Color fontColour)
		{

			string caption = AppController.GetKeyName(scancode, extended);

			if (caption == null)
			{
				caption = "Unknown";
			}

			// Blank keys.
			if (String.IsNullOrEmpty(caption))
				return bmp;

			bool overlong = false;
			bool localizable = false;

			int hash = AppController.GetHashFromKeyData(scancode, extended);

			if (AppController.IsOverlongKey(hash))
				overlong = true;

			if (AppController.IsLocalizableKey(hash))
				localizable = true;

			return WriteCaption(bmp, caption, overlong, localizable, fontColour);
		}

		private static Bitmap DrawCaptionLine(Bitmap button, string caption, float fontsize, bool localizable, Color fontColour)
		{
			return DrawCaptionLine(button, caption, fontsize, TextPosition.Middle, localizable, fontColour);
		}

		private static Bitmap DrawCaptionLine(Bitmap bmp, string caption, float fontsize, TextPosition where, bool localizable, Color fontColour)
		{

			// Each string needs to be tested to see if it fits in the button or if it has to be bumped down.

			// The upper 10/64 and lower 14/64 of the button are not considered drawing area 
			// so discount them when calculating the row position. This also knocks the centre down by 4/64.

			using (Graphics g = Graphics.FromImage(bmp))
			{
				g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

				Font font = AppController.GetFontFromCache(fontsize, localizable);

				// Use width of actual string for left placement:
				SizeF stringSize = SizeF.Empty;

				// This only takes tiny amount of time - 14ms for 10000 iterations..
				stringSize = g.MeasureString(caption, font);

				if (stringSize.Width > (bmp.Width * 0.85F))
				{
					fontsize *= ((bmp.Width * 0.85F) / stringSize.Width);
					font = AppController.GetFontFromCache(fontsize, localizable);
					stringSize = g.MeasureString(caption, font);
				}

				int left = (bmp.Width / 2) - (int)(stringSize.Width / 2);
				int top = 0;

				// Vertical centre justify within button and row:
				switch (where)
				{
					case TextPosition.Middle:
						top = (int)(((bmp.Height - stringSize.Height) / 2) + bmp.Height / 28);
						break;
					case TextPosition.TextTop:
						top = (int)(bmp.Height * 14F / 64F);
						break;
					case TextPosition.Bottom:
						top = (int)(bmp.Height / 2);
						break;
					case TextPosition.SymbolTop:
						top = (int)(bmp.Height * 8F / 64F);
						break;
				}

				using (SolidBrush b = new SolidBrush(fontColour))
				{
					g.DrawString(caption, font, b, new Point(left, top));
				}

			}

			return bmp;

		}

		#endregion


		#region Effects

		public static ColorMatrix GetMatrix(ButtonEffect effect)
		{
			return GetMatrix(effect, false) ;
		}

		public static ColorMatrix GetMatrix(ButtonEffect effect, bool ignoreUserSettings)
		{
			if (ignoreUserSettings == false)
			{
			UserColourSetting setting = UserColourSettingManager.GetColourSettings(effect);
			if (setting != null)
				return setting.Matrix;
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

				case ButtonEffect.NoMappingAllowed:
					cm = Darken(-0.1F);
					break;
			}

			return cm;

		}

		
		private static Bitmap ApplyEffect(Bitmap bmp, ButtonEffect effect)
		{
			ColorMatrix cm = GetMatrix(effect);

			if (cm == null)
				return bmp;
			else
				return Transform(bmp, cm);
		}

		private static ColorMatrix Darken(float darkFactor)
		{
			return new System.Drawing.Imaging.ColorMatrix(
				new float[][]
				 {
					new float[] {1, 0, 0, 0, 0},
					new float[] {0, 1, 0, 0, 0},
					new float[] {0, 0, 1, 0, 0},
					new float[] {0, 0, 0, 1, 0},
					new float[] {darkFactor, darkFactor, darkFactor, 0, 1}});

		}

		private static ColorMatrix DarkGold()
		{
			return new System.Drawing.Imaging.ColorMatrix(
				new float[][]
				 {
					new float[] {1, 0, 0, 0, 0},
					new float[] {0, 1, 0, 0, 0},
					new float[] {0, 0, 1, 0, 0},
					new float[] {0, 0, 0, 1, 0},
					new float[] {-0.2F, -0.2F, -0.3F, 0, 1}});

		}

		private static ColorMatrix GoldenDarken()
		{
			return new System.Drawing.Imaging.ColorMatrix(
				new float[][]
				 {
					new float[] {1, 0, 0, 0, 0},
					new float[] {0, 1, 0, 0, 0},
					new float[] {0, 0, 1, 0, 0},
					new float[] {0, 0, 0, 1, 0},
					new float[] {-0.1F, -0.2F, -0.3F, 0, 1}});
		}

		private static ColorMatrix GreenyBlue()
		{
			return new System.Drawing.Imaging.ColorMatrix(
							new float[][]
				{
				new float[] {0.5F, 0, 0, 0, 0},
				new float[] {0, 1, 0, 0, 0},
				new float[] {0, 0.1F, 1, 0, 0},
				new float[] {0, 0, 0, 1, 0},
				new float[] {0, 0, 0, 0, 1}});

		}

		private static ColorMatrix Blue()
		{
			return new System.Drawing.Imaging.ColorMatrix(
				new float[][]
				{
				new float[] {1, 0, 0, 0, 0},
				new float[] {0, 0.9F, 0, 0, 0},
				new float[] {0, 0, 1, 0, 0},
				new float[] {0, 0, 0, 1, 0},
				new float[] {-0.6F, -0.2F, 0.5F, 0, 1}});

		}

		private static ColorMatrix Golden()
		{
			// ColorMatrix cm = new ColorMatrix();
			return new System.Drawing.Imaging.ColorMatrix(
				new float[][]
				{
				new float[] {1, 0, 0, 0, 0},
				new float[] {0, 1, 0, 0, 0},
				new float[] {0, 0, 1, 0, 0},
				new float[] {0.5F, 0, 0, 1, 0},
				new float[] {-0.1F, -0.1F, -1F, 0, 1}});

		}

		#endregion

	}

	#region Enums

	public enum ButtonEffect
	{
		None, Mapped, MappedPending, Disabled, UnmappedPending, DisabledPending, EnabledPending, NoMappingAllowed
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
	} ;

	#endregion




}


