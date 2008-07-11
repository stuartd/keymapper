using System;
using System.Collections.ObjectModel;
using System.Data.SqlClient;


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

}
