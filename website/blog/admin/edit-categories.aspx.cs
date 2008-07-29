using System;
using System.Collections.ObjectModel;

namespace KMBlog
{
    public partial class edit_categories : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
			Collection<Category> catlist = Category.GetAllCategories();
			rptCategories.DataSource = catlist;
			rptCategories.DataBind();
        }
    }
}
