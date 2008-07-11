using System;
using System.Collections.ObjectModel;

namespace KMBlog
{
	internal interface IBloogData
	{

		#region posts

		Post GetPostByID(int postID);

		bool SavePost(Post p);

		bool DeletePost(int postID);

		#endregion

		#region categories

		bool AddCategoryToPost(int postID, int catID);

		bool DeleteCategoryFromPost(int postID, int catID);

		bool AddCategory(int categoryID, string categoryName);

		bool DeleteCategory(int categoryID);

        Collection<Category> GetAllCategories();

        bool SyncCategories(Collection<int> catlist, int postID);

		#endregion

		#region comments

		Collection<Comment> GetCommentsForPost(int postID);

		bool AddCommentToPost(int postID, string comment);

		bool DeleteComment(int commentID);

		#endregion
	}
}
