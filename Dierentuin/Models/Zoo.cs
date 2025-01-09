using Dierentuin.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dierentuin.Models
{
    public class Zoo
    {
        public int Id { get; set; }  // Primary
                                     // 
        public string? Name { get; set; }            // Name of the animal


        public List<Animal> Animals { get; set; }
        public List<Enclosure> Enclosures { get; set; }

   
        public List<int> EnclosureIds { get; set; } // New property for Enclosure IDs

        public Zoo()
        {
            Animals = new List<Animal>();
            Enclosures = new List<Enclosure>();
        }

        // List of animal IDs for creating or associating animals with this zoo
      
        public List<int> AnimalIds { get; set; } = new List<int>();
    }
}