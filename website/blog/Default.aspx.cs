using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Configuration;
using System.Collections.Specialized;
using System.Text;
using System.Web.UI.HtmlControls ;


namespace KMBlog
{
	public partial class _Default : System.Web.UI.Page
	{

		string _connstring;

		protected void Page_Load(object sender, EventArgs e)
		{

       

		_connstring = ConfigurationManager.ConnectionStrings["prod"].ConnectionString;
			GetPosts();
		}

		private void GetPosts()
		{

			DataSet categories = new DataSet();
			DataSet posts = new DataSet();
			DataSet comments = new DataSet();

			string categoriesQuery = "select ID, name from categories";

			using (SqlConnection connection = new SqlConnection(_connstring))
			{
				if (connection != null)
				{
					SqlDataAdapter adapter = new SqlDataAdapter();
					SqlCommand sc = new SqlCommand("GetAllPosts", connection) ;
					sc.CommandType = CommandType.StoredProcedure ;

					// sc.Parameters.AddWithValue("DateFrom", SqlDateTime.MinValue) ;
					// sc.Parameters.AddWithValue("DateTo", SqlDateTime.MaxValue);
					// sc.Parameters.AddWithValue("NumberOfPosts", 10);
					// sc.Parameters.AddWithValue("CategoryID", 0);

					adapter.SelectCommand = new SqlCommand(categoriesQuery, connection);
					adapter.Fill(categories);
					adapter.SelectCommand = sc ;
					adapter.Fill(posts);

				}
			}

			categoriesRepeater.DataSource = categories;
			categoriesRepeater.DataBind();

			postsRepeater.DataSource = posts;
			postsRepeater.DataBind();
		}

		public string GetCategoriesForPost(int postID)
		{

			StringBuilder cats = new StringBuilder();

			using (SqlConnection connection = new SqlConnection(_connstring))
			{

				if (connection != null)
				{

					connection.Open();

					SqlCommand sc = new SqlCommand("GetCategoriesByPost", connection);
					sc.CommandType = CommandType.StoredProcedure;
					sc.Parameters.AddWithValue("PostID", postID);

					SqlDataReader rdr = sc.ExecuteReader() ;
					while (rdr.Read())
					{
						cats.Append("<a href=\"?c=" + rdr[0] + "\">" + rdr[1] + "</a>");
					}
			

				}
			}

			return cats.ToString();


		}

        protected void Button1_Click(object sender, EventArgs e)
        {
            foreach (object o in Page.Controls)
            {
                Response.Write(o.ToString());
            }
        }

	}
}
