using Dierentuin.Models;
using Dierentuin.Services;
using Microsoft.AspNetCore.Mvc;

namespace Dierentuin.Controllers
{
    public class ZooController : Controller
    {
        private readonly ZooService _zooService;
        private readonly Zoo _zoo;  // Zoo instance

        // Dependency injection of ZooService and Zoo
        public ZooController(ZooService zooService, Zoo zoo)
        {
            _zooService = zooService;
            _zoo = zoo; // Zoo instance injected
        }

        // Action for Sunrise
        public IActionResult Sunrise()
        {
            _zooService.Sunrise(_zoo);  // Passing Zoo to service method
            return View();
        }

        // Action for Sunset
        public IActionResult Sunset()
        {
            _zooService.Sunset(_zoo);  // Passing Zoo to service method
            return View();
        }

        // Action for FeedingTime
        public IActionResult FeedingTime()
        {
            _zooService.FeedingTime(_zoo);  // Passing Zoo to service method
            return View();
        }

        // Action for CheckConstraints
        public IActionResult CheckConstraints()
        {
            _zooService.CheckConstraints(_zoo);  // Passing Zoo to service method
            return View();
        }

        // Action for AutoAssign
        public IActionResult AutoAssign()
        {
            _zooService.AutoAssign(_zoo);  // Passing Zoo to service method
            return View();
        }
    }
}
