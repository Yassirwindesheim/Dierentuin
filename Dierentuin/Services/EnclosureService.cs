using Dierentuin.Enum;
using Dierentuin.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dierentuin.Services
{
    public class EnclosureService
    {
        private readonly DBContext _context;

        // Constructor to inject DBContext via dependency injection
        public EnclosureService(DBContext context)
        {
            _context = context;
        }

        // CRUD Operations

        public List<Enclosure> GetAllEnclosures()
        {
            return _context.Enclosures.Include(e => e.Animals).ToList(); // Retrieve enclosures from the database including the animals they contain
        }

        public async Task<Enclosure> GetEnclosureById(int id)
        {
            var enclosure = await _context.Enclosures
                .Include(e => e.Animals)  // Include related animals
                .FirstOrDefaultAsync(e => e.Id == id); // Retrieve enclosure by ID

            return enclosure;
        }



        public async Task<Enclosure> CreateEnclosure(Enclosure enclosure)
        {
            _context.Enclosures.Add(enclosure);
            await _context.SaveChangesAsync();

            if (enclosure.AnimalIds != null && enclosure.AnimalIds.Any())
            {
                foreach (var animalId in enclosure.AnimalIds)
                {
                    await AssignAnimalToEnclosure(animalId, enclosure.Id.Value);
                }
            }

            return enclosure;
        }

        public async Task AssignAnimalToEnclosure(int animalId, int enclosureId)
        {
            var animal = await _context.Animals.FirstOrDefaultAsync(a => a.Id == animalId);
            var enclosure = await _context.Enclosures.FirstOrDefaultAsync(c => c.Id == enclosureId);
            if (animal != null && enclosure != null)
            {
                animal.EnclosureId = enclosureId;
                animal.Enclosure = enclosure;
                await _context.SaveChangesAsync();  // Save the updated animal's category to the database asynchronously
            }
        }
            public async Task<Enclosure> UpdateEnclosure(Enclosure updatedEnclosure)
            {
                var existingEnclosure= await _context.Enclosures.FirstOrDefaultAsync(c => c.Id == updatedEnclosure.Id);
                if (existingEnclosure != null)
                {
                    existingEnclosure.Name = updatedEnclosure.Name;

                    // Update animal associations
                    if (updatedEnclosure.AnimalIds != null)
                    {
                        // Remove all existing animal associations
                        var existingAnimals = await _context.Animals.Where(a => a.EnclosureId == existingEnclosure.Id).ToListAsync();
                        foreach (var animal in existingAnimals)
                        {
                            animal.EnclosureId = null;
                        }

                        // Add new animal associations
                        foreach (var animalId in updatedEnclosure.AnimalIds)
                        {
                            await AssignAnimalToEnclosure(animalId, existingEnclosure.Id.Value);
                        }
                    }

                    await _context.SaveChangesAsync();
                }
                return existingEnclosure;
            }

        public bool DeleteEnclosure(int id)
        {
            var enclosureToDelete = _context.Enclosures
                .Include(e => e.Animals)
                .FirstOrDefault(e => e.Id == id);

            if (enclosureToDelete == null)
            {
                return false;
            }

            // Check if there are any animals in the enclosure
            if (enclosureToDelete.Animals != null && enclosureToDelete.Animals.Any())
            {
                throw new InvalidOperationException(
                    $"Cannot delete enclosure '{enclosureToDelete.Name}' because it still contains {enclosureToDelete.Animals.Count} animals. " +
                    "Please relocate all animals before deleting the enclosure.");
            }

            _context.Enclosures.Remove(enclosureToDelete);
            _context.SaveChanges();
            return true;
        }
        // Additional actions for managing animals in enclosures

        public void AddAnimalToEnclosure(int enclosureId, Animal animal)
        {
            var enclosure = _context.Enclosures.FirstOrDefault(e => e.Id == enclosureId);
            if (enclosure != null)
            {
                enclosure.Animals.Add(animal);
                _context.SaveChanges();
                Console.WriteLine($"Animal {animal.Name} added to {enclosure.Name} enclosure.");
            }
        }

        public void RemoveAnimalFromEnclosure(int enclosureId, int animalId)
        {
            var enclosure = _context.Enclosures.Include(e => e.Animals).FirstOrDefault(e => e.Id == enclosureId);
            var animal = enclosure?.Animals.FirstOrDefault(a => a.Id == animalId);
            if (animal != null)
            {
                enclosure.Animals.Remove(animal);
                _context.SaveChanges();
                Console.WriteLine($"Animal {animal.Name} removed from {enclosure.Name} enclosure.");
            }
        }

        // Optional: Additional methods related to the Enclosure can be added here
    }
}
