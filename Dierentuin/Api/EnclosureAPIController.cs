using Dierentuin.Models;
using Dierentuin.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace Dierentuin.API
{
    [Route("Api/ApiEnclosure")]
    [ApiController]
    public class EnclosureAPIController : ControllerBase

    {
        private readonly EnclosureService _enclosureService;
        private readonly DBContext _context; // Add the DbContext here
        private readonly ILogger<CategoryAPIController> _logger;  // Add logger

        public EnclosureAPIController(EnclosureService enclosureService, DBContext context, ILogger<CategoryAPIController> logger)
        {
            _enclosureService = enclosureService;
            _context = context; // Initialize the context
            _logger = logger; // Initialize logger
        }

        // GET: api/enclosure
        [HttpGet]
        public ActionResult<List<Enclosure>> GetEnclosures()
        {
            var enclosures = _enclosureService.GetAllEnclosures();
            return Ok(enclosures);
        }

        // GET: api/enclosure/{id}
        [HttpGet("{id}")]
        public ActionResult<Enclosure> GetEnclosure(int id)
        {
            var enclosure = _enclosureService.GetEnclosureById(id);
            if (enclosure == null)
            {
                return NotFound();
            }
            return Ok(enclosure);
        }

        // POST: api/enclosure
        [HttpPost]
        public async Task<ActionResult<Enclosure>> CreateEnclosure([FromBody] Enclosure enclosure)
        {
            _logger.LogInformation("Creating a new enclosure with animals.");
            var createdEnclosure = await _enclosureService.CreateEnclosure(enclosure);
            _logger.LogInformation($"Category created with ID {createdEnclosure.Id} and {enclosure.AnimalIds?.Count ?? 0} animals assigned");
            return CreatedAtAction(nameof(GetEnclosure), new { id = createdEnclosure.Id }, createdEnclosure);
        }


        // PUT: api/enclosure/{id}
        [HttpPut("{id}")]

        public async Task<ActionResult<Enclosure>> UpdateEnclosure(int id, [FromBody] Enclosure updatedEnclosure)
        {
            if (id != updatedEnclosure.Id)
            {
                return BadRequest("ID mismatch");
            }

            _logger.LogInformation($"Updating category with ID: {id}");
            var enclosure = await _enclosureService.UpdateEnclosure(updatedEnclosure);
            if (enclosure == null)
            {
                _logger.LogWarning($"enclosure with ID {id} not found.");
                return NotFound($"enclosure with ID {id} not found.");
            }

            _logger.LogInformation($"enclosure updated with ID {id} and {updatedEnclosure.AnimalIds?.Count ?? 0} animals assigned");
            return Ok(enclosure);
        }

        // DELETE: api/enclosure/{id}
        [HttpDelete("{id}")]
        public ActionResult DeleteEnclosure(int id)
        {
            try
            {
                var success = _enclosureService.DeleteEnclosure(id);
                if (!success)
                {
                    return NotFound();
                }
                return Ok(new { message = $"Enclosure with ID {id} deleted successfully" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);  // Geeft een duidelijke foutmelding terug aan de client
            }
        }

        // ADD animal to the enclosure (POST action)
        [HttpPost("addanimal/{enclosureId}/{animalId}")]
        public async Task<ActionResult<Enclosure>> AddAnimalToEnclosure(int enclosureId, int animalId)
        {
            var enclosure = await _enclosureService.GetEnclosureById(enclosureId);
            if (enclosure == null)
            {
                return NotFound();
            }

            var animal = _context.Animals.FirstOrDefault(a => a.Id == animalId);
            if (animal == null)
            {
                return NotFound();
            }

            _enclosureService.AddAnimalToEnclosure(enclosureId, animal);
            return Ok($"Animal {animal.Name} added to enclosure {enclosure.Name}");
        }

    }
}