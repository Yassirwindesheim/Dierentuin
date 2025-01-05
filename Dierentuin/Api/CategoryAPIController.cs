using Dierentuin.Models;
using Dierentuin.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;  // Add this for logging

namespace Dierentuin.API
{
    [Route("Api/ApiCategory")]
    [ApiController]
    public class CategoryAPIController : ControllerBase
    {
        private readonly CategoryService _categoryService;
        private readonly ILogger<CategoryAPIController> _logger;  // Add logger

        public CategoryAPIController(CategoryService categoryService, ILogger<CategoryAPIController> logger)
        {
            _categoryService = categoryService;
            _logger = logger; // Initialize logger
        }

        // GET: Api/ApiCategory
        [HttpGet]
        public async Task<ActionResult<List<Category>>> GetCategories()
        {
            _logger.LogInformation("Getting all categories.");
            var categories = await _categoryService.GetAllCategories();
            if (categories == null || categories.Count == 0)
            {
                _logger.LogWarning("No categories found in the system.");
                return NotFound("No categories found.");
            }
            _logger.LogInformation($"{categories.Count} categories found.");
            return Ok(categories);
        }

        // GET: Api/ApiCategory/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Category>> GetCategory(int id)
        {
            _logger.LogInformation($"Getting category with ID: {id}");
            var category = await _categoryService.GetCategoryById(id);
            if (category == null)
            {
                _logger.LogWarning($"Category with ID {id} not found.");
                return NotFound($"Category with ID {id} not found.");
            }
            _logger.LogInformation($"Category with ID {id} found: {category.Name}");
            return Ok(category);
        }

        // POST: Api/ApiCategory
        [HttpPost]
        public async Task<ActionResult<Category>> CreateCategory([FromBody] Category category)
        {
            _logger.LogInformation("Creating a new category with animals.");
            var createdCategory = await _categoryService.CreateCategory(category);
            _logger.LogInformation($"Category created with ID {createdCategory.Id} and {category.AnimalIds?.Count ?? 0} animals assigned");
            return CreatedAtAction(nameof(GetCategory), new { id = createdCategory.Id }, createdCategory);
        }


        // PUT: Api/ApiCategory/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult<Category>> UpdateCategory(int id, [FromBody] Category updatedCategory)
        {
            if (id != updatedCategory.Id)
            {
                return BadRequest("ID mismatch");
            }

            _logger.LogInformation($"Updating category with ID: {id}");
            var category = await _categoryService.UpdateCategory(updatedCategory);
            if (category == null)
            {
                _logger.LogWarning($"Category with ID {id} not found.");
                return NotFound($"Category with ID {id} not found.");
            }

            _logger.LogInformation($"Category updated with ID {id} and {updatedCategory.AnimalIds?.Count ?? 0} animals assigned");
            return Ok(category);
        }


        // DELETE: Api/ApiCategory/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCategory(int id)
        {
            var success = await _categoryService.DeleteCategory(id);
            if (!success)
            {
                return NotFound();
            }

            return Ok(new { message = $"Category with ID {id} deleted successfully" });
        }

        // Assign an animal to a category
        // POST: Api/ApiCategory/{categoryId}/assign/{animalId}
        // POST: Api/ApiCategory/{categoryId}/assign/{animalId}
        [HttpPost("{categoryId}/assign/{animalId}")]
        public async Task<ActionResult> AssignAnimalToCategory(int categoryId, int animalId)
        {
            _logger.LogInformation($"Assigning animal {animalId} to category {categoryId}");
            await _categoryService.AssignAnimalToCategory(animalId, categoryId);
            return Ok(new { message = $"Animal {animalId} assigned to category {categoryId}" });
        }


        // GET: Api/ApiCategory/animals/{categoryId}
        [HttpGet("animals/{categoryId}")]
        public async Task<ActionResult<List<Animal>>> GetAnimalsByCategory(int categoryId)
        {
            _logger.LogInformation($"Getting animals for category ID: {categoryId}");
            var animals = await _categoryService.GetAnimalsByCategory(categoryId);
            if (animals == null || animals.Count == 0)
            {
                _logger.LogWarning($"No animals found in category {categoryId}.");
                return NotFound($"No animals found in category {categoryId}.");
            }
            _logger.LogInformation($"{animals.Count} animals found in category {categoryId}.");
            return Ok(animals);
        }
    }
}
