using Dierentuin.Models;
using Dierentuin.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;  
namespace Dierentuin.API

    //Dit is de api controller voor Animal. Deze controller is verantwoordelijk voor beheren van de dieren door http verzoeken. 
    // De controller bevat verschillende acties die samen werken met de AnimalService om de crud acties te doen. Dus GEEN views dat doet de normale controller.
{
    [Route("Api/ApiAnimal")] // Route voor de API-controller
    [ApiController]
    public class AnimalApiController : ControllerBase
    {
        private readonly AnimalService _animalService; // Service om de dieren te beheren
        private readonly ILogger<AnimalApiController> _logger;  // Logger voor het loggen van informatie

        // Constructor voor het injecteren van de AnimalService en Logger via dependency injection
        public AnimalApiController(AnimalService animalService, ILogger<AnimalApiController> logger)
        {
            _animalService = animalService;
            _logger = logger; // Initialiseer de logger
        }

        // GET: Api/ApiAnimal
        // Haalt een lijst op van alle dieren in de dierentuin
        [HttpGet]
        public async Task<ActionResult<List<Animal>>> GetAnimals()
        {
            _logger.LogInformation("Getting all animals."); // Log de actie
            var animals = await _animalService.GetAllAnimals(); // Haal de dieren op
            if (animals == null || animals.Count == 0)
            {
                _logger.LogWarning("No animals found in the system."); // Log een waarschuwing als er geen dieren zijn
                return NotFound("No animals found."); // Retourneer 404 als geen dieren gevonden zijn
            }
            _logger.LogInformation($"{animals.Count} animals found."); // Log het aantal gevonden dieren
            return Ok(animals); // Retourneer de lijst van dieren
        }

        // GET: Api/ApiAnimal/{id}
        // Haalt een specifiek dier op op basis van het ID
        [HttpGet("{id}")]
        public ActionResult<Animal> GetAnimal(int id)
        {
            _logger.LogInformation($"Getting animal with ID: {id}"); // Log het verzoek
            var animal = _animalService.GetAnimalById(id); // Haal het dier op
            if (animal == null)
            {
                _logger.LogWarning($"Animal with ID {id} not found."); // Log een waarschuwing als het dier niet gevonden is
                return NotFound($"Animal with ID {id} not found."); // Retourneer 404 als het dier niet gevonden is
            }
            _logger.LogInformation($"Animal with ID {id} found: {animal.Name}"); // Log als het dier is gevonden
            return Ok(animal); // Retourneer het gevonden dier
        }

        // POST: Api/ApiAnimal
        // Creëer een nieuw dier in de database
        [HttpPost]
        public ActionResult<Animal> CreateAnimal([FromBody] Animal animal)
        {
            _logger.LogInformation("Creating a new animal."); // Log het verzoek om een nieuw dier te creëren
            var createdAnimal = _animalService.CreateAnimal(animal); // Maak het dier aan
            _logger.LogInformation($"Animal created with ID {createdAnimal.Id}"); // Log het ID van het gemaakte dier
            return CreatedAtAction(nameof(GetAnimal), new { id = createdAnimal.Id }, createdAnimal); // Retourneer 201 met het gemaakte dier
        }

        // PUT: Api/ApiAnimal/{id}
        // Werk een bestaand dier bij in de database
        [HttpPut("{id}")]
        public ActionResult<Animal> UpdateAnimal(int id, [FromBody] Animal updatedAnimal)
        {
            // In dit geval hoeven we de ID niet te verifiëren, omdat deze al in de route wordt meegegeven
            var animal = _animalService.UpdateAnimal(updatedAnimal); // Werk het dier bij
            if (animal == null)
            {
                return NotFound(); // Retourneer 404 als het dier niet gevonden kan worden
            }

            return Ok(animal); // Retourneer het bijgewerkte dier
        }

        // DELETE: Api/ApiAnimal/{id}
        // Verwijder een dier op basis van het ID
        [HttpDelete("{id}")]
        public ActionResult DeleteAnimal(int id)
        {
            var success = _animalService.DeleteAnimal(id); // Verwijder het dier
            if (!success)
            {
                return NotFound(); // Retourneer 404 als het dier niet gevonden kan worden
            }

            return Ok(new { message = $"Animal with ID {id} deleted successfully" }); // Bevestig succesvolle verwijdering
        }

        // Extra acties (bijv. Sunrise, Sunset, FeedingTime)

        // Sunrise actie - Zorgt ervoor dat een dier in de ochtend activiteit wordt gezet
        [HttpPost("sunrise/{id}")]
        public IActionResult Sunrise(int id)
        {
            var animal = _animalService.GetAnimalById(id); // Haal het dier op
            if (animal == null)
            {
                return NotFound(); // Retourneer 404 als het dier niet gevonden is
            }

            _animalService.Sunrise(animal); // Voer de Sunrise actie uit op het dier
            return Ok("Sunrise action performed"); // Bevestig de uitvoering van de Sunrise actie
        }

        // Sunset actie - Zorgt ervoor dat een dier in de avond activiteit wordt gezet
        [HttpPost("sunset/{id}")]
        public IActionResult Sunset(int id)
        {
            var animal = _animalService.GetAnimalById(id); // Haal het dier op
            if (animal == null)
            {
                return NotFound(); // Retourneer 404 als het dier niet gevonden is
            }

            _animalService.Sunset(animal); // Voer de Sunset actie uit op het dier
            return Ok("Sunset action performed"); // Bevestig de uitvoering van de Sunset actie
        }

        // FeedingTime actie - Zorgt ervoor dat een dier zijn maaltijd krijgt
        [HttpPost("feedingtime/{id}")]
        public IActionResult FeedingTime(int id)
        {
            var animal = _animalService.GetAnimalById(id); // Haal het dier op
            if (animal == null)
            {
                return NotFound(); // Retourneer 404 als het dier niet gevonden is
            }

            _animalService.FeedingTime(animal); // Voer de FeedingTime actie uit op het dier
            return Ok("Feedingtime action performed"); // Bevestig de uitvoering van de FeedingTime actie
        }
    }
}
