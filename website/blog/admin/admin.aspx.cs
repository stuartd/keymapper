namespace KMBlog
{
    using System;
    using System.Collections.ObjectModel;
    using System.Web.UI.WebControls;

    public partial class Admin : System.Web.UI.Page
    {
        public void DeletePost(object sender, CommandEventArgs e)
        {
            if (KMAuthentication.IsUserAdmin(User) == false)
            {
                return;
            }

            int postId;
            if (Int32.TryParse(e.CommandArgument.ToString(), out postId) == false)
            {
                return;
            }

            Post.Delete(postId);
            this.GetPostList();
        }

        public string GetCommentLinkText(int postId, int commentCount)
        {
            if (commentCount == 0)
            {
                return "No comments";
            }

            // href is edit-comments?p=
            // (TODO: This is the same code as the admin page's GetCommentLinkText
            // - need to refactor when can think of somewhere to put the code - 
            // in the comment class maybe?)
            string href = "\"edit-comments.aspx?p=" + postId.ToString() + "\"";
            string text = commentCount.ToString() + " comment" + ((commentCount != 1) ? "s" : string.Empty);

            return "<a href=" + href + ">" + text + "</a>";
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            ((KMBlogMaster)Page.Master).SetTitle("Key Mapper Blog Admin");

            if (KMAuthentication.IsUserAdmin(User) == false)
            {
                lblUserLevel.Text = "Demonstration Mode - you won't be able to save any changes";
            }
            else
            {
                lblUserLevel.Style.Add("Display", "None");
            }

            if (ClientScript.IsClientScriptBlockRegistered("confirmButtonScript") == false)
            {
                ClientScript.RegisterClientScriptBlock(
                    this.GetType(),
                    "confirmButtonScript",
                    "function __doConfirm(){if (confirm('Are you sure you want to delete this post?')){return true;}else{return false;}}",
                    true);
            }

            this.GetPostList();
        }

        private void GetPostList()
        {
            Collection<Post> posts = Post.GetAllPosts(CommentType.All);
            postsRepeater.DataSource = posts;
            postsRepeater.DataBind();
        }
    }
}
