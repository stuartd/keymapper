using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using KMBlog;

public partial class CategoryEditor : System.Web.UI.UserControl
{
    public event EventHandler<EventArgs> CategorySaved;

    public int CategoryId
    {
        get
        {
            int catId;
            if (Int32.TryParse(fldCategoryId.Value, out catId))
            {
                return catId;
            }
            else
            {
                return 0;
            }
        }

        set
        {
            fldCategoryId.Value = value.ToString();
        }
    }

    public string Name
    {
        get
        {
            return txtCategoryName.Text;
        }

        set
        {
            txtCategoryName.Text = value;
        }
    }

    public string Slug
    {
        get
        {
            return txtCategorySlug.Text;
        }

        set
        {
            txtCategorySlug.Text = value;
        }
    }

    public void DisableSave()
    {
        // KMAuthentication.IsUserAdmin(Page.User)
        // in the control code doesn't work.
        btnSaveCategory.Enabled = false;
    }

    public void SetSaveAsDefaultButton()
    {
        Page.Form.DefaultButton = btnSaveCategory.UniqueID;
    }

    public void SaveCategory(object sender, EventArgs e)
    {
        if (KMAuthentication.IsUserAdmin(Page.User) == false)
        {
            return;
        }

        if (Page.IsValid == false)
        {
            return;
        }

        if (this.CategoryId != 0)
        {
            Category c = new Category(this.CategoryId, this.Name, this.Slug);
            if (c)
            {
                Category.Edit(c);
            }
        }
        else
        {
            Category.Add(this.Name, this.Slug);
            this.Name = String.Empty;
            this.Slug = String.Empty;
        }

        if (this.CategorySaved != null)
        {
            this.CategorySaved(null, null);
        }

        if (this.CategoryId != 0)
        {
            Response.Redirect("edit-categories.aspx");
        }
    }

    protected void ValidateName(object source, ServerValidateEventArgs args)
    {
        int catId;
        catId = Category.GetCategoryIdByName(args.Value);
        if (catId != 0 && catId != this.CategoryId)
        {
            args.IsValid = false;
        }
    }
}
