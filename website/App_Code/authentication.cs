using System;
using System.Web;
using System.Web.Security;
using System.Security.Principal;

namespace KMBlog
{
	public class KMAuthentication
	{

		private KMAuthentication() { }
		
		public static int AuthenticateUser(string userName, string passwordHash)
		{
			return DataAccess.CreateInstance().GetUserLevel(userName, passwordHash);
		}

		public static HttpCookie CreateAuthenticationTicket(string userName, string role)
		{
			FormsAuthenticationTicket ticket = new FormsAuthenticationTicket
				 (1, userName, DateTime.Now, DateTime.Now.AddMinutes(60), false, role);

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
}