
using System.ComponentModel.DataAnnotations.Schema;
using Dierentuin.Enum;

namespace Dierentuin.Models
{
    public class Enclosure
    {
        public int? Id { get; set; }                 // Unieke identifier voor het omheining (enclosure)

        public string Name { get; set; }            // Naam van de omheining

        public Climate Climate { get; set; }        // Klimaat van de omheining (enum: Tropisch, Gematigd, etc.)

        public HabitatType HabitatType { get; set; } // Type van het habitat (flags enum: Bos, Aquatisch, etc.)

        public SecurityLevel SecurityLevel { get; set; } // Beveiligingsniveau (Laag, Middel, Hoog)

        public double Size { get; set; }            // Grootte in vierkante meters

        // Relaties
        public List<Animal>? Animals { get; set; } = new List<Animal>();  // Lijst van dieren in deze omheining

        [NotMapped]  // Dit veld wordt niet naar de database gemapt
        public List<int> AnimalIds { get; set; } = new List<int>();  // Lijst van dier-ID's die gekoppeld zijn aan deze omheining

        // Constructor om de dierenlijst te initialiseren
        public Enclosure()
        {
            Animals = new List<Animal>();  // Initialiseer de lijst van dieren
        }
    }
}
