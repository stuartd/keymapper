using System;
using System.Collections.ObjectModel;
using System.Web.UI.WebControls;

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

        public void DeleteCategory(object sender, CommandEventArgs e)
        {
			// int CategoryID = 0;


        }

		public void SaveCategory(object sender, EventArgs e)
		{


		}
    }
}
