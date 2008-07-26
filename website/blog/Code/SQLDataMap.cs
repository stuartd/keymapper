using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;



public class SQLDataMap
{

    public static Collection<Comment> CreateCommentsFromReader(SqlDataReader reader)
    {

        Collection<Comment> commentlist = new Collection<Comment>();
        if (reader.HasRows)
        {
            while (reader.Read())
            {

                Comment c = new Comment();

                c.ID = Convert.ToInt32(reader["ID"]);
                c.PostID = Convert.ToInt32(reader["PostID"]);
                c.Text = Convert.ToString(reader["text"]);
                c.Name = Convert.ToString(reader["name"]);
                c.URL = Convert.ToString(reader["url"]);

                commentlist.Add(c);
            }
        }

        return commentlist;
     



    }

    public static Collection<Post> CreatePostsFromReader(SqlDataReader reader)
    {
        // First result set is the postcategories. 

        Dictionary<int, Category> postcats = new Dictionary<int, Category>() ;
        if (reader.HasRows)
        {
            while (reader.Read())
            {

                // Oops - key can't be PostID as key has to be unique. Need to use a different collection..
                int categoryID = Convert.ToInt32(reader["categoryID"]) ;
                int postHash = (Convert.ToInt32(reader["postID"]) * 10000) + categoryID ;
                postcats.Add(postHash, new Category(categoryID, Convert.ToString(reader["Name"])));
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

                p.ID = Convert.ToInt32(reader["ID"]);
                p.Title = Convert.ToString(reader["Title"]);
                p.Postdate = Convert.ToDateTime(reader["PostDate"]);
                p.Body = Convert.ToString(reader["Body"]);
                p.Stub = Convert.ToString(reader["Stub"]);
                p.CommentCount = Convert.ToInt32(reader["CommentCount"]);
                p.Published = Convert.ToBoolean(reader["Published"]);
                p.Categories = new Collection<Category>();

                // .. then the categories.
                IEnumerable<Category> cats =
                    from entry in postcats
                    where ((entry.Key / 10000) == p.ID)
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

                c.ID = Convert.ToInt32(reader["ID"]);
                c.Name = Convert.ToString(reader["Name"]);

                catlist.Add(c);
            }
        }

        return catlist;


    }



}

