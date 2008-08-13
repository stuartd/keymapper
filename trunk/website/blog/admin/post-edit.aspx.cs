using System;
using System.Web.UI;
using System.Collections.ObjectModel;
using System.Web.UI.WebControls;


namespace KMBlog
{
    public partial class post_edit : System.Web.UI.Page
    {

        public void CancelEdit(object sender, EventArgs e)
        {
            Response.Redirect("admin.aspx");
        }

        private void LoadMonthNames()
        {
            postmonth.Items.Add("January");
            postmonth.Items.Add("February");
            postmonth.Items.Add("March");
            postmonth.Items.Add("April");
            postmonth.Items.Add("May");
            postmonth.Items.Add("June");
            postmonth.Items.Add("July");
            postmonth.Items.Add("August");
            postmonth.Items.Add("September");
            postmonth.Items.Add("October");
            postmonth.Items.Add("November");
            postmonth.Items.Add("December");
        }

        protected void Page_Load(object sender, EventArgs e)
        {

			((KMBlogMaster)Page.Master).SetTitle("Blog Post Editor");

			if (KMAuthentication.IsUserAdmin(User) == false)
			{
				btnSavePost.Enabled = false;
				btnPublishPost.Enabled = false;

			}

            if (Page.IsPostBack == false)
            {
                LoadMonthNames();
                GetPost();
            }
        }

        private void GetPost()
        {
            int PostId = Post.GetPostIdFromQueryString(Request.QueryString);

            Collection<Category> allCats = Category.GetAllCategories();

            if (PostId != 0)
            {

                hiddenPostId.Value = PostId.ToString();

                Post p = Post.GetPostById(PostId);

                if (p == null)
                    editarea.Style.Add("display", "none");
                else
                {

                    blogpost.Text = p.Body;
                    posttitle.Text = p.Title;
                    postslug.Text = p.Slug;
                    DateTime postdate = p.Postdate;
                    postyear.Text = postdate.Year.ToString();
                    postday.Text = postdate.Day.ToString();
                    postmonth.Text = postdate.ToString("MMMM");
                    hiddenPostId.Value = PostId.ToString();

                    // Select the categories this post belongs to

                    foreach (Category cat in allCats)
                    {
                        System.Web.UI.WebControls.ListItem item = new System.Web.UI.WebControls.ListItem();
                        item.Text = cat.Name;
                        bool postInCategory = false;
                        foreach (Category c in p.Categories)
                        {
                            if (c.Id == cat.Id && c.Name == cat.Name)
                            {
                                postInCategory = true;
                                break;
                            }
                        }
                        item.Selected = postInCategory;
                        item.Value = cat.Id.ToString();
                        CatList.Items.Add(item);
                    }

                }
            }
            else
            {
                // New post.
                postyear.Text = DateTime.Now.Year.ToString();
                postday.Text = DateTime.Now.Day.ToString();
                postmonth.Text = DateTime.Now.ToString("MMMM");
                slugdiv.Style.Add("Display", "None");

                foreach (Category cat in allCats)
                {
                    System.Web.UI.WebControls.ListItem item = new System.Web.UI.WebControls.ListItem();
                    item.Text = cat.Name;
                    item.Value = cat.Id.ToString();
                    CatList.Items.Add(item);
                }
            }
        }



        private DateTime GetPostDate()
        {
            DateTime postdatetime = DateTime.MinValue;

            if (String.IsNullOrEmpty(postday.Text) || String.IsNullOrEmpty(postmonth.Text) || String.IsNullOrEmpty(postyear.Text))
                return postdatetime;

			string postdate = postday.Text + " " + postmonth.Text + " " + postyear.Text;

			if (DateTime.TryParse(postdate, out postdatetime))
			{
				if (postdatetime.Date == DateTime.Now.Date)
					postdatetime = DateTime.Now ;
			}

			return postdatetime;

        }

