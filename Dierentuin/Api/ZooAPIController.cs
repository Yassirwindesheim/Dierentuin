using Microsoft.AspNetCore.Mvc;
using Dierentuin.Services;
using Dierentuin.Models;
using Dierentuin.Enum;
using Microsoft.EntityFrameworkCore;
using Dierentuin.Data;

namespace Dierentuin.API
{
    [ApiController]  // Markeer deze klasse als een API-controller
    [Route("Api/ZooApi")]  // Basisroute voor alle acties in deze controller
    public class ZooApiController : ControllerBase
    {
        private readonly ZooService _zooService;  // Service voor het verwerken van dierentuin gerelateerde operaties
        private readonly Zoo _zoo;  // Het Zoo model dat in de controller wordt geïnjecteerd
        private readonly DBContext _context;  // Database context voor gegevens toegang

        // Dependency Injection van ZooService, Zoo en DBContext
        public ZooApiController(ZooService zooService, Zoo zoo, DBContext context)
        {
            _zooService = zooService ?? throw new ArgumentNullException(nameof(zooService));  // Zorg ervoor dat ZooService correct wordt geïnjecteerd
            _zoo = zoo ?? throw new ArgumentNullException(nameof(zoo));  // Zorg ervoor dat Zoo correct wordt geïnjecteerd
            _context = context ?? throw new ArgumentNullException(nameof(context));  // Zorg ervoor dat DBContext correct wordt geïnjecteerd
        }

        // GET: api/ZooApi/all - Haalt alle dierentuinen op via de service
        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<Zoo>>> GetAllZoos()
        {
            var zoos = await _zooService.GetZoosAsync();  // Haalt de dierentuinen op via de service
            if (zoos == null || !zoos.Any())  // Controleer of er geen dierentuinen gevonden zijn
            {
                return NotFound();  // Retourneert 404 als er geen dierentuinen gevonden zijn
            }
            return Ok(zoos);  // Retourneert de dierentuinen met een 200 OK respons
        }

        // Requestmodel voor het creëren van een dierentuin
        public class ZooRequest
        {
            public List<int> AnimalIds { get; set; }  // Lijst van dier-ID's gerelateerd aan de dierentuin
            public List<Enclosure> Enclosures { get; set; }  // Lijst van verblijven gerelateerd aan de dierentuin
        }

        // POST: api/ZooApi/create - Creëert een nieuwe dierentuin met verblijven en dieren
        [HttpPost("create")]
        public async Task<IActionResult> CreateZoo([FromBody] Zoo zoo)
        {
            if (zoo == null)
            {
                return BadRequest("Zoo data is missing.");  // Retourneert een foutmelding als de dierentuin gegevens ontbreken
            }

            // Haal de verblijven op via hun ID's uit de database
            var enclosures = await _context.Enclosures
                .Where(e => zoo.Enclosures.Select(enclosure => enclosure.Id).Contains(e.Id))
                .ToListAsync();

            // Wijs de opgehaalde verblijven toe aan de dierentuin
            zoo.Enclosures = enclosures;

            // Haal dieren op uit de database en wijs ze toe aan de dierentuin (optioneel, als AnimalIds verstrekt zijn)
            var animals = await _context.Animals
                .Where(a => zoo.AnimalIds.Contains(a.Id))
                .ToListAsync();
            zoo.Animals = animals;

            // Sla de nieuwe dierentuin op in de database
            _context.Zoos.Add(zoo);
            await _context.SaveChangesAsync();

            return Ok(zoo);  // Retourneert de nieuw aangemaakte dierentuin met een 200 OK respons
        }

        // Requestmodel voor een lijst van dier-ID's
        public class AnimalIdsRequest
        {
            public List<int> AnimalId { get; set; }  // Lijst van dier-ID's
        }

