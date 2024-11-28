using Dierentuin.Enum;
using Dierentuin.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dierentuin.Services
{
    public class AnimalService
    {
        private readonly List<Animal> _animals;

        public AnimalService()
        {
            var animals = new List<Animal>
        {
            new Animal { Id = 1, Name = "Lion", Species = "Panthera leo" },
            new Animal { Id = 2, Name = "Tiger", Species = "Panthera tigris" },
            new Animal { Id = 3, Name = "Elephant", Species = "Loxodonta" },
            new Animal { Id = 4, Name = "Giraffe", Species = "Giraffa camelopardalis" }
            };
        }

            // CRUD Operations

            public List<Animal> GetAllAnimals()
        {
            return _animals;
        }

        public Animal GetAnimalById(int id)
        {
            return _animals.FirstOrDefault(a => a.Id == id);
        }

        public Animal CreateAnimal(Animal animal)
        {
            animal.Id = _animals.Max(a => a.Id) + 1; // Assign a new Id
            _animals.Add(animal);
            return animal;
        }

        public Animal UpdateAnimal(Animal updatedAnimal)
        {
            var existingAnimal = _animals.FirstOrDefault(a => a.Id == updatedAnimal.Id);
            if (existingAnimal != null)
            {
                existingAnimal.Name = updatedAnimal.Name;
                existingAnimal.ActivityPattern = updatedAnimal.ActivityPattern;
                existingAnimal.Diet = updatedAnimal.Diet;
            }
            return existingAnimal;
        }

        public bool DeleteAnimal(int id)
        {
            var animalToDelete = _animals.FirstOrDefault(a => a.Id == id);
            if (animalToDelete != null)
            {
                _animals.Remove(animalToDelete);
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

        public void FeedingTime(Animal animal)
        {
            
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
}
