using Dierentuin.Models;
using Dierentuin.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Dierentuin.API
{
    [Route("Api/ApiEnclosure")]
    [ApiController]
    public class EnclosureAPIController : ControllerBase
        
    {
        private readonly EnclosureService _enclosureService;
        private readonly DBContext _context; // Add the DbContext here

        public EnclosureAPIController(EnclosureService enclosureService, DBContext context)
        {
            _enclosureService = enclosureService;
            _context = context; // Initialize the context
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
        public ActionResult<Enclosure> CreateEnclosure([FromBody] Enclosure enclosure)
        {
            var createdEnclosure = _enclosureService.CreateEnclosure(enclosure);
            return CreatedAtAction(nameof(GetEnclosure), new { id = createdEnclosure.Id }, createdEnclosure);
        }

        // PUT: api/enclosure/{id}
        [HttpPut("{id}")]
        public ActionResult<Enclosure> UpdateEnclosure(int id, [FromBody] Enclosure updatedEnclosure)
        {
            if (id != updatedEnclosure.Id)
            {
                return BadRequest();
            }

            var enclosure = _enclosureService.UpdateEnclosure(updatedEnclosure);
            if (enclosure == null)
            {
                return NotFound();
            }

            return Ok(enclosure);
        }

        // DELETE: api/enclosure/{id}
        [HttpDelete("{id}")]
        public ActionResult DeleteEnclosure(int id)
        {
            var success = _enclosureService.DeleteEnclosure(id);
            if (!success)
            {
                return NotFound();
            }

            return NoContent();
        }

        // ADD animal to the enclosure (POST action)
        [HttpPost("addanimal/{enclosureId}/{animalId}")]
        public IActionResult AddAnimalToEnclosure(int enclosureId, int animalId)
        {
            var enclosure = _enclosureService.GetEnclosureById(enclosureId);
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

        // REMOVE animal from the enclosure (DELETE action)
        [HttpDelete("removeanimal/{enclosureId}/{animalId}")]
        public IActionResult RemoveAnimalFromEnclosure(int enclosureId, int animalId)
        {
            var enclosure = _enclosureService.GetEnclosureById(enclosureId);
            if (enclosure == null)
            {
                return NotFound();
            }

            var animal = enclosure.Animals.FirstOrDefault(a => a.Id == animalId);
            if (animal == null)
            {
                return NotFound();
            }

            _enclosureService.RemoveAnimalFromEnclosure(enclosureId, animalId);
            return Ok($"Animal {animal.Name} removed from enclosure {enclosure.Name}");
        }
    }
}
