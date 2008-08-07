using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml.Linq;

namespace KMBlog
{
    public partial class edit_comments : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            int postID = Post.GetPostIDFromQueryString(Request.QueryString);
            if (postID == 0)
            {
                ((KMBlogMaster)Page.Master).SetTitle("Edit Comments");
                
            }
        }
    }
}
