using System;


	internal interface IBlogData
	{

		Post GetPostByID(int postID);

		Post AddPost(Post p);

		Post EditPost(Post p);

		bool AddCategoryToPost(int postID, Category cat);

		bool DeleteCategoryFromPost(int postID, Category cat);

		bool DeletePost(int id);

	}

