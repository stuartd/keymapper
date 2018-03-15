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
		private readonly decimal lowbound = -1M;
		private readonly decimal highbound = 1M;
		private readonly decimal step = 0.1M;

		private bool drawing;
		private bool initialised;
		private readonly string caption;

		private ColorMatrix currentMatrix;

		public ButtonEffect Effect { get; }

		private Color fontColour = Color.Black;

		public ColourEditor(ButtonEffect effect, string caption)
		{
			InitializeComponent();

			Effect = effect;
			this.caption = caption;

			Text = "Editing the " + caption + " button";

			LoadSetting();


			UserColourSettingManager.ColoursChanged += delegate(object sender, EventArgs e) { OnColoursChanged(); };
		}

		private void OnColoursChanged()
		{
			LoadSetting();
		}

		private void LoadSetting()
		{
			UpdateMatrix(ButtonImages.GetMatrix(Effect));
			fontColour = ButtonImages.GetFontColour(Effect);
			DrawKey();
		}

		private void UpdateMatrix(ColorMatrix cm)
		{
			currentMatrix = cm;
			SetUpdownValuesFromMatrix();
		}

		private void SetUpdownValuesFromMatrix()
		{
			drawing = false;

			foreach (Control con in Controls)
			{
				if (con is NumericUpDown updown)
				{
					if (!initialised)
					{
						updown.DecimalPlaces = 1;
						updown.Minimum = lowbound;
						updown.Maximum = highbound;
						updown.Increment = step;
					}

					updown.ValueChanged -= UpdownValueChanged;
					updown.Value = GetValue(updown.Name);
					updown.ValueChanged += UpdownValueChanged;
				}

			}

			drawing = true;
			initialised = true;

		}

		private decimal GetValue(string name)
		{
			// Access the appropriate property of the matrix:
			var value = currentMatrix.GetType().GetProperty(name).GetValue(currentMatrix, null);
			if (decimal.TryParse(value.ToString(), out decimal dvalue)) {
				return dvalue;
			}

			return decimal.Zero;
		}


		private void UpdownValueChanged(object sender, EventArgs e)
		{
			if (drawing)
			{
				UpdateMatrixFromControls();
				SaveSetting();
			}
		}

		private void UpdateMatrixFromControls()
		{
			currentMatrix = new ColorMatrix(
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

			var bmp = ButtonImages.GetButtonImage(BlankButton.MediumWideBlank, 0.75F, caption, currentMatrix, fontColour);

			KeyBox.Image?.Dispose();

			KeyBox.Image = bmp;

		}

		private void ResetButtonClick(object sender, EventArgs e)
		{
			// Passing true to ignore user colours and fonts.
			UpdateMatrix(ButtonImages.GetMatrix(Effect, true));
			fontColour = ButtonImages.GetFontColour(Effect, true);

			SaveSetting();
		}


		private void BlankButtonClick(object sender, EventArgs e)
		{
			UpdateMatrix(new ColorMatrix());
			fontColour = ButtonImages.GetFontColour(ButtonEffect.None, true);

			SaveSetting();
		}

		private void ColourEditorFormClosed(object sender, FormClosedEventArgs e)
		{
			SaveUserSettings();
		}

		private void SaveUserSettings()
		{
			var userSettings = new Properties.Settings {
				ColourEditorLocation = Location
			};

			userSettings.Save();
		}

		private void SaveSetting()
		{
			UserColourSettingManager.SaveSetting(Effect, currentMatrix, fontColour.ToArgb());
		}

		private void TextButtonClick(object sender, EventArgs e)
		{
			var colourPicker = new ColorDialog {
				Color = fontColour
			};

			// Sets the initial color select to the current text color.

			if (colourPicker.ShowDialog() == DialogResult.OK)
			{
				fontColour = colourPicker.Color;
				SaveSetting();

			}
		}


		private void RandomizeButtonClick(object sender, EventArgs e)
		{
			var cm = new ColorMatrix();

			int numberOfChanges = 5;

			var r = new Random();

			for (int i = 0; i < numberOfChanges; i++)
			{
				int x = r.Next(0, 5);
				int y = r.Next(0, 3);

				string name = "Matrix" + x.ToString(CultureInfo.InvariantCulture) + y.ToString(CultureInfo.InvariantCulture);
				float val = r.Next(-10, 11) / 10F;

				// Update control...
				// (this.Controls[name] as NumericUpDown).Value = val;

				// Or update field.
				cm.GetType().GetProperty(name).SetValue(cm, val, null);
			}

			fontColour = Color.FromArgb(r.Next(0, 256), r.Next(0, 256), r.Next(0, 256));
			UpdateMatrix(cm);

			SaveSetting();

		}
	}

}