using System;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI.HtmlControls;

namespace KMBlog
{
    public partial class post_edit : System.Web.UI.Page
    {


        protected void Page_Load(object sender, EventArgs e)
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


            int postID = GetPostID();

            if (postID != 0)
            {

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

                            }
                        }
                    }
                }
            }

        }


        protected int GetPostID()
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

        public void SavePost(Object sender, EventArgs e)
        {

            int postID = GetPostID();

            if (postID == 0)
            {

                string cs = ConfigurationManager.ConnectionStrings["default"].ConnectionString;

                using (SqlConnection connection = new SqlConnection(cs))
                {
                    if (connection != null)
                    {
                        connection.Open();
                        SqlCommand sc = new SqlCommand("CreatePost");
                        sc.Connection = connection;
                        sc.Parameters.AddWithValue("title", blogtitle.Text);
                        sc.Parameters.AddWithValue("stub", GetStub(blogtitle.Text));
                        sc.Parameters.AddWithValue("body", blogpost);
                        sc.CommandType = CommandType.StoredProcedure;

                        int result = sc.ExecuteNonQuery();

                    }
                }
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

}



