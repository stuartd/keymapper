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
		static HelpForm _helpForm = null;


		public static void RegisterMainForm(KeyboardForm form)
		{
			_mainForm = form;
		}

		public static void ChildFormClosed(object sender, FormClosedEventArgs e)
		{
			if (sender is ColourList)
				_colourMapForm = null;
			else if (sender is MappingListForm)
				_mapListForm = null;
			else if (sender is HelpForm)
				_helpForm = null;
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

			ColourEditor newForm;

			if (_editorForms.ContainsKey(effect))
			{
				if (_editorForms.TryGetValue(effect, out newForm))
				{
					newForm.BringToFront();
					return;
				}
			}

			newForm = new ColourEditor(effect, text);

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
						p = openform.Location;
				}
				p = new Point(p.X + 20, p.Y + 20);

				if (WillFormFitOnScreen(p, newForm.Size) == false)
					MessageBox.Show("Doesn't fit.");

			}

			newForm.Location = p;

			_editorForms.Add(effect, newForm);
			newForm.FormClosed += ChildFormClosed;
			newForm.Show((IWin32Window)_mainForm);

		}

		public static void ResetAllForms()
		{

			PositionMainForm();
			SizeMainForm();
			_mainForm.KeyboardFormResizeEnd(null, null);



			if (_mapListForm != null)
				PositionChildForm(_mapListForm, ChildFormPosition.BottomRight);

			if (_helpForm != null)
				PositionChildForm(_helpForm, ChildFormPosition.BottomLeft);

			if (_colourMapForm != null)
				PositionColourMapForm();

		}


		public static bool IsColourMapFormOpen()
		{
			return (_colourMapForm != null);
		}

		public static bool IsMappingListFormOpen()
		{
			return (_mapListForm != null);
		}

		public static void OpenChildForms()
		{

			Properties.Settings userSettings = new Properties.Settings();

			if (userSettings.KeyboardFormColourMapFormOpen)
				ToggleColourMapForm();

			if (userSettings.KeyboardFormHasMappingListFormOpen)
				ToggleMappingListForm();

			if (userSettings.ShowHelpFormAtAtStartup)
				ShowHelpForm();
		}

		public static void ShowHelpForm()
		{
			if (_helpForm == null)
			{
				_helpForm = new HelpForm();
				Properties.Settings userSettings = new Properties.Settings();
				Point formlocation = userSettings.HelpFormLocation;

				if (formlocation.IsEmpty)
					PositionChildForm(_helpForm, ChildFormPosition.BottomLeft);
				else
					_helpForm.Location = formlocation;

				_helpForm.Show((IWin32Window)_mainForm);
			}
		}

		public static void ShowEditMappingForm(KeyMapping km, bool useCapture)
		{
			EditMappingForm mf = new EditMappingForm(km, useCapture);

			Properties.Settings userSettings = new Properties.Settings();

			Point savedLocation = userSettings.EditMappingFormLocation;


			if (savedLocation.IsEmpty == false)
				mf.Location = savedLocation;
			else
				PositionChildForm(mf, ChildFormPosition.MiddleLeft);

			mf.ShowDialog((IWin32Window)_mainForm);

		}

		private static bool WillFormsOverlap(Point form1Location, Size form1Size, Point form2Location, Size form2Size)
		{
			return !(form1Location.X > form2Location.X + form2Size.Width
			  || form1Location.X + form1Size.Width < form2Location.X
			  || form1Location.Y > form2Location.Y + form2Size.Height
			  || form1Location.Y + form1Size.Height < form2Location.Y);
		}

		private static bool WillFormFitOnScreen(Point location, Size size)
		{

			if (SystemInformation.MonitorCount == 1)
			{
				if (location.X < 0 || location.X + size.Width > SystemInformation.PrimaryMonitorSize.Width)
					return false;
			}

			// Not going to check left and right for multiple monitors as it is too problematic
			// (if the secondary monitor is to the left of the primary, negative postioning numbers
			// are the norm)

			// Check it doesn't fall off the bottom, though.

			if (location.Y + size.Height > SystemInformation.PrimaryMonitorSize.Height)
				return false;
	

			return true;

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
					PositionChildForm(_mapListForm, ChildFormPosition.BottomRight);
				}
				else
					_mapListForm.Location = formLocation;

				_mapListForm.FormClosed += ChildFormClosed;

				_mapListForm.Show((IWin32Window)_mainForm);
			}
			else
			{
				_mapListForm.Close();
			}

		}

		public static void ToggleColourMapForm()
		{
			if (_colourMapForm != null)
			{
				_colourMapForm.Close();
				return ;
			}

				_colourMapForm = new ColourList();

				Properties.Settings userSettings = new Properties.Settings();

				Point formLocation = userSettings.ColourListFormLocation;

				if (formLocation.IsEmpty)
				{
					PositionColourMapForm();
				}
				else
				{
					_colourMapForm.Location = formLocation;
				}

				_colourMapForm.FormClosed += ChildFormClosed;
				_colourMapForm.Show((IWin32Window)_mainForm);
	

		}

		private static void PositionColourMapForm()
		{
			// If help form isn't open, just open on bottom left.
			if (_helpForm == null)
			{
				PositionChildForm(_colourMapForm, ChildFormPosition.BottomLeft);
			}
			else
			{
				// If help form is open and hasn't been moved from default position, 
				// position colour list form relative to it.
				if (_helpForm.Location == Properties.Settings.Default.HelpFormLocation)
					PositionChildForm(_helpForm, _colourMapForm, ChildFormPosition.BottomLeft);
				else
				{
					// Help form has been moved. If we put colour map form in the default location, will it overlap with helpform?
					if (WillFormsOverlap(_helpForm.Location, _helpForm.Size,
						GetNewLocation(_mainForm, _colourMapForm, ChildFormPosition.BottomLeft), _colourMapForm.Size))
					{
						PositionChildForm(_helpForm, _colourMapForm, ChildFormPosition.BottomLeft);
					}
					else
					{
						PositionChildForm(_colourMapForm, ChildFormPosition.BottomLeft);
					}

				}
			}



		}

		public static void SizeMainForm()
		{
			_mainForm.Width = (int)(SystemInformation.PrimaryMonitorSize.Width * 0.95F);
		}

		public static void PositionMainForm()
		{
			_mainForm.Location = new Point(
					(int)(SystemInformation.PrimaryMonitorSize.Width * 0.025F),
					(int)(SystemInformation.PrimaryMonitorSize.Height * 0.025F));
		}

		public static void PositionChildForm(Form child, ChildFormPosition position)
		{
			PositionChildForm(_mainForm, child, position);
		}

		public static void PositionChildForm(Form parent, Form child, ChildFormPosition position)
		{
			child.Location = GetNewLocation(parent, child, position);

		}

		private static Point GetNewLocation(Form parent, Form child, ChildFormPosition position)
		{
			Point newLocation = Point.Empty;

			switch (position)
			{
				case ChildFormPosition.BottomLeft:
					newLocation = new Point(parent.Location.X, parent.Location.Y + parent.Height + 1);
					break;
				case ChildFormPosition.BottomCentre:
					break;
				case ChildFormPosition.BottomRight:
					newLocation = new Point(
					parent.Location.X + parent.Width - child.Width,
					parent.Location.Y + parent.Height + 1);
					break;
				case ChildFormPosition.MiddleLeft:
					newLocation = new Point(
						parent.Location.X + 50,
						((parent.ClientSize.Height - child.ClientSize.Height) / 2) + parent.Location.Y);
					break;
				default:
					break;


			}

			return newLocation;

		}


	}

	public enum ChildFormPosition
	{
		BottomLeft, BottomCentre, BottomRight, MiddleLeft
	}
}
