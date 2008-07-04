using System;
using System.Data.SqlClient;
using System.Data;
using System.Data.SqlTypes;
using System.Configuration;

namespace KMBlog
{
    public partial class admin : System.Web.UI.Page
    {

        string _connstring;

        protected void Page_Load(object sender, EventArgs e)
        {
            _connstring = ConfigurationManager.ConnectionStrings["default"].ConnectionString;
            //  _connstring = ConfigurationManager.ConnectionStrings["home.jks"].ConnectionString;
        

            GetPostList();

        }

        void GetPostList()
        {

            DataTable posts = new DataTable();

            using (SqlConnection connection = new SqlConnection(_connstring))
            {
                if (connection != null)
                {
                    connection.Open();

                    SqlCommand sc = new SqlCommand();
                    SqlDataReader reader;

                    sc.CommandText = "GetPostList";
                    sc.Parameters.AddWithValue("NumberOfPosts", 10);

                    sc.CommandType = CommandType.StoredProcedure;
                    sc.Connection = connection;

                    using (reader = sc.ExecuteReader())
                        posts.Load(reader);


                    postsRepeater.DataSource = posts;
                    postsRepeater.DataBind();

                }

            }
        }
    }
}
