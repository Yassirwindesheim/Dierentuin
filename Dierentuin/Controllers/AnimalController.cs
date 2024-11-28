using Dierentuin.Models;
using Dierentuin.Services;
using Microsoft.AspNetCore.Mvc;

namespace Dierentuin.Controllers
{
    public class AnimalController : Controller
    {
        private readonly AnimalService _animalService;

        // Constructor to inject AnimalService via dependency injection
        public AnimalController(AnimalService animalService)
        {
            _animalService = animalService;
        }

        // Action for listing all animals
        public IActionResult Index()
        {
            Console.WriteLine("Index activated for animals");
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
        public IActionResult Create()
        {
            return View();  // Return the create view for a new animal
        }

        // Action for creating a new animal (POST)
        [HttpPost]
        public IActionResult Create(Animal animal)
        {
            if (ModelState.IsValid)
            {
                _animalService.CreateAnimal(animal);  // Call the service to create a new animal
                return RedirectToAction("Index");  // Redirect to the animals list
            }
            return View(animal);  // Return the create view if model is invalid
        }

        // Action for editing an existing animal (GET)
        public IActionResult Edit(int id)
        {
            var animal = _animalService.GetAnimalById(id);  // Retrieve animal by ID
            if (animal == null)
            {
                return NotFound();  // Return 404 if the animal does not exist
            }
            return View(animal);  // Pass the animal to the view
        }

        // Action for editing an existing animal (POST)
        [HttpPost]
        public IActionResult Edit(Animal updatedAnimal)
        {
            if (ModelState.IsValid)
            {
                _animalService.UpdateAnimal(updatedAnimal);  // Call the service to update the animal
                return RedirectToAction("Index");  // Redirect to the animals list after update
            }
            return View(updatedAnimal);  // Return the edit view if model is invalid
        }

        // Action for deleting an animal (GET)
        public IActionResult Delete(int id)
        {
            var animal = _animalService.GetAnimalById(id);  // Retrieve animal by ID
            if (animal == null)
            {
                return NotFound();  // Return 404 if the animal does not exist
            }
            return View(animal);  // Show confirmation page for deletion
        }

        // Action for deleting an animal (POST)
        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            _animalService.DeleteAnimal(id);  // Call the service to delete the animal
            return RedirectToAction("Index");  // Redirect to the animals list after deletion
        }

        // Action for Sunrise (for an individual animal)
        public IActionResult Sunrise(int id)
        {
            var animal = _animalService.GetAnimalById(id);  // Retrieve animal by ID
            if (animal == null)
            {
                return NotFound();  // Return 404 if the animal is not found
            }
            _animalService.Sunrise(animal);  // Call the service to perform Sunrise for the animal
            return RedirectToAction("Details", new { id });  // Redirect to the animal details page
        }

        // Action for Sunset (for an individual animal)
        public IActionResult Sunset(int id)
        {
            var animal = _animalService.GetAnimalById(id);  // Retrieve animal by ID
            if (animal == null)
            {
                return NotFound();  // Return 404 if the animal is not found
            }
            _animalService.Sunset(animal);  // Call the service to perform Sunset for the animal
            return RedirectToAction("Details", new { id });  // Redirect to the animal details page
        }

        // Action for FeedingTime (for an individual animal)
        public IActionResult FeedingTime(int id)
        {
            var animal = _animalService.GetAnimalById(id);  // Retrieve animal by ID
            if (animal == null)
            {
                return NotFound();  // Return 404 if the animal is not found
            }
            _animalService.FeedingTime(animal);  // Call the service to feed the animal
            return RedirectToAction("Details", new { id });  // Redirect to the animal details page
        }
    }
}
