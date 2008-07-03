using System;
using System.Collections.Generic;

namespace KMBlog
{
	internal interface IBlogData
	{

		#region posts

		Post GetPostByID(int postID);

		bool AddPost(Post p);

		bool EditPost(Post p);

		bool DeletePost(int postID);

		#endregion

		#region categories

		bool AddCategoryToPost(int postID, int catID);

		bool DeleteCategoryFromPost(int postID, int catID);

		bool AddCategory(int categoryID, string categoryName);

		bool DeleteCategory(int categoryID);

		#endregion

		#region comments

		List<Comment> GetCommentsForPost(int postID);

		bool AddCommentToPost(int postID, string comment);

		bool DeleteComment(int commentID);

		#endregion
	}
}
