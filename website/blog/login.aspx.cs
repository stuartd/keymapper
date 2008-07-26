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

namespace KMBlog
{
    public partial class login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Login1_Authenticate(object sender, AuthenticateEventArgs e)
        {
            IDataAccess da = DataAccess.CreateInstance();

            int userlevel = da.GetUserLevel(KMLogin.UserName,
                System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(KMLogin.Password, "SHA1"));

            if (userlevel < 1)
                e.Authenticated = false;
            else
            {
                e.Authenticated = true;
                // Store userlevel in encrypted cookie..

                string role;
                if (userlevel == 1)
                    role = "Admin";
                else
                    role = "Demo";

                HttpCookie cookie = AppController.CreateAuthenticationTicket(KMLogin.UserName, role);
                Response.Cookies.Add(cookie);
                Response.Redirect(FormsAuthentication.GetRedirectUrl(KMLogin.UserName, true)) ;

            }
        }

    }
}