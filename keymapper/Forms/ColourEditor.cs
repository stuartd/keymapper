using System;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Globalization;
using KeyMapper.Classes;

namespace KeyMapper.Forms
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
			get { return this._effect; }
			set { this._effect = value; }
		}

		Color _fontColour = Color.Black;

		public ColourEditor(ButtonEffect effect, string caption)
		{
			InitializeComponent();

			this._effect = effect;
			this._caption = caption;

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
			UpdateMatrix(ButtonImages.GetMatrix(this._effect));
			this._fontColour = ButtonImages.GetFontColour(this._effect);
			DrawKey();
		}

		private void UpdateMatrix(ColorMatrix cm)
		{
			this._currentMatrix = cm;
			SetUpdownValuesFromMatrix();
		}

		void SetUpdownValuesFromMatrix()
		{
			this._drawing = false;

			foreach (Control con in Controls)
			{
				NumericUpDown updown = (con as NumericUpDown);
				if (updown != null)
				{
					if (!this._initialised)
					{
						updown.DecimalPlaces = 1;
						updown.Minimum = this._lowbound;
						updown.Maximum = this._highbound;
						updown.Increment = this._step;
					}

					updown.ValueChanged -= UpdownValueChanged;
					updown.Value = GetValue(updown.Name);
					updown.ValueChanged += UpdownValueChanged;
				}

			}

			this._drawing = true;
			this._initialised = true;

		}

		Decimal GetValue(string name)
		{
			// Access the appropriate property of the matrix:
			object value = this._currentMatrix.GetType().GetProperty(name).GetValue(this._currentMatrix, null);
			Decimal dvalue;
			if (Decimal.TryParse(value.ToString(), out dvalue))
				return dvalue;
			else
				return Decimal.Zero;

		}


		void UpdownValueChanged(object sender, EventArgs e)
		{
			if (this._drawing)
			{
				UpdateMatrixFromControls();
				SaveSetting();
			}
		}

		private void UpdateMatrixFromControls()
		{
			this._currentMatrix = new ColorMatrix(
				new float[][]
				 {
					new float[] {(float)this.Matrix00.Value, (float)this.Matrix01.Value, (float)this.Matrix02.Value, (float)this.Matrix03.Value, (float)this.Matrix04.Value},
					new float[] {(float)this.Matrix10.Value, (float)this.Matrix11.Value, (float)this.Matrix12.Value, (float)this.Matrix13.Value, (float)this.Matrix14.Value},
					new float[] {(float)this.Matrix20.Value, (float)this.Matrix21.Value, (float)this.Matrix22.Value, (float)this.Matrix23.Value, (float)this.Matrix24.Value},
					new float[] {(float)this.Matrix30.Value, (float)this.Matrix31.Value, (float)this.Matrix32.Value, (float)this.Matrix33.Value, (float)this.Matrix34.Value},
					new float[] {(float)this.Matrix40.Value, (float)this.Matrix41.Value, (float)this.Matrix42.Value, (float)this.Matrix43.Value, (float)this.Matrix44.Value}});

		}

		private void DrawKey()
		{

			Bitmap bmp = ButtonImages.GetButtonImage(BlankButton.MediumWideBlank, 0.75F, this._caption, this._currentMatrix, this._fontColour);

			if (this.KeyBox.Image != null)
				this.KeyBox.Image.Dispose();

			this.KeyBox.Image = bmp;

		}

		private void ResetButtonClick(object sender, EventArgs e)
		{
			// Passing true to ignore user colours and fonts.
			UpdateMatrix(ButtonImages.GetMatrix(this._effect, true));
			this._fontColour = ButtonImages.GetFontColour(this._effect, true);

			SaveSetting();
		}


		private void BlankButtonClick(object sender, EventArgs e)
		{
			UpdateMatrix(new ColorMatrix());
			this._fontColour = ButtonImages.GetFontColour(ButtonEffect.None, true);

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
			UserColourSettingManager.SaveSetting(this._effect, this._currentMatrix, this._fontColour.ToArgb());
		}

		private void TextButtonClick(object sender, EventArgs e)
		{
			ColorDialog colourPicker = new ColorDialog();

			// Sets the initial color select to the current text color.
			colourPicker.Color = this._fontColour;

			if (colourPicker.ShowDialog() == DialogResult.OK)
			{
				this._fontColour = colourPicker.Color;
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

			this._fontColour = Color.FromArgb(r.Next(0, 256), r.Next(0, 256), r.Next(0, 256));
			UpdateMatrix(cm);

			SaveSetting();

		}
	}

}