using System;


    public class Category
    {
        public int ID { get; private set; }
        public string Name { get; set; }

        public Category CreateNewCategory()
        {
            return new Category();
        }

        private Category()
        {
            ID = 0;
            Name = String.Empty;
        }
    }

