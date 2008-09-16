namespace KMBlog
{
    using System;
    using System.Security.Principal;
    using System.Web;
    using System.Web.Security;

    public class KMAuthentication
    {
        private KMAuthentication()
        {
        }

        public static void LogOut()
        {
            CreateAuthenticationTicket(string.Empty, string.Empty);
        }

        public static int AuthenticateUser(string userName, string passwordHash)
        {
            return DataAccess.CreateInstance().GetUserLevel(userName, passwordHash);
        }

        public static void CreateAuthenticationTicket(string userName, string role)
        {
            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1, userName, DateTime.Now, DateTime.Now.AddMinutes(60), false, role);

            string encryptedTicket = FormsAuthentication.Encrypt(ticket);
            HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
            HttpContext.Current.Response.Cookies.Add(cookie);
        }

        public static bool IsUserAdmin(IPrincipal user)
        {
            return ((System.Web.Security.FormsIdentity)user.Identity).Ticket.UserData == "Admin";
            //// return user.IsInRole("Admin");
        }
    }
}