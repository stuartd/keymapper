using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace KeyMapper
{

	public class PanelFader : Control
	{

		public event EventHandler<EventArgs> FadeComplete;
		
		Bitmap _startimage;
		Bitmap _endimage;
		int _fade = 0;

		Timer _tmr = new Timer();


		public PanelFader()
		{

			SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint |
			ControlStyles.DoubleBuffer, true);
			_tmr.Tick += new EventHandler(TimerFire);

		}

		protected override void OnPaint(PaintEventArgs e)
		{

			if (e == null || _startimage == null || _endimage == null)
				return;

			e.Graphics.DrawImage(_startimage, this.ClientRectangle, 0, 0, _startimage.Width, _startimage.
			Height, GraphicsUnit.Pixel);

			ImageAttributes ia = new ImageAttributes();

			ColorMatrix cm = new ColorMatrix();

			cm.Matrix33 = 1.0f / 255 * _fade;

			ia.SetColorMatrix(cm);

			e.Graphics.DrawImage(_endimage, this.ClientRectangle, 0, 0, _startimage.Width, _startimage.
			Height, GraphicsUnit.Pixel, ia);

			base.OnPaint(e);

		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Mobility", "CA1601:DoNotUseTimersThatPreventPowerStateChanges")]
		public void DoFade(Control panel1, Control panel2)
		{
			if ((panel1 is Control && panel2 is Control) == false)
				return;

			_startimage = new Bitmap(panel1.Width, panel1.Height);
			_endimage = new Bitmap(panel2.Width, panel2.Height);

			panel1.DrawToBitmap(_startimage, panel1.ClientRectangle);
			panel2.DrawToBitmap(_endimage, panel2.ClientRectangle);

			this.Location = panel1.Location;
			this.Size = panel1.Size;

			_fade = 1;

			this._tmr.Interval = 1;
			this._tmr.Enabled = true;

		}

		private void TimerFire(object sender, EventArgs e)
		{

			_fade += 20;

			if (_fade >= 255)
			{
				_fade = 255;
				_tmr.Enabled = false;
				FadeComplete(null, null) ;
			}

			Invalidate();
		}

	}

}
