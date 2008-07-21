using System;
using System.Data.SqlClient;
using System.Data;
using System.Data.SqlTypes;
using System.Configuration;
using System.Collections.ObjectModel;
using System.Web.UI.WebControls;

namespace KMBlog
{
    public partial class admin : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            GetPostList();
        }

        void GetPostList()
        {
            IDataAccess da = DataAccess.CreateInstance();

            Collection<Post> posts = da.GetAllPosts();

            postsRepeater.DataSource = posts;
            postsRepeater.DataBind();

        }

        public void DeletePost(object sender, CommandEventArgs e)
        {

            int postID;
            if (Int32.TryParse(e.CommandArgument.ToString(), out postID) == false)
                return;

            Response.Redirect("post-delete.aspx?p=" + postID.ToString());

        }



    }
}
