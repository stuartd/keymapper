using System.Windows.Forms;
using System.Security.Permissions;
using KeyMapper.Classes;

namespace KeyMapper.Controls
{
	public class KeyboardListCombo : ComboBox
	{
		// Have to subclass this control just to suppress mouse wheel events.
		
		// Oh well, there's other things we can do as well, like setting up the OwnerDraw stuff.

		public KeyboardListCombo()
		{
			this.DrawMode = DrawMode.OwnerDrawVariable;
			this.MeasureItem += ComboItemSeparator.MeasureComboItem;
			this.DrawItem += ComboItemSeparator.DrawComboItem;
		}

        [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
        protected override void WndProc(ref Message m)
        {
if (m.HWnd != Handle)
            {
                return;
            }

            if (m.Msg == 0x020A) // WM_MOUSEWHEEL  
            {
                if (MouseIsOutsideControl())
                   return;
            }
			
			base.WndProc(ref m);
        }

		private bool MouseIsOutsideControl()
		{
			// Where is Mouse right now?
			System.Drawing.Point localPosition = this.PointToClient(MousePosition);

			if (localPosition.X < 0 || localPosition.Y < 0)
			{
				return true; // Mouse is to the left or above the control
			}

			if (localPosition.X > this.Width || localPosition.Y > this.Height + (this.DroppedDown ? this.DropDownHeight : 0))
			{
				return true; // Mouse is not over the combo or it's dropdown (if shown)
			}
                       
			return false; 
		}
	}
}
