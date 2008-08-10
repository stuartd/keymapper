using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Collections.ObjectModel;
using System.Configuration;

public class SQLBlogDataAccess : IDataAccess
{

    #region IDataAccess Members

    #region Posts

    public Collection<Post> GetAllPosts()
    {
        return GetAllPosts(0, SqlDateTime.MinValue.Value, SqlDateTime.MaxValue.Value, 999);
    }

    //public Collection<Post> GetAllPosts(int categoryID)
    //{
    //    return GetAllPosts(categoryID, SqlDateTime.MinValue.Value, SqlDateTime.MaxValue.Value);
    //}

    //public Collection<Post> GetAllPosts(DateTime startDate, DateTime endDate)
    //{
    //    return GetAllPosts(0, startDate, endDate);
    //}

    public Collection<Post> GetAllPosts(int categoryID, DateTime startDate, DateTime endDate)
    {
        return GetAllPosts(categoryID, startDate, endDate, 10);
    }

    public Collection<Post> GetAllPosts(int categoryID, DateTime startDate, DateTime endDate, int NumberOfPosts)
    {

        int numberOfPosts = 10;
        Collection<Post> posts = new Collection<Post>();

        using (SqlConnection connection = GetConnection())
        {
            connection.Open();

            SqlCommand sc = new SqlCommand();

            sc.CommandText = "GetAllPosts";

            sc.Parameters.AddWithValue("NumberOfPosts", numberOfPosts);
            sc.Parameters.AddWithValue("CategoryID", categoryID);
            sc.Parameters.AddWithValue("DateFrom", startDate);
            sc.Parameters.AddWithValue("DateTo", endDate);

            sc.CommandType = CommandType.StoredProcedure;
            sc.Connection = connection;

            using (SqlDataReader reader = sc.ExecuteReader())
            {
                posts = SQLDataMap.CreatePostsFromReader(reader);
            }


        }



        return posts;
    }

    public Post GetPostByID(int postID)
    {
        Collection<Post> posts = new Collection<Post>();

        using (SqlConnection connection = GetConnection())
        {

            connection.Open();

            SqlCommand sc = new SqlCommand();

            sc.CommandText = "GetPostByID";

            sc.Parameters.AddWithValue("PostID", postID);

            sc.CommandType = CommandType.StoredProcedure;
            sc.Connection = connection;

            using (SqlDataReader reader = sc.ExecuteReader())
            {
                posts = SQLDataMap.CreatePostsFromReader(reader);
            }
        }

        if (posts.Count > 0)
            return posts[0];
        else
            return null;

    }

    public int SavePost(Post p)
    {
        using (SqlConnection connection = GetConnection())
        {

            connection.Open();

            SqlCommand sc = new SqlCommand();

            if (p.ID == 0)
            {
                sc.CommandText = "CreatePost";
                SqlParameter sp = new SqlParameter("NewPostID", 0);
                sp.Direction = ParameterDirection.Output;
                sc.Parameters.Add(sp);
            }
            else
            {
                sc.CommandText = "EditPost";
                sc.Parameters.AddWithValue("PostID", p.ID);
            }

            sc.Parameters.AddWithValue("slug", p.Slug);
            sc.Parameters.AddWithValue("title", p.Title);
            sc.Parameters.AddWithValue("body", p.Body);
            sc.Parameters.AddWithValue("postdate", p.Postdate);
            sc.Parameters.AddWithValue("published", p.Published);

            sc.Connection = connection;
            sc.CommandType = CommandType.StoredProcedure;

            int result = sc.ExecuteNonQuery();

            if (p.ID == 0)
                return Convert.ToInt32(sc.Parameters["NewPostID"].Value);
            else
                return p.ID;



        }


    }

    public bool DoesSlugExist(string slug)
    {

        using (SqlConnection connection = GetConnection())
        {

            connection.Open();
            SqlCommand sc = new SqlCommand("DoesSlugExist", connection);
            sc.Parameters.AddWithValue("Slug", slug);
            sc.CommandType = CommandType.StoredProcedure;

            bool exists = ((int)sc.ExecuteScalar() != 0);

            return exists;

        }
    }

    public void DeletePost(int postID)
    {
        using (SqlConnection connection = GetConnection())
        {
            connection.Open();

            SqlCommand sc = new SqlCommand("DeletePost", connection);
            sc.CommandType = CommandType.StoredProcedure;
            sc.Parameters.AddWithValue("PostID", postID);

            sc.ExecuteNonQuery();

        }

    }

    #endregion

    #region Categories

    public Category GetCategoryByID(int categoryID)
    {
        Collection<Category> cats;

        using (SqlConnection connection = GetConnection())
        {
            connection.Open();
            SqlCommand sc = new SqlCommand("GetCategoryByID", connection);

            sc.CommandType = CommandType.StoredProcedure;
            sc.Parameters.AddWithValue("CategoryID", categoryID);

            using (SqlDataReader reader = sc.ExecuteReader())
            {
                cats = SQLDataMap.CreateCategoriesFromReader(reader);
            }
        }
        if (cats.Count > 0)
            return cats[0];
        else
            return null;
    }

    public int GetCategoryIDByName(string name)
    {
        using (SqlConnection connection = GetConnection())
        {
            connection.Open();
            SqlCommand sc = new SqlCommand("GetCategoryByName", connection);

            sc.CommandType = CommandType.StoredProcedure;
            sc.Parameters.AddWithValue("CategoryName", name);
            SqlParameter param = new SqlParameter("CategoryID", 0);
            param.Direction = ParameterDirection.Output;
            sc.Parameters.Add(param);

            sc.ExecuteNonQuery();

            return Convert.ToInt32(sc.Parameters["CategoryID"].Value);

        }
    }

