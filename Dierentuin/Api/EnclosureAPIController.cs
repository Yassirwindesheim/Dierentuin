using Dierentuin.Services;
using Microsoft.AspNetCore.Mvc;

[Route("api/ApiEnclosure")]
[ApiController]
public class EnclosureAPIController : ControllerBase
{
    private readonly EnclosureService _enclosureService;

    public EnclosureAPIController(EnclosureService enclosureService)
    {
        _enclosureService = enclosureService;
    }

    // POST: api/ApiEnclosure
    [HttpPost]
    public ActionResult<Enclosure> CreateEnclosure([FromBody] Enclosure enclosure)
    {
        var createdEnclosure = _enclosureService.CreateEnclosure(enclosure);
        return CreatedAtAction(nameof(GetEnclosure), new { id = createdEnclosure.Id }, createdEnclosure);
    }

    // GET: api/ApiEnclosure
    [HttpGet]
    public ActionResult<List<Enclosure>> GetEnclosures()
    {
        return Ok(_enclosureService.GetAllEnclosures());
    }

    // GET: api/ApiEnclosure/{id}
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

    // PUT: api/ApiEnclosure/{id}
    [HttpPut("{id}")]
    public ActionResult<Enclosure> UpdateEnclosure(int id, [FromBody] Enclosure updatedEnclosure)
    {
        if (id != updatedEnclosure.Id)
        {
            return BadRequest("The ID in the URL doesn't match the ID in the body.");
        }

        var enclosure = _enclosureService.UpdateEnclosure(updatedEnclosure);
        if (enclosure == null)
        {
            return NotFound();
        }

        return Ok(enclosure);
    }

    // DELETE: api/ApiEnclosure/{id}
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

    // POST: api/ApiEnclosure/{enclosureId}/assignanimal/{animalId}
    [HttpPost("{enclosureId}/assignanimal/{animalId}")]
    public IActionResult AssignAnimalToEnclosure(int enclosureId, int animalId)
    {
        // Call the service to assign the animal to the enclosure
        _enclosureService.AssignAnimalToEnclosure(animalId, enclosureId);

        // Return a success message
        return Ok($"Animal {animalId} has been assigned to Enclosure {enclosureId}.");
    }

    // Additional actions (e.g., Sunrise, Sunset, FeedingTime, etc.)
    // Example of Sunrise action
    [HttpPost("{id}/sunrise")]
    public IActionResult Sunrise(int id)
    {
        var enclosure = _enclosureService.GetEnclosureById(id);
        if (enclosure == null)
        {
            return NotFound();
        }

        _enclosureService.Sunrise(enclosure);
        return Ok("Sunrise action executed.");
    }

    // Example of Sunset action
    [HttpPost("{id}/sunset")]
    public IActionResult Sunset(int id)
    {
        var enclosure = _enclosureService.GetEnclosureById(id);
        if (enclosure == null)
        {
            return NotFound();
        }

        _enclosureService.Sunset(enclosure);
        return Ok("Sunset action executed.");
    }

    // Example of FeedingTime action
    [HttpPost("{id}/feedingtime")]
    public IActionResult FeedingTime(int id)
    {
        var enclosure = _enclosureService.GetEnclosureById(id);
        if (enclosure == null)
        {
            return NotFound();
        }

        _enclosureService.FeedingTime(enclosure);
        return Ok("Feeding time action executed.");
    }

    // Example of CheckConstraints action
    [HttpPost("{id}/checkconstraints")]
    public IActionResult CheckConstraints(int id)
    {
        var enclosure = _enclosureService.GetEnclosureById(id);
        if (enclosure == null)
        {
            return NotFound();
        }

        _enclosureService.CheckConstraints(enclosure);
        return Ok("Constraints checked.");
    }
}
