using System;
using System.Windows.Forms;
using System.Drawing;
using KeyMapper.Classes;
using KeyMapper.Classes.Interop;

namespace KeyMapper.Controls
{
    internal class KeyPictureBox : KMPictureBox
    {
        private IntPtr hicon;
        private Cursor dragcursor;
        private readonly float dragIconScale;
        private bool outsideForm;
        private readonly bool mapped;
        private readonly BlankButton button;
        private readonly int horizontalStretch;
        private readonly int verticalStretch;
        private readonly float scale;
        private Rectangle dragbox;

        private bool escapePressed;

        // These are always the physical values not any mapped ones.
        private readonly int scancode;
        private readonly int extended;

        public KeyMapping Map { get; }

        public KeyPictureBox(int scancode, int extended, BlankButton button, float scale, int horizontalStretch, int verticalStretch)
        {
            this.scancode = scancode;
            this.extended = extended;
            this.button = button;
            this.scale = scale;
            this.horizontalStretch = horizontalStretch;
            this.verticalStretch = verticalStretch;
            this.dragIconScale = 0.75F;
            this.dragbox = Rectangle.Empty;

            Map = MappingsManager.GetKeyMapping(scancode, extended);

            this.mapped = (Map.To.Scancode != -1);

            AllowDrop = true;

            // Box controls itself.
            DragOver += KeyPictureBoxDragOver;
            DragDrop += KeyPictureBoxDragDrop;
            DragLeave += KeyPictureBoxDragLeave;
            GiveFeedback += KeyPictureBoxGiveFeedback;
            MouseDown += KeyPictureBoxMouseDown;
            MouseMove += KeyPictureBoxMouseMove;
            MouseUp += KeyPictureBoxMouseUp;
            QueryContinueDrag += KeyPictureBoxQueryContinueDrag;

            DrawKey();
            Width = Image.Width;
            Height = Image.Height;
        }

        private void DrawKey()
        {
            int scancode = this.scancode;
            int extended = this.extended;

            ButtonEffect effect;

            if (MappingsManager.IsEmptyMapping(Map) == false)
            {
                //  Remapped or disabled?
                if (MappingsManager.IsDisabledMapping(Map))
                {
                    // Disabled
                    if (MappingsManager.IsMappingPending(Map))
                        effect = ButtonEffect.DisabledPending;
                    else
                        effect = ButtonEffect.Disabled;
                }
                else
                {
                    // Is this key mapped under the current filter?
                    if (MappingsManager.IsMappingPending(Map))
                    {
                        effect = ButtonEffect.MappedPending;
                    }
                    else
                    {
                        effect = ButtonEffect.Mapped;
                    }

                    // Either way, we want the button to show what it is (will be) mapped to:
                    scancode = Map.To.Scancode;
                    extended = Map.To.Extended;
                }
            }
            else
            {
                // Not mapped now, but was this key mapped before under the current filter??
                KeyMapping km = MappingsManager.GetClearedMapping(scancode, extended);
                if (MappingsManager.IsEmptyMapping(km))
                {
                    effect = ButtonEffect.None;
                }
                else if (MappingsManager.IsDisabledMapping(km))
                {
                    effect = ButtonEffect.EnabledPending;
                }
                else
                {
                    effect = ButtonEffect.UnmappedPending;
                }
            }

            Bitmap keybmp = ButtonImages.GetButtonImage(
                scancode, extended, button, horizontalStretch, verticalStretch, scale, effect);

            SetImage(keybmp);
        }


        private void KeyPictureBoxQueryContinueDrag(object sender, QueryContinueDragEventArgs e)
        {

            //  e.Action = DragAction.Continue;

            bool wasOutsideAlready = outsideForm;

            IsControlOutsideForm(sender);

            if (wasOutsideAlready && !outsideForm)
            {
                // Have reentered form
                SetDragCursor(
                    ButtonImages.GetButtonImage(
                        scancode, extended, button, horizontalStretch, verticalStretch, scale, ButtonEffect.None));
            }

            if (outsideForm)
            {
                if (mapped)
                {
                    // Change icon to be original.
                    SetDragCursor(
                        ButtonImages.GetButtonImage(
                            scancode, extended, button, horizontalStretch, verticalStretch, scale, ButtonEffect.None));
                }
                else
                {
                    // Show disabled
                    SetDragCursor(
                        ButtonImages.GetButtonImage(
                            scancode, extended, button, horizontalStretch, verticalStretch, scale, ButtonEffect.Disabled));
                }
            }

            if (e.EscapePressed)
            {
                e.Action = DragAction.Cancel;
                escapePressed = true;
            }
            else
                escapePressed = false;
        }

        private void SetDragCursor(Bitmap bmp)
        {
            ReleaseIconResources();
            bmp = ButtonImages.ResizeBitmap(bmp, dragIconScale, false);
            hicon = bmp.GetHicon();
            dragcursor = new Cursor(hicon);
            bmp.Dispose();
        }

