using Dierentuin.Models;
using Dierentuin.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;  // Add this for logging

namespace Dierentuin.API
{
    [Route("Api/ApiAnimal")]
    [ApiController]
    public class AnimalAPIController : ControllerBase
    {
        private readonly AnimalService _animalService;
        private readonly ILogger<AnimalAPIController> _logger;  // Add logger

        public AnimalAPIController(AnimalService animalService, ILogger<AnimalAPIController> logger)
        {
            _animalService = animalService;
            _logger = logger; // Initialize logger

        }



        // GET: Api/ApiAnimal
        [HttpGet]
        public async Task<ActionResult<List<Animal>>> GetAnimals()
        {
            _logger.LogInformation("Getting all animals.");
            var animals = await _animalService.GetAllAnimals();
            if (animals == null || animals.Count == 0)
            {
                _logger.LogWarning("No animals found in the system.");
                return NotFound("No animals found.");
            }
            _logger.LogInformation($"{animals.Count} animals found.");
            return Ok(animals);
        }


        // GET: Api/ApiAnimal/{id}
        [HttpGet("{id}")]
        public ActionResult<Animal> GetAnimal(int id)
        {
            _logger.LogInformation($"Getting animal with ID: {id}");
            var animal = _animalService.GetAnimalById(id);
            if (animal == null)
            {
                _logger.LogWarning($"Animal with ID {id} not found.");
                return NotFound($"Animal with ID {id} not found.");
            }
            _logger.LogInformation($"Animal with ID {id} found: {animal.Name}");
            return Ok(animal);
        }

        // POST: Api/ApiAnimal
        [HttpPost]
        public ActionResult<Animal> CreateAnimal([FromBody] Animal animal)
        {
            _logger.LogInformation("Creating a new animal.");
            var createdAnimal = _animalService.CreateAnimal(animal);
            _logger.LogInformation($"Animal created with ID {createdAnimal.Id}");
            return CreatedAtAction(nameof(GetAnimal), new { id = createdAnimal.Id }, createdAnimal);
        }

        // PUT: Api/ApiAnimal/{id}
        //dont forget to send the "id" : number object in the request body when sending a request!
        [HttpPut("{id}")]
        public ActionResult<Animal> UpdateAnimal(int id, [FromBody] Animal updatedAnimal)
        {
            // You don't need this check anymore since `id` is provided in the route
            // if (id != updatedAnimal.Id) { return BadRequest(); }

            var animal = _animalService.UpdateAnimal(updatedAnimal);
            if (animal == null)
            {
                return NotFound();
            }

            return Ok(animal);
        }


        // DELETE: Api/ApiAnimal/{id}
        [HttpDelete("{id}")]
        public ActionResult DeleteAnimal(int id)
        {
            var success = _animalService.DeleteAnimal(id);
            if (!success)
            {
                return NotFound();
            }

            return Ok(new { message = $"Animal with ID {id} deleted successfully" });
        }

        // Additional actions (e.g., Sunrise, Sunset, FeedingTime)

        // Sunrise action - needs an animal ID to perform the action on a specific animal
        [HttpPost("sunrise/{id}")]
        public IActionResult Sunrise(int id)
        {
            var animal = _animalService.GetAnimalById(id);
            if (animal == null)
            {
                return NotFound();
            }

            _animalService.Sunrise(animal); // Pass the animal object to the service method
            return Ok("Sunrise action performed");
        }

        // Sunset action - needs an animal ID to perform the action on a specific animal
        [HttpPost("sunset/{id}")]
        public IActionResult Sunset(int id)
        {
            var animal = _animalService.GetAnimalById(id);
            if (animal == null)
            {
                return NotFound();
            }

            _animalService.Sunset(animal); // Pass the animal object to the service method
            return Ok("Sunset action performed");
        }

        // FeedingTime action - needs an animal ID to perform the action on a specific animal
        [HttpPost("feedingtime/{id}")]
        public IActionResult FeedingTime(int id)
        {
            var animal = _animalService.GetAnimalById(id);
            if (animal == null)
            {
                return NotFound();
            }

            _animalService.FeedingTime(animal); // Pass the animal object to the service method
            return Ok("Feedingtime action performed");
        }
    }
}
