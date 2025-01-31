using Dierentuin.Enum;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations; // Zorg ervoor dat je deze namespace hebt

namespace Dierentuin.Models
{
    public class Animal
    {
        public int Id { get; set; }                // Unieke identifier voor het dier

        public string? Name { get; set; }            // Naam van het dier

        public string? Species { get; set; }         // Soort van het dier

        public AnimalSize Size { get; set; }        // Grootte van het dier (enum: Microscopisch, Klein, etc.)

        public DietaryClass Diet { get; set; }      // Dieet van het dier (enum: Carnivoor, Herbivoor, etc.)

        public ActivityPattern ActivityPattern { get; set; } // Activiteitspatroon van het dier (enum: Dagactief, etc.)

        public List<Animal>? Prey { get; set; }      // Lijst van dieren waar dit dier op jaagt (prooi)

        public double SpaceRequirement { get; set; } // Ruimtevereiste in vierkante meters per dier

        public SecurityLevel SecurityRequirement { get; set; } // Beveiligingsniveau (Laag, Medium, Hoog)

        // Relaties
        public int? CategoryId { get; set; }         // Foreign sleutel naar de Categorie
        public Category? Category { get; set; }      // Navigatie-eigenschap naar de Categorie

        public int? EnclosureId { get; set; }        // Foreign sleutel naar het Hok
        public Enclosure? Enclosure { get; set; }    // Navigatie-eigenschap naar het Hok
    }
}
