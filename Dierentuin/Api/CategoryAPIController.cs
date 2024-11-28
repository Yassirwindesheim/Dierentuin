using Dierentuin.Models;
using Dierentuin.Services;
using Microsoft.AspNetCore.Mvc;

[Route("api/ApiCategory")]
[ApiController]
public class CategoryAPIController : ControllerBase
{
    private readonly CategoryService _categoryService;

    public CategoryAPIController(CategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpPost]
    public ActionResult<Category> CreateCategory([FromBody] Category category)
    {
        var createdCategory = _categoryService.CreateCategory(category);
        return CreatedAtAction(nameof(GetCategory), new { id = createdCategory.Id }, createdCategory);
    }

    [HttpGet]
    public ActionResult<List<Category>> GetCategories()
    {
        return Ok(_categoryService.GetAllCategories());
    }

    [HttpGet("{id}")]
    public ActionResult<Category> GetCategory(int id)
    {
        var category = _categoryService.GetCategoryById(id);
        if (category == null)
        {
            return NotFound();
        }
        return Ok(category);
    }

    // Additional actions for updating and deleting categories
}
