using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;


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
			if (con.ID == "Header")
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





