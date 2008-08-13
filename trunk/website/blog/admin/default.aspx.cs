using System;

namespace KMBlog
{
    public partial class _default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Redirect("admin.aspx");
        }
    }
}
