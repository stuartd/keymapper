using System;

public class Comment
{

    public int ID { get; set; }
    public int PostID { get; set; }
    string _name;
    string _url;


    public String Name
    {
        get
        {
            return _name;
        }
        set
        {
            if (String.IsNullOrEmpty(value))
                _name  = "Anonymous";
            else
                _name = value;
        }
    }
    public String URL
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

    public static implicit operator bool(Comment c)
    {
        return !(string.IsNullOrEmpty(c.Text) || c.PostID < 1);
    }



}

