using Dierentuin.Enum;
using Dierentuin.Models;
using Dierentuin.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;  // This is necessary for Enum.GetValues

namespace Dierentuin.Controllers
{
    public class AnimalController : Controller
    {
        private readonly AnimalService _animalService;
        private readonly DBContext _context;

        // Constructor to inject AnimalService and DBContext via dependency injection
        public AnimalController(AnimalService animalService, DBContext context)
        {
            _animalService = animalService;
            _context = context;
        }

        // Action for listing all animals
        public IActionResult Index()
        {
            var animals = _animalService.GetAllAnimals();  // Retrieve all animals from service
            return View(animals);  // Pass the list of animals to the view
        }

        // Action for viewing details of a specific animal
        public IActionResult Details(int id)
        {
            var animal = _animalService.GetAnimalById(id);  // Retrieve animal details by ID
            if (animal == null)
            {
                return NotFound();  // Return 404 if the animal is not found
            }
            return View(animal);  // Pass the animal details to the view
        }


        // Action for creating a new animal (GET)
        // Action for creating a new animal (GET)
        public IActionResult Create()
        {
            var categories = _context.Categories.ToList();
            var enclosures = _context.Enclosures.ToList();

            // Log if categories or enclosures are empty
            if (!categories.Any())
            {
                Console.WriteLine("No categories found.");
            }
            if (!enclosures.Any())
            {
                Console.WriteLine("No enclosures found.");
            }

            // Pass data to ViewBag
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            ViewBag.Enclosures = new SelectList(enclosures, "Id", "Name");

            ViewBag.AnimalSizes = EnumHelper.GetSelectList<AnimalSize>();
            ViewBag.ActivityPatterns = EnumHelper.GetSelectList<ActivityPattern>();
            ViewBag.Diets = EnumHelper.GetSelectList<DietaryClass>();
            ViewBag.SecurityLevels = EnumHelper.GetSelectList<SecurityLevel>();

            return View();
        }



        // Action for creating a new animal (POST)

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public IActionResult Create(Animal animal)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var createdAnimal = _animalService.CreateAnimal(animal);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error creating animal: {ex.Message}");
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            else
            {
                // Log validation errors
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine(error.ErrorMessage);
                }
            }

            // Repopulate ViewBag items for the dropdowns
            ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name", animal.CategoryId);
            ViewBag.Enclosures = new SelectList(_context.Enclosures, "Id", "Name", animal.EnclosureId);
            ViewBag.AnimalSizes = EnumHelper.GetSelectList<AnimalSize>();
            ViewBag.ActivityPatterns = EnumHelper.GetSelectList<ActivityPattern>();
            ViewBag.Diets = EnumHelper.GetSelectList<DietaryClass>();
            ViewBag.SecurityLevels = EnumHelper.GetSelectList<SecurityLevel>();

            return View(animal); // Return to the form with validation errors
        }



        // Action for editing an existing animal (GET)
        public IActionResult Edit(int id)
        {
            var animal = _animalService.GetAnimalById(id);
            if (animal == null)
            {
                return NotFound();
            }
            return View(animal);
        }

        // Action for editing an existing animal (POST)
        [HttpPost]
        public IActionResult Edit(Animal updatedAnimal)
        {
            if (ModelState.IsValid)
            {
                _animalService.UpdateAnimal(updatedAnimal);
                return RedirectToAction("Index");
            }
            return View(updatedAnimal);
        }

        // Action for deleting an animal (GET)
        public IActionResult Delete(int id)
        {
            var animal = _animalService.GetAnimalById(id);
            if (animal == null)
            {
                return NotFound();
            }
            return View(animal);
        }

        // Action for deleting an animal (POST)
        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            _animalService.DeleteAnimal(id);
            return RedirectToAction("Index");
        }

        // Action for Sunrise (for an individual animal)
        public IActionResult Sunrise(int id)
        {
            var animal = _animalService.GetAnimalById(id);
            if (animal == null)
            {
                return NotFound();
            }
            _animalService.Sunrise(animal);
            return RedirectToAction("Details", new { id });
        }

        // Action for Sunset (for an individual animal)
        public IActionResult Sunset(int id)
        {
            var animal = _animalService.GetAnimalById(id);
            if (animal == null)
            {
                return NotFound();
            }
            _animalService.Sunset(animal);
            return RedirectToAction("Details", new { id });
        }

        // Action for FeedingTime (for an individual animal)
        public IActionResult FeedingTime(int id)
        {
            var animal = _animalService.GetAnimalById(id);
            if (animal == null)
            {
                return NotFound();
            }
            _animalService.FeedingTime(animal);
            return RedirectToAction("Details", new { id });
        }
    }
}
