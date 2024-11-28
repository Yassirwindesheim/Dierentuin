using Microsoft.AspNetCore.Mvc;
using Dierentuin.Models;
using Dierentuin.Services;
using System.Collections.Generic;

namespace Dierentuin.API
{
    [ApiController]  // Marks this as an API controller
    [Route("Api/ZooApi")]  // Base route for all actions in this controller
    public class ZooAPIController : ControllerBase
    {
        private readonly ZooService _zooService;

        // Dependency Injection of ZooService
        public ZooAPIController(ZooService zooService)
        {
            _zooService = zooService;
        }

        // Sunrise action with parameters
        [HttpPost("sunrise")]
        public IActionResult Sunrise([FromBody] Zoo zoo)
        // Accepts Zoo object as input
        {
            _zooService.Sunrise(zoo);  // Pass the zoo object to the service
            return Ok("Sunrise action performed.");
        }

        // Sunset action with parameters
        [HttpPost("sunset")]
        public IActionResult Sunset([FromBody] Zoo zoo)  // Accepts Zoo object as input
        {
            _zooService.Sunset(zoo);  // Pass the zoo object to the service
            return Ok("Sunset action performed.");
        }

        // FeedingTime action with parameters
        [HttpPost("feeding-time")]
        public IActionResult FeedingTime([FromBody] Zoo zoo)  // Accepts Zoo object as input
        {
            _zooService.FeedingTime(zoo);  // Pass the zoo object to the service
            return Ok("Feeding time action performed.");
        }

        // AutoAssign action with parameters
        [HttpPost("auto-assign")]
        public IActionResult AutoAssign([FromBody] Zoo zoo)  // Accepts Zoo object as input
        {
            _zooService.AutoAssign(zoo);  // Pass the zoo object to the service
            return Ok("Auto-assign action performed.");
        }

        // CheckConstraints action with parameters
        [HttpPost("check-constraints")]
        public IActionResult CheckConstraints([FromBody] Zoo zoo)  // Accepts Zoo object as input
        {
            _zooService.CheckConstraints(zoo);  // Pass the zoo object to the service
            return Ok("Constraints checked.");
        }
    }
}
