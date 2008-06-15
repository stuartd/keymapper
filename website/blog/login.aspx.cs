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
                sc.CommandType = CommandType.StoredProcedure;

                sc.Parameters.AddWithValue("username", Login1.UserName);
                sc.Parameters.AddWithValue("password", Login1.Password);
                SqlParameter userlevel = new SqlParameter("userlevel", SqlDbType.Int);

                userlevel.Direction = ParameterDirection.Output;
                sc.Parameters.Add(userlevel);

                SqlDataReader r = sc.ExecuteReader();
                // sc.ExecuteNonQuery();


                int? authUserLevel = (int?)sc.Parameters["UserLevel"].Value;


                if (authUserLevel == null || authUserLevel < 1)
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
