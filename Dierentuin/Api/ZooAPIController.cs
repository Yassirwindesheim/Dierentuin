using Microsoft.AspNetCore.Mvc;
using Dierentuin.Services;
using Dierentuin.Models;
using Dierentuin.Enum;
using Microsoft.EntityFrameworkCore;

namespace Dierentuin.API
{
    [ApiController]  // Marks this as an API controller
    [Route("Api/ZooApi")]  // Base route for all actions in this controller
    public class ZooAPIController : ControllerBase
    {
        private readonly ZooService _zooService;
        private readonly Zoo _zoo;
        private readonly DBContext _context; // Use DBContext here, not DbContext

        // Dependency Injection of ZooService
        public ZooAPIController(ZooService zooService, Zoo zoo, DBContext context)
        {   
            _zooService = zooService ?? throw new ArgumentNullException(nameof(zooService));
            _zoo = zoo ?? throw new ArgumentNullException(nameof(zoo));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<Zoo>>> GetAllZoos()
        {
            var zoos = await _zooService.GetZoosAsync(); // Replace this with your actual method to retrieve zoos from your service
            if (zoos == null || !zoos.Any())
            {
                return NotFound();
            }
            return Ok(zoos);
        }



        public class ZooRequest
        {
            public List<int> AnimalIds { get; set; }
            public List<Enclosure> Enclosures { get; set; }
        }
        [HttpPost("create")]
        public async Task<IActionResult> CreateZoo([FromBody] Zoo zoo)
        {
            if (zoo == null)
            {
                return BadRequest("Zoo data is missing.");
            }

            // Retrieve the enclosures by IDs
            var enclosures = await _context.Enclosures
                .Where(e => zoo.Enclosures.Select(enclosure => enclosure.Id).Contains(e.Id))
                .ToListAsync();

            // Assign the retrieved enclosures to the zoo
            zoo.Enclosures = enclosures;

            // Handle the animal assignments similarly (optional)
            var animals = await _context.Animals
                .Where(a => zoo.AnimalIds.Contains(a.Id)) // Assuming you still have AnimalIds field for animals
                .ToListAsync();
            zoo.Animals = animals;

            // Save the zoo
            _context.Zoos.Add(zoo);
            await _context.SaveChangesAsync();

            return Ok(zoo);
        }




        public class AnimalIdsRequest
        {
            public List<int> AnimalId { get; set; }
        }
        [HttpPost("sunrise/{zooId}")]
        public IActionResult Sunrise(int zooId, [FromBody] AnimalIdsRequest request)
        {
            var zoo = _zooService.GetZooById(zooId);
            if (zoo == null)
            {
                return NotFound("Zoo not found.");
            }

            // Now the zoo should have its animals properly loaded
            var result = new List<string>();
            foreach (var animalId in request.AnimalId)
            {
                var animal = zoo.Animals.FirstOrDefault(a => a.Id == animalId);
                if (animal == null)
                {
                    result.Add($"Animal with ID {animalId} not found.");
                }
                else
                {
                    result.Add(
                        animal.ActivityPattern == ActivityPattern.Diurnal
                        ? $"{animal.Name} is wakker geworden."
                        : $"{animal.Name} slaapt nog."
                    );
                }
            }

            return Ok(result);
        }

        [HttpPost("sunset/{zooId}")]
        public IActionResult Sunset(int zooId)
        {
            var zoo = _zooService.GetZooById(zooId); // Use dynamic Zoo ID from route
            if (zoo == null)
            {
                return NotFound("Zoo not found.");
            }

            var result = new List<string>();

            foreach (var animal in zoo.Animals)
            {
                if (animal.ActivityPattern == ActivityPattern.Nocturnal)
                {
                    result.Add($"{animal.Name} is wakker geworden."); // Nocturnal animals wake up
                }
                else if (animal.ActivityPattern == ActivityPattern.Diurnal)
                {
                    result.Add($"{animal.Name} gaat slapen."); // Diurnal animals go to sleep
                }
            }

            return Ok(result); // Return the list of messages
        }






        // Sunset action for a specific animal
        [HttpPost("feeding-time/{zooId}")]
        public IActionResult FeedingTime(int zooId)
        {
            var zoo = _zooService.GetZooById(zooId); // Use dynamic Zoo ID from route
            if (zoo == null)
            {
                return NotFound("Zoo not found.");
            }

            // Call the service method to handle feeding time logic and capture the result
            var feedingMessages = _zooService.FeedingTime(zoo);

            // Return the feeding messages
            return Ok(feedingMessages);
        }




        // Auto-assign action for a specific animal (assign it to an enclosure)
        [HttpPost("auto-assign")]
        public IActionResult AutoAssign([FromBody] int animalId)
        {
            _zooService.AutoAssign(_zoo);  // Call service and pass Zoo object
            return Ok("Auto-assign action performed.");
        }




        // CheckConstraints action for a specific animal (check if constraints are met)
        [HttpPost("check-constraints")]
        public IActionResult CheckConstraints([FromBody] int animalId)
        {
            _zooService.CheckConstraints(_zoo);  // Call service and pass Zoo object
            return Ok("Constraints checked.");
        }
    }
}