using System;
using System.Web;
using System.Web.Security;
using System.Security.Principal;

namespace KMBlog
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {

        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
            HttpCookie cookie = Context.Request.Cookies[FormsAuthentication.FormsCookieName];

            if (cookie == null)
                return;

            FormsAuthenticationTicket ticket = null;
            try
            {
                ticket = FormsAuthentication.Decrypt(cookie.Value);
            }
            catch (Exception)
            {
                return;
            }


            if (ticket == null)
                return;

            string role = ticket.UserData;

            FormsIdentity id = new FormsIdentity(ticket);

            GenericPrincipal principal = new GenericPrincipal(id, new string[] { role });
            Context.User = principal;


        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}