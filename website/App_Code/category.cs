using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace KMBlog
{
	public class Category
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Slug { get; set; }

		public Category()
		{ }

		public Category(int categoryId, string categoryName, string categorySlug)
		{
			Id = categoryId;
			Name = categoryName;
			Slug = categorySlug;
		}

		public static int GetCategoryIdFromQueryString(NameValueCollection parameters)
		{

			string[] keys = parameters.AllKeys;

			int categoryId = 0;
			foreach (string key in keys)
			{
				if (key.ToUpperInvariant() == "C")
				{
					foreach (string value in parameters.GetValues(key))
					{
						if (System.Int32.TryParse(value, out categoryId))
						{
							break;
						}
					}
				}
			}
			return categoryId;
		}

		public static int GetCategoryIdByName(string name)
		{
			return DataAccess.CreateInstance().GetCategoryIdByName(name);
		}

		public static Collection<Category> GetAllCategories()
		{
			return DataAccess.CreateInstance().GetAllCategories();
		}

		public static Collection<Category> GetCategoriesForPost(int postId)
		{
			return DataAccess.CreateInstance().GetPostById(postId).Categories;
		}

		public static void SyncCategories(int PostId, Collection<int> categories)
		{
			DataAccess.CreateInstance().SyncCategories(PostId, categories);
		}

		public static bool Add(string categoryName, string categorySlug)
		{
			if (String.IsNullOrEmpty(categoryName))
				throw new ArgumentException("Category name can't be empty");

			// TODO: Sort out all this slug stuff! 

			if (String.IsNullOrEmpty(categorySlug)) // Hmm. Need to check if category slug exists already, like with post
			{
				categorySlug = CommonMethods.GetSlug(categoryName);
			}
			return DataAccess.CreateInstance().AddCategory(categoryName, categorySlug);
		}

		public static bool Edit(Category cat)
		{
			return DataAccess.CreateInstance().EditCategory(cat);
		}

		public static bool Delete(int categoryId)
		{
			return DataAccess.CreateInstance().DeleteCategory(categoryId);
		}

		public static Category GetCategoryById(int categoryId)
		{
			return DataAccess.CreateInstance().GetCategoryById(categoryId);
		}

		// Operator overload
		public static implicit operator bool(Category cat)
		{
			return cat.Id != 0 && String.IsNullOrEmpty(cat.Name) == false && String.IsNullOrEmpty(cat.Slug) == false;
		}

	}

}