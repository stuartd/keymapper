using System;
using System.Data.SqlClient;
using System.Data;
using System.Data.SqlTypes;
using System.Configuration;
using System.Collections.ObjectModel;
using System.Web.UI.WebControls;

namespace KMBlog
{
    public partial class post_delete : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
			if (User.IsInRole("Admin") == false)
				btnYes.Enabled = false;
        }


        public void DeletePost(object sender, EventArgs e)
        {

			if (User.IsInRole("Admin") == false)
				return;

            int postID = Post.GetPostIDFromQueryString(Request.QueryString);

            Post.Delete(postID);
            Response.Redirect("admin.aspx");
        }

        protected void CancelDelete(object sender, EventArgs e)
        {
            Response.Redirect("admin.aspx");
        }


    }
}
