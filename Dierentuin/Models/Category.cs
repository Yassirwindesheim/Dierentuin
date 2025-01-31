using System.ComponentModel.DataAnnotations.Schema;

namespace Dierentuin.Models
{
    public class Category
    {
        public int? Id { get; set; }                 // Unieke identifier voor de categorie (bijv. Zoogdieren, Vogels)

        public string? Name { get; set; }            // Naam van de categorie (bijv. Zoogdieren, Vogels)

        // Relaties
        public List<Animal>? Animals { get; set; }   // Optionele lijst van dieren in deze categorie

        // Lijst van dier-ID's voor het creëren of associëren van dieren met deze categorie
        [NotMapped]  // Dit veld wordt niet gemapt naar de database
        public List<int> AnimalIds { get; set; } = new List<int>();  // Lijst van dier-ID's die gekoppeld zijn aan deze categorie
    }
}
