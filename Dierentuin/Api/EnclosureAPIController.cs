using Dierentuin.Data;
using Dierentuin.Models;
using Dierentuin.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

// Zelfde geldt hier gewoon een api controller die cruds acties uitvoert door middel van de services (samenwerking).
// Was zeker een uitdaging.

namespace Dierentuin.API
{
    [Route("Api/ApiEnclosure")]  // Specificeert het routepad voor de Enclosure API controller
    [ApiController]  // Geeft aan dat dit een API-controller is
    public class EnclosureApiController : ControllerBase
    {
        private readonly EnclosureService _enclosureService;  // De service die de bedrijfslogica voor de verblijven afhandelt
        private readonly DBContext _context;  // De context voor toegang tot de database
        private readonly ILogger<CategoryApiController> _logger;  // Logger voor het loggen van belangrijke gebeurtenissen

        // Constructor die de EnclosureService, de DBContext en de logger injecteert via dependency injection
        public EnclosureApiController(EnclosureService enclosureService, DBContext context, ILogger<CategoryApiController> logger)
        {
            _enclosureService = enclosureService;
            _context = context;  // Initialiseer de context
            _logger = logger;  // Initialiseer de logger
        }

        // GET: api/enclosure - Haalt een lijst van alle verblijven op
        [HttpGet]
        public ActionResult<List<Enclosure>> GetEnclosures()
        {
            var enclosures = _enclosureService.GetAllEnclosures();  // Haalt alle verblijven op via de service
            return Ok(enclosures);  // Retourneert de verblijven als een succesvolle response
        }

        // GET: api/enclosure/{id} - Haalt een specifiek verblijf op op basis van het ID
        [HttpGet("{id}")]
        public ActionResult<Enclosure> GetEnclosure(int id)
        {
            var enclosure = _enclosureService.GetEnclosureById(id);  // Haalt het verblijf op via de service
            if (enclosure == null)
            {
                return NotFound();  // Retourneert een 404-statuscode als het verblijf niet wordt gevonden
            }
            return Ok(enclosure);  // Retourneert het verblijf als een succesvolle response
        }

        // POST: api/enclosure - Creëert een nieuw verblijf
        [HttpPost]
        public async Task<ActionResult<Enclosure>> CreateEnclosure([FromBody] Enclosure enclosure)
        {
            _logger.LogInformation("Creating a new enclosure with animals.");  // Logt dat we een nieuw verblijf aanmaken
            var createdEnclosure = await _enclosureService.CreateEnclosure(enclosure);  // Maakt het verblijf aan via de service
            _logger.LogInformation($"Enclosure created with ID {createdEnclosure.Id} and {enclosure.AnimalIds?.Count ?? 0} animals assigned");  // Logt het ID van het aangemaakte verblijf en het aantal toegewezen dieren
            return CreatedAtAction(nameof(GetEnclosure), new { id = createdEnclosure.Id }, createdEnclosure);  // Retourneert het aangemaakte verblijf met een 201-statuscode
        }

        // PUT: api/enclosure/{id} - Werkt een bestaand verblijf bij
        [HttpPut("{id}")]
        public async Task<ActionResult<Enclosure>> UpdateEnclosure(int id, [FromBody] Enclosure updatedEnclosure)
        {
            if (id != updatedEnclosure.Id)  // Controleert of het ID in de URL overeenkomt met het ID in het object
            {
                return BadRequest("ID mismatch");  // Retourneert een foutmelding als de ID's niet overeenkomen
            }

            _logger.LogInformation($"Updating enclosure with ID: {id}");  // Logt dat we een verblijf bijwerken
            var enclosure = await _enclosureService.UpdateEnclosure(updatedEnclosure);  // Werk het verblijf bij via de service
            if (enclosure == null)
            {
                _logger.LogWarning($"Enclosure with ID {id} not found.");  // Logt een waarschuwing als het verblijf niet wordt gevonden
                return NotFound($"Enclosure with ID {id} not found.");  // Geeft een 404-statuscode als het verblijf niet wordt gevonden
            }

            _logger.LogInformation($"Enclosure updated with ID {id} and {updatedEnclosure.AnimalIds?.Count ?? 0} animals assigned");  // Logt de ID van het bijgewerkte verblijf en het aantal toegewezen dieren
            return Ok(enclosure);  // Retourneert het bijgewerkte verblijf als een succesvolle response
        }

        // DELETE: api/enclosure/{id} - Verwijdert een verblijf op basis van het ID
        [HttpDelete("{id}")]
        public ActionResult DeleteEnclosure(int id)
        {
            try
            {
                var success = _enclosureService.DeleteEnclosure(id);  // Verwijdert het verblijf via de service
                if (!success)
                {
                    return NotFound();  // Retourneert een 404-statuscode als het verblijf niet gevonden kan worden
                }
                return Ok(new { message = $"Enclosure with ID {id} deleted successfully" });  // Retourneert een bevestigingsbericht als het verblijf succesvol is verwijderd
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);  // Geeft een duidelijke foutmelding terug aan de client
            }
        }

        // ADD animal to the enclosure (POST action) - Voegt een dier toe aan een verblijf
        [HttpPost("addanimal/{enclosureId}/{animalId}")]
        public async Task<ActionResult<Enclosure>> AddAnimalToEnclosure(int enclosureId, int animalId)
        {
            var enclosure = await _enclosureService.GetEnclosureById(enclosureId);  // Haalt het verblijf op via de service
            if (enclosure == null)
            {
                return NotFound();  // Retourneert een 404-statuscode als het verblijf niet wordt gevonden
            }

            var animal = _context.Animals.FirstOrDefault(a => a.Id == animalId);  // Haalt het dier op via de context
            if (animal == null)
            {
                return NotFound();  // Retourneert een 404-statuscode als het dier niet wordt gevonden
            }

            _enclosureService.AddAnimalToEnclosure(enclosureId, animal);  // Voegt het dier toe aan het verblijf via de service
            return Ok($"Animal {animal.Name} added to enclosure {enclosure.Name}");  // Retourneert een bevestigingsbericht met het toegevoegde dier en verblijf
        }
    }
}
