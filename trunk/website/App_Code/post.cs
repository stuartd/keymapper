using System;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Collections.Specialized;

namespace KMBlog
{


	public class Post
	{

		public int Id { get; set; }
		public string Title { get; set; }
		public Collection<Category> Categories { get; set; }
		public DateTime Postdate { get; set; }
		public string Body { get; set; }
		public string Slug { get; set; }
		public int CommentCount { get; set; }
		public bool Published { get; set; }

		public static int GetPostIdFromQueryString(NameValueCollection parameters)
		{

			int PostId;
			string postParameter = parameters["P"];
			if (System.Int32.TryParse(postParameter, out PostId))
				return PostId;
			else
				return 0;

			//string[] keys = parameters.AllKeys;

			//int PostId = 0;
			//foreach (string key in keys)
			//{
			//    if (key.ToUpperInvariant() == "P")
			//    {
			//        foreach (string value in parameters.GetValues(key))
			//        {
			//            if (System.Int32.TryParse(value, out PostId))
			//            {
			//                break;
			//            }
			//        }
			//    }
			//}
			//return PostId;
		}

		public static void Delete(int PostId)
		{
			DataAccess.CreateInstance().DeletePost(PostId);

		}

		public static Post GetPostById(int PostId)
		{
			return DataAccess.CreateInstance().GetPostById(PostId);
		}

		public static bool DoesSlugExist(string slug)
		{
			return DataAccess.CreateInstance().DoesSlugExist(slug);
		}

		public static Collection<Comment> GetCommentsForPost(int PostId)
		{
			// Could be part of comment class?
			return DataAccess.CreateInstance().GetCommentsForPost(PostId);
		}

		public static Collection<Post> GetAllPosts()
		{
			return DataAccess.CreateInstance().GetAllPosts();
		}

		public static Collection<Post> GetAllPosts(int categoryId, DateTime startDate, DateTime endDate)
		{
			return DataAccess.CreateInstance().GetAllPosts(categoryId, startDate, endDate);
		}


		public static bool Save(Post p)
		{

			if (!p)
				return false;

			IDataAccess da = DataAccess.CreateInstance();
			p.Id = da.SavePost(p);
			return (p.Id > 0);
		}

		public static implicit operator bool(Post p)
		{
			return !(string.IsNullOrEmpty(p.Slug) || string.IsNullOrEmpty(p.Title));
		}



	}
}