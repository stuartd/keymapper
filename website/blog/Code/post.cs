using System;
using System.Collections.Generic;
using System.Data.SqlClient;


public class Post
{

	public int ID { get; set; }
	public string Title { get; set; }
	public List<Category> Categories { get; set; }
	public DateTime Postdate { get; set; }
	public string Body { get; set; }
	public string Stub { get; set; }
	public int CommentCount { get; set; }

	public static Post CreateNewPost()
	{
		Post p = new Post();
		p.Title = String.Empty;
		p.Stub = String.Empty;
		p.Postdate = DateTime.Now;
		p.Categories = new List<Category>();
		p.CommentCount = 0;

		return p;
	}

}
