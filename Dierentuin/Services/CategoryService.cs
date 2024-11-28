using Dierentuin.Models;
using System.Collections.Generic;
using System.Linq;

namespace Dierentuin.Services
{
    public class CategoryService
    {
        private readonly List<Category> _categories;
        private readonly List<Animal> _animals;

        public CategoryService()
        {
            _categories = new List<Category>();
            _animals = new List<Animal>();
        }

        // CRUD Operations

        public List<Category> GetAllCategories()
        {
            return _categories;
        }

        public Category GetCategoryById(int id)
        {
            return _categories.FirstOrDefault(c => c.Id == id);
        }

        public Category CreateCategory(Category category)
        {
            category.Id = _categories.Max(c => c.Id) + 1; // Assign new Id
            _categories.Add(category);
            return category;
        }

        public Category UpdateCategory(Category updatedCategory)
        {
            var existingCategory = _categories.FirstOrDefault(c => c.Id == updatedCategory.Id);
            if (existingCategory != null)
            {
                existingCategory.Name = updatedCategory.Name;
            }
            return existingCategory;
        }

        public bool DeleteCategory(int id)
        {
            var category = _categories.FirstOrDefault(c => c.Id == id);
            if (category != null)
            {
                _categories.Remove(category);
                return true;
            }
            return false;
        }

        // Assign animal to category
        public void AssignAnimalToCategory(int animalId, int categoryId)
        {
            var animal = _animals.FirstOrDefault(a => a.Id == animalId);
            var category = _categories.FirstOrDefault(c => c.Id == categoryId);
            if (animal != null && category != null)
            {
                animal.CategoryId = categoryId;
                animal.Category = category;
            }
        }

        // Filter animals by category
        public List<Animal> GetAnimalsByCategory(int categoryId)
        {
            return _animals.Where(a => a.CategoryId == categoryId).ToList();
        }
    }
}
