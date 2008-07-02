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

            sc.Parameters.AddWithValue("username", KMLogin.UserName);
            sc.Parameters.AddWithValue("password", KMLogin.Password);
            SqlParameter userlevel = new SqlParameter("userlevel", SqlDbType.Int);

            userlevel.Direction = ParameterDirection.Output;
            sc.Parameters.Add(userlevel);

            sc.ExecuteNonQuery();

            string value = sc.Parameters["UserLevel"].Value.ToString();

            if (String.IsNullOrEmpty(value))
            {
                e.Authenticated = false;
            }
            else
            {
                int authUserLevel;
                if (Int32.TryParse(value, out authUserLevel))
                    if (authUserLevel > 0)
                    {
                        e.Authenticated = true;
                        // Do something with userlevel
                        return;
                    }
                e.Authenticated = false;
            }
        }
    }
}
