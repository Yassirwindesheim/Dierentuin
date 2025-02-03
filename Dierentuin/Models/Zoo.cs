namespace Dierentuin.Models
{
    public class Zoo
    {
        public int Id { get; set; }  // Primaire sleutel (unieke identificatie voor de dierentuin)

        public string? Name { get; set; }            // Naam van de dierentuin

        // Relaties
        public List<Animal> Animals { get; set; }   // Lijst van dieren in deze dierentuin
        public List<Enclosure> Enclosures { get; set; } // Lijst van omheiningen in deze dierentuin

        // Nieuwe eigenschap voor Enclosure IDs
        public List<int> EnclosureIds { get; set; }  // Lijst van omheining-ID's die aan deze dierentuin gekoppeld zijn

        // Constructor om de lijsten te initialiseren
        public Zoo()
        {
            Animals = new List<Animal>();     // Initialiseer de dierenlijst
            Enclosures = new List<Enclosure>(); // Initialiseer de omheiningenlijst
            EnclosureIds = new List<int>(); // initialiseert enclosureids lijst naar een lege lijst (dit heb ik ook nodig voor de testen)
        }

        // Lijst van dier-ID's voor het creëren of koppelen van dieren aan deze dierentuin
        public List<int> AnimalIds { get; set; } = new List<int>();  // Lijst van dier-ID's die aan deze dierentuin gekoppeld zijn
    }
}
