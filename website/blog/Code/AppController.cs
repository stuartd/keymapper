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

namespace KMBlog
{
    public class AppController
    {

        public static HttpCookie CreateAuthenticationTicket(string username, string role)
        {
            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket
                 (2, username, DateTime.Now, DateTime.Now.AddMinutes(60), false, role);

             string encryptedTicket = FormsAuthentication.Encrypt(ticket) ;
            HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket) ;
            return cookie;
        }

    }
}
