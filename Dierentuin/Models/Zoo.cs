// Models/Zoo.cs
using System.Collections.Generic;

namespace Dierentuin.Models
{
    public class Zoo
    {
        public List<Animal> Animals { get; set; }
        public List<Enclosure> Enclosures { get; set; }

        public Zoo()
        {
            Animals = new List<Animal>();
            Enclosures = new List<Enclosure>();
        }
    }
}
