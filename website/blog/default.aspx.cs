using System;
using System.Data.SqlTypes;
using System.Collections.Specialized;
using System.Text;
using System.Collections.ObjectModel;
using System.Web;
using System.Data;
using System.Globalization;
using System.Web.UI.HtmlControls;


namespace KMBlog
{
	public partial class DefaultPage : System.Web.UI.Page
	{

		protected void Page_Load(object sender, EventArgs e)
		{

            // If URL doesn't contain 'default.aspx' then set style sheet programatically
            if (Request.RawUrl.ToString().IndexOf("default.aspx") < 0)
            {
                HtmlLink blogCSS = new HtmlLink();
                blogCSS.Href = KMBlog.Global.GetBlogPath() + "kmblog.css";
                blogCSS.Attributes.Add("type", "text/css");
                blogCSS.Attributes.Add("rel", "stylesheet");
                Page.Header.Controls.Add(blogCSS);
            }


			((KMBlogMaster)Page.Master).SetTitle("Key Mapper Developer Blog");
			GetPosts();

		
			
		}

		// Specific post: ?p=1 -- overrides all other options.
		// All posts by category: ?c=1
		// Posts on a specific date: ?d=20080603
		// Specific month: ?d=200806
		// Specific year: ?d=2008

        public string GetCategoryLink(string slug)
        {
            return KMBlog.Global.GetBlogPath() + @"category/" + slug;
        }

        public string GetPostLink(string slug)
        {
            return KMBlog.Global.GetBlogPath() + slug;
        }

        public string GetAdminPageLink()
        {
            return "hello world!!!"; // return KMBlog.Global.GetBlogPath() + @"admin/admin.aspx";
        }

		private void GetPosts()
		{

			Collection<Post> posts;
			bool singlePost = false;

			// A request for a specific post overrides all other parameters
			int postId = IsQueryForSpecificPost();

			if (postId != 0)
			{
				Post post = Post.GetPostById(postId);
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

				int categoryId = GetCategoryFromQueryString();
				DateTime dateFrom, dateTo;
				GetDateRangeFromQueryString(out dateFrom, out dateTo);

				posts = Post.GetAllPosts(categoryId, dateFrom, dateTo, CommentType.Approved);

			}

			if (singlePost == false)
				comments_inner.Style.Add("Display", "None");
			else
				if (posts.Count != 0)
				{
					GetCommentsForPost(postId);
				}


			Collection<Category> catlist = Category.GetAllCategories();

			categoriesRepeater.DataSource = catlist;
			categoriesRepeater.DataBind();

			postsRepeater.DataSource = posts;
			postsRepeater.DataBind();

			PopulateArchive();

		}

		private void PopulateArchive()
		{

			// Get a list of years and months which have posts in from the database

			DataTable archives = DataAccess.CreateInstance().GetArchives();

			if (archives == null || archives.Rows.Count == 0)
				return;

			StringBuilder alist = new StringBuilder();

			int currentYear = 0;

			// Moving forward:
			// 1) Use nested repeaters and handle the ItemDataBound method as in
			// http://www.codeproject.com/KB/aspnet/AspNetNestedRepeaters.aspx

			// 2) Use LInq to get List<T> for years and months

			// 

			// ListDictionary<int> years = from 

			alist.Append("<ul>");

			foreach (DataRow row in archives.Rows)
			{
				int year = Convert.ToInt32(row["year"]);
				int month = Convert.ToInt32(row["month"]);
				int posts = Convert.ToInt32(row["posts"]);

				if (currentYear != year)
				{
					if (currentYear != 0)
						alist.Append("</ul>");

					alist.Append("<li><a href=\"" + KMBlog.Global.GetBlogPath() + year.ToString() + "\">" + year.ToString() + "</a></li><ul>");
					currentYear = year;
				}

				string monthname = DateTimeFormatInfo.InvariantInfo.GetMonthName(month);

                alist.Append("<li class=\"archivelist\">" + "<a href=\"" + KMBlog.Global.GetBlogPath() + year.ToString() + "/"
					+ month.ToString().PadLeft(2, '0') + "\">" + monthname 
					+ " - (" + posts.ToString() + " post" 
					+ (posts > 1 ? "s" : "") + ")</a></li>");

			}

			alist.Append("</ul></ul>");

			archivelist.InnerHtml = alist.ToString();

		}

		/// <summary>
		/// Parse querystring for a specific post request.
		/// If found, return PostId
		/// </summary>
		private int IsQueryForSpecificPost()
		{

			NameValueCollection parameters = Request.QueryString;

			string[] keys = parameters.AllKeys;

			int PostId = 0;
			foreach (string key in keys)
			{
				if (key.ToUpperInvariant() == "P")
				{
					foreach (string value in parameters.GetValues(key))
					{
						if (System.Int32.TryParse(value, out PostId))
						{
							break;
						}
					}
				}
			}
			return PostId;
		}

		private int GetCategoryFromQueryString()
		{
			return Category.GetCategoryIdFromQueryString(Request.QueryString);
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

			// If there's no date specified or the year is invalid..
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

		public void GetCommentsForPost(int postId)
		{
			Collection<Comment> commentlist = Comment.GetComments(postId, CommentType.Approved);
			if (commentlist.Count == 0)
			{
				commentsheader.Style.Add("Display", "None");
				return;
			}

			commentsRepeater.DataSource = commentlist;
			commentsRepeater.DataBind();
		}

		public string FormatPostCategories(Collection<Category> catList)
		{

			StringBuilder categories = new StringBuilder();

			foreach (Category cat in catList)
				categories.Append("<a href=\"" + GetCategoryLink(cat.Slug) + "\">" + cat.Name + "</a>"); 

			return categories.ToString();


		}

		public string GetCommentNameLink(string name, string URL)
		{
			if (String.IsNullOrEmpty(URL))
				return name;
			else
			{
				return "<a href=\"" + URL + "\">" + name + "</a>";
			}
		}

       

		public string GetCommentLinkText(string slug, int commentCount)
		{

			string href = GetPostLink(slug) + "#comments";
			string text;

			if (commentCount == 0)
				text = "No comments";
			else
			{
				text = commentCount.ToString() + " comment";
				if (commentCount > 1)
					text += "s";
			}

			String comment = "<a href=\"" + href + "\">" + text + "</a>";

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
			c.PostId = Post.GetPostIdFromQueryString(Request.QueryString);
			c.Text = editcomment.Text;
			c.Posted = DateTime.Now;

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

			editcomment.ClearValues();

			if (Request.Url.Fragment != "#comments")
			{
				Response.Redirect(Request.Url + "#comments");
			}

			GetPosts();

		}

		public void CancelComment(object sender, EventArgs e)
		{

		}
	}
}

