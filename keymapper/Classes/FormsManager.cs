using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using KeyMapper.Classes.Interop;
using KeyMapper.Forms;

namespace KeyMapper.Classes
{
    internal static class FormsManager
    {
        private static KeyboardForm mainForm;
        private static ColourMap colourMapForm;
        private static MappingListForm mappingListForm;
        private static readonly Dictionary<ButtonEffect, ColourEditor> editorForms = new Dictionary<ButtonEffect, ColourEditor>();
        private static HelpForm helpForm;

        public static TaskDialogResult ShowTaskDialog(string text, string instruction, string caption, TaskDialogButtons buttons, TaskDialogIcon icon)
        {
            if (NativeMethods.TaskDialog(IntPtr.Zero, IntPtr.Zero, caption, instruction, text, (int)buttons, new IntPtr((int)icon), out int p) != 0)
            {
                throw new InvalidOperationException("Error occurred calling TaskDialog.");
            }

            switch (p)
            {
                case 1: return TaskDialogResult.Ok;
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
            mainForm = form;
        }

        public static void ChildFormClosed(object sender, FormClosedEventArgs e)
        {
            if (sender is ColourMap)
            {
                colourMapForm = null;
            }
            else if (sender is MappingListForm)
            {
                mappingListForm = null;
            }
            else if (sender is HelpForm)
            {
                helpForm = null;
            }
            else if (sender is ColourEditor)
            {
                var ce = (ColourEditor)sender;
                if (editorForms.ContainsKey(ce.Effect))
                {
                    editorForms.Remove(ce.Effect);
                }
            }
            mainForm.RegenerateMenuExternal();

        }

        public static void ShowColourEditorForm(string message)
        {
            string effectname = message.Substring(0, message.IndexOf(" ", StringComparison.Ordinal));
            string text = message.Substring(message.IndexOf(" ", StringComparison.Ordinal) + 1);
            var effect = (ButtonEffect)Enum.Parse(typeof(ButtonEffect), effectname);

            ColourEditor newForm;

            if (editorForms.ContainsKey(effect))
            {
                if (editorForms.TryGetValue(effect, out newForm))
                {
                    newForm.BringToFront();
                    return;
                }
            }

            newForm = new ColourEditor(effect, text);

            PositionColourEditorForm(newForm);

            editorForms.Add(effect, newForm);
            newForm.FormClosed += ChildFormClosed;
            newForm.Show(mainForm);

        }

        private static Point GetColourEditorFormStartingPosition(ColourEditor ce)
        {
            if (colourMapForm != null)
            {
                // Start from top right of colour map
                return GetNewLocation(colourMapForm, ce, ChildFormPosition.TopRight);
            }

            return GetNewLocation(mainForm, ChildFormPosition.BottomLeft); // Risque as isn't a child, but will work.
        }

        private static void PositionColourEditorForm(ColourEditor ce)
        {
            // Now, where to put it..

            // TODO - Why is this value not being used?
            var formLocation = GetColourEditorFormStartingPosition(ce);

            // If there are no other forms use the last closed position (if there is one)
            // (current form must be new and hasn't been added to collection yet)
            if (editorForms.Count == 0)
            {

                var userSettings = new Properties.Settings();
                var savedLocation = userSettings.ColourEditorLocation;
                if (savedLocation == Point.Empty)
                {
                    // Colour Map form must be open as we must be spawning a new editor (as the count is zero)
                    formLocation = GetNewLocation(colourMapForm, ce, ChildFormPosition.TopRight);
                }
                else
                {
                    formLocation = savedLocation;
                }
            }
            else
            {
                // There are 1-many forms open. They could be all over the screen:
                // pick the bottommost rightmost one and cascade off it
                // (Using top left means having to find the topleftmost form
                // that isn't cascaded, 

                var p = new Point(-5000, -5000);

                foreach (var openform in editorForms.Values)
                {
                    if (openform == null || openform == ce)
                    {
                        continue;
                    }

                    if (openform.Location.X > p.X && openform.Location.Y > p.Y)
                    {
                        p = openform.Location;
                    }
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

            if (mappingListForm != null)
            {
                PositionMappingListForm();
            }

            if (helpForm != null)
            {
                PositionHelpForm(true);
            }

            if (colourMapForm != null)
            {
                PositionColourMapForm();
            }

            CascadeColourEditorForms();
        }

        private static void CascadeColourEditorForms()
        {
            int i = 0;

            var startingPosition = Point.Empty;

            foreach (var ce in editorForms.Values)
            {
                if (ce != null)
                {
                    if (startingPosition.IsEmpty)
                    {
                        startingPosition = GetColourEditorFormStartingPosition(ce);
                    }

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
                var userSettings = new Properties.Settings();
                userSettings.ColourEditorLocation = Point.Empty;
                userSettings.Save();
            }

        }

        public static void CloseAllEditorForms()
        {
            foreach (ButtonEffect effect in Enum.GetValues(typeof(ButtonEffect)))
            {
                if (editorForms.ContainsKey(effect))
                {
                    var ce = editorForms[effect];
                    ce?.Close();
                }
            }
        }


        public static bool IsColourMapFormOpen()
        {
            return (colourMapForm != null);
        }

        public static bool IsMappingListFormOpen()
        {
            return (mappingListForm != null);
        }

        public static void ActivateMainForm()
        {
            // Mapping list form steals focus when DataGridView is refreshed
            // so it needs to call this method.
            mainForm.Activate();
        }

        public static void OpenChildForms()
        {

            var userSettings = new Properties.Settings();

            if (userSettings.ColourMapFormOpen)
            {
                ToggleColourMapForm();
            }

            if (userSettings.MappingListFormOpen)
            {
                ToggleMappingListForm();
            }

            if (userSettings.ShowHelpFormAtStartup)
            {
                ShowHelpForm();
            }
        }

        private static void PositionHelpForm(bool resetting)
        {
            if (resetting)
            {
                if (colourMapForm == null)
                {
                    PositionChildForm(helpForm, ChildFormPosition.BottomLeft);
                    return;
                }
                if (mappingListForm == null)
                {
                    PositionChildForm(helpForm, ChildFormPosition.BottomRight);
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

                PositionChildForm(colourMapForm, helpForm, ChildFormPosition.TopRight);
                return;

            }


            // First run - put help form in front of keyboard so users notice it.
            PositionChildForm(helpForm, ChildFormPosition.MiddleLeft);
        }

        private static void PositionColourMapForm()
        {
            PositionChildForm(colourMapForm, ChildFormPosition.BottomLeft);
        }

        private static void PositionMappingListForm()
        {
            PositionChildForm(mappingListForm, ChildFormPosition.BottomRight);
        }


        public static void ShowHelpForm()
        {
            if (helpForm == null)
            {
                helpForm = new HelpForm();
                var userSettings = new Properties.Settings();
                var formlocation = userSettings.HelpFormLocation;

                if (formlocation.IsEmpty)
                {
                    PositionHelpForm(false);
                }
                else
                {
                    helpForm.Location = formlocation;
                }

                helpForm.Show(mainForm);
            }
        }

        public static void ShowEditMappingForm(KeyMapping km, bool useCapture)
        {
            var mf = new AddEditMapping(km, useCapture);

            var userSettings = new Properties.Settings();

            var savedLocation = userSettings.EditMappingFormLocation;

            if (savedLocation.IsEmpty == false)
            {
                mf.Location = savedLocation;
            }
            else
            {
                PositionChildForm(mf, ChildFormPosition.MiddleLeft);
            }

            mf.ShowDialog(mainForm);

        }

        public static void ShowAboutForm()
        {
            var af = new AboutForm();
            PositionChildForm(af, ChildFormPosition.MiddleCentre);
            af.ShowDialog(mainForm);

        }

        public static void ToggleMappingListForm()
        {

            if (mappingListForm == null)
            {

                var userSettings = new Properties.Settings();

                var formLocation = userSettings.MappingListFormLocation;
                mappingListForm = new MappingListForm();

                // Load settings before positioning so we know how wide form is
                mappingListForm.LoadUserSettings();

                if (formLocation.IsEmpty)
                {
                    PositionMappingListForm();
                }
                else
                {
                    mappingListForm.Location = formLocation;
                }

                mappingListForm.FormClosed += ChildFormClosed;

                mappingListForm.Show(mainForm);
            }
            else
            {
                mappingListForm.Close();
            }

        }

        public static void ToggleColourMapForm()
        {
            if (colourMapForm != null)
            {
                colourMapForm.Close();
                return;
            }

            colourMapForm = new ColourMap();

            var userSettings = new Properties.Settings();

            var formLocation = userSettings.ColourListFormLocation;

            if (formLocation.IsEmpty)
            {
                PositionColourMapForm();
            }
            else
            {
                colourMapForm.Location = formLocation;
            }

            colourMapForm.FormClosed += ChildFormClosed;
            colourMapForm.Show(mainForm);

        }

        public static void SizeMainForm()
        {
            mainForm.Width = (int)(SystemInformation.WorkingArea.Width * 0.95F);
        }

        public static void PositionMainForm()
        {
            mainForm.Location = new Point(
                (int)(SystemInformation.WorkingArea.Width * 0.025F),
                (int)(SystemInformation.WorkingArea.Height * 0.025F));
        }


        private static void PositionChildForm(Form child, ChildFormPosition position)
        {
            PositionChildForm(mainForm, child, position);
        }

        private static void PositionChildForm(Form parent, Form child, ChildFormPosition position)
        {
            child.Location = GetNewLocation(parent, child, position);

        }

        private static Point GetNewLocation(Form child, ChildFormPosition position)
        {
            return GetNewLocation(mainForm, child, position);
        }

        private static Point GetNewLocation(Form parent, Form child, ChildFormPosition position)
        {
            var newLocation = Point.Empty;

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
