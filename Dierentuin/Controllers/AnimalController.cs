using Dierentuin.Data;
using Dierentuin.Enum;
using Dierentuin.Models;
using Dierentuin.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;


//AnimalController beheert de dieren, biedt API voor het ophalen, aanmaken, bijwerken en verwijderen van dieren.
//Dit stelt gebruikers in staat om dieren via de controller te manipuleren.
//Het verschil met een API-controller is dat deze meer specifiek gericht is op de webinterface en niet direct API-aanroepen verwerkt.
//De API-controller doet dat wel, maar zonder directe gebruikersinteractie via een UI.


namespace Dierentuin.Controllers
{
    public class AnimalController : Controller
    {
        private readonly AnimalService _animalService;  // Service voor het beheer van dieren
        private readonly DBContext _context;  // Database context voor gegevens toegang

        // Constructor voor het injecteren van AnimalService en DBContext via dependency injection
        public AnimalController(AnimalService animalService, DBContext context)
        {
            _animalService = animalService;
            _context = context;
        }

        // Actie voor het weergeven van alle dieren
        public async Task<IActionResult> Index()
        {
            var animals = await _animalService.GetAllAnimals();  // Wacht op de asynchrone operatie om de dieren op te halen
            return View(animals);  // Geef de lijst van dieren door aan de view
        }

        // Actie voor het weergeven van de details van een specifiek dier
        public IActionResult Details(int id)
        {
            var animal = _animalService.GetAnimalById(id);  // Haalt het dier op via het ID
            if (animal == null)
            {
                return NotFound(); // Retourneert 404 als het dier niet gevonden wordt
            }
            return View(animal); // Geef de dierspecifieke details door aan de view
        }

        // Actie voor het creëren van een nieuw dier (GET)
        public IActionResult Create()
        {
            var categories = _context.Categories.ToList();  // Haal alle categorieën op
            var enclosures = _context.Enclosures.ToList();  // Haal alle verblijven op

            // Log als categorieën of verblijven leeg zijn
            if (!categories.Any())
            {
                Console.WriteLine("No categories found.");
            }
            if (!enclosures.Any())
            {
                Console.WriteLine("No enclosures found.");
            }

            // Geef data door naar de ViewBag voor dropdowns
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            ViewBag.Enclosures = new SelectList(enclosures, "Id", "Name");

            ViewBag.AnimalSizes = EnumHelper.GetSelectList<AnimalSize>();
            ViewBag.ActivityPatterns = EnumHelper.GetSelectList<ActivityPattern>();
            ViewBag.Diets = EnumHelper.GetSelectList<DietaryClass>();
            ViewBag.SecurityLevels = EnumHelper.GetSelectList<SecurityLevel>();

            return View();
        }

