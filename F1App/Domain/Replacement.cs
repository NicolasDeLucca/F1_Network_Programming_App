using Exceptions;
using System.Collections.Generic;

namespace Domain
{
    public class Replacement
    {
        private object _categoryLocker = new object();
        public int Id { get; set; }
        public string Name { get; set; }
        public string Provider { get; set; }
        public string Brand { get; set; }
        public string PhotoPath { get; set; }
        public List<Category> Categories { get; set; }

        public Replacement()
        {
            Categories = new List<Category>();
        }

        public int AddCategory(Category category)
        {
            lock (_categoryLocker)
            {
                Category newCategory = new Category()
                {
                    Id = Categories.Count,
                    Name = category.Name
                };
                Categories.Add(newCategory);

                return newCategory.Id;
            }
        }

        public Category GetCategory(int id)
        {
            lock (_categoryLocker)
            {
                if (id < 0 || id >= Categories.Count || Categories[id] == null)
                    throw new ResourceNotFoundException("Specified category not found");

                return Categories[id];
            }
        }

        public override string ToString()
        {
            return "Id: " + Id + ", Name: " + Name + ", Provider: " + Provider + ", Brand: " + Brand;
        }
    }
}
