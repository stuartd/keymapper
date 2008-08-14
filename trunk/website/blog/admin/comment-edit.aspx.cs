using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class blog_admin_comment_edit : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
		((KMBlogMaster)Page.Master).SetTitle("Edit A Comment");
    }
}
