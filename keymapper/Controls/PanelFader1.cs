using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace KeyMapper.Controls
{
public class PanelFader : Control
	{
		public event EventHandler<EventArgs> FadeComplete;

        private Bitmap startImage;
        private Bitmap endImage;
        private int fade;

        private readonly Timer timer = new Timer();
    
		public PanelFader()
		{
			SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint |
			ControlStyles.DoubleBuffer, true);
			timer.Tick += TimerFire;

		}

		protected override void OnPaint(PaintEventArgs e)
		{
			if (startImage == null || endImage == null)
			{
			    return;
			}

			e.Graphics.DrawImage(startImage, ClientRectangle, 0, 0, startImage.Width, startImage.
			Height, GraphicsUnit.Pixel);

			var ia = new ImageAttributes();

			var cm = new ColorMatrix
			                     {
			                         Matrix33 = 1.0f/255*fade
			                     };

		    ia.SetColorMatrix(cm);

			e.Graphics.DrawImage(endImage, ClientRectangle, 0, 0, startImage.Width, startImage.
			Height, GraphicsUnit.Pixel, ia);

			base.OnPaint(e);

		}

		public void DoFade(Control panel1, Control panel2)
		{
			startImage = new Bitmap(panel1.Width, panel1.Height);
			endImage = new Bitmap(panel2.Width, panel2.Height);

			panel1.DrawToBitmap(startImage, panel1.ClientRectangle);
			panel2.DrawToBitmap(endImage, panel2.ClientRectangle);

            Location = panel1.Location;
            Size = panel1.Size;

			fade = 1;

            timer.Interval = 1;
            timer.Enabled = true;

		}

		private void TimerFire(object sender, EventArgs e)
		{
            fade += 20;

			if (fade >= 255)
			{
				fade = 255;
				timer.Enabled = false;
				FadeComplete(null, null) ;
			}

			Invalidate();
		}

	}

}
