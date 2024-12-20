using Dierentuin.Models;
using Dierentuin.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Dierentuin.Controllers
{
    public class CategoryController : Controller
    {
        private readonly CategoryService _categoryService;

        // Constructor to inject CategoryService via dependency injection
        public CategoryController(CategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        // Action for listing all categories (GET)
        public async Task<IActionResult> Index()
        {
            var categories = await _categoryService.GetAllCategories();  // Fetch categories asynchronously
            return View(categories);  // Pass categories to the view
        }

        // Action for viewing details of a specific category (GET)
        public async Task<IActionResult> Details(int id)
        {
            var category = await _categoryService.GetCategoryById(id);  // Fetch category asynchronously
            if (category == null)
            {
                return NotFound();  // Return 404 if the category doesn't exist
            }
            return View(category);  // Pass category to the view for details
        }

        // Action for creating a new category (GET)
        public IActionResult Create()
        {
            return View(new Category());  // Ensure you pass a new instance of Category to the view
        }


        // Action for creating a new category (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            if (ModelState.IsValid)
            {
                await _categoryService.CreateCategory(category);  // Create the new category in the database asynchronously
                return RedirectToAction(nameof(Index));  // Redirect to the index page after creation
            }

            return View(category);  // Return the view if the model is invalid
        }

        // Action for updating an existing category (GET)
        public async Task<IActionResult> Update(int id)
        {
            var category = await _categoryService.GetCategoryById(id);  // Fetch category asynchronously
            if (category == null)
            {
                return NotFound();  // Return 404 if the category doesn't exist
            }
            return View(category);  // Return the category to the view for editing
        }

        // Action for updating an existing category (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(Category updatedCategory)
        {
            if (ModelState.IsValid)
            {
                var category = await _categoryService.UpdateCategory(updatedCategory);  // Update the category in the database asynchronously
                if (category == null)
                {
                    return NotFound();  // Return 404 if the category update failed
                }
                return RedirectToAction(nameof(Index));  // Redirect to index after successful update
            }

            return View(updatedCategory);  // Return the view if the model is invalid
        }

        // Action for deleting a category (GET)
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _categoryService.GetCategoryById(id);  // Fetch category asynchronously
            if (category == null)
            {
                return NotFound();  // Return 404 if the category doesn't exist
            }

            return View(category);  // Pass category to the view for deletion confirmation
        }

        // Action for deleting a category (POST)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var success = await _categoryService.DeleteCategory(id);  // Delete the category asynchronously
            if (success)
            {
                return RedirectToAction(nameof(Index));  // Redirect to the index page after deletion
            }

            return NotFound();  // Return 404 if the category wasn't found
        }
    }
}
