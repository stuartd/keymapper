using System;
using System.Web.UI;
using System.Web.UI.WebControls;


public partial class CategoryEditor : System.Web.UI.UserControl
{

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
			int catID;
			if (Int32.TryParse(fldCategoryID.Value, out catID))
				return catID;
			else
				return 0;
		}
		set
		{
			fldCategoryID.Value = value.ToString();
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
		if (Authentication.IsUserAdmin(Page.User) == false)
			return;

		if (Page.IsValid == false)
			return;

		if (CategoryID != 0)
		{
			Category c = new Category(CategoryID, Name, Slug);
			if (c)
			{
				Category.Edit(c);
			}
		}
		else
		{
			Category.Add(Name, Slug);
			Name = String.Empty;
			Slug = String.Empty;
		}

		if (this.CategorySaved != null)
		{
			CategorySaved(null, null);
		}

		if (CategoryID != 0)
			Response.Redirect("edit-categories.aspx");

	}



	protected void CategoryNameExistsValidator_ServerValidate(object source, ServerValidateEventArgs args)
	{
		int catID;
		catID = Category.GetCategoryIDByName(args.Value);
		if (catID != 0 && catID != CategoryID)
		{
			args.IsValid = false;
		}
	}
}
