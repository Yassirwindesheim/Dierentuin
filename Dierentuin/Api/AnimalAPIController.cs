using Dierentuin.Models;
using Dierentuin.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Dierentuin.API
{
    [Route("Api/ApiAnimal")]
    [ApiController]
    public class AnimalAPIController : ControllerBase
    {
        private readonly AnimalService _animalService;

        public AnimalAPIController(AnimalService animalService)
        {
            _animalService = animalService;
        }



        // GET: api/animal
        [HttpGet]
        public ActionResult<List<Animal>> GetAnimals()
        {
            var animals = _animalService.GetAllAnimals();
            return Ok(animals);
        }

        // GET: api/animal/{id}
        [HttpGet("{id}")]
        public ActionResult<Animal> GetAnimal(int id)
        {
            var animal = _animalService.GetAnimalById(id);
            if (animal == null)
            {
                return NotFound();
            }
            return Ok(animal);
        }

        // POST: api/animal
        [HttpPost]
        public ActionResult<Animal> CreateAnimal([FromBody] Animal animal)
        {
            var createdAnimal = _animalService.CreateAnimal(animal);
            return CreatedAtAction(nameof(GetAnimal), new { id = createdAnimal.Id }, createdAnimal);
        }

        // PUT: api/animal/{id}
        [HttpPut("{id}")]
        public ActionResult<Animal> UpdateAnimal(int id, [FromBody] Animal updatedAnimal)
        {
            if (id != updatedAnimal.Id)
            {
                return BadRequest();
            }

            var animal = _animalService.UpdateAnimal(updatedAnimal);
            if (animal == null)
            {
                return NotFound();
            }

            return Ok(animal);
        }

        // DELETE: api/animal/{id}
        [HttpDelete("{id}")]
        public ActionResult DeleteAnimal(int id)
        {
            var success = _animalService.DeleteAnimal(id);
            if (!success)
            {
                return NotFound();
            }

            return NoContent();
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
