using System;
using System.Collections.ObjectModel;

public class Comment
{

    public int ID { get; set; }
    public int PostID { get; set; }
    string _name;
    string _url;
    DateTime _posted;

    public DateTime Posted
    {
        get
        {
            return _posted;
        }
        set
        {
            if (value == DateTime.MinValue)
                _posted = DateTime.Now;
            else
                _posted = value;
        }
    }

    public String Name
    {
        get
        {
            return _name;
        }
        set
        {
            if (String.IsNullOrEmpty(value))
                _name = "Anonymous";
            else
                _name = value;
        }
    }
    public string Url
    {
        get
        {
            return _url;
        }
        set
        {
            if (value.StartsWith(@"http://") == false && String.IsNullOrEmpty(value) == false)
                _url = @"http://" + value;
            else
                _url = value;

        }
    }

    public String Text { get; set; }

    public void Save()
    {
        if (this)
        {
            DataAccess.CreateInstance().AddCommentToPost(this);
        }
    }

    public static Collection<Comment> GetComments(int postID)
    {

        return null;
    }

    public static Collection<Comment> GetAllComments(int pageNo)
    {
        return null;
    }


    public static implicit operator bool(Comment com)
    {
        return string.IsNullOrEmpty(com.Text) == false && com.PostID > 0;
    }



}

