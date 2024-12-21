using Dierentuin.Enum;
using Dierentuin.Models;
using Dierentuin.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;

namespace Dierentuin.Controllers
{
    public class EnclosureController : Controller
    {
        private readonly EnclosureService _enclosureService;
        private readonly DBContext _context;

        // Constructor to inject EnclosureService and DBContext via dependency injection
        public EnclosureController(EnclosureService enclosureService, DBContext context)
        {
            _enclosureService = enclosureService;
            _context = context;
        }

        // Action for listing all enclosures
        public IActionResult Index()
        {
            var enclosures = _enclosureService.GetAllEnclosures();
            return View(enclosures); // Pass the list of enclosures to the view
        }

        // Action for viewing details of a specific enclosure
        public IActionResult Details(int id)
        {
            var enclosure = _enclosureService.GetEnclosureById(id);
            if (enclosure == null)
            {
                return NotFound(); // Return 404 if the enclosure is not found
            }
            return View(enclosure);
        }

        // Action for creating a new enclosure (GET)
        public IActionResult Create()
        {
            ViewBag.Climates = EnumHelper.GetSelectList<Climate>();
            ViewBag.HabitatTypes = EnumHelper.GetSelectList<HabitatType>();
            ViewBag.SecurityLevels = EnumHelper.GetSelectList<SecurityLevel>();
            return View();
        }

        // Action for creating a new enclosure (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Enclosure enclosure)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _enclosureService.CreateEnclosure(enclosure);
                    return RedirectToAction(nameof(Index)); // Redirect after successful creation
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Unable to save changes. Try again. Error: {ex.Message}");
                }
            }
            ViewBag.Climates = EnumHelper.GetSelectList<Climate>();
            ViewBag.HabitatTypes = EnumHelper.GetSelectList<HabitatType>();
            ViewBag.SecurityLevels = EnumHelper.GetSelectList<SecurityLevel>();
            return View(enclosure);
        }

        // Action for editing an existing enclosure (GET)
        public IActionResult Update(int id)
        {
            var enclosure = _enclosureService.GetEnclosureById(id);
            if (enclosure == null)
            {
                return NotFound();
            }

            // Populate Enum-based dropdowns
            ViewBag.Climates = EnumHelper.GetSelectList<Climate>();
            ViewBag.HabitatTypes = EnumHelper.GetSelectList<HabitatType>();
            ViewBag.SecurityLevels = EnumHelper.GetSelectList<SecurityLevel>();

            // Check if Animals exists and is not null before processing
            ViewBag.AnimalList = _context.Animals
                                          .Select(a => new SelectListItem
                                          {
                                              Value = a.Id.ToString(),
                                              Text = a.Name
                                          })
                                          .ToList(); // Using the default list directly if no animals available

            return View(enclosure);
        }

        // Action for editing an existing enclosure (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(Enclosure updatedEnclosure)
        {
            if (ModelState.IsValid)
            {
                _enclosureService.UpdateEnclosure(updatedEnclosure);
                return RedirectToAction(nameof(Index));
            }
            return View(updatedEnclosure);
        }

        // Action for deleting an enclosure (GET)
        public IActionResult Delete(int id)
        {
            var enclosure = _enclosureService.GetEnclosureById(id);
            if (enclosure == null)
            {
                return NotFound();
            }
            return View(enclosure);
        }

        // Action for deleting an enclosure (POST)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            bool success = _enclosureService.DeleteEnclosure(id);
            if (success)
            {
                return RedirectToAction(nameof(Index)); // Redirect if deletion was successful
            }
            return NotFound(); // Return 404 if deletion failed
        }
    }
}
