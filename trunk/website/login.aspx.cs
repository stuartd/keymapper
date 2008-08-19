using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web;
using System.Web.Security;
using KMBlog;


public partial class login : System.Web.UI.Page
{
	protected void Page_Load(object sender, EventArgs e)
	{
		((KMBlogMaster)Page.Master).SetTitle("Admin Login");
	}

	protected void Login1_Authenticate(object sender, AuthenticateEventArgs e)
	{
		int userlevel = KMAuthentication.AuthenticateUser(KMLogin.UserName,
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

		KMAuthentication.CreateAuthenticationTicket(KMLogin.UserName, role);
	
		// Need to redirect now as otherwise our cookie is overwritten
		Response.Redirect(FormsAuthentication.GetRedirectUrl(KMLogin.UserName, true));

	}

}
