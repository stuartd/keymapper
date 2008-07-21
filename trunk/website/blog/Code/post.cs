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
	public string Stub { get; set; }
	public int CommentCount { get; set; }
    public int Published { get; set; }

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

}
