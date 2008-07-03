using System;


    public class Category
    {
        public int ID { get; private set; }
        public string Name { get; set; }

        public Category CreateNewCategory()
        {
            return new Category();
        }

        public Category()
        {
            ID = 0;
            Name = String.Empty;
        }

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