    public void SyncCategories(int postID, Collection<int> categories)
    {
        using (SqlConnection connection = GetConnection())
        {
            connection.Open();

            SqlCommand sc = new SqlCommand("DeleteCategoriesFromPost", connection);
            sc.CommandType = CommandType.StoredProcedure;
            sc.Parameters.AddWithValue("PostID", postID);

            sc.ExecuteNonQuery();

            sc.CommandText = "AddCategoryToPost";

            foreach (int catID in categories)
            {
                sc.Parameters.Clear();
                sc.Parameters.AddWithValue("PostID", postID);
                sc.Parameters.AddWithValue("CategoryID", catID);
                sc.ExecuteNonQuery();
            }
        }
    }

    public Collection<Category> GetAllCategories()
    {
        Collection<Category> cats;
        using (SqlConnection connection = GetConnection())
        {
            connection.Open();
            SqlCommand sc = new SqlCommand("GetAllCategories", connection);

            sc.CommandType = CommandType.StoredProcedure;
            using (SqlDataReader reader = sc.ExecuteReader())
            {
                cats = SQLDataMap.CreateCategoriesFromReader(reader);
            }
        }
        return cats;
    }

    public bool AddCategory(string categoryName, string categorySlug)
    {
        int result;
        using (SqlConnection connection = GetConnection())
        {
            connection.Open();

            SqlCommand sc = new SqlCommand("CreateCategory", connection);
            sc.CommandType = CommandType.StoredProcedure;
            sc.Parameters.AddWithValue("Name", categoryName);
            sc.Parameters.AddWithValue("Slug", categorySlug);

            result = sc.ExecuteNonQuery();
        }

        return (result == 1);

    }

    public bool DeleteCategory(int categoryID)
    {
        int result;
        using (SqlConnection connection = GetConnection())
        {
            connection.Open();

            SqlCommand sc = new SqlCommand("DeleteCategory", connection);
            sc.CommandType = CommandType.StoredProcedure;
            sc.Parameters.AddWithValue("categoryID", categoryID);

            result = sc.ExecuteNonQuery();
        }

        return (result == 1);
    }

    public bool EditCategory(Category c)
    {
        int result;
        using (SqlConnection connection = GetConnection())
        {
            connection.Open();

            SqlCommand sc = new SqlCommand("EditCategory", connection);
            sc.CommandType = CommandType.StoredProcedure;
            sc.Parameters.AddWithValue("CategoryID", c.ID);
            sc.Parameters.AddWithValue("Name", c.Name);
            sc.Parameters.AddWithValue("Slug", c.Slug);
            result = sc.ExecuteNonQuery();
        }

        return (result == 1);
    }

    #endregion

    #region Comments

    public bool AddCommentToPost(Comment c)
    {
        int rowcount = 0;
        using (SqlConnection connection = GetConnection())
        {

            connection.Open();

            SqlCommand sc = new SqlCommand("AddComment", connection);
            sc.CommandType = CommandType.StoredProcedure;

            sc.Parameters.AddWithValue("PostID", c.PostID);
            sc.Parameters.AddWithValue("Name", c.Name);
            sc.Parameters.AddWithValue("URL", c.Url);
            sc.Parameters.AddWithValue("Text", c.Text);
            sc.Parameters.AddWithValue("Posted", c.Posted);

            rowcount = sc.ExecuteNonQuery();

        }
        return (rowcount > 0);
    }

    public bool DeleteComment(int commentID)
    {
        throw new NotImplementedException();
    }

    public Collection<Comment> GetCommentsForPost(int postID)
    {
        using (SqlConnection connection = GetConnection())
        {

            connection.Open();

            SqlCommand sc = new SqlCommand("GetCommentsByPost", connection);
            sc.CommandType = CommandType.StoredProcedure;

            sc.Parameters.AddWithValue("PostID", postID);

            Collection<Comment> clist = SQLDataMap.CreateCommentsFromReader(sc.ExecuteReader());

            return clist;
        }

    }

    #endregion

    #endregion

    public SqlConnection GetConnection()
    {
        SqlConnection sc = new SqlConnection(ConfigurationManager.ConnectionStrings["default"].ConnectionString);
        if (sc == null)
            throw new Exception("Connection is null");
        else
            return sc;

    }

    public void LogDownload(string filename, string IP, string referrer, string useragent)
    {
        using (SqlConnection conn = GetConnection())
        {
            conn.Open();

            SqlCommand sc = new SqlCommand("LogDownload", conn);
            sc.CommandType = CommandType.StoredProcedure;

            sc.Parameters.AddWithValue("@downloadfile", filename);
            sc.Parameters.AddWithValue("@IP", IP);
            sc.Parameters.AddWithValue("@referrer", referrer ?? String.Empty);
            sc.Parameters.AddWithValue("@useragent", useragent);
            

            sc.ExecuteNonQuery();
        }
    }


    public int GetUserLevel(string username, string passwordhash)
    {


        using (SqlConnection conn = GetConnection())
        {
            conn.Open();

            SqlCommand sc = new SqlCommand("CheckUser", conn);
            sc.CommandType = CommandType.StoredProcedure;

            sc.Parameters.AddWithValue("username", username);
            sc.Parameters.AddWithValue("passwordhash", passwordhash);
            SqlParameter userlevel = new SqlParameter("userlevel", SqlDbType.Int);

            userlevel.Direction = ParameterDirection.Output;
            sc.Parameters.Add(userlevel);

            sc.ExecuteNonQuery();

            string value = Convert.ToString(sc.Parameters["UserLevel"].Value);

            int authUserLevel;
            Int32.TryParse(value, out authUserLevel);
            return authUserLevel;
        }
    }

}
