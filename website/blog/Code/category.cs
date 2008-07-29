using System;
using System.Collections.ObjectModel;

public class Category
{
    public int ID { get; set; }
    public string Name { get; set; }

    public Category()
    { }

    public Category(int categoryID, string categoryName)
    {
        ID = categoryID;
        Name = categoryName;
    }

	public static Collection<Category> GetAllCategories()
	{
		return DataAccess.CreateInstance().GetAllCategories();
	}

	public static bool Add(string categoryName)
	{
		return DataAccess.CreateInstance().AddCategory(categoryName);
	}

	 public static bool Edit(Category c)
    {
		return DataAccess.CreateInstance().EditCategory(c);
    }

	public static bool Delete(int categoryID)
	{
		return DataAccess.CreateInstance().DeleteCategory(categoryID);

	}

}

