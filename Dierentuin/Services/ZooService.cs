using Dierentuin.Enum;
using Dierentuin.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dierentuin.Services
{
    public class ZooService
    {
        // Method for Sunrise action
        public void Sunrise(Zoo zoo)
        {
            foreach (var animal in zoo.Animals)
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

        // Method for Sunset action
        public void Sunset(Zoo zoo)
        {
            foreach (var animal in zoo.Animals)
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
        }

        // Method for FeedingTime action
        public void FeedingTime(Zoo zoo)
        {
            foreach (var animal in zoo.Animals)
            {
                // Logic to handle feeding time, depending on dietary needs
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
                // Add more logic as needed for other dietary classes
            }
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
                    // Create new enclosures if needed (this logic can be expanded)
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
