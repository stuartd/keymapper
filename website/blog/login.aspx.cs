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

            string _connstring = ConfigurationManager.ConnectionStrings["default"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(_connstring))
            {
                conn.Open();

                SqlCommand sc = new SqlCommand("CheckUser", conn);
                sc.Parameters.AddWithValue("UserName", Login1.UserName);
                sc.Parameters.AddWithValue("Password", Login1.Password);
                SqlParameter userlevel = new SqlParameter("UserLevel", SqlDbType.Int, 1, ParameterDirection.Output, true, 1);

                sc.Parameters.Add(userlevel);


                

                if (userlevel == null || userlevel < 1)
                    e.Authenticated = false;
                else
                {
                    e.Authenticated = true;
                    // Do something with userlevel
                }


            }
        }
    }
}
