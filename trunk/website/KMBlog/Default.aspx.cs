using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections.Specialized;
using System.Text;


namespace KMBlog
{
	public partial class _Default : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{

			string postsQuery = GetPostQuery();

			Response.Write(postsQuery + "<br />") ;

			string cs = ConfigurationManager.ConnectionStrings["blogConnectionStringWork"].ConnectionString;
			DataSet categories = new DataSet();
			DataSet posts = new DataSet();
			DataSet comments = new DataSet();

			string categoriesQuery = "select ID, name from categories";

			using (SqlConnection connection = new SqlConnection(cs))
			{
				if (connection != null)
				{
					SqlDataAdapter adapter = new SqlDataAdapter();
					adapter.SelectCommand = new SqlCommand(categoriesQuery, connection);
					adapter.Fill(categories);
					adapter.SelectCommand = new SqlCommand(postsQuery, connection);
					adapter.Fill(posts);
				}
			}

			categoriesRepeater.DataSource = categories;
			categoriesRepeater.DataBind();

			postsRepeater.DataSource = posts;
			postsRepeater.DataBind();
		}


		private string GetPostQuery()
		{

			string defaultQuery = "select top 10 * from posts";

			NameValueCollection parameters = Request.QueryString;
			if (parameters.HasKeys() == false)
				return defaultQuery;

			StringBuilder query = new StringBuilder("select posts.* from posts ");

			// Parse query string so can determine what posts to show:
			// (Using Wordpress's conventions as a guide)
			// All posts by category: ?c=1
			// Specific post: ?p=1
			// Specific date: ?d=20080603
			// Specific month: ?d=200806
			// Specific year: ?d=2008
			// anything else: is it a stub? if not, show default page.
			// eg ?user_mappings

			// WBN to find a way to map /kmblog/user_mappings to here 
			// without using routing. Or check if can use routing on jks?

			string[] keys = parameters.AllKeys;

			// Look for a p key with a valid value: if found, this overrides any other key.
			foreach (string key in keys)
			{
				if (key.ToUpperInvariant() == "P")
				{
					int postID;
					foreach (string value in parameters.GetValues(key))
					{
						if (System.Int32.TryParse(value, out postID))
						{
							query.Append("where ID = " + postID.ToString());
							return query.ToString();
						}
					}

				}
			}


			foreach (string key in keys)
			{
				// No specific post. Has a specific category been requested?

				if (key.ToUpperInvariant() == "C")
				{
					int categoryID;
					foreach (string value in parameters.GetValues(key))
					{
						if (System.Int32.TryParse(value, out categoryID))
						{
							query.Append(" join postcategories on posts.ID = postcategories.postID and postcategories.CategoryID = " + categoryID.ToString());
						}
					}

				}
			}
			// Has a date range been requested (perhaps in addition to the category)?

			// Need to know whether we have criteria. Todo.

			return query.ToString();



		}

		protected void postsRepeater_DataBinding(object sender, EventArgs e)
		{
		}
	}
}
