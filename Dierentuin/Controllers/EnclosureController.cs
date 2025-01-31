using Dierentuin.Data;
using Dierentuin.Enum;
using Dierentuin.Models;
using Dierentuin.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace Dierentuin.Controllers
{
    public class EnclosureController : Controller
    {
        private readonly EnclosureService _enclosureService;  // Service voor het beheren van hokken
        private readonly DBContext _context;  // Database context
        private readonly AnimalService _animalService;  // Service voor het beheren van dieren

        // Constructor om EnclosureService, DBContext en AnimalService via dependency injection in te voeren
        public EnclosureController(EnclosureService enclosureService, DBContext context, AnimalService animalService)
        {
            _enclosureService = enclosureService;
            _context = context;
            _animalService = animalService;
        }

        // Actie voor het weergeven van alle hokken
        public IActionResult Index()
        {
            var enclosures = _enclosureService.GetAllEnclosures();  // Haal alle hokken op
            return View(enclosures);  // Geef de lijst van hokken door naar de view
        }

        // Actie voor het weergeven van de details van een specifiek hok
        public async Task<IActionResult> Details(int id)
        {
            var enclosure = await _enclosureService.GetEnclosureById(id);  // Haal het specifieke hok op via ID
            if (enclosure == null)
            {
                return NotFound();  // Retourneer 404 als het hok niet gevonden wordt
            }
            return View(enclosure);  // Geef het hok door naar de view voor details
        }

        // Actie voor het creëren van een nieuw hok (GET)
        public async Task<IActionResult> Create()
        {
            // Vul de ViewBag met enum-waarden voor klimaat, habitattypes en beveiligingsniveaus
            ViewBag.Climates = EnumHelper.GetSelectList<Climate>();
            ViewBag.HabitatTypes = EnumHelper.GetSelectList<HabitatType>();
            ViewBag.SecurityLevels = EnumHelper.GetSelectList<SecurityLevel>();

            var animals = await _animalService.GetAllAnimals();  // Haal alle dieren op
            ViewBag.Animals = new MultiSelectList(animals, "Id", "Name");  // Maak een MultiSelectList voor dieren

            return View();
        }

        // Actie voor het creëren van een nieuw hok (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Enclosure enclosure)
        {
            if (ModelState.IsValid)  // Als het model geldig is
            {
                try
                {
                    await _enclosureService.CreateEnclosure(enclosure);  // Maak het hok aan
                    return RedirectToAction(nameof(Index));  // Redirect naar de indexpagina na succesvolle creatie
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Unable to save changes. Try again. Error: {ex.Message}");  // Voeg een foutmelding toe als de creatie mislukt
                }
            }

            var animals = await _animalService.GetAllAnimals();  // Haal de dieren opnieuw op als het model ongeldig is
            ViewBag.Animals = new MultiSelectList(animals, "Id", "Name");

            return View(enclosure);  // Retourneer het formulier als het model ongeldig is
        }

        // Actie voor het bewerken van een bestaand hok (GET)
        public async Task<IActionResult> Update(int id)
        {
            var enclosure = await _enclosureService.GetEnclosureById(id);  // Haal het hok op via ID
            if (enclosure == null)
            {
                return NotFound();  // Retourneer 404 als het hok niet gevonden wordt
            }

            // Vul de ViewBag met enum-waarden voor klimaat, habitattypes en beveiligingsniveaus
            ViewBag.Climates = EnumHelper.GetSelectList<Climate>();
            ViewBag.HabitatTypes = EnumHelper.GetSelectList<HabitatType>();
            ViewBag.SecurityLevels = EnumHelper.GetSelectList<SecurityLevel>();

            var animals = await _animalService.GetAllAnimals();  // Haal alle dieren op
            var selectedAnimalIds = await _enclosureService.GetAnimalIdsByEnclosureId(id);  // Haal de geselecteerde dier-ID's op voor het hok
            enclosure.AnimalIds = selectedAnimalIds;  // Zet de geselecteerde dier-ID's voor het hok

            // Maak een MultiSelectList voor de dieren en geef de geselecteerde dier-ID's door naar de view
            ViewBag.Animals = new MultiSelectList(animals, "Id", "Name", selectedAnimalIds);
            return View(enclosure);  // Geef het hok en de dieren door naar de view
        }

        // Actie voor het bewerken van een bestaand hok (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(Enclosure updatedEnclosure)
        {
            if (ModelState.IsValid)  // Als het model geldig is
            {
                try
                {
                    await _enclosureService.UpdateEnclosure(updatedEnclosure);  // Werk het hok bij
                    return RedirectToAction(nameof(Index));  // Redirect naar de indexpagina na succesvolle update
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Unable to save changes. Try again. Error: {ex.Message}");  // Voeg een foutmelding toe als de update mislukt
                }
            }

            // Vul de ViewBag opnieuw in als het model ongeldig is
            ViewBag.Climates = EnumHelper.GetSelectList<Climate>();
            ViewBag.HabitatTypes = EnumHelper.GetSelectList<HabitatType>();
            ViewBag.SecurityLevels = EnumHelper.GetSelectList<SecurityLevel>();

            var animals = await _animalService.GetAllAnimals();  // Haal alle dieren op
            ViewBag.Animals = new MultiSelectList(animals, "Id", "Name", updatedEnclosure.AnimalIds);  // Geef de geselecteerde dier-ID's door naar de view

            return View(updatedEnclosure);  // Retourneer de view met het bijgewerkte hok
        }

        // Actie voor het verwijderen van een hok (GET)
        public async Task<IActionResult> Delete(int id)
        {
            var enclosure = await _enclosureService.GetEnclosureById(id);  // Haal het hok op via ID
            if (enclosure == null)
            {
                return NotFound();  // Retourneer 404 als het hok niet gevonden wordt
            }
            return View(enclosure);  // Geef het hok door naar de view voor bevestiging van verwijdering
        }

        // Actie voor het bevestigen van het verwijderen van een hok (POST)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            bool success = _enclosureService.DeleteEnclosure(id);  // Verwijder het hok
            if (success)
            {
                return RedirectToAction(nameof(Index));  // Redirect naar de indexpagina na succesvolle verwijdering
            }
            return NotFound();  // Retourneer 404 als het verwijderen mislukt
        }
    }
}
