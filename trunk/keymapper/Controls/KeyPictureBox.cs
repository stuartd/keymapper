using System;
using System.Windows.Forms;
using System.Drawing;
using KeyMapper.Classes;
using KeyMapper.Classes.Interop;

namespace KeyMapper.Controls
{
	class KeyPictureBox : KMPictureBox
	{
        IntPtr _hicon;
		Cursor _dragcursor;
	    readonly float _dragIconScale;
		bool _outsideForm;
	    readonly bool _mapped;
	    readonly BlankButton _button;
	    readonly int _horizontalStretch;
	    readonly int _verticalStretch;
	    readonly float _scale;
		Rectangle _dragbox;

		bool _escapePressed;

		// These are always the physical values not any mapped ones.
	    readonly int _scancode;
	    readonly int _extended;

	    public KeyMapping Map { get; private set; }

	    public KeyPictureBox(int scancode, int extended, BlankButton button, float scale, int horizontalStretch, int verticalStretch)
		{
			_scancode = scancode;
			_extended = extended;
			_button = button;
			_scale = scale;
			_horizontalStretch = horizontalStretch;
			_verticalStretch = verticalStretch;
			_dragIconScale = 0.75F;
			_dragbox = Rectangle.Empty;
			
			Map = MappingsManager.GetKeyMapping(_scancode, _extended);

			_mapped = (Map.To.Scancode != -1);

			this.AllowDrop = true;

			// Box controls itself.
			this.DragOver += KeyPictureBoxDragOver;
			this.DragDrop += KeyPictureBoxDragDrop;
			this.DragLeave += KeyPictureBoxDragLeave;
			this.GiveFeedback += KeyPictureBoxGiveFeedback;
			this.MouseDown += KeyPictureBoxMouseDown;
			this.MouseMove += KeyPictureBoxMouseMove;
			this.MouseUp += KeyPictureBoxMouseUp;
			this.QueryContinueDrag += KeyPictureBoxQueryContinueDrag;

			DrawKey();
			this.Width = this.Image.Width;
			this.Height = this.Image.Height;
        }

		private void DrawKey()
		{
		    int scancode = _scancode;
			int extended = _extended;

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
						effect = ButtonEffect.MappedPending;
					else
						effect = ButtonEffect.Mapped;
					// Either way, we want the button to show what it is (will be) mapped to:
					scancode = Map.To.Scancode;
					extended = Map.To.Extended;

				}
			}
			else
			{
				// Not mapped now, but was this _key_ mapped before under the current filter??
				KeyMapping km = MappingsManager.GetClearedMapping(_scancode, _extended);
				if (MappingsManager.IsEmptyMapping(km))
				{
					effect = ButtonEffect.None;
				}
				else if (MappingsManager.IsDisabledMapping(km))
					effect = ButtonEffect.EnabledPending;
				else
					effect = ButtonEffect.UnmappedPending;


			}


			Bitmap keybmp = ButtonImages.GetButtonImage(
			    scancode, extended, _button, _horizontalStretch, _verticalStretch, _scale, effect);

