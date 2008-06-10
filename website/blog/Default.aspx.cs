using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Configuration;
using System.Collections.Specialized;
using System.Text;
using System.Web.UI.HtmlControls;


namespace KMBlog
{
	public partial class _Default : System.Web.UI.Page
	{

		string _connstring;
		bool _showComments = false;

		protected void Page_Load(object sender, EventArgs e)
		{
			_connstring = ConfigurationManager.ConnectionStrings["work"].ConnectionString;
			GetPosts();
		}
		

		// Specific post: ?p=1 -- overrides all other options.
		// All posts by category: ?c=1
		// Posts on a specific date: ?d=20080603
		// Specific month: ?d=200806
		// Specific year: ?d=2008
		// anything else: is it a stub? if not, show default page.
		// eg ?user_mappings

		// Nice to actually expose blog/posts/1
		// (Google friendly, as G dislikes querystrings?)
		// blog/category/1 etc
		// blog/2008/06/01 
		
		// URLRemapper could be the answer.
		// Need to figure out how to combine 

		private void GetPosts()
		{

			DataTable categories = new DataTable();
			DataTable posts = new DataTable();

			using (SqlConnection connection = new SqlConnection(_connstring))
			{
				if (connection != null)
				{

					connection.Open();

					SqlCommand sc = new SqlCommand();
					SqlDataReader reader;

					// A request for a specific post overrides all other parameters
					int postID = IsQueryForSpecificPost();
					if (postID != 0)
					{
						sc.CommandText = "GetPostByID";
						sc.Parameters.AddWithValue("PostID", postID);
					}
					else
					{
						sc.CommandText = "GetAllPosts";

						// Parameters for GetAllPosts
						// sc.Parameters.AddWithValue("DateFrom", SqlDateTime.MinValue) ;
						// sc.Parameters.AddWithValue("DateTo", SqlDateTime.MaxValue);
						// sc.Parameters.AddWithValue("NumberOfPosts", 10); TODO: Get from settings.
						// sc.Parameters.AddWithValue("CategoryID", 0);

						int categoryID = IsQueryForSpecificCategory();
						if (categoryID > 0)
							sc.Parameters.AddWithValue("CategoryID", categoryID);

						DateTime dateFrom, dateTo;
						if (IsQueryForSpecificDateRange(out dateFrom, out dateTo))
						{
							sc.Parameters.AddWithValue("DateFrom", dateFrom);
							sc.Parameters.AddWithValue("DateTo", dateTo);
						}

					}

					sc.CommandType = CommandType.StoredProcedure;
					sc.Connection = connection;

					using (reader = sc.ExecuteReader())
						posts.Load(reader);

					sc.Parameters.Clear();

					sc.CommandText = "GetAllCategories";
					using (reader = sc.ExecuteReader())
						categories.Load(reader);

				}
			}

			categoriesRepeater.DataSource = categories;
			categoriesRepeater.DataBind();

			postsRepeater.DataSource = posts;
			postsRepeater.DataBind();
		}

		/// <summary>
		/// Parse querystring for a specific post request.
		/// If found, return postID
		/// </summary>
		private int IsQueryForSpecificPost()
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

		private int IsQueryForSpecificCategory()
		{

			NameValueCollection parameters = Request.QueryString;

			string[] keys = parameters.AllKeys;

			int categoryID = 0;
			foreach (string key in keys)
			{
				if (key.ToUpperInvariant() == "C")
				{
					foreach (string value in parameters.GetValues(key))
					{
						if (System.Int32.TryParse(value, out categoryID))
						{
							break;
						}
					}
				}
			}
			return categoryID;
		}



		private bool IsQueryForSpecificDateRange(out DateTime dateFrom, out DateTime dateTo)
		{

			NameValueCollection parameters = Request.QueryString;

			string[] keys = parameters.AllKeys;

			int dateRange = 0;

			int year = 0;
			int month = 0;
			int day = 0;

			foreach (string key in keys)
			{
				if (key.ToUpperInvariant() == "D")
				{
					foreach (string value in parameters.GetValues(key))
					{
						if (System.Int32.TryParse(value, out dateRange))
						{
							if (dateRange < 9999)
							{
								year = dateRange;
							}
							else if (dateRange < 999999)
							{
								// year and month
								year = (int)(dateRange / 100);
								month = dateRange - (year * 100);
							}
							else if (dateRange < 99999999)
							{
								year = dateRange / 10000;
								month = (dateRange - (year * 10000)) / 100;
								day = dateRange - (year * 10000) - (month * 100);
							}
						}
					}
					break;
				}
			}

			// Invalid year is a fail, everything else is fixable.
			if (year < SqlDateTime.MinValue.Value.Year || year > SqlDateTime.MaxValue.Value.Year)
			{
				dateFrom = SqlDateTime.MinValue.Value;
				dateTo = SqlDateTime.MaxValue.Value;
				return false;
			}

			if (month < 1 || month > 12)
			{
				// Valid month not supplied in query string, so range is whole year.
				dateFrom = new DateTime(year, 1, 1, 0, 0, 0);
				dateTo = new DateTime(year, 12, 31, 23, 59, 59);
			}
			else if (day < 1 || day > DateTime.DaysInMonth(year, month))
			{
				// No valid day supplied, so use whole month
				dateFrom = new DateTime(year, month, 1, 0, 0, 0);
				dateTo = new DateTime(year, month, DateTime.DaysInMonth(year, month), 23, 59, 59);
			}
			else
			{
				// All parameters valid!
				dateFrom = new DateTime(year, month, day, 0, 0, 0);
				dateTo = new DateTime(year, month, day, 23, 59, 59);
			}

			return true;
		}

		public void GetCommentsForPost(int postID)
		{
			DataTable comments = new DataTable();

			using (SqlConnection connection = new SqlConnection(_connstring))
			{
				if (connection != null)
				{

					connection.Open();

					SqlCommand sc = new SqlCommand("GetCommentsByPost", connection);
					sc.CommandType = CommandType.StoredProcedure;

					sc.Parameters.AddWithValue("PostID", postID);

					SqlDataReader reader = sc.ExecuteReader();
					comments.Load(reader);

				}
			}


		}

		public string GetCategoriesForPost(int postID)
		{

			StringBuilder categories = new StringBuilder();

			using (SqlConnection connection = new SqlConnection(_connstring))
			{

				if (connection != null)
				{

					connection.Open();

					SqlCommand sc = new SqlCommand("GetCategoriesByPost", connection);
					sc.CommandType = CommandType.StoredProcedure;
					sc.Parameters.AddWithValue("PostID", postID);

					SqlDataReader rdr = sc.ExecuteReader();
					while (rdr.Read())
					{
						categories.Append("<a href=\"?c=" + rdr[0] + "\">" + rdr[1] + "</a>");
					}


				}
			}

			return categories.ToString();


		}

		public string GetCommentLink(int postID, int commentCount)
		{

			if (_showComments)
				return String.Empty;

			// href is something like ?p=1#comments
			string href = "\"?p=" + postID.ToString() + "#comments\"";

			String comment = "<a href=" + href + ">Comment";

			if (commentCount > 0)
				comment += "s: " + commentCount.ToString();

			comment += "</a>";

			return comment;


		}

		public void ShowComments()
		{




		}



	}
}

