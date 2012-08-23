using System.Windows.Forms;

namespace KeyMapper.Forms
{
	public partial class AboutForm : KMBaseForm
	{
		public AboutForm()
		{
			InitializeComponent();
			this.lblAppTitle.Text = "KeyMapper " + Application.ProductVersion.ToString();
		}

	}
}