        private void ReleaseIconResources()
        {
            if (hicon != IntPtr.Zero)
            {
                if (dragcursor != null)
                {
                    dragcursor.Dispose();
                    dragcursor = null;
                }
                NativeMethods.DestroyIcon(hicon);
            }
        }

        private void KeyPictureBoxMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {

                // Create a dragbox so we can tell if the mouse moves far enough while down to trigger a drag event
                Size dragSize = SystemInformation.DragSize;
                dragbox = new Rectangle(new Point(e.X - (dragSize.Width / 2), e.Y - (dragSize.Height / 2)), dragSize);
            }
        }

        // This only fires when no drag operation commences.
        private void KeyPictureBoxMouseUp(object sender, MouseEventArgs e)
        {
            dragbox = Rectangle.Empty;
        }

        private void KeyPictureBoxMouseMove(object sender, MouseEventArgs e)
        {
            if (dragbox == Rectangle.Empty || dragbox.Contains(e.X, e.Y) == false)
                return;

            dragbox = Rectangle.Empty;

            // Draw self to bitmap, then convert to an icon via a handle
            // both of shich which we must release

            Bitmap bmp = new Bitmap(Width, Height);
            DrawToBitmap(bmp, new Rectangle(0, 0, Size.Width, Size.Height));

            SetDragCursor(bmp);

            DragDropEffects de = DoDragDrop(Map, DragDropEffects.Copy);

            if (escapePressed == false)
            {
                if (outsideForm)
                {
                    // Outside drag.
                    if (mapped)
                    {
                        DeleteCurrentMapping();
                    }
                    else
                    {
                        DisableKey();
                    }
                }
            }
            // Now we are done. Release icon.
            ReleaseIconResources();
        }

        private void DeleteCurrentMapping()
        {
            MappingsManager.DeleteMapping(Map);
        }

        private void DisableKey()
        {
            MappingsManager.AddMapping(new KeyMapping(Map.From, new Key(0, 0)));
        }

        private void KeyPictureBoxGiveFeedback(object sender, GiveFeedbackEventArgs e)
        {

            //e.UseDefaultCursors = false;
            //Cursor.Current = cur;

            IsControlOutsideForm(sender);

            // Console.WriteLine("Effect: {0} OutsideForm: {1}", e.Effect, outsideForm);

            if (e.Effect == DragDropEffects.None && !outsideForm)
            {
                e.UseDefaultCursors = true;
            }
            else
            {
                e.UseDefaultCursors = false;
                Cursor.Current = dragcursor;
            }

        }

        private void IsControlOutsideForm(object originator)
        {
            Control ctrl = originator as Control;
            if (ctrl != null)
            {
                Form frm = ctrl.FindForm();
                if (frm != null)
                {
                    Point loc = SystemInformation.WorkingArea.Location;

                    outsideForm =
                        ((MousePosition.X - loc.X) < frm.DesktopBounds.Left) ||
                        ((MousePosition.X - loc.X) > frm.DesktopBounds.Right) ||
                        ((MousePosition.Y - loc.Y) < frm.DesktopBounds.Top) ||
                        ((MousePosition.Y - loc.Y) > frm.DesktopBounds.Bottom);

                }
            }
        }

        private void KeyPictureBoxDragLeave(object sender, EventArgs e)
        {
            DrawKey();
        }

        private void KeyPictureBoxDragDrop(object sender, DragEventArgs e)
        {

            if (e.Data.GetDataPresent("KeyMapper.KeyMapping"))
            {
                KeyMapping draggedmap = (KeyMapping)e.Data.GetData("KeyMapper.KeyMapping");

                if (MappingsManager.AddMapping(new KeyMapping(Map.From, draggedmap.From)) == false)
                {
                    // Mapping failed. Need to revert our appearance..
                    DrawKey();
                }
            }
        }

        private void KeyPictureBoxDragOver(object sender, DragEventArgs e)
        {

            if (e.Data.GetDataPresent("KeyMapper.KeyMapping") == false)
            {
                e.Effect = DragDropEffects.None;
                return;
            }

            KeyMapping draggedmap = (KeyMapping)e.Data.GetData("KeyMapper.KeyMapping");

            if (draggedmap.To.Scancode >= 0)
            {
                // Can't drop a mapped key onto another key
                e.Effect = DragDropEffects.None;
                return;
            }

            if (draggedmap.From == Map.From)
                return; // No need to redraw self

            // Console.WriteLine("Dragover: " + scancode)

            SetImage(ButtonImages.GetButtonImage
                (draggedmap.From.Scancode, draggedmap.From.Extended,
                button, horizontalStretch, verticalStretch, scale, ButtonEffect.MappedPending));

            e.Effect = DragDropEffects.Copy;

        }

        // When disposing, make sure that final bitmap is released.
        ~KeyPictureBox()
        {
            ReleaseImage();
            ReleaseIconResources();

        }
    }
}
