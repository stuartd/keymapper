using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;


public partial class category_editor : System.Web.UI.UserControl
{

    int _categoryID;

    public event EventHandler<EventArgs> CategorySaved;

    public void DisableSave()
    {
        btnSaveCategory.Enabled = false;
    }

    public void SetSaveAsDefaultButton()
    {
        Page.Form.DefaultButton = btnSaveCategory.UniqueID;
    }

    public int CategoryID
    {
        get
        {
            return _categoryID;
        }
        set
        {
            _categoryID = value;
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


    public void SaveCategory(object sender, EventArgs e)
    {
        if (AppController.IsUserAdmin(Page.User) == false)
            return;

        if (Page.IsValid == false)
            return;

        Category.Add(Name, Slug);

        if (this.CategorySaved != null)
        {
            CategorySaved(null, null);
        }
        Name = String.Empty;
        Slug = String.Empty;
    }



    protected void CategoryNameExistsValidator_ServerValidate(object source, ServerValidateEventArgs args)
    {
        if (Category.DoesCategoryExist(args.Value))
        {
            args.IsValid = false;
        }
    }
}