        private string GetPostDateErrors()
        {

            if (String.IsNullOrEmpty(postday.Text) || String.IsNullOrEmpty(postmonth.Text) || String.IsNullOrEmpty(postyear.Text))
                return "The Day, Month and Year must all be entered and can't be blank";

            // So why is this not a valid date?

            int year = Int32.Parse(postyear.Text);
            if (year < 1 || year > 9999)
                return "The Year must be between 0001 and 9999";

            // Check the month text is valid:

            DateTime dt;
            int month;

            if (DateTime.TryParse("01 " + postmonth.Text + " 1900", out dt) == false)
                return postmonth.Text + " is not a valid month";
            else
                month = dt.Month;

            // Now check that the day is valid for the month and year.

            int day = Int32.Parse(postday.Text);
            if (day < 1 || day > DateTime.DaysInMonth(year, month))
                return day.ToString() + " is not a valid day for " + postmonth.Text + " " + postyear.Text;

            return "The date is not valid";


        }

        public void SavePost(object sender, CommandEventArgs e)
        {

			if (KMAuthentication.IsUserAdmin(User) == false)
				return;

            Page.Validate();

            bool pageIsValid = Page.IsValid;

            DateTime dt = GetPostDate();
            if (dt == DateTime.MinValue)
            {
                string errors = GetPostDateErrors();
                date_error.Text = errors;
                pageIsValid = false;
            }
            else
                date_error.Text = String.Empty;

            if (pageIsValid == false)
            {
                return;
            }

            Post p = new Post();

            int PostId;
            if (Int32.TryParse(hiddenPostId.Value, out PostId) == false)
            {
                PostId = 0;
                p.Slug = GetSlug(posttitle.Text);
            }
            else
            {
				p.Slug = postslug.Text;
            }

            p.Id = PostId;

            p.Title = posttitle.Text;
            p.Body = blogpost.Text;
            p.Postdate = dt;

            p.Published = (e.CommandName == "Publish");

            if (Post.Save(p))
            {
                SyncCategories(p.Id);

                if (p.Published)
                    Response.Redirect("admin.aspx");
                else if (PostId == 0)
                    Response.Redirect("post-edit.aspx?p=" + Convert.ToString(p.Id));
            }
        }


        bool SyncCategories(int PostId)
        {
            Collection<int> categories = new Collection<int>();

            foreach (System.Web.UI.WebControls.ListItem item in CatList.Items)
            {
                if (item.Selected)
                {
                    int catId;
                    if (Int32.TryParse(item.Value, out catId))
                        categories.Add(catId);

                }
            }

            Collection<Category> postcats;
			if (PostId == 0)
				postcats = new Collection<Category>();
			else
				postcats = Category.GetCategoriesForPost(PostId);

            bool categoriesChanged = false;
            if (postcats.Count == categories.Count)
            {
                foreach (Category cat in postcats)
                {
                    if (categories.Contains(cat.Id) == false)
                    {
                        categoriesChanged = true;
                        break;
                    }
                }
            }
            else
            {
                categoriesChanged = true;
            }

            if (categoriesChanged == false)
                return true;

			Category.SyncCategories(PostId, categories); 
			
            return true;
        }


        string GetSlug(string title)
        {

            string slug = CommonMethods.GetSlug(title);

            int suffix = 1;
            while (String.IsNullOrEmpty(slug) || DoesSlugAlreadyExist(slug) == true)
            {
                slug += suffix.ToString();
                suffix++;
            }

            return slug;


        }

        bool DoesSlugAlreadyExist(string slug)
        {
		       return Post.DoesSlugExist(slug);
        }

        protected void RegenerateSlug(object sender, EventArgs e)
        {
            string title = posttitle.Text;
            if (String.IsNullOrEmpty(title))
                return;

            string slug = GetSlug(title);
            if (slug != postslug.Text)
                postslug.Text = slug;
        }









    }

}





