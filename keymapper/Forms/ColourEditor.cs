using System;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Xml.Serialization;
using System.IO;
using System.Xml;
using System.Text;
using System.Globalization;
using KeyMapper.Classes;

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

		public ButtonEffect Effect
		{
			get { return _effect; }
			set { _effect = value; }
		}

		Color _fontColour = Color.Black;

		public ColourEditor(ButtonEffect effect, string caption)
		{
			InitializeComponent();

			_effect = effect;
			_caption = caption;

			this.Text = "Editing the " + caption + " button";

			LoadSetting();


			UserColourSettingManager.ColoursChanged += delegate(object sender, EventArgs e) { OnColoursChanged(); };
		}

		private void OnColoursChanged()
		{
			LoadSetting();
		}

		private void LoadSetting()
		{
			UpdateMatrix(ButtonImages.GetMatrix(_effect));
			_fontColour = ButtonImages.GetFontColour(_effect);
			DrawKey();
		}

		private void UpdateMatrix(ColorMatrix cm)
		{
			_currentMatrix = cm;
			SetUpdownValuesFromMatrix();
		}

		void SetUpdownValuesFromMatrix()
		{
			_drawing = false;

			foreach (Control con in Controls)
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
					updown.ValueChanged += UpdownValueChanged;
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
				SaveSetting();
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

			Bitmap bmp = ButtonImages.GetButtonImage(BlankButton.MediumWideBlank, 0.75F, _caption, _currentMatrix, _fontColour);

			if (KeyBox.Image != null)
				KeyBox.Image.Dispose();

			KeyBox.Image = bmp;

		}

		private void ResetButtonClick(object sender, EventArgs e)
		{
			// Passing true to ignore user colours and fonts.
			UpdateMatrix(ButtonImages.GetMatrix(_effect, true));
			_fontColour = ButtonImages.GetFontColour(_effect, true);

			SaveSetting();
		}


		private void BlankButtonClick(object sender, EventArgs e)
		{
			UpdateMatrix(new ColorMatrix());
			_fontColour = ButtonImages.GetFontColour(ButtonEffect.None, true);

			SaveSetting();
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

		private void SaveSetting()
		{
			UserColourSettingManager.SaveSetting(_effect, _currentMatrix, _fontColour.ToArgb());
		}

		private void TextButtonClick(object sender, EventArgs e)
		{
			ColorDialog colourPicker = new ColorDialog();

			// Sets the initial color select to the current text color.
			colourPicker.Color = _fontColour;

			if (colourPicker.ShowDialog() == DialogResult.OK)
			{
				_fontColour = colourPicker.Color;
				SaveSetting();

			}
		}


		private void RandomizeButtonClick(object sender, EventArgs e)
		{
			ColorMatrix cm = new ColorMatrix();

			int numberOfChanges = 5;

			Random r = new Random();

			for (int i = 0; i < numberOfChanges; i++)
			{
				int x = r.Next(0, 5);
				int y = r.Next(0, 3);

				string name = "Matrix" + x.ToString(CultureInfo.InvariantCulture) + y.ToString(CultureInfo.InvariantCulture);
				float val = (r.Next(-10, 11) / 10F);

				// Update control...
				// (this.Controls[name] as NumericUpDown).Value = val;

				// Or update field.
				cm.GetType().GetProperty(name).SetValue(cm, val, null);
			}

			_fontColour = Color.FromArgb(r.Next(0, 256), r.Next(0, 256), r.Next(0, 256));
			UpdateMatrix(cm);

			SaveSetting();

		}
	}

}