using Dierentuin.Models;
using Dierentuin.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;  // Add this for logging

// Hier hebben we de volgende Api controller namelijk CategoryApiController. Hij beheert de catogerien door http verzoeken. Je kan met deze controller crud acties doen op de categorien.
// Het maakt natuurlijk weer gebruik van de services om dit mogelijk te maken. Services is dus een heel belangrijk onderdeel van het project. 
// Zie de services file voor meer uitleg!

namespace Dierentuin.API
{
    [Route("Api/ApiCategory")]  // Specificeert het routepad voor deze API-controller
    [ApiController]  // Geeft aan dat dit een API-controller is
    public class CategoryApiController : ControllerBase
    {
        private readonly CategoryService _categoryService;  // De service die de bedrijfslogica voor categorieën afhandelt
        private readonly ILogger<CategoryApiController> _logger;  // Logger voor het loggen van belangrijke gebeurtenissen

        // Constructor die de CategoryService en de logger injecteert via dependency injection
        public CategoryApiController(CategoryService categoryService, ILogger<CategoryApiController> logger)
        {
            _categoryService = categoryService;
            _logger = logger; // Initialiseer de logger
        }

        // GET: Api/ApiCategory - Haalt een lijst van alle categorieën op
        [HttpGet]
        public async Task<ActionResult<List<Category>>> GetCategories()
        {
            _logger.LogInformation("Getting all categories.");  // Logt dat we de categorieën ophalen
            var categories = await _categoryService.GetAllCategories();  // Haalt de categorieën op via de service
            if (categories == null || categories.Count == 0)
            {
                _logger.LogWarning("No categories found in the system.");  // Logt een waarschuwing als er geen categorieën zijn
                return NotFound("No categories found.");  // Geeft een 404-statuscode als geen categorieën worden gevonden
            }
            _logger.LogInformation($"{categories.Count} categories found.");  // Logt het aantal gevonden categorieën
            return Ok(categories);  // Retourneert de gevonden categorieën als een succesvolle response
        }

        // GET: Api/ApiCategory/{id} - Haalt een specifieke categorie op op basis van het ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Category>> GetCategory(int id)
        {
            _logger.LogInformation($"Getting category with ID: {id}");  // Logt dat we een specifieke categorie ophalen
            var category = await _categoryService.GetCategoryById(id);  // Haalt de categorie op via de service
            if (category == null)
            {
                _logger.LogWarning($"Category with ID {id} not found.");  // Logt een waarschuwing als de categorie niet gevonden wordt
                return NotFound($"Category with ID {id} not found.");  // Geeft een 404-statuscode als de categorie niet wordt gevonden
            }
            _logger.LogInformation($"Category with ID {id} found: {category.Name}");  // Logt de naam van de gevonden categorie
            return Ok(category);  // Retourneert de gevonden categorie als een succesvolle response
        }

        // POST: Api/ApiCategory - Creëert een nieuwe categorie
        [HttpPost]
        public async Task<ActionResult<Category>> CreateCategory([FromBody] Category category)
        {
            _logger.LogInformation("Creating a new category with animals.");  // Logt dat we een nieuwe categorie aanmaken
            var createdCategory = await _categoryService.CreateCategory(category);  // Maakt de categorie aan via de service
            _logger.LogInformation($"Category created with ID {createdCategory.Id} and {category.AnimalIds?.Count ?? 0} animals assigned");  // Logt de ID van de aangemaakte categorie en het aantal toegewezen dieren
            return CreatedAtAction(nameof(GetCategory), new { id = createdCategory.Id }, createdCategory);  // Retourneert de aangemaakte categorie met een 201-statuscode
        }

        // PUT: Api/ApiCategory/{id} - Werkt een bestaande categorie bij
        [HttpPut("{id}")]
        public async Task<ActionResult<Category>> UpdateCategory(int id, [FromBody] Category updatedCategory)
        {
            if (id != updatedCategory.Id)  // Controleert of het ID in de URL overeenkomt met het ID in het object
            {
                return BadRequest("ID mismatch");  // Retourneert een foutmelding als de ID's niet overeenkomen
            }

            _logger.LogInformation($"Updating category with ID: {id}");  // Logt dat we een categorie bijwerken
            var category = await _categoryService.UpdateCategory(updatedCategory);  // Werk de categorie bij via de service
            if (category == null)
            {
                _logger.LogWarning($"Category with ID {id} not found.");  // Logt een waarschuwing als de categorie niet wordt gevonden
                return NotFound($"Category with ID {id} not found.");  // Geeft een 404-statuscode als de categorie niet wordt gevonden
            }

            _logger.LogInformation($"Category updated with ID {id} and {updatedCategory.AnimalIds?.Count ?? 0} animals assigned");  // Logt de ID van de bijgewerkte categorie en het aantal toegewezen dieren
            return Ok(category);  // Retourneert de bijgewerkte categorie als een succesvolle response
        }

        // DELETE: Api/ApiCategory/{id} - Verwijdert een categorie op basis van het ID
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCategory(int id)
        {
            var success = await _categoryService.DeleteCategory(id);  // Verwijdert de categorie via de service
            if (!success)
            {
                return NotFound();  // Retourneert een 404-statuscode als de categorie niet gevonden kan worden
            }

            return Ok(new { message = $"Category with ID {id} deleted successfully" });  // Retourneert een bevestigingsbericht als de categorie succesvol is verwijderd
        }

        // POST: Api/ApiCategory/{categoryId}/assign/{animalId} - Wijst een dier toe aan een categorie
        [HttpPost("{categoryId}/assign/{animalId}")]
        public async Task<ActionResult> AssignAnimalToCategory(int categoryId, int animalId)
        {
            _logger.LogInformation($"Assigning animal {animalId} to category {categoryId}");  // Logt dat we een dier toewijzen aan een categorie
            await _categoryService.AssignAnimalToCategory(animalId, categoryId);  // Wijst het dier toe aan de categorie via de service
            return Ok(new { message = $"Animal {animalId} assigned to category {categoryId}" });  // Retourneert een bevestigingsbericht
        }

        // GET: Api/ApiCategory/animals/{categoryId} - Haalt de dieren op die aan een specifieke categorie zijn toegewezen
        [HttpGet("animals/{categoryId}")]
        public async Task<ActionResult<List<Animal>>> GetAnimalsByCategory(int categoryId)
        {
            _logger.LogInformation($"Getting animals for category ID: {categoryId}");  // Logt dat we de dieren voor een categorie ophalen
            var animals = await _categoryService.GetAnimalsByCategory(categoryId);  // Haalt de dieren op via de service
            if (animals == null || animals.Count == 0)
            {
                _logger.LogWarning($"No animals found in category {categoryId}.");  // Logt een waarschuwing als er geen dieren in de categorie zijn
                return NotFound($"No animals found in category {categoryId}.");  // Geeft een 404-statuscode als er geen dieren worden gevonden
            }
            _logger.LogInformation($"{animals.Count} animals found in category {categoryId}.");  // Logt het aantal gevonden dieren in de categorie
            return Ok(animals);  // Retourneert de gevonden dieren als een succesvolle response
        }
    }
}
