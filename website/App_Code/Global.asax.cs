using System;
using System.Web;
using System.Web.Security;
using System.Security.Principal;
using System.Text.RegularExpressions;
using System.Diagnostics;

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

            // Regex pattern = new Regex(@"(.+)\/");
            // Match matches = pattern.Match(HttpContext.Current.Request.Path);
            
            // Much as I love (haha) regular expressions, "Now you have two problems!"
            // http://regex.info/blog/2006-09-15/247
           
            // If the path doesn't end in a slash, ignore it:

            string path = HttpContext.Current.Request.Path;
            if (path.EndsWith("/") == false)
                return;

            // Get everything between the second-last and the last slash. This is the slug.

            string slug = path.Substring(path.Substring(0, path.Length - 1).LastIndexOf("/"));

            if (!string.IsNullOrEmpty(slug))
            {
                int postId = DataAccess.CreateInstance().GetPostIdFromSlug(slug.Replace("/", ""));
                if (postId > 0)
                {
                    string newpath = path.Replace(slug, "/default.aspx?p=" + postId.ToString());
                    HttpContext.Current.RewritePath(newpath);
                }
            }
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
            Trace.Write("Role: " + role);

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