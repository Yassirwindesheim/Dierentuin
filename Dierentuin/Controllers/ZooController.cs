using Dierentuin.Enum;
using Dierentuin.Models;
using Dierentuin.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Dierentuin.Services;

namespace Dierentuin.Controllers
{
    public class ZooController : Controller
    {
        private readonly ZooService _zooService;
        private readonly Zoo _zoo;  // Zoo instance
        private readonly AnimalService _animalService;

        private readonly DBContext _context;
        // Dependency injection of ZooService and Zoo
        public ZooController(ZooService zooService, Zoo zoo, DBContext context, AnimalService animalService)
        {
            _zooService = zooService;
            _zoo = zoo; // Zoo instance injected
            _context = context; // Database context
            _animalService = animalService;  // Inject AnimalService
        }

        // GET: /Zoo/Create
        // GET: /Zoo/Create
        public async Task<IActionResult> Create()
        {
            // Fetch all animals from the service
            var animals = await _animalService.GetAllAnimals();

            // Create a MultiSelectList for the AnimalIds dropdown
            ViewBag.Animals = new MultiSelectList(animals, "Id", "Name");

            // Fetch all enclosures from the database
            var enclosures = await _context.Enclosures.ToListAsync();

            // Create a MultiSelectList for Enclosures dropdown
            ViewBag.Enclosures = new MultiSelectList(enclosures, "Id", "Name");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Zoo zoo)
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

            // Handle the animal assignments similarly
            var animals = await _context.Animals
                .Where(a => zoo.AnimalIds.Contains(a.Id))  // Assuming zoo has a list of animal IDs
                .ToListAsync();
            zoo.Animals = animals;

            // Save the zoo to the database
            _context.Zoos.Add(zoo);
            await _context.SaveChangesAsync();

            // Redirect to the Index page after saving
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int id)
        {
            var zoo = await _context.Zoos
                                    .Include(z => z.Animals)
                                    .Include(z => z.Enclosures) // Include related enclosures
                                    .FirstOrDefaultAsync(z => z.Id == id);
            if (zoo == null)
            {
                return NotFound();
            }
            return View(zoo);
        }



        public async Task<IActionResult> Delete(int id)
        {
            var zoo = await _context.Zoos
                .Include(z => z.Animals)
                .Include(z => z.Enclosures)
                .FirstOrDefaultAsync(z => z.Id == id);

            if (zoo == null)
            {
                return NotFound();
            }

            return View(zoo);
        }

        // POST: Zoo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var zoo = await _context.Zoos
                .Include(z => z.Animals)
                .Include(z => z.Enclosures)
                .FirstOrDefaultAsync(z => z.Id == id);

            if (zoo != null)
            {
                _context.Zoos.Remove(zoo);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    

    public async Task<IActionResult> Update(int id)
        {
            var zoo = await _context.Zoos
                .Include(z => z.Animals)  // Load associated animals
                .Include(z => z.Enclosures)  // Load associated enclosures
                .FirstOrDefaultAsync(z => z.Id == id);

            if (zoo == null)
            {
                return NotFound();
            }

            // Populate the ViewBag with data for the dropdowns
            ViewBag.Animals = new SelectList(await _context.Animals.ToListAsync(), "Id", "Name", zoo.Animals.Select(a => a.Id).ToList());
            ViewBag.Enclosures = new SelectList(await _context.Enclosures.ToListAsync(), "Id", "Name", zoo.Enclosures.Select(e => e.Id).ToList());

            return View(zoo);
        }

        // POST: Update Zoo
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, Zoo zoo, List<int> AnimalIds, List<int> EnclosureIds)
        {
            if (id != zoo.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingZoo = await _context.Zoos
                        .Include(z => z.Animals)
                        .Include(z => z.Enclosures)
                        .FirstOrDefaultAsync(z => z.Id == id);

                    if (existingZoo == null)
                    {
                        return NotFound();
                    }

                    // Update zoo properties
                    existingZoo.Name = zoo.Name;

                    // Update associated animals
                    existingZoo.Animals.Clear();
                    foreach (var animalId in AnimalIds)
                    {
                        var animal = await _context.Animals.FindAsync(animalId);
                        if (animal != null)
                        {
                            existingZoo.Animals.Add(animal);
                        }
                    }

                    // Update associated enclosures
                    existingZoo.Enclosures.Clear();
                    foreach (var enclosureId in EnclosureIds)
                    {
                        var enclosure = await _context.Enclosures.FindAsync(enclosureId);
                        if (enclosure != null)
                        {
                            existingZoo.Enclosures.Add(enclosure);
                        }
                    }

                    _context.Update(existingZoo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Zoos.Any(z => z.Id == zoo.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index)); // Redirect to list or another page
            }

            // If the model is invalid, re-populate the dropdowns and return the view
            ViewBag.Animals = new SelectList(await _context.Animals.ToListAsync(), "Id", "Name", AnimalIds);
            ViewBag.Enclosures = new SelectList(await _context.Enclosures.ToListAsync(), "Id", "Name", EnclosureIds);

            return View(zoo);
        }
    


    // Action to display all zoos
    public IActionResult Index()
        {
            // Get the list of zoos from the service or database
            var zoos = _zooService.GetAllZoos(); // Ensure this method exists in the service
            return View(zoos);
        }


        // Action for Sunrise
        [HttpPost]
        public IActionResult Sunrise(int id)
        {
            var zoo = _zooService.GetZooById(id); // Fetch the zoo details
            if (zoo == null) return NotFound();

            var messages = _zooService.Sunrise(zoo);
            ViewBag.ActionResults = messages;

            return View("Details", zoo);
        }

        [HttpPost]
        public IActionResult Sunset(int id)
        {
            var zoo = _zooService.GetZooById(id); // Fetch the zoo details
            if (zoo == null) return NotFound();

            var messages = _zooService.Sunset(zoo);
            ViewBag.ActionResults = messages;

            return View("Details", zoo);
        }

        [HttpPost]
        public IActionResult FeedingTime(int id)
        {
            var zoo = _zooService.GetZooById(id); // Fetch the zoo details
            if (zoo == null) return NotFound();

            var messages = _zooService.FeedingTime(zoo);
            ViewBag.ActionResults = messages;

            return View("Details", zoo);
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
