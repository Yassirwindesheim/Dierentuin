using Dierentuin.Enum;
using Dierentuin.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dierentuin.Services
{
    public class ZooService
    {
        // Method for Sunrise action
        
        private readonly DBContext _context;


        public ZooService(DBContext context)
        {
            _context = context;
        }


        public async Task<List<Zoo>> GetZoosAsync()
        {
            // Fetch zoos from the database asynchronously
            return await _context.Zoos.Include(z => z.Animals).ToListAsync();
        }


        // Get all Zoos
        public List<Zoo> GetAllZoos()
        {
            return _context.Zoos.Include(z => z.Animals).ToList();
        }

        // Get a Zoo by ID

        public Zoo GetZooById(int id)
        {
            // First get the zoo with its AnimalIds
            var zoo = _context.Zoos
                .Include(z => z.Animals) // Include the Animals navigation property
                .FirstOrDefault(z => z.Id == id);

            if (zoo != null && zoo.AnimalIds != null && zoo.AnimalIds.Any())
            {
                // If the Animals collection is empty but we have AnimalIds, load the animals
                if ((zoo.Animals == null || !zoo.Animals.Any()) && zoo.AnimalIds.Any())
                {
                    // Load the actual animals based on the AnimalIds
                    var animals = _context.Animals
                        .Where(a => zoo.AnimalIds.Contains(a.Id))
                        .ToList();

                    zoo.Animals = animals;
                }
            }

            return zoo;
        }

        public List<string> Sunrise(Zoo zoo)
        {
            var result = new List<string>();

            // Make sure we have the animals loaded
            if (zoo.Animals == null || !zoo.Animals.Any())
            {
                // Load animals if they're not already loaded
                var animals = _context.Animals
                    .Where(a => zoo.AnimalIds.Contains(a.Id))
                    .ToList();
                zoo.Animals = animals;
            }

            foreach (var animal in zoo.Animals)
            {
                if (animal.ActivityPattern == ActivityPattern.Diurnal)
                {
                    result.Add($"{animal.Name} is wakker geworden.");
                }
                else
                {
                    result.Add($"{animal.Name} slaapt nog.");
                }
            }

            return result;
        }

        public List<string> Sunset(Zoo zoo)
        {
            var result = new List<string>();

            // Make sure we have the animals loaded
            if (zoo.Animals == null || !zoo.Animals.Any())
            {
                // Load animals if they're not already loaded
                var animals = _context.Animals
                    .Where(a => zoo.AnimalIds.Contains(a.Id))
                    .ToList();
                zoo.Animals = animals;
            }

            foreach (var animal in zoo.Animals)
            {
                if (animal.ActivityPattern == ActivityPattern.Nocturnal)
                {
                    result.Add($"{animal.Name} is wakker geworden.");
                }
                else if (animal.ActivityPattern == ActivityPattern.Diurnal)
                {
                    result.Add($"{animal.Name} gaat slapen.");
                }
            }

            return result;
        }
        public async Task<Zoo> CreateZoo(Zoo zoo)
        {
            _context.Zoos.Add(zoo);
            await _context.SaveChangesAsync();

            if (zoo.AnimalIds != null && zoo.AnimalIds.Any())
            {
                foreach (var animalId in zoo.AnimalIds)
                {
                    var animal = await _context.Animals.FindAsync(animalId);
                    if (animal != null)
                    {
                        zoo.Animals.Add(animal);
                    }
                }
                await _context.SaveChangesAsync();
            }

            return zoo;
        }


        

        // Method for FeedingTime action
        public List<string> FeedingTime(Zoo zoo)
        {
            var feedingMessages = new List<string>();

            foreach (var animal in zoo.Animals)
            {
                if (animal.Diet == DietaryClass.Carnivore)
                {
                    feedingMessages.Add($"{animal.Name} eet vlees.");
                }
                else if (animal.Diet == DietaryClass.Herbivore)
                {
                    feedingMessages.Add($"{animal.Name} eet planten.");
                }
                else if (animal.Diet == DietaryClass.Omnivore)
                {
                    feedingMessages.Add($"{animal.Name} eet zowel vlees als planten.");
                }
            }

            return feedingMessages;
        }
    

        // Method for AutoAssign action (assign animals to enclosures)
        public void AutoAssign(Zoo zoo)
        {
            foreach (var animal in zoo.Animals)
            {
                var availableEnclosures = zoo.Enclosures.FindAll(e => e.Animals.Count < e.Size);

                if (availableEnclosures.Count > 0)
                {
                    var selectedEnclosure = availableEnclosures[0];
                    selectedEnclosure.Animals.Add(animal);
                    animal.Enclosure = selectedEnclosure;
                    Console.WriteLine($"{animal.Name} is toegewezen aan verblijf {selectedEnclosure.Name}.");
                }
                else
                {
                    var newEnclosure = new Enclosure
                    {
                        Name = "Nieuw Verblijf",
                        Size = 100, // Example size, set appropriate size based on needs
                        Animals = new List<Animal> { animal }
                    };
                    zoo.Enclosures.Add(newEnclosure);
                    animal.Enclosure = newEnclosure;
                    Console.WriteLine($"{animal.Name} is toegewezen aan nieuw verblijf.");
                }
            }
        }

        // Method for CheckConstraints action (checking constraints)
        public void CheckConstraints(Zoo zoo)
        {
            foreach (var animal in zoo.Animals)
            {
                bool meetsSpaceRequirement = animal.Enclosure.Size >= animal.SpaceRequirement;
                bool meetsSecurityRequirement = animal.Enclosure.SecurityLevel >= animal.SecurityRequirement;

                Console.WriteLine($"{animal.Name} voldoet aan de eisen:");
                if (meetsSpaceRequirement && meetsSecurityRequirement)
                {
                    Console.WriteLine("Ruimte en beveiligingseisen zijn voldaan.");
                }
                else
                {
                    if (!meetsSpaceRequirement)
                    {
                        Console.WriteLine("Ruimtevereiste is niet voldaan.");
                    }
                    if (!meetsSecurityRequirement)
                    {
                        Console.WriteLine("Beveiligingseis is niet voldaan.");
                    }
                }
            }
        }
    }
}