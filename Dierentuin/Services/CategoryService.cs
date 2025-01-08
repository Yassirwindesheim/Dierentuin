using Dierentuin.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dierentuin.Services
{
    public class CategoryService
    {
        private readonly DBContext _context;

        // Constructor to inject DBContext
        public CategoryService(DBContext context)
        {
            _context = context;
        }

        // Fetch all categories from the database asynchronously
        public async Task<List<Category>> GetAllCategories()
        {
            return await _context.Categories.ToListAsync();  // Retrieve all categories from the database asynchronously
        }

        // Get a specific category by its ID asynchronously
        public async Task<Category> GetCategoryById(int id)
        {
            return await _context.Categories
                .Include(c => c.Animals) // Ensure related animals are loaded
                .FirstOrDefaultAsync(c => c.Id == id);
        }


        // Create a new category and save it to the database
        public async Task<Category> CreateCategory(Category category)
        {
            if (category.AnimalIds != null && category.AnimalIds.Any())
            {
                var animals = await _context.Animals
                    .Where(a => category.AnimalIds.Contains(a.Id))
                    .ToListAsync();
                category.Animals = animals;
            }

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return category;
        }


        // Update an existing category
        public async Task<Category> UpdateCategory(Category updatedCategory)
        {
            var existingCategory = await _context.Categories
                .Include(c => c.Animals) // Load the current animals
                .FirstOrDefaultAsync(c => c.Id == updatedCategory.Id);

            if (existingCategory != null)
            {
                existingCategory.Name = updatedCategory.Name;

                if (updatedCategory.AnimalIds != null)
                {
                    // Update Animals collection based on AnimalIds
                    var animals = await _context.Animals
                        .Where(a => updatedCategory.AnimalIds.Contains(a.Id))
                        .ToListAsync();
                    existingCategory.Animals = animals;
                }

                await _context.SaveChangesAsync();
            }
            return existingCategory;
        }


        // Delete a category
        public async Task<bool> DeleteCategory(int id)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
            if (category != null)
            {
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();  // Save the changes to the database asynchronously
                return true;
            }
            return false;
        }

        // Assign an animal to a category
        public async Task AssignAnimalToCategory(int animalId, int categoryId)
        {
            var animal = await _context.Animals.FirstOrDefaultAsync(a => a.Id == animalId);
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == categoryId);
            if (animal != null && category != null)
            {
                animal.CategoryId = categoryId;
                animal.Category = category;
                await _context.SaveChangesAsync();  // Save the updated animal's category to the database asynchronously
            }
        }


        public async Task<List<int>> GetAnimalIdsByCategoryId(int categoryId)
        {
            return await _context.Animals
                .Where(a => a.CategoryId == categoryId)
                .Select(a => a.Id)
                .ToListAsync();
        }



        // Filter animals by category
        public async Task<List<Animal>> GetAnimalsByCategory(int categoryId)
        {
            return await _context.Animals.Where(a => a.CategoryId == categoryId).ToListAsync();  // Get animals by category ID from the DB asynchronously
        }
    }
}