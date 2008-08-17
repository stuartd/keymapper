using System.Web.UI;
using System.Web.UI.HtmlControls;


public partial class KMBlogMaster : System.Web.UI.MasterPage
{

 	public void SetTitle(string title)
	{
        RecurseControls(this.Controls, title);
	}

	private void RecurseControls(ControlCollection c, string title)
	{

		foreach (Control con in c)
		{
			if (con.ID == "header")
			{
				HtmlGenericControl header = con as HtmlGenericControl;
				if (header != null)
				{
					header.InnerText = title;
					return;
				}

			}
			RecurseControls(con.Controls, title);
		}
	}
}





