using System;
using System.Web.UI;
using KMBlog;


	public partial class category_edit : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{

			((KMBlogMaster)Page.Master).SetTitle("Edit A Category");

			if (KMAuthentication.IsUserAdmin(User) == false)
				editcategory.DisableSave();

			if (Page.IsPostBack)
				return;

            editcategory.CategorySaved += CategorySaved;
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

