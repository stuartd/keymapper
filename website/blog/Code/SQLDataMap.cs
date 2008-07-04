using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;



public class SQLDataMap
{

	public static Collection<Post> CreatePostsFromReader(SqlDataReader reader)
	{
		// First result set is the postcategories.

		// Put these in a dictionary.
		Dictionary<int, Category> postcats = new Dictionary<int, Category>();

		while (reader.Read())
		{
			postcats.Add(Convert.ToInt32(reader["postID"]), 
				new Category(Convert.ToInt32(reader["categoryID"]), Convert.ToString(reader["Name"])));
		}

		// Second resultset is the categories
		reader.NextResult();

				Collection<Post> postlist = new Collection<Post>();

		while (reader.Read())
		{

			Post p = new Post();

			p.ID = Convert.ToInt32(reader["ID"]);
			p.Title = Convert.ToString(reader["Title"]);
			p.Postdate = Convert.ToDateTime(reader["PostDate"]);
			p.Body = Convert.ToString(reader["Body"]);
			p.Stub = Convert.ToString(reader["Stub"]);
			p.CommentCount = Convert.ToInt32(reader["CommentCount"]);
			p.Categories = new List<Category>();

			// .. then the categories.
			IEnumerable<Category> cats =
				from entry in postcats
				where (entry.Key == p.ID)
				select entry.Value;

			foreach (Category cat in cats)
				p.Categories.Add(cat);

			postlist.Add(p);
		}

		return postlist;

	}


}