			this.SetImage(keybmp);

		}


		void KeyPictureBoxQueryContinueDrag(object sender, QueryContinueDragEventArgs e)
		{

			//  e.Action = DragAction.Continue;

			bool wasOutsideAlready = _outsideForm;

			IsControlOutsideForm(sender);

			if (wasOutsideAlready && !_outsideForm)
			{
				// Have reentered form
				SetDragCursor(
					ButtonImages.GetButtonImage(
						_scancode, _extended, _button, _horizontalStretch, _verticalStretch, _scale, ButtonEffect.None));
			}

			if (_outsideForm)
			{
				if (_mapped)
				{
					// Change icon to be original.
					SetDragCursor(
						ButtonImages.GetButtonImage(
							_scancode, _extended, _button, _horizontalStretch, _verticalStretch, _scale, ButtonEffect.None));
				}
				else
				{
					// Show disabled
					SetDragCursor(
						ButtonImages.GetButtonImage(
							_scancode, _extended, _button, _horizontalStretch, _verticalStretch, _scale, ButtonEffect.Disabled));
				}
			}

			if (e.EscapePressed)
			{
				e.Action = DragAction.Cancel;
				_escapePressed = true;
			}
			else
				_escapePressed = false;
		}

		void SetDragCursor(Bitmap bmp)
		{
			ReleaseIconResources();
			bmp = ButtonImages.ResizeBitmap(bmp, _dragIconScale, false);
			_hicon = bmp.GetHicon();
			_dragcursor = new Cursor(_hicon);
			bmp.Dispose();
		}

		void ReleaseIconResources()
		{
			if (_hicon != IntPtr.Zero)
			{
				if (_dragcursor != null)
				{
					_dragcursor.Dispose();
					_dragcursor = null;
				}
				NativeMethods.DestroyIcon(_hicon);
			}
		}

		void KeyPictureBoxMouseDown(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{

					// Create a dragbox so we can tell if the mouse moves far enough while down to trigger a drag event
					Size dragSize = SystemInformation.DragSize;
					_dragbox = new Rectangle(new Point(e.X - (dragSize.Width / 2), e.Y - (dragSize.Height / 2)), dragSize);
			}
		}

		// This only fires when no drag operation commences.
		void KeyPictureBoxMouseUp(object sender, MouseEventArgs e)
		{
			_dragbox = Rectangle.Empty;
		}

		void KeyPictureBoxMouseMove(object sender, MouseEventArgs e)
		{

			// If user can't write to HKLM and this is W2K then everything is readonly
			// So don't let drag start!
			if (AppController.UserCannotWriteMappings)
				return;
			
			if (_dragbox == Rectangle.Empty || _dragbox.Contains(e.X, e.Y) == false)
				return;

			_dragbox = Rectangle.Empty;

			// Draw self to bitmap, then convert to an icon via a handle
			// both of shich which we _must release_

			Bitmap bmp = new Bitmap(this.Width, this.Height);
			this.DrawToBitmap(bmp, new Rectangle(0, 0, this.Size.Width, this.Size.Height));

			SetDragCursor(bmp);

			DragDropEffects de = this.DoDragDrop(Map, DragDropEffects.Copy);

			if (_escapePressed == false)
			{
				if (_outsideForm)
				{
					// Outside drag.
					if (_mapped)
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
			this.ReleaseIconResources();
		}

	    private void DeleteCurrentMapping()
		{
			MappingsManager.DeleteMapping(Map);
		}

	    private void DisableKey()
		{
			MappingsManager.AddMapping(new KeyMapping(Map.From, new Key(0, 0)));
		}

		void KeyPictureBoxGiveFeedback(object sender, GiveFeedbackEventArgs e)
		{

			//e.UseDefaultCursors = false;
			//Cursor.Current = _cur;

			IsControlOutsideForm(sender);

			// Console.WriteLine("Effect: {0} OutsideForm: {1}", e.Effect, _outsideForm);

			if (e.Effect == DragDropEffects.None && !_outsideForm)
			{
				e.UseDefaultCursors = true;
			}
			else
			{
				e.UseDefaultCursors = false;
				Cursor.Current = _dragcursor;
			}

		}

		void IsControlOutsideForm(object originator)
		{
			Control ctrl = originator as Control;
            if (ctrl != null)
            {
                Form frm = ctrl.FindForm();
                if (frm != null)
                {
                    Point loc = SystemInformation.WorkingArea.Location;

                    _outsideForm =
                        ((MousePosition.X - loc.X) < frm.DesktopBounds.Left) ||
                        ((MousePosition.X - loc.X) > frm.DesktopBounds.Right) ||
                        ((MousePosition.Y - loc.Y) < frm.DesktopBounds.Top) ||
                        ((MousePosition.Y - loc.Y) > frm.DesktopBounds.Bottom);

                }
            }
		}

		void KeyPictureBoxDragLeave(object sender, EventArgs e)
		{
			this.DrawKey();
		}

		void KeyPictureBoxDragDrop(object sender, DragEventArgs e)
		{

			if (e.Data.GetDataPresent("KeyMapper.KeyMapping"))
			{
				KeyMapping dragged_map = (KeyMapping)e.Data.GetData("KeyMapper.KeyMapping");

				if (MappingsManager.AddMapping(new KeyMapping(this.Map.From, dragged_map.From)) == false)
				{
					// Mapping failed. Need to revert our appearance..
					this.DrawKey();
				}
			}
		}

		void KeyPictureBoxDragOver(object sender, DragEventArgs e)
		{

			if (e.Data.GetDataPresent("KeyMapper.KeyMapping") == false)
			{
				e.Effect = DragDropEffects.None;
				return;
			}

			KeyMapping dragged_map = (KeyMapping)e.Data.GetData("KeyMapper.KeyMapping");

			if (dragged_map.To.Scancode >= 0)
			{
				// Can't drop a mapped key onto another key
				e.Effect = DragDropEffects.None;
				return;
			}

			if (dragged_map.From == Map.From)
				return; // No need to redraw self

			// Console.WriteLine("Dragover: " + _scancode)

			this.SetImage(ButtonImages.GetButtonImage
				(dragged_map.From.Scancode, dragged_map.From.Extended,
				_button, _horizontalStretch, _verticalStretch, _scale, ButtonEffect.MappedPending));

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
