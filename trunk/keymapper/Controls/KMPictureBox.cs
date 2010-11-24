using System;
using System.Windows.Forms;
using System.Drawing;

namespace KeyMapper.Controls
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
            IDisposable cleaner = this.Image;
            if (cleaner != null)
            {
                cleaner.Dispose();
            }
            this.Image = null;
        }
    }
}
