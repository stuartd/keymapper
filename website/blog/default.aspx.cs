using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Configuration;
using System.Collections.Specialized;
using System.Text;
using System.Collections.ObjectModel;
using System.Net;
using System.Web;


namespace KMBlog
{
	public partial class DefaultPage : System.Web.UI.Page
	{

		protected void Page_Load(object sender, EventArgs e)
		{
			GetPosts();
		}

		// Specific post: ?p=1 -- overrides all other options.
		// All posts by category: ?c=1
		// Posts on a specific date: ?d=20080603
		// Specific month: ?d=200806
		// Specific year: ?d=2008
		// anything else: is it a slug? if not, show default page.
		// eg ?user_mappings

		// Nice to actually expose blog/posts/1
		// (Google friendly, as G dislikes querystrings?)
		// blog/category/1 etc
		// blog/2008/06/01 

		// URLRemapper could be the answer.
		// Need to figure out how to combine, say, categories and months.
		// blog/2008/06/01/category/1 maybe?

		private void GetPosts()
		{

			Collection<Post> posts;
			bool singlePost = false;

			// A request for a specific post overrides all other parameters
			int postID = IsQueryForSpecificPost();
			if (postID != 0)
			{
				Post post = Post.GetPostByID(postID);
				posts = new Collection<Post>();
				posts.Add(post);
				singlePost = true;

				if (Request.Cookies["userDetails"] != null)
				{
					editcomment.Name = Server.HtmlEncode(Request.Cookies["userDetails"]["Name"]);
					editcomment.URL = Server.HtmlEncode(Request.Cookies["userDetails"]["URL"]);
					chkRememberDetails.Checked = true;
				}
			}
			else
			{

				int categoryID = GetCategoryFromQueryString();
				DateTime dateFrom, dateTo;
				GetDateRangeFromQueryString(out dateFrom, out dateTo);

				posts = Post.GetAllPosts(categoryID, dateFrom, dateTo);

			}

			if (singlePost == false)
				comments.Style.Add("Display", "None");
			else
				if (posts.Count > 0)
				{
					Collection<Comment> clist = Post.GetCommentsForPost(postID);
					commentsRepeater.DataSource = clist;
					commentsRepeater.DataBind();
				}

			Collection<Category> catlist = Category.GetAllCategories();

			categoriesRepeater.DataSource = catlist;
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

		private int GetCategoryFromQueryString()
		{
            return Category.GetCategoryIDFromQueryString(Request.QueryString);
		}



		private void GetDateRangeFromQueryString(out DateTime dateFrom, out DateTime dateTo)
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
				return;
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
		}

		public void GetCommentsForPost(int postID)
		{
			Collection<Comment> commentlist = Post.GetCommentsForPost(postID);
			commentsRepeater.DataSource = commentlist;
			commentsRepeater.DataBind();
		}

		public string GetCategoriesForPost(Collection<Category> catlist)
		{

			StringBuilder categories = new StringBuilder();

			foreach (Category cat in catlist)
				categories.Append("<a href=\"?c=" + cat.ID + "\">" + cat.Name + "</a> ");

			return categories.ToString();


		}

		public string GetCommentLink(string name, string URL)
		{
			if (String.IsNullOrEmpty(URL))
				return name;
			else
			{
				return "<a href='" + URL + "'>" + name + "</a>";
			}
		}

		public string GetCommentLinkText(int postID, int commentCount)
		{

			// href is ?p=1#comments
			string href = "\"?p=" + postID.ToString() + "#comments\"";
			string text;

			if (commentCount == 0)
				text = "No comments";
			else
			{
				text = commentCount.ToString() + " comment";
				if (commentCount > 0)
					text += "s";
			}

			String comment = "<a href=" + href + ">" + text + "</a>";

			return comment;

		}

		public void SaveComment(object sender, EventArgs e)
		{

			Page.Validate();

			if (Page.IsValid == false)
				return;

			Comment c = new Comment();

			c.Url = editcomment.URL;
			c.Name = editcomment.Name;
			c.PostID = Post.GetPostIDFromQueryString(Request.QueryString);
			c.Text = editcomment.Text;

			c.Save();

			if (chkRememberDetails.Checked)
			{

				HttpCookie details = new HttpCookie("userDetails");
				details.Values.Add("Name", c.Name);
				details.Values.Add("URL", c.Url);
				Response.Cookies.Add(details);
			}
			else
			{
				if (Request.Cookies["userDetails"] != null)
				{
					HttpCookie details = new HttpCookie("userDetails");
					details.Values.Add("Name", String.Empty);
					details.Values.Add("URL", String.Empty);
					Response.Cookies.Add(details);
				}
			}


			GetPosts();

		}

		public void CancelComment(object sender, EventArgs e)
		{

		}
	}
}

