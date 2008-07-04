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


/// <summary>
/// Summary description for DataAccess
/// </summary>
public interface IDataAccess
{
	#region posts

	Collection<Post> GetAllPosts();

	Collection<Post> GetAllPosts(int categoryID);

	Collection<Post> GetAllPosts(DateTime startDate, DateTime endDate);

	Collection<Post> GetAllPosts(int categoryID, DateTime startDate, DateTime endDate);

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

	bool AddCommentToPost(int postID, string comment);

	bool DeleteComment(int commentID);

	#endregion
}

internal class DataAccess
{
	internal static IDataAccess CreateInstance()
	{
		return new SQLBlogDataAccess();
	}
}