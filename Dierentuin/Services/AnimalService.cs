using Dierentuin.Enum;
using Dierentuin.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dierentuin.Services
{
    public class AnimalService
    {
        private readonly DBContext _context;

        // Constructor to inject DBContext via dependency injection
        public AnimalService(DBContext context)
        {
            _context = context;
        }

        // CRUD Operations

        public List<Animal> GetAllAnimals()
        {
            return _context.Animals.ToList(); // Retrieve animals from the database
        }

        public Animal GetAnimalById(int id)
        {
            return _context.Animals.FirstOrDefault(a => a.Id == id); // Retrieve animal by ID
        }

        public Animal CreateAnimal(Animal animal)
        {
            try
            {
                _context.Animals.Add(animal);
                _context.SaveChanges();
                Console.WriteLine($"Animal {animal.Name} created successfully");
                return animal;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating animal: {ex.Message}");
                throw;
            }
        }

        public Animal UpdateAnimal(Animal updatedAnimal)
        {
            var existingAnimal = _context.Animals.FirstOrDefault(a => a.Id == updatedAnimal.Id);
            if (existingAnimal != null)
            {
                existingAnimal.Name = updatedAnimal.Name;
                existingAnimal.ActivityPattern = updatedAnimal.ActivityPattern;
                existingAnimal.Diet = updatedAnimal.Diet;
                existingAnimal.CategoryId = updatedAnimal.CategoryId;
                existingAnimal.EnclosureId = updatedAnimal.EnclosureId;
                existingAnimal.SecurityRequirement = updatedAnimal.SecurityRequirement;
                existingAnimal.SpaceRequirement = updatedAnimal.SpaceRequirement;
                existingAnimal.Size = updatedAnimal.Size;
                existingAnimal.Species = updatedAnimal.Species;
                _context.SaveChanges();
            }
            return existingAnimal;
        }

        public bool DeleteAnimal(int id)
        {
            var animalToDelete = _context.Animals.FirstOrDefault(a => a.Id == id);
            if (animalToDelete != null)
            {
                _context.Animals.Remove(animalToDelete); // Remove animal from the database
                _context.SaveChanges(); // Save changes to the database
                return true;
            }
            return false;
        }

        // Actions

        public void Sunrise(Animal animal)
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

        public void Sunset(Animal animal)
        {
            if (animal.ActivityPattern == ActivityPattern.Nocturnal)
            {
                Console.WriteLine($"{animal.Name} is wakker geworden.");
            }
            else if (animal.ActivityPattern == ActivityPattern.Diurnal)
            {
                Console.WriteLine($"{animal.Name} gaat slapen.");
            }
        }

        public void FeedingTime(Animal animal)
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
}
