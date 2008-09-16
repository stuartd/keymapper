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
            return (DataAccess.CreateInstance().GetPostIdFromSlug(slug) > 0);
		}

		public static Collection<Post> GetAllPosts(CommentType ctype)
		{
			return DataAccess.CreateInstance().GetAllPosts(ctype);
		}

		public static Collection<Post> GetAllPosts(int categoryId, DateTime startDate, DateTime endDate, CommentType ctype)
		{
			return DataAccess.CreateInstance().GetAllPosts(categoryId, startDate, endDate, ctype);
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