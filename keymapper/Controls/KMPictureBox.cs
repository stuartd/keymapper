using System;
using System.Windows.Forms;
using System.Drawing;

namespace KeyMapper
{
    class KMPictureBox : PictureBox
    {
        public void SetImage(Bitmap bmp)
        {
            this.ReleaseImage();
            this.Image = bmp;
        }

        internal void ReleaseImage()
        {

            IDisposable cleaner = (IDisposable)(this.Image);
            if (cleaner != null)
            {
                cleaner.Dispose();
            }
            this.Image = null;

        }
    }
}
