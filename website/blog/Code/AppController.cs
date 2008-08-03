using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using System.Security.Principal;


public class AppController
{

    public static int AuthenticateUser(string username, string passwordhash)
    {
        return DataAccess.CreateInstance().GetUserLevel(username, passwordhash);
    }


    public static HttpCookie CreateAuthenticationTicket(string username, string role)
    {
        FormsAuthenticationTicket ticket = new FormsAuthenticationTicket
             (2, username, DateTime.Now, DateTime.Now.AddMinutes(60), false, role);

        string encryptedTicket = FormsAuthentication.Encrypt(ticket);
        HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
        return cookie;
    }

    public static string GetSlug(string seed)
    {
        // Rewrite spaces and full stops as hyphens, strip out everything else non-alpha
        return Regex.Replace(seed, @"[^\w\-]", "-").ToLower();
    }

    public static bool IsUserAdmin(IPrincipal User)
    {
        return User.IsInRole("Admin");
    }

}
