using System.Collections.Generic;
using System.ComponentModel.DataAnnotations; // Zorg ervoor dat deze namespace is toegevoegd
using Dierentuin.Enum;
using Dierentuin.Models;

public class Enclosure
{
    public int Id { get; set; }                 // Unique identifier

    [Required]
    public string Name { get; set; }            // Name of the enclosure

    public Climate Climate { get; set; }        // Climate of the enclosure (enum: Tropical, Temperate, etc.)

    public HabitatType HabitatType { get; set; } // Habitat type (flags enum: Forest, Aquatic, etc.)

    public SecurityLevel SecurityLevel { get; set; } // Security level (Low, Medium, High)

    public double Size { get; set; }            // Size in square meters

    // Relationships
    public List<Animal> Animals { get; set; }   // List of animals in the enclosure

    // Constructor to initialize the Animals list
    public Enclosure()
    {
        Animals = new List<Animal>();
    }
}