        // POST: api/ZooApi/sunrise/{zooId} - Controleert welke dieren wakker zijn bij zonsopgang op basis van hun activiteitspatroon
        [HttpPost("sunrise/{zooId}")]
        public IActionResult Sunrise(int zooId, [FromBody] AnimalIdsRequest request)
        {
            var zoo = _zooService.GetZooById(zooId);  // Haalt de dierentuin op via het ID
            if (zoo == null)
            {
                return NotFound("Zoo not found.");  // Retourneert een foutmelding als de dierentuin niet gevonden kan worden
            }

            var result = new List<string>();  // Lijst om de resultaten op te slaan
            foreach (var animalId in request.AnimalId)
            {
                var animal = zoo.Animals.FirstOrDefault(a => a.Id == animalId);
                if (animal == null)
                {
                    result.Add($"Animal with ID {animalId} not found.");  // Foutmelding als dier niet gevonden is
                }
                else
                {
                    result.Add(
                        animal.ActivityPattern == ActivityPattern.Diurnal
                        ? $"{animal.Name} is wakker geworden."  // Diurnale dieren worden wakker
                        : $"{animal.Name} slaapt nog."  // Nachtactieve dieren slapen nog
                    );
                }
            }

            return Ok(result);  // Retourneert de resultaten met een 200 OK respons
        }

        // POST: api/ZooApi/sunset/{zooId} - Controleert welke dieren gaan slapen bij zonsondergang op basis van hun activiteitspatroon
        [HttpPost("sunset/{zooId}")]
        public IActionResult Sunset(int zooId)
        {
            var zoo = _zooService.GetZooById(zooId);  // Haalt de dierentuin op via het ID
            if (zoo == null)
            {
                return NotFound("Zoo not found.");  // Retourneert een foutmelding als de dierentuin niet gevonden kan worden
            }

            var result = new List<string>();  // Lijst om de resultaten op te slaan

            foreach (var animal in zoo.Animals)
            {
                if (animal.ActivityPattern == ActivityPattern.Nocturnal)
                {
                    result.Add($"{animal.Name} is wakker geworden.");  // Nachtactieve dieren worden wakker
                }
                else if (animal.ActivityPattern == ActivityPattern.Diurnal)
                {
                    result.Add($"{animal.Name} gaat slapen.");  // Diurnale dieren gaan slapen
                }
            }

            return Ok(result);  // Retourneert de resultaten met een 200 OK respons
        }

        // POST: api/ZooApi/feeding-time/{zooId} - Retourneert de voedertijd status voor alle dieren in de dierentuin
        [HttpPost("feeding-time/{zooId}")]
        public IActionResult FeedingTime(int zooId)
        {
            var zoo = _zooService.GetZooById(zooId);  // Haalt de dierentuin op via het ID
            if (zoo == null)
            {
                return NotFound("Zoo not found.");  // Retourneert een foutmelding als de dierentuin niet gevonden kan worden
            }

            // Roept de service methode aan om de voedertijdlogica voor de dieren te verwerken
            var feedingMessages = _zooService.FeedingTime(zoo);

            return Ok(feedingMessages);  // Retourneert de voedertijd berichten met een 200 OK respons
        }

        // POST: api/ZooApi/auto-assign - Wijs automatisch een dier toe aan een verblijf
        [HttpPost("auto-assign")]
        public IActionResult AutoAssign([FromBody] int animalId)
        {
            _zooService.AutoAssign(_zoo);  // Roept de service methode aan om automatisch een dier aan een verblijf toe te wijzen
            return Ok("Auto-assign action performed.");  // Bevestigt dat de auto-assign actie uitgevoerd is
        }

        // POST: api/ZooApi/check-constraints - Controleert de beperkingen van een specifiek dier (bijvoorbeeld verblijfsgrootte, voersoort)
        [HttpPost("check-constraints")]
        public IActionResult CheckConstraints([FromBody] int animalId)
        {
            _zooService.CheckConstraints(_zoo);  // Roept de service aan om te controleren of de beperkingen voor het dier zijn nageleefd
            return Ok("Constraints checked.");  // Bevestigt dat de beperkingen gecontroleerd zijn
        }
    }
}
