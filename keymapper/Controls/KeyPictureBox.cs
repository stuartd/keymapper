using System;
using System.Windows.Forms;
using System.Drawing;
using KeyMapper.Classes;
using KeyMapper.Classes.Interop;

namespace KeyMapper.Controls
{
    internal sealed class KeyPictureBox : KMPictureBox
    {
        private IntPtr iconHandle;
        private Cursor dragCursor;
        private readonly float dragIconScale;
        private bool outsideForm;
        private readonly bool mapped;
        private readonly BlankButton button;
        private readonly int horizontalStretch;
        private readonly int verticalStretch;
        private readonly float scale;
        private Rectangle dragBox;

        private bool escapePressed;

        // These are always the physical values not any mapped ones.
        private readonly int scanCode;
        private readonly int extended;

        public KeyMapping Map { get; }

        public KeyPictureBox(int scanCode, int extended, BlankButton button, float scale, int horizontalStretch, int verticalStretch)
        {
            this.scanCode = scanCode;
            this.extended = extended;
            this.button = button;
            this.scale = scale;
            this.horizontalStretch = horizontalStretch;
            this.verticalStretch = verticalStretch;
            dragIconScale = 0.75F;
            dragBox = Rectangle.Empty;

            Map = MappingsManager.GetKeyMapping(scanCode, extended);

            mapped = Map.To.ScanCode != -1;

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
            int scanCode = this.scanCode;
            int extended = this.extended;

            ButtonEffect effect;

            if (MappingsManager.IsEmptyMapping(Map) == false)
            {
                //  Remapped or disabled?
                if (MappingsManager.IsDisabledMapping(Map))
                {
                    // Disabled
                    if (MappingsManager.IsMappingPending(Map))
                    {
                        effect = ButtonEffect.DisabledPending;
                    }
                    else
                    {
                        effect = ButtonEffect.Disabled;
                    }
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
                    scanCode = Map.To.ScanCode;
                    extended = Map.To.Extended;
                }
            }
            else
            {
                // Not mapped now, but was this key mapped before under the current filter??
                var km = MappingsManager.GetClearedMapping(scanCode, extended);
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

            var buttonImage = ButtonImages.GetButtonImage(
                scanCode, extended, button, horizontalStretch, verticalStretch, scale, effect);

            SetImage(buttonImage);
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
                        scanCode, extended, button, horizontalStretch, verticalStretch, scale, ButtonEffect.None));
            }

            if (outsideForm)
            {
                if (mapped)
                {
                    // Change icon to be original.
                    SetDragCursor(
                        ButtonImages.GetButtonImage(
                            scanCode, extended, button, horizontalStretch, verticalStretch, scale, ButtonEffect.None));
                }
                else
                {
                    // Show disabled
                    SetDragCursor(
                        ButtonImages.GetButtonImage(
                            scanCode, extended, button, horizontalStretch, verticalStretch, scale, ButtonEffect.Disabled));
                }
            }

            if (e.EscapePressed)
            {
                e.Action = DragAction.Cancel;
                escapePressed = true;
            }
            else
            {
                escapePressed = false;
            }
        }

        private void SetDragCursor(Bitmap bmp)
        {
            ReleaseIconResources();
            bmp = ButtonImages.ResizeBitmap(bmp, dragIconScale, false);
            iconHandle = bmp.GetHicon();
            dragCursor = new Cursor(iconHandle);
            bmp.Dispose();
        }

        private void ReleaseIconResources()
        {
            if (iconHandle != IntPtr.Zero)
            {
                if (dragCursor != null)
                {
                    dragCursor.Dispose();
                    dragCursor = null;
                }
                NativeMethods.DestroyIcon(iconHandle);
            }
        }

        private void KeyPictureBoxMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {

                // Create a dragbox so we can tell if the mouse moves far enough while down to trigger a drag event
                var dragSize = SystemInformation.DragSize;
                dragBox = new Rectangle(new Point(e.X - dragSize.Width / 2, e.Y - dragSize.Height / 2), dragSize);
            }
        }

        // This only fires when no drag operation commences.
        private void KeyPictureBoxMouseUp(object sender, MouseEventArgs e)
        {
            dragBox = Rectangle.Empty;
        }

        private void KeyPictureBoxMouseMove(object sender, MouseEventArgs e)
        {
            if (dragBox == Rectangle.Empty || dragBox.Contains(e.X, e.Y) == false)
            {
                return;
            }

            dragBox = Rectangle.Empty;

            // Draw self to bitmap, then convert to an icon via a handle
            // both of shich which we must release

            var bmp = new Bitmap(Width, Height);
            DrawToBitmap(bmp, new Rectangle(0, 0, Size.Width, Size.Height));

            SetDragCursor(bmp);

            var de = DoDragDrop(Map, DragDropEffects.Copy);

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
                Cursor.Current = dragCursor;
            }

        }

        private void IsControlOutsideForm(object originator)
        {
            if (originator is Control ctrl)
            {
                var frm = ctrl.FindForm();
                if (frm != null)
                {
                    var loc = SystemInformation.WorkingArea.Location;

                    outsideForm =
                        MousePosition.X - loc.X < frm.DesktopBounds.Left ||
                        MousePosition.X - loc.X > frm.DesktopBounds.Right ||
                        MousePosition.Y - loc.Y < frm.DesktopBounds.Top ||
                        MousePosition.Y - loc.Y > frm.DesktopBounds.Bottom;

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
                var draggedMap = (KeyMapping)e.Data.GetData("KeyMapper.KeyMapping");

                if (MappingsManager.AddMapping(new KeyMapping(Map.From, draggedMap.From)) == false)
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

            var draggedMap = (KeyMapping)e.Data.GetData("KeyMapper.KeyMapping");

            if (draggedMap.To.ScanCode >= 0)
            {
                // Can't drop a mapped key onto another key
                e.Effect = DragDropEffects.None;
                return;
            }

            if (draggedMap.From == Map.From)
            {
                return; // No need to redraw self
            }

            // Console.WriteLine("Dragover: " + scanCode)

            SetImage(ButtonImages.GetButtonImage
                (draggedMap.From.ScanCode, draggedMap.From.Extended,
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
