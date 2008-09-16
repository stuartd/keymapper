using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Collections.ObjectModel;

namespace KMBlog
{

	public interface IDataAccess
	{
		#region posts

		Collection<Post> GetAllPosts(CommentType ctype);

		//Collection<Post> GetAllPosts(int categoryId);

		//Collection<Post> GetAllPosts(DateTime startDate, DateTime endDate);

		Collection<Post> GetAllPosts(int categoryId, DateTime startDate, DateTime endDate, CommentType ctype);

		Collection<Post> GetAllPosts(int categoryId, DateTime startDate, DateTime endDate, CommentType ctype, int NumberOfPosts);

		Post GetPostById(int postId);

		int SavePost(Post p);

		void DeletePost(int postId);

        int GetPostIdFromSlug(string slug);

        #endregion

		#region categories

		void SyncCategories(int postId, Collection<int> categories);

		bool AddCategory(string categoryName, string categorySlug);

		bool DeleteCategory(int categoryId);

		bool EditCategory(Category cat);

		Collection<Category> GetAllCategories();

		int GetCategoryIdByName(string name);

		Category GetCategoryById(int categoryId);

        int GetCategoryIdFromSlug(string p);

		#endregion

		#region comments

		bool AddCommentToPost(Comment com);

		bool DeleteComment(int commentId);

		Collection<Comment> GetCommentsForPost(int postId, CommentType ctype);

		Collection<Comment> GetAllComments(CommentType ctype);

		bool ApproveComment(int commentId);

		#endregion

		#region Users

		int GetUserLevel(string userName, string passwordHash);

		#endregion

		#region Misc

		void LogDownload(string fileName, string ip, string referrer, string userAgent);

		DataTable GetArchives();

		#endregion
        
       
    }

	public class DataAccess
	{
		public static IDataAccess CreateInstance()
		{
			return new SqlBlogDataAccess();
		}
	}

}