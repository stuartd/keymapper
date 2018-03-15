using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace KeyMapper.Controls
{
public class PanelFader : Control
	{
		public event EventHandler<EventArgs> FadeComplete;

		private Bitmap startimage;
		private Bitmap endimage;
		private int fade;

		private readonly Timer tmr = new Timer();
    
		public PanelFader()
		{

			SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint |
			ControlStyles.DoubleBuffer, true);
			tmr.Tick += TimerFire;

		}

		protected override void OnPaint(PaintEventArgs e)
		{
			if (startimage == null || endimage == null)
			{
			    return;
			}

			e.Graphics.DrawImage(startimage, ClientRectangle, 0, 0, startimage.Width, startimage.
			Height, GraphicsUnit.Pixel);

			var ia = new ImageAttributes();

			var cm = new ColorMatrix
			                     {
			                         Matrix33 = 1.0f/255*fade
			                     };

		    ia.SetColorMatrix(cm);

			e.Graphics.DrawImage(endimage, ClientRectangle, 0, 0, startimage.Width, startimage.
			Height, GraphicsUnit.Pixel, ia);

			base.OnPaint(e);

		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Mobility", "CA1601:DoNotUseTimersThatPreventPowerStateChanges")]
		public void DoFade(Control panel1, Control panel2)
		{
			startimage = new Bitmap(panel1.Width, panel1.Height);
			endimage = new Bitmap(panel2.Width, panel2.Height);

			panel1.DrawToBitmap(startimage, panel1.ClientRectangle);
			panel2.DrawToBitmap(endimage, panel2.ClientRectangle);

			Location = panel1.Location;
			Size = panel1.Size;

			fade = 1;

			tmr.Interval = 1;
			tmr.Enabled = true;

		}

		private void TimerFire(object sender, EventArgs e)
		{
            fade += 20;

			if (fade >= 255)
			{
				fade = 255;
				tmr.Enabled = false;
				FadeComplete(null, null) ;
			}

			Invalidate();
		}

	}

}
