using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Collections.Generic;
using System.Configuration;

public class SQLBlogDataAccess : IDataAccess
{

	#region IDataAccess Members

	public System.Collections.Generic.List<Post> GetAllPosts()
	{
		return GetAllPosts(0, SqlDateTime.MinValue.Value, SqlDateTime.MaxValue.Value);
	}

	public System.Collections.Generic.List<Post> GetAllPosts(int categoryID)
	{
		return GetAllPosts(categoryID, SqlDateTime.MinValue.Value, SqlDateTime.MaxValue.Value);
	}

	public System.Collections.Generic.List<Post> GetAllPosts(DateTime startDate, DateTime endDate)
	{
		return GetAllPosts(0, startDate, endDate);
	}

	public System.Collections.Generic.List<Post> GetAllPosts(int categoryID, DateTime startDate, DateTime endDate)
	{

		int numberOfPosts = 10;
		List<Post> posts = new List<Post>();

		using (SqlConnection connection = new SqlConnection(GetConnectionString()))
		{
			if (connection != null)
			{
				connection.Open();

				SqlCommand sc = new SqlCommand();
				
				sc.CommandText = "GetAllPosts";

				sc.Parameters.AddWithValue("NumberOfPosts", numberOfPosts);
				sc.Parameters.AddWithValue("CategoryID", categoryID);
				sc.Parameters.AddWithValue("DateFrom", startDate);
				sc.Parameters.AddWithValue("DateTo", endDate);

				sc.CommandType = CommandType.StoredProcedure;
				sc.Connection = connection;

				using (SqlDataReader reader = sc.ExecuteReader())
				{
					while (reader.Read())
					{
						Post p = SQLDataMap.CreatePostFromReader(reader);
						posts.Add(p);
					}
				}


			}

		}

		return posts;
	}

	public Post GetPostByID(int postID)
	{
		throw new NotImplementedException();
	}

	public bool AddPost(Post p)
	{
		throw new NotImplementedException();
	}

	public bool EditPost(Post p)
	{
		throw new NotImplementedException();
	}

	public bool DeletePost(int postID)
	{
		throw new NotImplementedException();
	}

	public bool AddCategoryToPost(int postID, int catID)
	{
		throw new NotImplementedException();
	}

	public bool DeleteCategoryFromPost(int postID, int catID)
	{
		throw new NotImplementedException();
	}

	public bool AddCategory(int categoryID, string categoryName)
	{
		throw new NotImplementedException();
	}

	public bool DeleteCategory(int categoryID)
	{
		throw new NotImplementedException();
	}

	public bool AddCommentToPost(int postID, string comment)
	{
		throw new NotImplementedException();
	}

	public bool DeleteComment(int commentID)
	{
		throw new NotImplementedException();
	}

	public List<Comment> GetCommentsForPost(int postID)
	{
		return new List<Comment>();
	}

	#endregion

	public string GetConnectionString()
	{
		return ConfigurationManager.ConnectionStrings["default"].ConnectionString;
	}
}
