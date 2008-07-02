using System;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI;
using System.Text;


public partial class EditPost : System.Web.UI.Page
{

    public void CancelEdit(object sender, EventArgs e)
    {
        Response.Redirect("admin.aspx");
    }

    protected void Page_Load(object sender, EventArgs e)
    {

        if (Page.IsPostBack)
        {
            if (SavePost())
            {
                resultlabel.Text = "Your post has been saved.";
            }
            else
            {
                resultlabel.Text = "There are some missing fields: your post has not been saved.";
            }
            return;
        }

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

        int postID = GetPostIDFromQueryString();

        if (postID != 0)
        {

            hiddenPostID.Value = postID.ToString();

            string cs = ConfigurationManager.ConnectionStrings["default"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(cs))
            {
                if (connection != null)
                {

                    connection.Open();
                    SqlCommand sc = new SqlCommand();

                    sc.Connection = connection;

                    sc.CommandText = "GetPostByID";
                    sc.Parameters.AddWithValue("PostID", postID);
                    sc.CommandType = CommandType.StoredProcedure;

                    form1.Style.Add("display", "none");

                    using (SqlDataReader dr = sc.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            // Only one result will be returned.
                            form1.Style.Remove("display");
                            form1.Style.Add("display", "block");
                            blogpost.InnerHtml = dr["body"].ToString();
                            blogtitle.Text = dr["title"].ToString();
                            DateTime postdate = (DateTime)dr["postdate"];
                            postyear.Text = postdate.Year.ToString();
                            postday.Text = postdate.Day.ToString();
                            postmonth.Text = postdate.ToString("MMMM");
                            hiddenPostID.Value = postID.ToString();


                        }
                    }
                }
            }
        }
        else
        {
            // New post.
            postyear.Text = DateTime.Now.Year.ToString();
            postday.Text = DateTime.Now.Day.ToString();
            postmonth.Text = DateTime.Now.ToString("MMMM");
        }
    }


    protected int GetPostIDFromQueryString()
    {

        NameValueCollection parameters = Request.QueryString;

        string[] keys = parameters.AllKeys;

        int postID = 0;
        foreach (string key in keys)
        {
            if (key.ToUpperInvariant() == "P")
            {
                foreach (string value in parameters.GetValues(key))
                {
                    if (System.Int32.TryParse(value, out postID))
                    {
                        break;
                    }
                }
            }
        }
        return postID;
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
            return "The Day, Month and Year fields must all be entered";

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

    public bool SavePost()
    {


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
            return false;
        }

        int postID;
        if (Int32.TryParse(hiddenPostID.Value, out postID) == false)
            postID = 0;

        string cs = ConfigurationManager.ConnectionStrings["default"].ConnectionString;

        using (SqlConnection connection = new SqlConnection(cs))
        {
            if (connection == null)
                return false;

            connection.Open();

            SqlCommand sc;

            if (postID == 0)
            {
                sc = new SqlCommand("CreatePost");
                sc.Parameters.AddWithValue("stub", GetStub(blogtitle.Text));
            }
            else
            {
                sc = new SqlCommand("EditPost");
                sc.Parameters.AddWithValue("PostID", postID);
            }

            sc.Parameters.AddWithValue("title", blogtitle.Text);
            sc.Parameters.AddWithValue("body", blogpost.InnerHtml);
            sc.Parameters.AddWithValue("postdate", dt);

            sc.Connection = connection;
            sc.CommandType = CommandType.StoredProcedure;

            int result = sc.ExecuteNonQuery();

            return (result > -2);

        }

    }

    string GetStub(string title)
    {
        string stub = title.Replace(" ", "-").ToLower();
        int suffix = 1;
        while (DoesStubAlreadyExist(stub) == true)
        {
            stub += suffix.ToString();
            suffix++;
        }

        return stub;


    }

    bool DoesStubAlreadyExist(string stub)
    {

        string cs = ConfigurationManager.ConnectionStrings["default"].ConnectionString;

        using (SqlConnection connection = new SqlConnection(cs))
        {
            if (connection != null)
            {

                connection.Open();
                SqlCommand sc = new SqlCommand();

                sc.Connection = connection;

                sc.CommandText = "DoesStubExist";
                sc.Parameters.AddWithValue("Stub", stub);
                sc.CommandType = CommandType.StoredProcedure;

                bool exists = ((int)sc.ExecuteScalar() != 0);

                return exists;
            }
            else
            {
                return true; // If can't determine, assume it exists
            }
        }

    }


}



