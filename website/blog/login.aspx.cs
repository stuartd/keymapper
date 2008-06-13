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

namespace KMBlog
{
	public partial class login : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{

		}

		protected void Login1_Authenticate(object sender, AuthenticateEventArgs e)
		{

			_connstring = ConfigurationManager.ConnectionStrings["work"].ConnectionString;
			//  _connstring = ConfigurationManager.ConnectionStrings["home.jks"].ConnectionString;

			using (SqlConnection conn = new SqlConnection(_connstring))

			{

				SqlCommand sc = new SqlCommand("GetAdminDetails") ;
				sc.Parameters.Add("UserName") ;
				sc.Parameters.Add("Password") ;
				



			if ()
				
			{
				e.Authenticated = true;
			}
			else
			{
				e.Authenticated = false;
			}
		}
	}
}
