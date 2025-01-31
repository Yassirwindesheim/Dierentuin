
using Dierentuin.Models;
using Dierentuin.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Dierentuin.Data;


//ZooController biedt functionaliteit voor het beheren van dierentuinen via de controller, met CRUD-operaties voor dierentuinen en hun dieren.
//Het verschil met een API-controller is dat deze controller vooral gebruikt wordt in de webapplicatie hetzelfde dus zoals animal en alle andere controllers.
//terwijl een API-controller puur de communicatie met een externe API regelt en geen weergave van de data bevat.


namespace Dierentuin.Controllers
{
    public class ZooController : Controller
    {
        private readonly ZooService _zooService;  // Service voor het beheren van dierentuinen
        private readonly Zoo _zoo;  // Zoo instantie (afhankelijk van de injectie)
        private readonly AnimalService _animalService;  // Service voor het beheren van dieren
        private readonly DBContext _context;  // Database context

        // Constructor om de benodigde services en contexten in te voeren via dependency injection
        public ZooController(ZooService zooService, Zoo zoo, DBContext context, AnimalService animalService)
        {
            _zooService = zooService;
            _zoo = zoo; // Dierentuin instantie ingevoerd
            _context = context; // Database context ingevoerd
            _animalService = animalService;  // Dieren service ingevoerd
        }

        // Actie voor het weergeven van het formulier voor het maken van een nieuwe dierentuin (GET)
        public async Task<IActionResult> Create()
        {
            // Haal alle dieren op via de service
            var animals = await _animalService.GetAllAnimals();

            // Maak een MultiSelectList voor de dieren (meerdere selectie mogelijk)
            ViewBag.Animals = new MultiSelectList(animals, "Id", "Name");

            // Haal alle hokken op uit de database
            var enclosures = await _context.Enclosures.ToListAsync();

            // Maak een MultiSelectList voor de hokken (meerdere selectie mogelijk)
            ViewBag.Enclosures = new MultiSelectList(enclosures, "Id", "Name");

            return View();
        }

        // Actie voor het verwerken van het formulier voor het maken van een nieuwe dierentuin (POST)
        [HttpPost]
        public async Task<IActionResult> Create(Zoo zoo)
        {
            // Controleer of de zoo data aanwezig is
            if (zoo == null)
            {
                return BadRequest("Zoo data is missing.");
            }

            // Haal de geselecteerde hokken op via hun IDs
            var enclosures = await _context.Enclosures
                .Where(e => zoo.Enclosures.Select(enclosure => enclosure.Id).Contains(e.Id))
                .ToListAsync();

            // Ken de opgehaalde hokken toe aan de dierentuin
            zoo.Enclosures = enclosures;

            // Haal de geselecteerde dieren op via hun IDs
            var animals = await _context.Animals
                .Where(a => zoo.AnimalIds.Contains(a.Id))  // Veronderstel dat de zoo een lijst van dier-ID's bevat
                .ToListAsync();
            zoo.Animals = animals;

            // Voeg de nieuwe dierentuin toe aan de database
            _context.Zoos.Add(zoo);
            await _context.SaveChangesAsync();

            // Redirect naar de Index pagina na succesvolle creatie
            return RedirectToAction(nameof(Index));
        }

        // Actie voor het weergeven van de details van een specifieke dierentuin
        public async Task<IActionResult> Details(int id)
        {
            // Haal de dierentuin op met bijbehorende dieren en hokken
            var zoo = await _context.Zoos
                                    .Include(z => z.Animals)
                                    .Include(z => z.Enclosures) // Inclusief gerelateerde hokken
                                    .FirstOrDefaultAsync(z => z.Id == id);
            if (zoo == null)
            {
                return NotFound();
            }
            return View(zoo);  // Geef de dierentuin door naar de view
        }

        // Actie voor het weergeven van de verwijderpagina voor een dierentuin
        public async Task<IActionResult> Delete(int id)
        {
            var zoo = await _context.Zoos
                .Include(z => z.Animals)
                .Include(z => z.Enclosures)
                .FirstOrDefaultAsync(z => z.Id == id);

            if (zoo == null)
            {
                return NotFound();
            }

            return View(zoo);  // Geef de dierentuin door naar de view voor bevestiging van verwijdering
        }

        // Actie voor het bevestigen van de verwijdering van een dierentuin (POST)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var zoo = await _context.Zoos
                .Include(z => z.Animals)
                .Include(z => z.Enclosures)
                .FirstOrDefaultAsync(z => z.Id == id);

