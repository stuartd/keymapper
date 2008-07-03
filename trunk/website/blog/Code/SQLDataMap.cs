using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;



	public class SQLDataMap
	{

		public static Post CreatePostFromReader(SqlDataReader reader)
		{
			// First result set is the post..
			Post p = new Post();

			p.ID = Convert.ToInt32(reader["ID"]);
			p.Title = Convert.ToString(reader["Title"]);
			p.Postdate = Convert.ToDateTime(reader["PostDate"]);
			p.Body = Convert.ToString(reader["Body"]);
			p.Stub = Convert.ToString(reader["Stub"]);
			p.CommentCount = Convert.ToInt32(reader["CommentCount"]);

			// .. then the categories.

			p.Categories = new List<Category>();

			reader.NextResult();
			while (reader.Read())
			{
				p.Categories.Add(new Category(Convert.ToInt32(reader["ID"]), Convert.ToString(reader["Name"])));
			}

			return p;

		}


	}

