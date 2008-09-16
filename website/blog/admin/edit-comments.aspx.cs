using System;
using System.Collections.ObjectModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using KMBlog;

public partial class EditComments : System.Web.UI.Page
{
    public void DeleteComment(object sender, CommandEventArgs e)
    {
        if (KMAuthentication.IsUserAdmin(User) == false)
        {
            return;
        }

        int commentId;
        if (Int32.TryParse(e.CommandArgument.ToString(), out commentId) == false)
        {
            return;
        }

        Comment.Delete(commentId);
        this.LoadPostComments();
    }

    public void ApproveComment(object sender, CommandEventArgs e)
    {
        if (KMAuthentication.IsUserAdmin(User) == false)
        {
            return;
        }

        int commentId;
        if (Int32.TryParse(e.CommandArgument.ToString(), out commentId) == false)
        {
            return;
        }

        Comment.Approve(commentId);
        this.LoadPostComments();
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        ((KMBlogMaster)Page.Master).SetTitle("Edit Comments");

        int postId = Post.GetPostIdFromQueryString(Request.QueryString);
        if (postId == 0)
        {
            this.LoadAllComments();
        }
        else
        {
            this.LoadPostComments();
        }
    }

    private void LoadAllComments()
    {
    }

    private void LoadPostComments()
    {
        int postId = Post.GetPostIdFromQueryString(Request.QueryString);
        Post p = Post.GetPostById(postId);
        postname.Text = p.Title;

        Collection<Comment> comlist = Comment.GetComments(postId, CommentType.All);
        comments.DataSource = comlist;
        comments.DataBind();
    }
}

