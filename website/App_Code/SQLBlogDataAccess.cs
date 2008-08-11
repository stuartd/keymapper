using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Globalization;

namespace KMBlog
{

	public class SqlBlogDataAccess : IDataAccess
	{

		#region Posts

		public Collection<Post> GetAllPosts()
		{
			return GetAllPosts(0, SqlDateTime.MinValue.Value, SqlDateTime.MaxValue.Value, 999);
		}

		//public Collection<Post> GetAllPosts(int categoryId)
		//{
		//    return GetAllPosts(categoryId, SqlDateTime.MinValue.Value, SqlDateTime.MaxValue.Value);
		//}

		//public Collection<Post> GetAllPosts(DateTime startDate, DateTime endDate)
		//{
		//    return GetAllPosts(0, startDate, endDate);
		//}

		public Collection<Post> GetAllPosts(int categoryId, DateTime startDate, DateTime endDate)
		{
			return GetAllPosts(categoryId, startDate, endDate, 10);
		}

		public Collection<Post> GetAllPosts(int categoryId, DateTime startDate, DateTime endDate, int NumberOfPosts)
		{

			int numberOfPosts = 10;
			Collection<Post> posts = new Collection<Post>();

			using (SqlConnection connection = GetConnection())
			{
				connection.Open();

				SqlCommand sc = new SqlCommand();

				sc.CommandText = "GetAllPosts";

				sc.Parameters.AddWithValue("NumberOfPosts", numberOfPosts);
				sc.Parameters.AddWithValue("CategoryId", categoryId);
				sc.Parameters.AddWithValue("DateFrom", startDate);
				sc.Parameters.AddWithValue("DateTo", endDate);

				sc.CommandType = CommandType.StoredProcedure;
				sc.Connection = connection;

				using (SqlDataReader reader = sc.ExecuteReader())
				{
					posts = SqlDataMap.CreatePostsFromReader(reader);
				}


			}



			return posts;
		}

		public Post GetPostById(int postId)
		{
			Collection<Post> posts = new Collection<Post>();

			using (SqlConnection connection = GetConnection())
			{

				connection.Open();

				SqlCommand sc = new SqlCommand();

				sc.CommandText = "GetPostById";

				sc.Parameters.AddWithValue("PostId", postId);

				sc.CommandType = CommandType.StoredProcedure;
				sc.Connection = connection;

				using (SqlDataReader reader = sc.ExecuteReader())
				{
					posts = SqlDataMap.CreatePostsFromReader(reader);
				}
			}

			if (posts.Count > 0)
				return posts[0];
			else
				return null;

		}

		public int SavePost(Post p)
		{
			using (SqlConnection connection = GetConnection())
			{

				connection.Open();

				SqlCommand sc = new SqlCommand();

				if (p.Id == 0)
				{
					sc.CommandText = "CreatePost";
					SqlParameter sp = new SqlParameter("NewPostId", 0);
					sp.Direction = ParameterDirection.Output;
					sc.Parameters.Add(sp);
				}
				else
				{
					sc.CommandText = "EditPost";
					sc.Parameters.AddWithValue("PostId", p.Id);
				}

				sc.Parameters.AddWithValue("slug", p.Slug);
				sc.Parameters.AddWithValue("title", p.Title);
				sc.Parameters.AddWithValue("body", p.Body);
				sc.Parameters.AddWithValue("postdate", p.Postdate);
				sc.Parameters.AddWithValue("published", p.Published);

				sc.Connection = connection;
				sc.CommandType = CommandType.StoredProcedure;

				int result = sc.ExecuteNonQuery();

				if (p.Id == 0)
					return Convert.ToInt32(sc.Parameters["NewPostId"].Value, CultureInfo.InvariantCulture);
				else
					return p.Id;



			}


		}

		public bool DoesSlugExist(string slug)
		{

			using (SqlConnection connection = GetConnection())
			{

				connection.Open();
				SqlCommand sc = new SqlCommand("DoesSlugExist", connection);
				sc.Parameters.AddWithValue("Slug", slug);
				sc.CommandType = CommandType.StoredProcedure;

				bool exists = ((int)sc.ExecuteScalar() != 0);

				return exists;

			}
		}

