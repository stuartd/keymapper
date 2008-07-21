using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Collections.ObjectModel;
using System.Configuration;

public class SQLBlogDataAccess : IDataAccess
{

    #region IDataAccess Members

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

            sc.Parameters.AddWithValue("stub", p.Stub);
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

    public bool DoesStubExist(string stub)
    {

        using (SqlConnection connection = GetConnection())
        {

            connection.Open();
            SqlCommand sc = new SqlCommand("DoesStubExist", connection);
            sc.Parameters.AddWithValue("Stub", stub);
            sc.CommandType = CommandType.StoredProcedure;

            bool exists = ((int)sc.ExecuteScalar() != 0);

            return exists;

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


    public bool AddCategory(int categoryID, string categoryName)
    {
        throw new NotImplementedException();
    }

    public bool DeleteCategory(int categoryID)
    {
        throw new NotImplementedException();
    }



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
            sc.Parameters.AddWithValue("Email", c.Email);
            sc.Parameters.AddWithValue("URL", c.URL);
            sc.Parameters.AddWithValue("Text", c.Text);
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

    public SqlConnection GetConnection()
    {
        SqlConnection sc = new SqlConnection(ConfigurationManager.ConnectionStrings["default"].ConnectionString);
        if (sc == null)
            throw new Exception("Connection is null");
        else
            return sc;

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
