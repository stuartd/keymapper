using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace KeyMapper
{
	static class FormsManager
	{

		static KeyboardForm _mainForm;
		static ColourList _colourMapForm = null;
		static MappingListForm _mapListForm = null;
		static Dictionary<ButtonEffect, ColourEditor> _editorForms = new Dictionary<ButtonEffect, ColourEditor>();


		public static void RegisterMainForm(KeyboardForm form)
		{
			_mainForm = form;
		}

		public static void SubFormClosed(object sender, FormClosedEventArgs e)
		{
			if (sender is ColourList)
				_colourMapForm = null;
			else if (sender is MappingListForm)
				_mapListForm = null;
			else if (sender is ColourEditor)
			{
				if (_editorForms.ContainsKey(((ColourEditor)sender).Effect))
					_editorForms.Remove(((ColourEditor)sender).Effect);
			}
			_mainForm.RegenerateWindowMenu();

		}

		public static void ShowColourEditorForm(string message)
		{
			string effectname = message.Substring(0, message.IndexOf(" ", StringComparison.Ordinal));
			string text = message.Substring(message.IndexOf(" ", StringComparison.Ordinal) + 1);
			ButtonEffect effect = (ButtonEffect)System.Enum.Parse(typeof(ButtonEffect), effectname);

			ColourEditor form;

			if (_editorForms.ContainsKey(effect))
			{
				if (_editorForms.TryGetValue(effect, out form))
				{
					form.BringToFront();
					return;
				}
			}

			form = new ColourEditor(effect, text);

			// Now, where to put it..
			Point p = Point.Empty;

			// If there are no forms open, use the last closed position.
			if (_editorForms.Count == 0)
			{
				Properties.Settings userSettings = new Properties.Settings();
				p = userSettings.ColourEditorLocation;
			}
			else
			{
				// TODO not good enough.
				foreach (ColourEditor openform in _editorForms.Values)
				{
					if (openform != null)
						if (openform.Location.X > p.X && openform.Location.Y > p.Y)
							p = openform.Location;
				}
				p = new Point(p.X + 50, p.Y + 50);

			}

			form.Location = p;

			_editorForms.Add(effect, form);
			form.FormClosed += SubFormClosed;
			form.Show((IWin32Window)_mainForm);

		}

		public static void ArrangeAllForms()
		{

			if (_mapListForm != null)
				PostionSubform(_mapListForm, SubformPosition.BottomRight);

			if (_colourMapForm != null)
				PostionSubform(_colourMapForm, SubformPosition.BottomLeft);

		}


		public static bool IsColourMapFormOpen()
		{
			return (_colourMapForm != null);
		}

		public static bool IsMappingListFormOpen()
		{
			return (_mapListForm != null);
		}

		public static void OpenSubForms()
		{

			Properties.Settings userSettings = new Properties.Settings();

			if (userSettings.KeyboardFormColourMapFormOpen)
				ToggleColourMapForm();

			if (userSettings.KeyboardFormHasMappingListFormOpen)
				ToggleMappingListForm();
		}

		public static void ShowEditMappingForm(KeyMapping km, bool useCapture)
		{
			EditMappingForm mf = new EditMappingForm(km, useCapture);

			Properties.Settings userSettings = new Properties.Settings();

			Point savedLocation = userSettings.EditMappingFormLocation;

			if (savedLocation.IsEmpty == false)
			{
				mf.Location = savedLocation;
			}
			else
			{
				PostionSubform(mf, SubformPosition.MiddleLeft);
			}

			mf.ShowDialog((IWin32Window)_mainForm);
		}


		public static void ToggleMappingListForm()
		{

			if (_mapListForm == null)
			{

				Properties.Settings userSettings = new Properties.Settings();

				Point formLocation = userSettings.MappingListFormLocation;
				_mapListForm = new MappingListForm();

				// Load settings before positioning so we know how wide form is
				_mapListForm.LoadUserSettings();
				
				if (formLocation.IsEmpty)
				{
					PostionSubform(_mapListForm, SubformPosition.BottomRight);
				}
				else
					_mapListForm.Location = formLocation;

				_mapListForm.FormClosed += SubFormClosed;

				_mapListForm.Show((IWin32Window)_mainForm);
			}
			else
			{
				_mapListForm.Close();
			}

		}

		public static void ToggleColourMapForm()
		{
			if (_colourMapForm == null)
			{

				_colourMapForm = new ColourList();

				Properties.Settings userSettings = new Properties.Settings();

				Point formLocation = userSettings.ColourListFormLocation;

				if (formLocation.IsEmpty)
					PostionSubform(_colourMapForm, SubformPosition.BottomLeft);
				else
					_colourMapForm.Location = formLocation;


				_colourMapForm.FormClosed += SubFormClosed;
				_colourMapForm.Show((IWin32Window)_mainForm);
			}
			else
			{
				_colourMapForm.Close();
			}

		}

		public static void PostionSubform(Form subform, SubformPosition position)
		{
			switch (position)
			{
				case SubformPosition.BottomLeft:
					subform.Location = new Point(_mainForm.Location.X, _mainForm.Location.Y + _mainForm.Height + 1);
					break;
				case SubformPosition.BottomCentre:
					break;
				case SubformPosition.BottomRight:
					subform.Location = new Point(
					_mainForm.Location.X + _mainForm.Width - subform.Width,
					_mainForm.Location.Y + _mainForm.Height + 1);
					break;
				case SubformPosition.MiddleLeft:
					subform.Location = new Point(
						_mainForm.Location.X + 50,
						((_mainForm.ClientSize.Height - subform.ClientSize.Height) / 2) + _mainForm.Location.Y);
					break;

				default:
					break;
			}


		}



	}

	public enum SubformPosition
	{
		BottomLeft, BottomCentre, BottomRight, MiddleLeft
	}
}
