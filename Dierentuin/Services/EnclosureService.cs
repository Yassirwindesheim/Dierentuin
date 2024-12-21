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

        public Enclosure GetEnclosureById(int id)
        {
            var enclosure = _context.Enclosures
                .Include(e => e.Animals)  // Include related animals
                .FirstOrDefault(e => e.Id == id); // Retrieve enclosure by ID

            return enclosure;
        }

        public Enclosure CreateEnclosure(Enclosure enclosure)
        {
            try
            {
                _context.Enclosures.Add(enclosure);
                _context.SaveChanges();
                Console.WriteLine($"Enclosure {enclosure.Name} created successfully");
                return enclosure;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating enclosure: {ex.Message}");
                throw;
            }
        }

        public Enclosure UpdateEnclosure(Enclosure updatedEnclosure)
        {
            var existingEnclosure = _context.Enclosures.FirstOrDefault(e => e.Id == updatedEnclosure.Id);
            if (existingEnclosure != null)
            {
                existingEnclosure.Name = updatedEnclosure.Name;
                existingEnclosure.Climate = updatedEnclosure.Climate;
                existingEnclosure.HabitatType = updatedEnclosure.HabitatType;
                existingEnclosure.SecurityLevel = updatedEnclosure.SecurityLevel;
                existingEnclosure.Size = updatedEnclosure.Size;
                _context.SaveChanges();
            }
            return existingEnclosure;
        }

        public bool DeleteEnclosure(int id)
        {
            var enclosureToDelete = _context.Enclosures.FirstOrDefault(e => e.Id == id);
            if (enclosureToDelete != null)
            {
                _context.Enclosures.Remove(enclosureToDelete); // Remove enclosure from the database
                _context.SaveChanges(); // Save changes to the database
                return true;
            }
            return false;
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
