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

    //Collection<Post> GetAllPosts(int categoryID);

    //Collection<Post> GetAllPosts(DateTime startDate, DateTime endDate);

    Collection<Post> GetAllPosts(int categoryID, DateTime startDate, DateTime endDate);

	Collection<Post> GetAllPosts(int categoryID, DateTime startDate, DateTime endDate, int NumberOfPosts);

	Post GetPostByID(int postID);

	int SavePost(Post p);

	void DeletePost(int postID);

    bool DoesStubExist(string stub);

	#endregion

	#region categories

    void SyncCategories(int postID, Collection<int> categories);

	bool AddCategory(int categoryID, string categoryName);

	bool DeleteCategory(int categoryID);

    Collection<Category> GetAllCategories();

	#endregion

	#region comments

	bool AddCommentToPost(int postID, string comment);

	bool DeleteComment(int commentID);

    Collection<Comment> GetCommentsForPost(int postID);

	#endregion


    #region Users

    int GetUserLevel(string username, string passwordhash);


    #endregion

}

internal class DataAccess
{
	internal static IDataAccess CreateInstance()
	{
		return new SQLBlogDataAccess();
	}
}