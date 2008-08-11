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
using KMBlog;


	public partial class category_edit : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{

			((KMBlogMaster)Page.Master).SetTitle("Edit A Category");

			if (Page.IsPostBack)
				return;

            editcategory.CategorySaved += new EventHandler<EventArgs>(CategorySaved);
            int categoryId = Category.GetCategoryIdFromQueryString(Request.QueryString);

            // Load category
            Category c = Category.GetCategoryById(categoryId);
            if (categoryId == 0 || c == null)
            {
                lblCategoryDoesNotExist.Text = "The requested category does not exist. Perhaps it has been deleted?";
                editcategory.Visible = false;
            }
            else
            {
                editcategory.Name = c.Name;
                editcategory.Slug = c.Slug;
                editcategory.CategoryId = c.Id;
            }

		}

        void CategorySaved(object sender, EventArgs e)
        {
            Response.Redirect("edit-categories.aspx");
        }
	}

