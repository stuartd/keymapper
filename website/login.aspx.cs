using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Configuration;
using System.Collections.Specialized;
using System.Text;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web;
using System.Security.Principal;
using System.Web.Security;


public partial class login : System.Web.UI.Page
{
	protected void Page_Load(object sender, EventArgs e)
	{
		((KMBlogMaster)Page.Master).SetTitle("Admin Login");
	}

	protected void Login1_Authenticate(object sender, AuthenticateEventArgs e)
	{
		int userlevel = AppController.AuthenticateUser(KMLogin.UserName,
			System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(KMLogin.Password, "SHA1"));

		if (userlevel < 1)
		{
			e.Authenticated = false;
			return;
		}

		e.Authenticated = true;

		string role;
		if (userlevel == 1)
			role = "Admin";
		else
			role = "Demo";

		HttpCookie cookie = AppController.CreateAuthenticationTicket(KMLogin.UserName, role);
		Response.Cookies.Add(cookie);
		// Need to redirect now as otherwise our cookie is overwritten
		Response.Redirect(FormsAuthentication.GetRedirectUrl(KMLogin.UserName, true));

	}

}