		public void DeletePost(int postId)
		{
			using (SqlConnection connection = GetConnection())
			{
				connection.Open();

				SqlCommand sc = new SqlCommand("DeletePost", connection);
				sc.CommandType = CommandType.StoredProcedure;
				sc.Parameters.AddWithValue("PostId", postId);

				sc.ExecuteNonQuery();

			}

		}

		#endregion

		#region Categories

		public Category GetCategoryById(int categoryId)
		{
			Collection<Category> cats;

			using (SqlConnection connection = GetConnection())
			{
				connection.Open();
				SqlCommand sc = new SqlCommand("GetCategoryById", connection);

				sc.CommandType = CommandType.StoredProcedure;
				sc.Parameters.AddWithValue("CategoryId", categoryId);

				using (SqlDataReader reader = sc.ExecuteReader())
				{
					cats = SqlDataMap.CreateCategoriesFromReader(reader);
				}
			}
			if (cats.Count > 0)
				return cats[0];
			else
				return null;
		}

		public int GetCategoryIdByName(string name)
		{
			using (SqlConnection connection = GetConnection())
			{
				connection.Open();
				SqlCommand sc = new SqlCommand("GetCategoryByName", connection);

				sc.CommandType = CommandType.StoredProcedure;
				sc.Parameters.AddWithValue("CategoryName", name);
				SqlParameter param = new SqlParameter("CategoryId", 0);
				param.Direction = ParameterDirection.Output;
				sc.Parameters.Add(param);

				sc.ExecuteNonQuery();

				return Convert.ToInt32(sc.Parameters["CategoryId"].Value, CultureInfo.InvariantCulture);

			}
		}

		public void SyncCategories(int PostId, Collection<int> categories)
		{
			using (SqlConnection connection = GetConnection())
			{
				connection.Open();

				SqlCommand sc = new SqlCommand("DeleteCategoriesFromPost", connection);
				sc.CommandType = CommandType.StoredProcedure;
				sc.Parameters.AddWithValue("PostId", PostId);

				sc.ExecuteNonQuery();

				sc.CommandText = "AddCategoryToPost";

				foreach (int catId in categories)
				{
					sc.Parameters.Clear();
					sc.Parameters.AddWithValue("PostId", PostId);
					sc.Parameters.AddWithValue("CategoryId", catId);
					sc.ExecuteNonQuery();
				}
			}
		}

		public Collection<Category> GetAllCategories()
		{
			Collection<Category> cats;
			using (SqlConnection connection = GetConnection())
			{
				connection.Open();
				SqlCommand sc = new SqlCommand("GetAllCategories", connection);

				sc.CommandType = CommandType.StoredProcedure;
				using (SqlDataReader reader = sc.ExecuteReader())
				{
					cats = SqlDataMap.CreateCategoriesFromReader(reader);
				}
			}
			return cats;
		}

		public bool AddCategory(string categoryName, string categorySlug)
		{
			int result;
			using (SqlConnection connection = GetConnection())
			{
				connection.Open();

				SqlCommand sc = new SqlCommand("CreateCategory", connection);
				sc.CommandType = CommandType.StoredProcedure;
				sc.Parameters.AddWithValue("Name", categoryName);
				sc.Parameters.AddWithValue("Slug", categorySlug);

				result = sc.ExecuteNonQuery();
			}

			return (result == 1);

		}

		public bool DeleteCategory(int categoryId)
		{
			int result;
			using (SqlConnection connection = GetConnection())
			{
				connection.Open();

				SqlCommand sc = new SqlCommand("DeleteCategory", connection);
				sc.CommandType = CommandType.StoredProcedure;
				sc.Parameters.AddWithValue("categoryId", categoryId);

				result = sc.ExecuteNonQuery();
			}

			return (result == 1);
		}

		public bool EditCategory(Category cat)
		{
			int result;
			using (SqlConnection connection = GetConnection())
			{
				connection.Open();

				SqlCommand sc = new SqlCommand("EditCategory", connection);
				sc.CommandType = CommandType.StoredProcedure;
				sc.Parameters.AddWithValue("CategoryId", cat.Id);
				sc.Parameters.AddWithValue("Name", cat.Name);
				sc.Parameters.AddWithValue("Slug", cat.Slug);
				result = sc.ExecuteNonQuery();
			}

			return (result == 1);
		}

		#endregion

		#region Comments

