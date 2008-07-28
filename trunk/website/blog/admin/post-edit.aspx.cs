using System;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI;
using System.Text;
using System.Collections.ObjectModel;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;


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

			if (User.IsInRole("Admin") == false)
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
            int postID = Post.GetPostIDFromQueryString(Request.QueryString);

            IDataAccess da = DataAccess.CreateInstance();

            Collection<Category> allCats = da.GetAllCategories();

            if (postID != 0)
            {

                hiddenPostID.Value = postID.ToString();

                Post p = da.GetPostByID(postID);

                if (p == null)
                    form1.Style.Add("display", "none");
                else
                {

                    blogpost.Text = p.Body;
                    posttitle.Text = p.Title;
                    poststub.Text = p.Stub;
                    DateTime postdate = p.Postdate;
                    postyear.Text = postdate.Year.ToString();
                    postday.Text = postdate.Day.ToString();
                    postmonth.Text = postdate.ToString("MMMM");
                    hiddenPostID.Value = postID.ToString();

                    // Select the categories this post belongs to

                    foreach (Category cat in allCats)
                    {
                        System.Web.UI.WebControls.ListItem item = new System.Web.UI.WebControls.ListItem();
                        item.Text = cat.Name;
                        bool postInCategory = false;
                        foreach (Category c in p.Categories)
                        {
                            if (c.ID == cat.ID && c.Name == cat.Name)
                            {
                                postInCategory = true;
                                break;
                            }
                        }
                        item.Selected = postInCategory;
                        item.Value = cat.ID.ToString();
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
                stubdiv.Style.Add("Display", "None");

                foreach (Category cat in allCats)
                {
                    System.Web.UI.WebControls.ListItem item = new System.Web.UI.WebControls.ListItem();
                    item.Text = cat.Name;
                    item.Value = cat.ID.ToString();
                    CatList.Items.Add(item);
                }
            }
        }



        private DateTime GetPostDate()
        {
            DateTime dt = DateTime.MinValue;

            if (String.IsNullOrEmpty(postday.Text) || String.IsNullOrEmpty(postmonth.Text) || String.IsNullOrEmpty(postyear.Text))
                return dt;

            string postdate = postday.Text + " " + postmonth.Text + " " + postyear.Text;

            DateTime.TryParse(postdate, out dt);
            return dt;

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

			if (User.IsInRole("Admin") == false)
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

            int postID;
            if (Int32.TryParse(hiddenPostID.Value, out postID) == false)
            {
                postID = 0;
                p.Stub = GetStub(posttitle.Text);
            }
            else
            {
                p.Stub = poststub.Text;
            }

            p.ID = postID;

            p.Title = posttitle.Text;
            p.Body = blogpost.Text;
            p.Postdate = dt;

            p.Published = (e.CommandName == "Publish");

            if (Post.Save(p))
            {
                SyncCategories(p.ID);

                if (p.Published)
                    Response.Redirect("admin.aspx");
                else if (postID == 0)
                    Response.Redirect("post-edit.aspx?p=" + Convert.ToString(p.ID));
            }
        }

        bool SyncCategories(int postID)
        {
            Collection<int> categories = new Collection<int>();

            foreach (System.Web.UI.WebControls.ListItem item in CatList.Items)
            {
                if (item.Selected)
                {
                    int catID;
                    if (Int32.TryParse(item.Value, out catID))
                        categories.Add(catID);

                }
            }

            IDataAccess da = DataAccess.CreateInstance();
            Collection<Category> postcats;
            if (postID == 0)
                postcats = new Collection<Category>();
            else
                postcats = (da.GetPostByID(postID).Categories);

            bool categoriesChanged = false;
            if (postcats.Count == categories.Count)
            {
                foreach (Category cat in postcats)
                {
                    if (categories.Contains(cat.ID) == false)
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

            da.SyncCategories(postID, categories);


            return true;
        }


        string GetStub(string title)
        {

            string stub = Regex.Replace(title.Replace(" ", "-"), @"[^\w\-]", "").ToLower();

            int suffix = 1;
            while (String.IsNullOrEmpty(stub) || DoesStubAlreadyExist(stub) == true)
            {
                stub += suffix.ToString();
                suffix++;
            }

            return stub;


        }

        bool DoesStubAlreadyExist(string stub)
        {

            IDataAccess da = DataAccess.CreateInstance();
            return da.DoesStubExist(stub);

        }

        protected void RegenerateStub(object sender, EventArgs e)
        {
            string title = posttitle.Text;
            if (String.IsNullOrEmpty(title))
                return;

            string stub = GetStub(title);
            poststub.Text = stub;
        }









    }

}





