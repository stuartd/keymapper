using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace KeyMapper
{
    public class KMBaseForm : Form
    {
       public KMBaseForm() 
        {
			SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.DoubleBuffer, true);
            Font = new System.Drawing.Font("Verdana", 8.25F);
			StartPosition = FormStartPosition.Manual;
        }
	
    }
}
