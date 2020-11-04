using System.Windows.Forms;

namespace KeyMapper.Forms
{
    public class KMBaseForm : Form
    {
        public KMBaseForm()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.DoubleBuffer, true);
            // ReSharper disable once VirtualMemberCallInConstructor
            Font = new System.Drawing.Font("Verdana", 8.25F);
            StartPosition = FormStartPosition.Manual;
        }
    }
}
