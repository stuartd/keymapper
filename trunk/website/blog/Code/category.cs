using System;


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

    public bool Save()
    {
        return true;
    }
}

