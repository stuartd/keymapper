using System;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Collections.Specialized;


public class Post
{

    public int ID { get; set; }
    public string Title { get; set; }
    public Collection<Category> Categories { get; set; }
    public DateTime Postdate { get; set; }
    public string Body { get; set; }
    public string Slug { get; set; }
    public int CommentCount { get; set; }
    public bool Published { get; set; }

    public static int GetPostIDFromQueryString(NameValueCollection parameters)
    {

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

    public static void Delete(int postID)
    {
        IDataAccess da = DataAccess.CreateInstance();
        da.DeletePost(postID);

    }

    public static Collection<Post> GetAllPosts()
    {

        IDataAccess da = DataAccess.CreateInstance();

        Collection<Post> posts = da.GetAllPosts();

        return posts;
    }

    public static bool Save(Post p)
    {

        if (!p)
            return false;

        IDataAccess da = DataAccess.CreateInstance();
        int savedPostID = da.SavePost(p);
        p.ID = savedPostID;
        return (p.ID > 0);
    }

    public static implicit operator bool(Post p)
    {
        return !(string.IsNullOrEmpty(p.Slug) || string.IsNullOrEmpty(p.Title));
    }



}
