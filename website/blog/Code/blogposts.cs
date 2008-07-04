using System;
using System.Collections.ObjectModel ;


    public class BlogPosts
    {

		public Collection<Post> GetPosts(int PostID, DateTime startDate, DateTime endDate, int CategoryID)
		{
			return new Collection<Post>() ;
		}


    }
