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

        }


        public void DeletePost(object sender, EventArgs e)
        {
            int postID = Post.GetPostIDFromQueryString(Request.QueryString);

            IDataAccess da = DataAccess.CreateInstance();
            da.DeletePost(postID);
            Response.Redirect("admin.aspx");
        }

        protected void CancelDelete(object sender, EventArgs e)
        {
            Response.Redirect("admin.aspx");
        }


    }
}
