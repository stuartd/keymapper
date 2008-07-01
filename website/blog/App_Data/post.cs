using System;
using System.Collections.Generic;

namespace KMBlog
{
    public class post
    {

        public int ID { get; private set; }
        public string Title { get; set; }
        public List<Category> Categories { get; private set; }
        public DateTime Postdate { get; set; }

        public post CreateNewPost()
        {
            Title = String.Empty;
            Postdate = DateTime.Now;
            Categories = new List<Category>();
        }

        public void AddCategoryToPost() { }

        public void DeleteCategoryFromPost() { }

        public void DeletePost(int id) { }


    }
}