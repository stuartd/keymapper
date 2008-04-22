using System;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Xml.Serialization;
using System.IO;
using System.Xml;
using System.Text;

namespace KeyMapper
{
	public partial class ColourEditor : KMBaseForm
	{
		decimal _lowbound = -1M;
		decimal _highbound = 1M;
		decimal _step = 0.1M;

		bool _drawing;
		bool _initialised;
		string _caption;

		ColorMatrix _currentMatrix;
		ButtonEffect _effect;

		bool _customFontColor = false;
		Color _fontColour = Color.Black;

		public ColourEditor(ButtonEffect effect, string caption)
		{
			InitializeComponent();

			LoadUserSettings();

			_currentMatrix = ButtonImages.GetMatrix(effect);

			_effect = effect;
			_caption = caption;

			SetValues();
			DrawKey();
		}

		private void LoadUserSettings()
		{
			Properties.Settings userSettings = new Properties.Settings();
			Point savedPosition = userSettings.ColourEditorLocation;
			this.Location = savedPosition;
		}

		void SetValues()
		{
			_drawing = false;

			foreach (Control con in SpinnerPanel.Controls)
			{
				NumericUpDown updown = (con as NumericUpDown);
				if (updown != null)
				{
					if (!_initialised)
					{
						updown.DecimalPlaces = 1;
						updown.Minimum = _lowbound;
						updown.Maximum = _highbound;
						updown.Increment = _step;
					}

					updown.ValueChanged -= UpdownValueChanged;
					updown.Value = GetValue(updown.Name);
					updown.ValueChanged += new EventHandler(UpdownValueChanged);
				}

			}

			_drawing = true;
			_initialised = true;

		}

		Decimal GetValue(string name)
		{
			// Access the appropriate property of the matrix:
			object value = _currentMatrix.GetType().GetProperty(name).GetValue(_currentMatrix, null);
			Decimal dvalue;
			if (Decimal.TryParse(value.ToString(), out dvalue))
				return dvalue;
			else
				return Decimal.Zero;

		}


		void UpdownValueChanged(object sender, EventArgs e)
		{
			if (_drawing)
			{
				UpdateMatrixFromControls();
				DrawKey();
			}
		}

		private void UpdateMatrixFromControls()
		{
			_currentMatrix = new ColorMatrix(
				new float[][]
				 {
					new float[] {(float)Matrix00.Value, (float)Matrix01.Value, (float)Matrix02.Value, (float)Matrix03.Value, (float)Matrix04.Value},
					new float[] {(float)Matrix10.Value, (float)Matrix11.Value, (float)Matrix12.Value, (float)Matrix13.Value, (float)Matrix14.Value},
					new float[] {(float)Matrix20.Value, (float)Matrix21.Value, (float)Matrix22.Value, (float)Matrix23.Value, (float)Matrix24.Value},
					new float[] {(float)Matrix30.Value, (float)Matrix31.Value, (float)Matrix32.Value, (float)Matrix33.Value, (float)Matrix34.Value},
					new float[] {(float)Matrix40.Value, (float)Matrix41.Value, (float)Matrix42.Value, (float)Matrix43.Value, (float)Matrix44.Value}});

		}

		private void DrawKey()
		{
				
			Bitmap bmp;

			if (_customFontColor)
				bmp = ButtonImages.GetButtonImage(BlankButton.MediumWideBlank, 1F, _caption, _currentMatrix, _fontColour);
			else
				bmp = ButtonImages.GetButtonImage(BlankButton.MediumWideBlank, 1F, _caption, _currentMatrix, _effect);

			if (KeyBox.Image != null)
				KeyBox.Image.Dispose();

			KeyBox.Image = bmp;

		}

		private void ResetButtonClick(object sender, EventArgs e)
		{
			_currentMatrix = ButtonImages.GetMatrix(_effect, true);
			_fontColour = ButtonImages.GetFontColour(_effect, true);
			_customFontColor = true; // Until it's saved, anyway.
			SetValues();
			DrawKey();
		}


		private void BlankButtonClick(object sender, EventArgs e)
		{
			_currentMatrix = new ColorMatrix();
			_fontColour = ButtonImages.GetFontColour(ButtonEffect.None, true);
			_customFontColor = true; // It still could be different from the default for this effect
			SetValues();
			DrawKey();
		}

		private void ColourEditorFormClosed(object sender, FormClosedEventArgs e)
		{
			SaveUserSettings();
		}

		private void SaveUserSettings()
		{
			Properties.Settings userSettings = new Properties.Settings();
			userSettings.ColourEditorLocation = this.Location;
			userSettings.Save();
		}

		private void SaveButtonClick(object sender, EventArgs e)
		{
			SaveCurrentButton();
		}

		private void SaveCurrentButton()
		{
			UserColourSettingManager.SaveSetting(_effect, _currentMatrix, _fontColour.ToArgb());
		}

		private void TextButtonClick(object sender, EventArgs e)
		{
			ColorDialog colourPicker = new ColorDialog();

			// Sets the initial color select to the current text color.
			colourPicker.Color = _fontColour;

			// Update the text box color if the user clicks OK 
			if (colourPicker.ShowDialog() == DialogResult.OK)
			{
				_fontColour = colourPicker.Color;
				_customFontColor = true;
				DrawKey();
			}

		}












	}

}