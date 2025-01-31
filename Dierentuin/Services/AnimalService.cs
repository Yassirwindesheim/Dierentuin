using Dierentuin.Enum;
using Dierentuin.Models;
using Microsoft.EntityFrameworkCore;
using Dierentuin.Data;


//De AnimalService beheert dieren, hun verblijven en dieet, en biedt CRUD-operaties. Het controleert ook ruimte- en beveiligingseisen voor dieren.
//Moeilijkheden hierbij zijn het beheren van de relaties tussen dieren en verblijven, en de logica voor het controleren van ruimte- en veiligheidsvereisten.
// Was uiteindelijk wel te doen!


namespace Dierentuin.Services
{
    public class AnimalService
    {
        private readonly DBContext _context;

        // Constructor om DBContext via dependency injection in te voegen
        public AnimalService(DBContext context)
        {
            _context = context;
        }

        // CRUD Operaties
        public async Task<List<Animal>> GetAllAnimals()
        {
            var animals = await _context.Animals.ToListAsync(); // Haal alle dieren op
            return animals;
        }

        public Animal GetAnimalById(int id)
        {
            var animal = _context.Animals
                .Include(a => a.Category)  // Laad de gerelateerde Categorie
                .Include(a => a.Enclosure) // Laad de gerelateerde Enclosure
                .FirstOrDefault(a => a.Id == id); // Haal het dier op via ID

            // Als Categorie of Enclosure null is, stel een default in
            if (animal != null)
            {
                animal.Category ??= new Category { Name = "Unknown" };  // Standaard Categorie als null
                animal.Enclosure ??= new Enclosure { Name = "Unknown" };  // Standaard Enclosure als null
            }

            return animal;
        }

        public Animal CreateAnimal(Animal animal)
        {
            try
            {
                _context.Animals.Add(animal); // Voeg nieuw dier toe
                _context.SaveChanges(); // Sla de veranderingen op in de database
                Console.WriteLine($"Dier {animal.Name} succesvol aangemaakt");
                return animal;
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"Databasefout bij het aanmaken van dier: {ex.InnerException?.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fout bij het aanmaken van dier: {ex.Message}");
                throw;
            }
        }

        public Animal UpdateAnimal(Animal updatedAnimal)
        {
            var existingAnimal = _context.Animals.FirstOrDefault(a => a.Id == updatedAnimal.Id); // Zoek bestaand dier
            if (existingAnimal != null)
            {
                // Werk de eigenschappen van het dier bij
                existingAnimal.Name = updatedAnimal.Name;
                existingAnimal.ActivityPattern = updatedAnimal.ActivityPattern;
                existingAnimal.Diet = updatedAnimal.Diet;
                existingAnimal.CategoryId = updatedAnimal.CategoryId;
                existingAnimal.EnclosureId = updatedAnimal.EnclosureId;
                existingAnimal.SecurityRequirement = updatedAnimal.SecurityRequirement;
                existingAnimal.SpaceRequirement = updatedAnimal.SpaceRequirement;
                existingAnimal.Size = updatedAnimal.Size;
                existingAnimal.Species = updatedAnimal.Species;
                _context.SaveChanges(); // Sla de veranderingen op
            }
            return existingAnimal;
        }

        public bool DeleteAnimal(int id)
        {
            var animalToDelete = _context.Animals.FirstOrDefault(a => a.Id == id); // Zoek dier om te verwijderen
            if (animalToDelete != null)
            {
                _context.Animals.Remove(animalToDelete); // Verwijder het dier uit de database
                _context.SaveChanges(); // Sla de veranderingen op in de database
                return true;
            }
            return false;
        }

        // Acties

        public void Sunrise(Animal animal)
        {
            if (animal.ActivityPattern == ActivityPattern.Diurnal) // Als dier dagactief is
            {
                Console.WriteLine($"{animal.Name} is wakker geworden.");
            }
            else
            {
                Console.WriteLine($"{animal.Name} slaapt nog.");
            }
        }

        public void Sunset(Animal animal)
        {
            if (animal.ActivityPattern == ActivityPattern.Nocturnal) // Als dier nachtdier is
            {
                Console.WriteLine($"{animal.Name} is wakker geworden.");
            }
            else if (animal.ActivityPattern == ActivityPattern.Diurnal) // Als dier dagactief is
            {
                Console.WriteLine($"{animal.Name} gaat slapen.");
            }
        }

        public void FeedingTime(Animal animal)
        {
            if (animal.Diet == DietaryClass.Carnivore) // Carnivoor dieet
            {
                Console.WriteLine($"{animal.Name} eet vlees.");
            }
            else if (animal.Diet == DietaryClass.Herbivore) // Herbivoor dieet
            {
                Console.WriteLine($"{animal.Name} eet planten.");
            }
            else if (animal.Diet == DietaryClass.Omnivore) // Omnivoor dieet
            {
                Console.WriteLine($"{animal.Name} eet zowel vlees als planten.");
            }
        }
    }
}
