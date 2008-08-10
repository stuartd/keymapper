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
using System.Security.Principal;

public class Authentication
{
    public static int AuthenticateUser(string username, string passwordhash)
    {
        return DataAccess.CreateInstance().GetUserLevel(username, passwordhash);
    }


    public static HttpCookie CreateAuthenticationTicket(string username, string role)
    {
        FormsAuthenticationTicket ticket = new FormsAuthenticationTicket
             (1, username, DateTime.Now, DateTime.Now.AddMinutes(60), false, role);

        string encryptedTicket = FormsAuthentication.Encrypt(ticket);
        HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
        return cookie;
    }

    public static bool IsUserAdmin(IPrincipal user)
    {
        return ((System.Web.Security.FormsIdentity)(user.Identity)).Ticket.UserData == "Admin";
        // return user.IsInRole("Admin");
    }
}