        // Actie voor het creëren van een nieuw dier (POST)
        [HttpPost]
        [IgnoreAntiforgeryToken]
        public IActionResult Create(Animal animal)
        {
            if (ModelState.IsValid)  // Als het model geldig is
            {
                try
                {
                    var createdAnimal = _animalService.CreateAnimal(animal);  // Maak het dier aan
                    // Retourneer het aangemaakte dier als JSON reactie
                    return CreatedAtAction(nameof(Details), new { id = createdAnimal.Id }, createdAnimal);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error creating animal: {ex.Message}");  // Log de fout
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            else
            {
                // Log de validatiefouten
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine(error.ErrorMessage);
                }
            }

            // Herpopulateer ViewBag items voor de dropdowns
            ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name", animal.CategoryId);
            ViewBag.Enclosures = new SelectList(_context.Enclosures, "Id", "Name", animal.EnclosureId);
            ViewBag.AnimalSizes = EnumHelper.GetSelectList<AnimalSize>();
            ViewBag.ActivityPatterns = EnumHelper.GetSelectList<ActivityPattern>();
            ViewBag.Diets = EnumHelper.GetSelectList<DietaryClass>();
            ViewBag.SecurityLevels = EnumHelper.GetSelectList<SecurityLevel>();

            return View(animal);  // Retourneer het formulier met validatiefouten
        }

        // Actie voor het bewerken van een bestaand dier (GET)
        public IActionResult Update(int id)
        {
            Console.WriteLine($"Update GET action called for id: {id}");
            var animal = _animalService.GetAnimalById(id);  // Haal het dier op via het ID
            if (animal == null)
            {
                Console.WriteLine($"Animal with id {id} not found");
                return NotFound();  // Retourneer 404 als het dier niet gevonden wordt
            }
            PopulateViewBagForAnimalForm();  // Vul de dropdowns voor het formulier
            return View(animal);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(Animal updatedAnimal)
        {
            if (ModelState.IsValid)  // Als het model geldig is
            {
                _animalService.UpdateAnimal(updatedAnimal);  // Werk het dier bij
                return RedirectToAction("Index");  // Redirect naar de indexpagina
            }
            PopulateViewBagForAnimalForm();  // Vul de dropdowns voor het formulier
            return View(updatedAnimal);  // Retourneer het bijgewerkte dier bij validatiefouten
        }

        // Hulpmethode om ViewBag items voor het formulier opnieuw in te vullen
        private void PopulateViewBagForAnimalForm()
        {
            ViewBag.ActivityPatterns = EnumHelper.GetSelectList<ActivityPattern>();
            ViewBag.Diets = EnumHelper.GetSelectList<DietaryClass>();
            ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name");
            ViewBag.Enclosures = new SelectList(_context.Enclosures, "Id", "Name");
            ViewBag.AnimalSizes = EnumHelper.GetSelectList<AnimalSize>();
            ViewBag.SecurityLevels = EnumHelper.GetSelectList<SecurityLevel>();
        }

        // Actie voor het verwijderen van een dier (GET)
        public IActionResult Delete(int id)
        {
            var animal = _animalService.GetAnimalById(id);  // Haal het dier op via het ID
            if (animal == null)
            {
                Console.WriteLine("Animal not found for ID: " + id);  // Logfout
                return NotFound();  // Retourneer 404 als het dier niet gevonden wordt
            }
            Console.WriteLine("Animal found: " + animal.Name);  // Log het gevonden dier
            return View(animal);  // Geef het dier door naar de view
        }

        // Actie voor het bevestigen van het verwijderen van een dier (POST)
        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            Console.WriteLine($"Attempting to delete animal with ID: {id}");
            var success = _animalService.DeleteAnimal(id);  // Verwijder het dier
            if (success)
            {
                Console.WriteLine("Animal deleted successfully.");  // Log succes
                return RedirectToAction("Index");  // Redirect naar de indexpagina
            }
            else
            {
                Console.WriteLine("Animal not found for deletion.");  // Log fout
                return NotFound();  // Retourneer 404 als het dier niet gevonden wordt
            }
        }

        // Actie voor het wakker maken van een dier (Sunrise)
        public IActionResult Sunrise(int id)
        {
            var animal = _animalService.GetAnimalById(id);  // Haal het dier op via het ID
            if (animal == null)
            {
                return NotFound();  // Retourneer 404 als het dier niet gevonden wordt
            }
            _animalService.Sunrise(animal);  // Maak het dier wakker
            return RedirectToAction("Details", new { id });  // Redirect naar de details van het dier
        }

        // Actie voor het naar bed sturen van een dier (Sunset)
        public IActionResult Sunset(int id)
        {
            var animal = _animalService.GetAnimalById(id);  // Haal het dier op via het ID
            if (animal == null)
            {
                return NotFound();  // Retourneer 404 als het dier niet gevonden wordt
            }
            _animalService.Sunset(animal);  // Stuur het dier naar bed
            return RedirectToAction("Details", new { id });  // Redirect naar de details van het dier
        }

        // Actie voor het voeren van een dier (FeedingTime)
        public IActionResult FeedingTime(int id)
        {
            var animal = _animalService.GetAnimalById(id);  // Haal het dier op via het ID
            if (animal == null)
            {
                return NotFound();  // Retourneer 404 als het dier niet gevonden wordt
            }
            _animalService.FeedingTime(animal);  // Voer het dier
            return RedirectToAction("Details", new { id });  // Redirect naar de details van het dier
        }
    }
}
