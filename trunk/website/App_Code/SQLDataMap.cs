using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;


namespace KMBlog
{

	public class SqlDataMap
	{

		public static Collection<Comment> CreateCommentsFromReader(SqlDataReader reader)
		{

			Collection<Comment> commentlist = new Collection<Comment>();
			if (reader.HasRows)
			{
				while (reader.Read())
				{

					Comment com = new Comment();

					com.Id = Convert.ToInt32(reader["Id"]);
					com.PostId = Convert.ToInt32(reader["PostId"]);
					com.Text = Convert.ToString(reader["text"]);
					com.Name = Convert.ToString(reader["name"]);
					com.Url = Convert.ToString(reader["url"]);
					com.Approved = Convert.ToBoolean(reader["Approved"]);

					commentlist.Add(com);
				}
			}

			return commentlist;




		}

		public static Collection<Post> CreatePostsFromReader(SqlDataReader reader)
		{
			// First result set is the postcategories. 

			Dictionary<int, Category> postcats = new Dictionary<int, Category>();
			if (reader.HasRows)
			{
				while (reader.Read())
				{

					// Oops - key can't be PostId as key has to be unique. Need to use a different collection..
					// .. for now, make a hash of the post and category IDs.
					int categoryId = Convert.ToInt32(reader["categoryId"]);
					int postHash = (Convert.ToInt32(reader["PostId"]) * 10000) + categoryId;
					postcats.Add(postHash, new Category(categoryId, Convert.ToString(reader["Name"]), Convert.ToString(reader["Slug"])));
				}
			}
			// Second resultset is the post(s)
			reader.NextResult();

			Collection<Post> postlist = new Collection<Post>();
			if (reader.HasRows)
			{
				while (reader.Read())
				{

					Post p = new Post();

					p.Id = Convert.ToInt32(reader["Id"]);
					p.Title = Convert.ToString(reader["Title"]);
					p.Postdate = Convert.ToDateTime(reader["PostDate"]);
					p.Body = Convert.ToString(reader["Body"]);
					p.Slug = Convert.ToString(reader["Slug"]);
					p.CommentCount = Convert.ToInt32(reader["CommentCount"]);
					p.Published = Convert.ToBoolean(reader["Published"]);
					p.Categories = new Collection<Category>();

					// .. then the categories.
					IEnumerable<Category> cats =
						from entry in postcats
						where ((entry.Key / 10000) == p.Id)
						select entry.Value;

					foreach (Category cat in cats)
						p.Categories.Add(cat);

					postlist.Add(p);
				}
			}
			return postlist;

		}

		public static Collection<Category> CreateCategoriesFromReader(SqlDataReader reader)
		{
			Collection<Category> catlist = new Collection<Category>();
			if (reader.HasRows)
			{
				while (reader.Read())
				{

					Category c = new Category();

					c.Id = Convert.ToInt32(reader["Id"]);
					c.Name = Convert.ToString(reader["Name"]);
					c.Slug = Convert.ToString(reader["Slug"]);

					catlist.Add(c);
				}
			}

			return catlist;


		}
	}
}

