using System;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI.HtmlControls;

namespace KMBlog
{
	public partial class post_edit : System.Web.UI.Page
	{


		protected void Page_Load(object sender, EventArgs e)
		{

			int postID = GetPostID();

			if (postID != 0)
			{

				string cs = ConfigurationManager.ConnectionStrings["default"].ConnectionString;

				using (SqlConnection connection = new SqlConnection(cs))
				{
					if (connection != null)
					{

						connection.Open();
						SqlCommand sc = new SqlCommand();

						sc.Connection = connection;

						sc.CommandText = "GetPostByID";
						sc.Parameters.AddWithValue("PostID", postID);
						sc.CommandType = CommandType.StoredProcedure;

						blogpost.Style.Add("display", "none");

						using (SqlDataReader dr = sc.ExecuteReader())
						{
							while (dr.Read())
							{
								// Only one result will be returned.
								blogpost.Style.Remove("display");
								blogpost.Style.Add("display", "block");
								blogpost.InnerHtml = dr["body"].ToString();
							}
						}
					}
				}
			}

		}


		protected int GetPostID()
		{

			NameValueCollection parameters = Request.QueryString;

			string[] keys = parameters.AllKeys;

			int postID = 0;
			foreach (string key in keys)
			{
				if (key.ToUpperInvariant() == "P")
				{
					foreach (string value in parameters.GetValues(key))
					{
						if (System.Int32.TryParse(value, out postID))
						{
							break;
						}
					}
				}
			}
			return postID;
		}

		public void SavePost(Object sender, EventArgs e)
		{

			int postID = GetPostID();

			if (postID == 0)
			{

				string cs = ConfigurationManager.ConnectionStrings["default"].ConnectionString;

				using (SqlConnection connection = new SqlConnection(cs))
				{
					if (connection != null)
					{
						connection.Open();
						SqlCommand sc = new SqlCommand("CreatePost");
						sc.Connection = connection;
						sc.Parameters.AddWithValue("title", "foo");
						sc.Parameters.AddWithValue("stub", "bar");
						sc.Parameters.AddWithValue("body", "foo");
						sc.CommandType = CommandType.StoredProcedure;

						int result = sc.ExecuteNonQuery();
						
					}
				}
			}
		}
	}

}



