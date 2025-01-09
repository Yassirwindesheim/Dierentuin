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
            var animal = _animalService.GetAnimalById(id);
            if (animal == null)
            {
                return NotFound(); // Return 404 if the animal is not found
            }
            return View(animal); // Pass the animal details to the view
        }




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
                    // Return the created animal as a JSON response
                    return CreatedAtAction(nameof(Details), new { id = createdAnimal.Id }, createdAnimal);
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
        public IActionResult Update(int id)
        {
            Console.WriteLine($"Update GET action called for id: {id}");
            var animal = _animalService.GetAnimalById(id);
            if (animal == null)
            {
                Console.WriteLine($"Animal with id {id} not found");
                return NotFound();
            }
            PopulateViewBagForAnimalForm();
            return View(animal);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(Animal updatedAnimal)
        {
            if (ModelState.IsValid)
            {
                _animalService.UpdateAnimal(updatedAnimal);
                return RedirectToAction("Index");
            }
            PopulateViewBagForAnimalForm();
            return View(updatedAnimal);
        }

        private void PopulateViewBagForAnimalForm()
        {
            ViewBag.ActivityPatterns = EnumHelper.GetSelectList<ActivityPattern>();
            ViewBag.Diets = EnumHelper.GetSelectList<DietaryClass>();
            ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name");
            ViewBag.Enclosures = new SelectList(_context.Enclosures, "Id", "Name");
            ViewBag.AnimalSizes = EnumHelper.GetSelectList<AnimalSize>();
            ViewBag.SecurityLevels = EnumHelper.GetSelectList<SecurityLevel>();
        }



        // Action for deleting an animal (GET)
        public IActionResult Delete(int id)
        {
            var animal = _animalService.GetAnimalById(id);  // Fetch the animal by ID
            if (animal == null)
            {
                Console.WriteLine("Animal not found for ID: " + id);  // Debugging log
                return NotFound();  // Return 404 if the animal doesn't exist
            }
            Console.WriteLine("Animal found: " + animal.Name);  // Debugging log
            return View(animal);  // Pass the animal to the view
        }


        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            Console.WriteLine($"Attempting to delete animal with ID: {id}");
            var success = _animalService.DeleteAnimal(id);
            if (success)
            {
                Console.WriteLine("Animal deleted successfully.");
                return RedirectToAction("Index");
            }
            else
            {
                Console.WriteLine("Animal not found for deletion.");
                return NotFound();
            }
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