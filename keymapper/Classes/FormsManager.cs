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
        static ColourMap _colourMapForm;
        static MappingListForm _mapListForm;
        static Dictionary<ButtonEffect, ColourEditor> _editorForms = new Dictionary<ButtonEffect, ColourEditor>();
        static HelpForm _helpForm;

        public static TaskDialogResult ShowTaskDialog(string text, string instruction, string caption, TaskDialogButtons buttons, TaskDialogIcon icon)
        {
            int p;
            if (NativeMethods.TaskDialog(IntPtr.Zero, IntPtr.Zero, caption, instruction, text, (int)buttons, new IntPtr((int)icon), out p) != 0)
                throw new InvalidOperationException("Error occurred calling TaskDialog.");

            switch (p)
            {
                case 1: return TaskDialogResult.OK;
                case 2: return TaskDialogResult.Cancel;
                case 4: return TaskDialogResult.Retry;
                case 6: return TaskDialogResult.Yes;
                case 7: return TaskDialogResult.No;
                case 8: return TaskDialogResult.Close;
                default: return TaskDialogResult.None;
            }
        }

        public static void RegisterMainForm(KeyboardForm form)
        {
            _mainForm = form;
        }

        public static void ChildFormClosed(object sender, FormClosedEventArgs e)
        {
            if (sender is ColourMap)
                _colourMapForm = null;
            else if (sender is MappingListForm)
                _mapListForm = null;
            else if (sender is HelpForm)
                _helpForm = null;
            else if (sender is ColourEditor)
            {
                ColourEditor ce = sender as ColourEditor;
                if (_editorForms.ContainsKey(ce.Effect))
                    _editorForms.Remove(ce.Effect);
            }
            _mainForm.RegenerateMenuExternal();

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

            PositionColourEditorForm(newForm);

            _editorForms.Add(effect, newForm);
            newForm.FormClosed += ChildFormClosed;
            newForm.Show((IWin32Window)_mainForm);

        }

        private static Point GetColourEditorFormStartingPosition(ColourEditor ce)
        {
            if (_colourMapForm != null) // Start from top right of colour map
                return GetNewLocation(_colourMapForm, ce, ChildFormPosition.TopRight);
            else // Start from bottom left of main form.
                return GetNewLocation(_mainForm, ChildFormPosition.BottomLeft);  // Risque as isn't a child, but will work.

        }

        private static void PositionColourEditorForm(ColourEditor ce)
        {

            // Now, where to put it..
            Point formLocation = GetColourEditorFormStartingPosition(ce);

            // If there are no other forms use the last closed position (if there is one)
            // (current form must be new and hasn't been added to collection yet)
            if (_editorForms.Count == 0)
            {

                Properties.Settings userSettings = new Properties.Settings();
                Point savedLocation = userSettings.ColourEditorLocation;
                if (savedLocation == Point.Empty)
                {
                    // Colour Map form must be open as we must be spawning a new editor (as the count is zero)
                    formLocation = GetNewLocation(_colourMapForm, ce, ChildFormPosition.TopRight);
                }
                else
                {
                    formLocation = savedLocation;
                }
            }
            else
            {
                Point p;

                // There are 1-many forms open. They could be all over the screen:
                // pick the bottommost rightmost one and cascade off it
                // (Using top left means having to find the topleftmost form
                // that isn't cascaded, 

                p = new Point(-5000, -5000);

                foreach (ColourEditor openform in _editorForms.Values)
                {
                    if (openform == null || openform == ce)
                        continue;

                    if (openform.Location.X > p.X && openform.Location.Y > p.Y)
                        p = openform.Location;
                }

                formLocation = new Point(p.X + 10, p.Y + SystemInformation.CaptionHeight + 5);


            }

            ce.Location = formLocation;

        }

        public static void ArrangeAllOpenForms()
        {

            // PositionMainForm();
            // SizeMainForm();
            // _mainForm.KeyboardFormResizeEnd(null, null);

            if (_mapListForm != null)
                PositionMappingListForm();

            if (_helpForm != null)
                PositionHelpForm(true);

            if (_colourMapForm != null)
                PositionColourMapForm();

            CascadeColourEditorForms();
        }

        private static void CascadeColourEditorForms()
        {
            int i = 0;

            Point startingPosition = Point.Empty;

            foreach (ColourEditor ce in _editorForms.Values)
            {
                if (ce != null)
                {
                    if (startingPosition.IsEmpty)
                        startingPosition = GetColourEditorFormStartingPosition(ce);
                    i++;
                    int offset = SystemInformation.CaptionHeight * (i - 1);
                    ce.Location = new Point(startingPosition.X + offset, startingPosition.Y + offset);
                    ce.BringToFront();
                    // This means bringing all to the front in turn, so they display in correct order.
                    // (or, at least, with the latest one on top)
                }
            }

            if (i == 0)
            {
                // There were no open forms: reset the default position instead.
                Properties.Settings userSettings = new KeyMapper.Properties.Settings();
                userSettings.ColourEditorLocation = Point.Empty;
                userSettings.Save();
            }

        }

        public static void CloseAllEditorForms()
        {
            foreach (ButtonEffect effect in Enum.GetValues(typeof(ButtonEffect)))
            {
                if (_editorForms.ContainsKey(effect))
                {
                    ColourEditor ce = _editorForms[effect];
                    if (ce != null)
                    {
                        ce.Close();
                    }
                }
            }
        }


        public static bool IsColourMapFormOpen()
        {
            return (_colourMapForm != null);
        }

        public static bool IsMappingListFormOpen()
        {
            return (_mapListForm != null);
        }

        public static void ActivateMainForm()
        {
            // Mapping list form steals focus when DataGridView is refreshed
            // so it needs to call this method.
            _mainForm.Activate();
        }

        public static void OpenChildForms()
        {

            Properties.Settings userSettings = new Properties.Settings();

            if (userSettings.ColourMapFormOpen)
                ToggleColourMapForm();

            if (userSettings.MappingListFormOpen)
                ToggleMappingListForm();

            if (userSettings.ShowHelpFormAtStartup)
            {
                ShowHelpForm();
            }
        }

        private static void PositionHelpForm(bool resetting)
        {
            if (resetting)
            {
                if (_colourMapForm == null)
                {
                    PositionChildForm(_helpForm, ChildFormPosition.BottomLeft);
                    return;
                }
                if (_mapListForm == null)
                {
                    PositionChildForm(_helpForm, ChildFormPosition.BottomRight);
                    return;
                }

                // Right, both other subforms are open (never mind editing forms for now)

                // Faced with a hard choice:
                // 1) Dump the help screen in front of the main form. Bad solution for a reset, covering up the main form..
                // 2) Start testing whether it will fit above the main form or below the colourmap form
                // without going outside the screen bounds. Perfectly feasible, but there's no reason to expect that 
                // there will be enough space at a low resolution.
                // 3) Dump it bottom-centre and accept the fact it probably overlays the other forms.
                // 4) A combination of 2) and 3)
                // 4a) Put it next to the colour map form, accept it may overlap the mapping list form.

                PositionChildForm(_colourMapForm, _helpForm, ChildFormPosition.TopRight);
                return;

            }


            // First run - put help form in front of keyboard so users notice it.
            PositionChildForm(_helpForm, ChildFormPosition.MiddleLeft);
        }

        private static void PositionColourMapForm()
        {
            PositionChildForm(_colourMapForm, ChildFormPosition.BottomLeft);
        }

        private static void PositionMappingListForm()
        {
            PositionChildForm(_mapListForm, ChildFormPosition.BottomRight);
        }


        public static void ShowHelpForm()
        {
            if (_helpForm == null)
            {
                _helpForm = new HelpForm();
                Properties.Settings userSettings = new Properties.Settings();
                Point formlocation = userSettings.HelpFormLocation;

                if (formlocation.IsEmpty)
                    PositionHelpForm(false);
                else
                    _helpForm.Location = formlocation;

                _helpForm.Show((IWin32Window)_mainForm);
            }
        }

        public static void ShowEditMappingForm(KeyMapping km, bool useCapture)
        {
            AddEditMapping mf = new AddEditMapping(km, useCapture);

            Properties.Settings userSettings = new Properties.Settings();

            Point savedLocation = userSettings.EditMappingFormLocation;

            if (savedLocation.IsEmpty == false)
                mf.Location = savedLocation;
            else
                PositionChildForm(mf, ChildFormPosition.MiddleLeft);

            mf.ShowDialog((IWin32Window)_mainForm);

        }

        public static void ShowAboutForm()
        {
            AboutForm af = new AboutForm();
            PositionChildForm(af, ChildFormPosition.MiddleCentre);
            af.ShowDialog((IWin32Window)_mainForm);

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
                    PositionMappingListForm();
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
                return;
            }

            _colourMapForm = new ColourMap();

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

        public static void SizeMainForm()
        {
            _mainForm.Width = (int)(SystemInformation.WorkingArea.Width * 0.95F);
        }

        public static void PositionMainForm()
        {
            _mainForm.Location = new Point(
                    (int)(SystemInformation.WorkingArea.Width * 0.025F),
                    (int)(SystemInformation.WorkingArea.Height * 0.025F));
        }


        private static void PositionChildForm(Form child, ChildFormPosition position)
        {
            PositionChildForm(_mainForm, child, position);
        }

        private static void PositionChildForm(Form parent, Form child, ChildFormPosition position)
        {
            child.Location = GetNewLocation(parent, child, position);

        }

        private static Point GetNewLocation(Form child, ChildFormPosition position)
        {
            return GetNewLocation(_mainForm, child, position);
        }

        private static Point GetNewLocation(Form parent, Form child, ChildFormPosition position)
        {
            Point newLocation = Point.Empty;

            switch (position)
            {
                case ChildFormPosition.TopLeft:
                    newLocation = new Point(parent.Location.X, parent.Location.Y - child.Size.Height);
                    break;
                case ChildFormPosition.TopRight:
                    newLocation = new Point(parent.Location.X + parent.Size.Width, parent.Location.Y);
                    break;
                case ChildFormPosition.BottomLeft:
                    newLocation = new Point(parent.Location.X, parent.Location.Y + parent.Height + 1);
                    break;
                case ChildFormPosition.BottomCentre:
                    newLocation = new Point(
                    parent.Location.X + ((parent.Size.Width - child.Size.Width) / 2),
                    parent.Location.Y + parent.Size.Height);
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
                case ChildFormPosition.MiddleCentre:
                    newLocation = new Point(
                        ((parent.ClientSize.Width - child.ClientSize.Width) / 2) + parent.Location.X,
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
        TopLeft, TopRight, BottomLeft, BottomCentre, BottomRight, MiddleLeft, MiddleCentre
    }
}
