using Dierentuin.Enum;
using Dierentuin.Models;
using System.Collections.Generic;
using System.Linq;

namespace Dierentuin.Services
{
    public class EnclosureService
    {
        private readonly List<Enclosure> _enclosures;
        private readonly List<Animal> _animals;

        public EnclosureService()
        {
            _enclosures = new List<Enclosure>();
            _animals = new List<Animal>();
        }

        // CRUD Operations

        public List<Enclosure> GetAllEnclosures()
        {
            return _enclosures;
        }

        public Enclosure GetEnclosureById(int id)
        {
            return _enclosures.FirstOrDefault(e => e.Id == id);
        }

        public Enclosure CreateEnclosure(Enclosure enclosure)
        {
            enclosure.Id = _enclosures.Max(e => e.Id) + 1; // Assign new Id
            _enclosures.Add(enclosure);
            return enclosure;
        }

        public Enclosure UpdateEnclosure(Enclosure updatedEnclosure)
        {
            var existingEnclosure = _enclosures.FirstOrDefault(e => e.Id == updatedEnclosure.Id);
            if (existingEnclosure != null)
            {
                existingEnclosure.Name = updatedEnclosure.Name;
                existingEnclosure.Size = updatedEnclosure.Size;
                existingEnclosure.Climate = updatedEnclosure.Climate;
                existingEnclosure.SecurityLevel = updatedEnclosure.SecurityLevel;
            }
            return existingEnclosure;
        }

        public bool DeleteEnclosure(int id)
        {
            var enclosure = _enclosures.FirstOrDefault(e => e.Id == id);
            if (enclosure != null)
            {
                _enclosures.Remove(enclosure);
                return true;
            }
            return false;
        }

        // Assign animal to enclosure
        public void AssignAnimalToEnclosure(int animalId, int enclosureId)
        {
            var animal = _animals.FirstOrDefault(a => a.Id == animalId);
            var enclosure = _enclosures.FirstOrDefault(e => e.Id == enclosureId);
            if (animal != null && enclosure != null)
            {
                animal.EnclosureId = enclosureId;
                animal.Enclosure = enclosure;
            }
        }

        // Filter animals by enclosure
        public List<Animal> GetAnimalsByEnclosure(int enclosureId)
        {
            return _animals.Where(a => a.EnclosureId == enclosureId).ToList();
        }

        // Actions

        public void Sunrise(Enclosure enclosure)
        {
            // Logic for sunrise
            foreach (var animal in enclosure.Animals)
            {
                if (animal.ActivityPattern == ActivityPattern.Diurnal)
                {
                    Console.WriteLine($"{animal.Name} is wakker geworden.");
                }
                else
                {
                    Console.WriteLine($"{animal.Name} slaapt nog.");
                }
            }
        }

        public void Sunset(Enclosure enclosure)
        {
            // Logic for sunset
            foreach (var animal in enclosure.Animals)
            {
                if (animal.ActivityPattern == ActivityPattern.Nocturnal)
                {
                    Console.WriteLine($"{animal.Name} is wakker geworden.");
                }
                else
                {
                    Console.WriteLine($"{animal.Name} gaat slapen.");
                }
            }
        }

        public void FeedingTime(Enclosure enclosure)
        {
            // Logic for feeding time
            foreach (var animal in enclosure.Animals)
            {
                if (animal.Diet == DietaryClass.Carnivore)
                {
                    Console.WriteLine($"{animal.Name} eet vlees.");
                }
                else if (animal.Diet == DietaryClass.Herbivore)
                {
                    Console.WriteLine($"{animal.Name} eet planten.");
                }
                else if (animal.Diet == DietaryClass.Omnivore)
                {
                    Console.WriteLine($"{animal.Name} eet zowel vlees als planten.");
                }
            }
        }

        // Check constraints (e.g., space, security, etc.)
        public void CheckConstraints(Enclosure enclosure)
        {
            // Logic to check constraints
        }
    }
}
