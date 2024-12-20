using Dierentuin.Enum;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations; // Zorg ervoor dat je deze namespace hebt

namespace Dierentuin.Models
{
    public class Animal
    {
        public int Id { get; set; }                // Unique identifier

       
        public string? Name { get; set; }            // Name of the animal

        public string? Species { get; set; }         // Species of the animal

        public AnimalSize Size { get; set; }        // Size (enum: Microscopic, Small, etc.)

        public DietaryClass Diet { get; set; }      // Diet (enum: Carnivore, Herbivore, etc.)

        public ActivityPattern ActivityPattern { get; set; } // When the animal is active (enum: Diurnal, etc.)

        public List<Animal>? Prey { get; set; }      // List of animals that this animal preys on

        public double SpaceRequirement { get; set; } // Space in square meters required per animal

        public SecurityLevel SecurityRequirement { get; set; } // Security level (Low, Medium, High)

        // Relationships
        public int? CategoryId { get; set; }         // Foreign key to Category
        public Category? Category { get; set; }      // Navigation property to Category

        public int? EnclosureId { get; set; }        // Foreign key to Enclosure
        public Enclosure? Enclosure { get; set; }    // Navigation property to Enclosure
    }
}
