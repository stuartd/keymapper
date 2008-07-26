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

            if (User.IsInRole("Admin"))
                Response.Write("User is admin");
            else
                Response.Write("User is not admin");

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

        public string GetCommentLinkText(int postID, int commentCount)
        {

            if (commentCount == 0)
                return "No comments";


            // href is edit-comments?p=
            // (TODO: This is the same code as the admin page's GetCommentLinkText
            // - need to refactor when can think of somewhere to put the code - 
            // in the comment class maybe?)
            string href = "\"edit-comments.aspx?p=" + postID.ToString() + "\"";
            string text;

            text = commentCount.ToString() + " comment";
            if (commentCount > 0)
                text += "s";

            String comment = "<a href=" + href + ">" + text + "</a>";

            return comment;

        }




    }
}
