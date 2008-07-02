using System;
using System.Collections.Generic;

namespace KMBlog
{
    public class Post
    {

        public int ID { get; private set; }
        public string Title { get; set; }
        public List<Category> Categories { get; private set; }
        public DateTime Postdate { get; set; }

        public static Post CreateNewPost()
        {
			Post p = new Post();
            p.Title = String.Empty;
            p.Postdate = DateTime.Now;
            p.Categories = new List<Category>();

			return p;
        }

    }
}