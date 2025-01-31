using Dierentuin.Models;
using Dierentuin.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace Dierentuin.Controllers
{
    public class CategoryController : Controller
    {
        private readonly CategoryService _categoryService;  // Service voor het beheer van categorieën
        private readonly AnimalService _animalService;  // Service voor het beheren van dieren

        // Constructor om CategoryService en AnimalService via dependency injection te injecteren
        public CategoryController(CategoryService categoryService, AnimalService animalService)
        {
            _categoryService = categoryService;
            _animalService = animalService;
        }

        // Actie voor het weergeven van alle categorieën (GET)
        public async Task<IActionResult> Index()
        {
            var categories = await _categoryService.GetAllCategories();  // Haal de categorieën asynchroon op
            return View(categories);  // Geef de categorieën door naar de view
        }

        // Actie voor het weergeven van de details van een specifieke categorie (GET)
        public async Task<IActionResult> Details(int id)
        {
            var category = await _categoryService.GetCategoryById(id);  // Haal de categorie asynchroon op
            if (category == null)
            {
                return NotFound();  // Retourneer 404 als de categorie niet bestaat
            }
            return View(category);  // Geef de categorie door naar de view voor details
        }

        // Actie voor het creëren van een nieuwe categorie (GET)
        public async Task<IActionResult> Create()
        {
            var animals = await _animalService.GetAllAnimals();  // Haal alle dieren op
            ViewBag.Animals = new MultiSelectList(animals, "Id", "Name");  // Maak een MultiSelectList voor dieren
            return View();
        }

        // Actie voor het creëren van een nieuwe categorie (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            if (ModelState.IsValid)  // Als het model geldig is
            {
                await _categoryService.CreateCategory(category);  // Maak de categorie aan
                return RedirectToAction(nameof(Index));  // Redirect naar de indexpagina na het aanmaken van de categorie
            }
            var animals = await _animalService.GetAllAnimals();  // Haal de dieren opnieuw op voor de dropdown
            ViewBag.Animals = new MultiSelectList(animals, "Id", "Name");  // Geef de dieren door naar de view
            return View(category);  // Retourneer het formulier als het model niet geldig is
        }

        // Actie voor het bijwerken van een bestaande categorie (GET)
        public async Task<IActionResult> Update(int id)
        {
            var category = await _categoryService.GetCategoryById(id);  // Haal de categorie op via het ID
            if (category == null)
            {
                return NotFound();  // Retourneer 404 als de categorie niet gevonden wordt
            }

            var animals = await _animalService.GetAllAnimals();  // Haal de dieren op
            var selectedAnimalIds = await _categoryService.GetAnimalIdsByCategoryId(id);  // Haal de geselecteerde dier-ID's op voor de categorie

            // Zet de geselecteerde dier-ID's voor de view
            category.AnimalIds = selectedAnimalIds;

            ViewBag.Animals = new MultiSelectList(animals, "Id", "Name", selectedAnimalIds);  // Geef de dieren en geselecteerde ID's door naar de view
            return View(category);
        }

        // Actie voor het bijwerken van een bestaande categorie (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(Category updatedCategory)
        {
            if (ModelState.IsValid)  // Als het model geldig is
            {
                var category = await _categoryService.UpdateCategory(updatedCategory);  // Werk de categorie bij in de database
                if (category == null)
                {
                    return NotFound();  // Retourneer 404 als de categorie-update mislukt
                }
                return RedirectToAction(nameof(Index));  // Redirect naar de indexpagina na succesvolle update
            }

            return View(updatedCategory);  // Retourneer de view als het model ongeldig is
        }

        // Actie voor het verwijderen van een categorie (GET)
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _categoryService.GetCategoryById(id);  // Haal de categorie op via het ID
            if (category == null)
            {
                return NotFound();  // Retourneer 404 als de categorie niet gevonden wordt
            }

            return View(category);  // Geef de categorie door naar de view voor de bevestiging van verwijdering
        }

        // Actie voor het bevestigen van het verwijderen van een categorie (POST)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var success = await _categoryService.DeleteCategory(id);  // Verwijder de categorie asynchroon
            if (success)
            {
                return RedirectToAction(nameof(Index));  // Redirect naar de indexpagina na verwijdering
            }

            return NotFound();  // Retourneer 404 als de categorie niet gevonden wordt
        }
    }
}