            if (zoo != null)
            {
                _context.Zoos.Remove(zoo);  // Verwijder de dierentuin uit de database
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));  // Redirect naar de Index pagina na succesvolle verwijdering
        }

        // Actie voor het bewerken van een bestaande dierentuin (GET)
        public async Task<IActionResult> Update(int id)
        {
            var zoo = await _context.Zoos
                .Include(z => z.Animals)  // Laad de bijbehorende dieren
                .Include(z => z.Enclosures)  // Laad de bijbehorende hokken
                .FirstOrDefaultAsync(z => z.Id == id);

            if (zoo == null)
            {
                return NotFound();
            }

            // Vul de ViewBag met gegevens voor de dropdownlijsten
            ViewBag.Animals = new SelectList(await _context.Animals.ToListAsync(), "Id", "Name", zoo.Animals.Select(a => a.Id).ToList());
            ViewBag.Enclosures = new SelectList(await _context.Enclosures.ToListAsync(), "Id", "Name", zoo.Enclosures.Select(e => e.Id).ToList());

            return View(zoo);  // Geef de dierentuin door naar de view voor bewerking
        }

        // Actie voor het verwerken van de updates van een bestaande dierentuin (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, Zoo zoo, List<int> AnimalIds, List<int> EnclosureIds)
        {
            if (id != zoo.Id)
            {
                return NotFound();  // Retourneer 404 als de ID niet overeenkomt
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingZoo = await _context.Zoos
                        .Include(z => z.Animals)
                        .Include(z => z.Enclosures)
                        .FirstOrDefaultAsync(z => z.Id == id);

                    if (existingZoo == null)
                    {
                        return NotFound();
                    }

                    // Werk de eigenschappen van de dierentuin bij
                    existingZoo.Name = zoo.Name;

                    // Werk de bijbehorende dieren bij
                    existingZoo.Animals.Clear();
                    foreach (var animalId in AnimalIds)
                    {
                        var animal = await _context.Animals.FindAsync(animalId);
                        if (animal != null)
                        {
                            existingZoo.Animals.Add(animal);  // Voeg het dier toe aan de dierentuin
                        }
                    }

                    // Werk de bijbehorende hokken bij
                    existingZoo.Enclosures.Clear();
                    foreach (var enclosureId in EnclosureIds)
                    {
                        var enclosure = await _context.Enclosures.FindAsync(enclosureId);
                        if (enclosure != null)
                        {
                            existingZoo.Enclosures.Add(enclosure);  // Voeg het hok toe aan de dierentuin
                        }
                    }

                    _context.Update(existingZoo);  // Werk de bestaande dierentuin bij in de database
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Zoos.Any(z => z.Id == zoo.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));  // Redirect naar de Index pagina na succesvolle update
            }

            // Als het model ongeldig is, vul de dropdowns opnieuw in en toon de bewerkingspagina
            ViewBag.Animals = new SelectList(await _context.Animals.ToListAsync(), "Id", "Name", AnimalIds);
            ViewBag.Enclosures = new SelectList(await _context.Enclosures.ToListAsync(), "Id", "Name", EnclosureIds);

            return View(zoo);
        }

        // Actie voor het weergeven van de lijst van alle dierentuinen
        public IActionResult Index()
        {
            // Haal de lijst van dierentuinen op via de service of database
            var zoos = _zooService.GetAllZoos();  // Zorg ervoor dat deze methode bestaat in de service
            return View(zoos);  // Geef de lijst van dierentuinen door naar de view
        }

        // Actie voor het verwerken van de zonsopgang in de dierentuin (POST)
        [HttpPost]
        public IActionResult Sunrise(int id)
        {
            var zoo = _zooService.GetZooById(id);  // Haal de details van de dierentuin op
            if (zoo == null) return NotFound();

            var messages = _zooService.Sunrise(zoo);  // Voer de zonsopgang actie uit
            ViewBag.ActionResults = messages;

            return View("Details", zoo);  // Toon de details van de dierentuin met de resultaten
        }

        // Actie voor het verwerken van de zonsondergang in de dierentuin (POST)
        [HttpPost]
        public IActionResult Sunset(int id)
        {
            var zoo = _zooService.GetZooById(id);  // Haal de details van de dierentuin op
            if (zoo == null) return NotFound();

            var messages = _zooService.Sunset(zoo);  // Voer de zonsondergang actie uit
            ViewBag.ActionResults = messages;

            return View("Details", zoo);  // Toon de details van de dierentuin met de resultaten
        }

        // Actie voor het verwerken van het voertijden in de dierentuin (POST)
        [HttpPost]
        public IActionResult FeedingTime(int id)
        {
            var zoo = _zooService.GetZooById(id);  // Haal de details van de dierentuin op
            if (zoo == null) return NotFound();

            var messages = _zooService.FeedingTime(zoo);  // Voer de voedertijd actie uit
            ViewBag.ActionResults = messages;

            return View("Details", zoo);  // Toon de details van de dierentuin met de resultaten
        }

        // Actie voor het controleren van beperkingen
        public IActionResult CheckConstraints()
        {
            _zooService.CheckConstraints(_zoo);  // Controleer de beperkingen van de dierentuin
            return View();
        }

        // Actie voor automatisch toewijzen van dieren aan hokken
        public IActionResult AutoAssign()
        {
            _zooService.AutoAssign(_zoo);  // Voer de automatische toewijzing van dieren uit
            return View();
        }
    }
}
