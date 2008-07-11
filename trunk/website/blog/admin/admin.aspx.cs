using System;
using System.Data.SqlClient;
using System.Data;
using System.Data.SqlTypes;
using System.Configuration;
using System.Collections.ObjectModel;

namespace KMBlog
{
    public partial class admin : System.Web.UI.Page
    {

        string _connstring;

        protected void Page_Load(object sender, EventArgs e)
        {
            _connstring = ConfigurationManager.ConnectionStrings["default"].ConnectionString;
            //  _connstring = ConfigurationManager.ConnectionStrings["home.jks"].ConnectionString;


            GetPostList();

        }

        void GetPostList()
        {
            IDataAccess da = DataAccess.CreateInstance();

            Collection<Post> posts = da.GetAllPosts();

            postsRepeater.DataSource = posts;
            postsRepeater.DataBind();

        }


    }
}
