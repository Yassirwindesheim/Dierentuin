using Dierentuin.Enum;
using Dierentuin.Models;
using Dierentuin.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;

namespace Dierentuin.Controllers
{
    public class EnclosureController : Controller
    {
        private readonly EnclosureService _enclosureService;
        private readonly DBContext _context;
        private readonly AnimalService _animalService; // Add this

        // Constructor to inject EnclosureService and DBContext via dependency injection
        public EnclosureController(EnclosureService enclosureService, DBContext context, AnimalService animalService)
        {
            _enclosureService = enclosureService;
            _context = context;
            _animalService = animalService;
        }

        // Action for listing all enclosures
        public IActionResult Index()
        {
            var enclosures = _enclosureService.GetAllEnclosures();
            return View(enclosures); // Pass the list of enclosures to the view
        }

        // Action for viewing details of a specific enclosure
        public IActionResult Details(int id)
        {
            var enclosure = _enclosureService.GetEnclosureById(id);
            if (enclosure == null)
            {
                return NotFound(); // Return 404 if the enclosure is not found
            }
            return View(enclosure);
        }

        // Action for creating a new enclosure (GET)
        // Action for creating a new enclosure (GET)
        public async Task<IActionResult> Create()
        {
            ViewBag.Climates = EnumHelper.GetSelectList<Climate>();
            ViewBag.HabitatTypes = EnumHelper.GetSelectList<HabitatType>();
            ViewBag.SecurityLevels = EnumHelper.GetSelectList<SecurityLevel>();

            var animals = await _animalService.GetAllAnimals(); // Get all animals
            ViewBag.Animals = new MultiSelectList(animals, "Id", "Name"); // Populate MultiSelectList

            return View();
        }



        // Action for creating a new enclosure (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Enclosure enclosure)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _enclosureService.CreateEnclosure(enclosure); // Ensure this method can handle animal assignments
                    return RedirectToAction(nameof(Index)); // Redirect after successful creation
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Unable to save changes. Try again. Error: {ex.Message}");
                }
            }

            var animals = await _animalService.GetAllAnimals(); // Get all animals again if model is invalid
            ViewBag.Animals = new MultiSelectList(animals, "Id", "Name");

            return View(enclosure);
        }


        // Action for editing an existing enclosure (GET)
        // Action for editing an existing enclosure (GET)
        public async Task<IActionResult> Update(int id)
        {
            var enclosure = await _enclosureService.GetEnclosureById(id);
            if (enclosure == null)
            {
                return NotFound();
            }

            // Populate Enum-based dropdowns
            ViewBag.Climates = EnumHelper.GetSelectList<Climate>();
            ViewBag.HabitatTypes = EnumHelper.GetSelectList<HabitatType>();
            ViewBag.SecurityLevels = EnumHelper.GetSelectList<SecurityLevel>();

            // Get all animals for the dropdown
            var animals = await _animalService.GetAllAnimals();
            ViewBag.Animals = new MultiSelectList(animals, "Id", "Name", enclosure.AnimalIds); // Pre-select animals associated with this enclosure

            return View(enclosure);
        }
        // Action for editing an existing enclosure (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(Enclosure updatedEnclosure)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _enclosureService.UpdateEnclosure(updatedEnclosure); // Ensure this method is async and returns Task
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Unable to save changes. Try again. Error: {ex.Message}");
                }
            }

            // Repopulate ViewBag.Animals if model is invalid
            var animals = await _animalService.GetAllAnimals(); // This should be awaited
            ViewBag.Animals = new MultiSelectList(animals, "Id", "Name", updatedEnclosure.AnimalIds);

            return View(updatedEnclosure);
        }



        // Action for deleting an enclosure (GET)
        public IActionResult Delete(int id)
        {
            var enclosure = _enclosureService.GetEnclosureById(id);
            if (enclosure == null)
            {
                return NotFound();
            }
            return View(enclosure);
        }

        // Action for deleting an enclosure (POST)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            bool success = _enclosureService.DeleteEnclosure(id);
            if (success)
            {
                return RedirectToAction(nameof(Index)); // Redirect if deletion was successful
            }
            return NotFound(); // Return 404 if deletion failed
        }
    }
}
