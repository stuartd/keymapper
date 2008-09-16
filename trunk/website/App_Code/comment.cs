namespace KMBlog
{
    using System;
    using System.Collections.ObjectModel;

    public enum CommentType
    {
        /// <summary>
        /// All comments
        /// </summary>
        All,

        /// <summary>
        /// Approved comments only
        /// </summary>
        Approved,

        /// <summary>
        /// Unapproved comments only
        /// </summary>
        UnApproved
    }

    public class Comment
    {
        private string _name;
        private string _url;
        private DateTime _posted;

        public int Id { get; set; }

        public int PostId { get; set; }

        public bool Approved { get; set; }

        public DateTime Posted
        {
            get
            {
                return this._posted;
            }

            set
            {
                if (value == DateTime.MinValue)
                {
                    this._posted = DateTime.Now;
                }
                else
                {
                    this._posted = value;
                }
            }
        }

        public string Name
        {
            get
            {
                return this._name;
            }

            set
            {
                if (String.IsNullOrEmpty(value))
                {
                    this._name = "Anonymous";
                }
                else
                {
                    this._name = value;
                }
            }
        }

        public string Url
        {
            get
            {
                return this._url;
            }

            set
            {
                if (value.StartsWith(@"http://") == false && String.IsNullOrEmpty(value) == false)
                {
                    this._url = @"http://" + value;
                }
                else
                {
                    this._url = value;
                }
            }
        }

        public string Text { get; set; }

        public static bool Approve(int commentId)
        {
            return DataAccess.CreateInstance().ApproveComment(commentId);
        }

        public static bool Delete(int commentId)
        {
            return DataAccess.CreateInstance().DeleteComment(commentId);
        }

        public static Collection<Comment> GetComments(int postId, CommentType type)
        {
            return DataAccess.CreateInstance().GetCommentsForPost(postId, type);
        }

        public static Collection<Comment> GetAllComments(int pageNo)
        {
            return null;
        }

        public static implicit operator bool(Comment com)
        {
            return string.IsNullOrEmpty(com.Text) == false && com.PostId > 0;
        }

        public void Save()
        {
            if (this)
            {
                DataAccess.CreateInstance().AddCommentToPost(this);
            }
        }
    }
}