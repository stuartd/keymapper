using System;
using System.Windows.Forms;
using System.Drawing;

namespace KeyMapper.Controls
{
    internal class KMPictureBox : PictureBox
    {
        public void SetImage(Bitmap bmp)
        {
            ReleaseImage();
            Image = bmp;
        }

        internal void ReleaseImage()
        {
            IDisposable cleaner = Image;
            cleaner?.Dispose();
            Image = null;
        }
    }
}