		public bool AddCommentToPost(Comment com)
		{
			int rowcount = 0;
			using (SqlConnection connection = GetConnection())
			{

				connection.Open();

				SqlCommand sc = new SqlCommand("AddComment", connection);
				sc.CommandType = CommandType.StoredProcedure;

				sc.Parameters.AddWithValue("PostId", com.PostId);
				sc.Parameters.AddWithValue("Name", com.Name);
				sc.Parameters.AddWithValue("URL", com.Url);
				sc.Parameters.AddWithValue("Text", com.Text);
				sc.Parameters.AddWithValue("Posted", com.Posted);

				rowcount = sc.ExecuteNonQuery();

			}
			return (rowcount > 0);
		}

		public bool DeleteComment(int commentId)
		{
			int result;
			using (SqlConnection connection = GetConnection())
			{
				connection.Open();

				SqlCommand sc = new SqlCommand("DeleteComment", connection);
				sc.CommandType = CommandType.StoredProcedure;
				sc.Parameters.AddWithValue("commentId", commentId);

				result = sc.ExecuteNonQuery();
			}

			return (result == 1);
		}

		public Collection<Comment> GetCommentsForPost(int postId, CommentType ctype)
		{
			using (SqlConnection connection = GetConnection())
			{

				connection.Open();

				SqlCommand sc = new SqlCommand("GetCommentsByPost", connection);
				sc.CommandType = CommandType.StoredProcedure;

				sc.Parameters.AddWithValue("PostId", postId);

				Collection<Comment> clist = SqlDataMap.CreateCommentsFromReader(sc.ExecuteReader());

				return clist;
			}

		}

		public Collection<Comment> GetAllComments(CommentType ctype)
		{
			using (SqlConnection connection = GetConnection())
			{

				connection.Open();

				SqlCommand sc = new SqlCommand("GetAllComments", connection);
				sc.CommandType = CommandType.StoredProcedure;

				Collection<Comment> clist = SqlDataMap.CreateCommentsFromReader(sc.ExecuteReader());

				return clist;
			}

		}

		public bool ApproveComment(int commentId)
		{
			int result;
			using (SqlConnection connection = GetConnection())
			{

				connection.Open();

				SqlCommand sc = new SqlCommand("ApproveComment", connection);
				sc.CommandType = CommandType.StoredProcedure;

				sc.Parameters.AddWithValue("commentId", commentId);
	
					result = sc.ExecuteNonQuery();

		
			}
		return (result == 1);
			
		}

		#endregion

		public SqlConnection GetConnection()
		{
			SqlConnection sc = new SqlConnection(ConfigurationManager.ConnectionStrings["default"].ConnectionString);
			if (sc == null)
				throw new ArgumentException("Connection is null");
			else
				return sc;

		}

		public void LogDownload(string fileName, string IP, string referrer, string useragent)
		{
			using (SqlConnection conn = GetConnection())
			{
				conn.Open();

				SqlCommand sc = new SqlCommand("LogDownload", conn);
				sc.CommandType = CommandType.StoredProcedure;

				sc.Parameters.AddWithValue("@downloadfile", fileName);
				sc.Parameters.AddWithValue("@IP", IP);
				sc.Parameters.AddWithValue("@referrer", referrer ?? String.Empty);
				sc.Parameters.AddWithValue("@useragent", useragent);


				sc.ExecuteNonQuery();
			}
		}

		public int GetUserLevel(string userName, string passwordHash)
		{


			using (SqlConnection conn = GetConnection())
			{
				conn.Open();

				SqlCommand sc = new SqlCommand("CheckUser", conn);
				sc.CommandType = CommandType.StoredProcedure;

				sc.Parameters.AddWithValue("username", userName);
				sc.Parameters.AddWithValue("passwordhash", passwordHash);
				SqlParameter userlevel = new SqlParameter("userlevel", SqlDbType.Int);

				userlevel.Direction = ParameterDirection.Output;
				sc.Parameters.Add(userlevel);

				sc.ExecuteNonQuery();

				string value = Convert.ToString(sc.Parameters["UserLevel"].Value, CultureInfo.InvariantCulture);

				int authUserLevel;
				if (Int32.TryParse(value, out authUserLevel))
					return authUserLevel;
				else
					return 0;
			}
		}

	}
}
