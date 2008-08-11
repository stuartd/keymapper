using System;
using System.Collections.ObjectModel;

namespace KMBlog
{


	public class Comment
	{

		string _name;
		string _url;
		DateTime _posted;

		public int Id { get; set; }
		public int PostId { get; set; }
		public bool Approved { get; set; }

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

		public static bool Approve(int commentId)
		{
			return DataAccess.CreateInstance().ApproveComment(commentId);
		}

		public static bool Delete(int commentId)
		{
			return DataAccess.CreateInstance().DeleteComment(commentId) ;
		}

		public static Collection<Comment> GetComments(int postId)
		{
			return DataAccess.CreateInstance().GetCommentsForPost(postId);
		}

		public static Collection<Comment> GetAllComments(int pageNo)
		{
			return null;
		}

		public static implicit operator bool(Comment com)
		{
			return string.IsNullOrEmpty(com.Text) == false && com.PostId > 0;
		}



	}

	public enum CommentType
	{
		All, 
		Approved, 
		UnApproved
	}


}