using Dierentuin.Data;
using Dierentuin.Enum;
using Dierentuin.Models;
using Microsoft.EntityFrameworkCore;




// Dit is de ZooService alweer de laatste (heerlijk), hierin worden de activiteiten van dieren zoals slaap en eetpatronen beheert.
// Dieren worden automatisch verwezen en ruimtes etc worden gecontroleert. Een uitdaging was bijvoorbeeld dynamisch laden van dieren en verblijven.
// Dit komt doordat er relaties zijn tussen de entiteiten zoals dieren met meerdere enclosures etc. 



namespace Dierentuin.Services
{
    public class ZooService
    {
        private readonly DBContext _context;

        // Constructor voor de ZooService, waarbij de DBContext wordt geïnjecteerd
        public ZooService(DBContext context)
        {
            _context = context;  // Toegang tot de database via de injected context
        }

        // Haal alle Zoo's asynchroon op
        public async Task<List<Zoo>> GetZoosAsync()
        {
            // Haal de Zoo's op, inclusief de dieren die erin zitten, asynchroon
            return await _context.Zoos.Include(z => z.Animals).ToListAsync();
        }

        // Haal alle Zoo's op
        public List<Zoo> GetAllZoos()
        {
            // Haal de Zoo's op, inclusief de dieren die erin zitten, synchrone versie
            return _context.Zoos.Include(z => z.Animals).ToList();
        }

        // Haal een specifieke Zoo op via het ID
        public Zoo GetZooById(int id)
        {
            // Zoek de Zoo met het opgegeven ID en laadt de bijbehorende dieren
            var zoo = _context.Zoos
                .Include(z => z.Animals)  // Laad de gerelateerde dieren
                .FirstOrDefault(z => z.Id == id);

            // Als de Zoo bestaat en de dieren nog niet geladen zijn, laad de dieren
            if (zoo != null && zoo.AnimalIds != null && zoo.AnimalIds.Any())
            {
                // Laad de dieren indien ze nog niet geladen zijn, zelfs als ze zijn aangegeven via AnimalIds
                if ((zoo.Animals == null || !zoo.Animals.Any()) && zoo.AnimalIds.Any())
                {
                    var animals = _context.Animals
                        .Where(a => zoo.AnimalIds.Contains(a.Id))  // Haal de dieren op met de AnimalIds van de Zoo
                        .ToList();

                    zoo.Animals = animals;  // Zet de geladen dieren in de Zoo
                }
            }

            return zoo;
        }

        // Methode voor de Sunrise actie (de dieren wakker maken)
        public List<string> Sunrise(Zoo zoo)
        {
            var result = new List<string>();

            // Zorg ervoor dat we de dieren geladen hebben
            if (zoo.Animals == null || !zoo.Animals.Any())
            {
                // Laad de dieren als ze nog niet geladen zijn
                var animals = _context.Animals
                    .Where(a => zoo.AnimalIds.Contains(a.Id))  // Haal de dieren op met de AnimalIds van de Zoo
                    .ToList();
                zoo.Animals = animals;  // Zet de geladen dieren in de Zoo
            }

            // Loop door de dieren en bepaal of ze wakker zijn of nog slapen
            foreach (var animal in zoo.Animals)
            {
                if (animal.ActivityPattern == ActivityPattern.Diurnal)  // Diurnale dieren zijn 's ochtends actief
                {
                    result.Add($"{animal.Name} is wakker geworden.");
                }
                else
                {
                    result.Add($"{animal.Name} slaapt nog.");  // Nachtactieve dieren slapen nog
                }
            }

            return result;  // Retourneer een lijst van berichten
        }

        // Methode voor de Sunset actie (de dieren laten slapen gaan)
        public List<string> Sunset(Zoo zoo)
        {
            var result = new List<string>();

            // Zorg ervoor dat we de dieren geladen hebben
            if (zoo.Animals == null || !zoo.Animals.Any())
            {
                // Laad de dieren als ze nog niet geladen zijn
                var animals = _context.Animals
                    .Where(a => zoo.AnimalIds.Contains(a.Id))
                    .ToList();
                zoo.Animals = animals;
            }

            // Loop door de dieren en bepaal of ze wakker worden of slapen gaan
            foreach (var animal in zoo.Animals)
            {
                if (animal.ActivityPattern == ActivityPattern.Nocturnal)  // Nachtactieve dieren worden 's avonds wakker
                {
                    result.Add($"{animal.Name} is wakker geworden.");
                }
                else if (animal.ActivityPattern == ActivityPattern.Diurnal)  // Diurnale dieren gaan slapen
                {
                    result.Add($"{animal.Name} gaat slapen.");
                }
            }

            return result;  // Retourneer een lijst van berichten
        }

        // Maakt een nieuwe Zoo aan en slaat deze op in de database
        public async Task<Zoo> CreateZoo(Zoo zoo)
        {
            _context.Zoos.Add(zoo);  // Voeg de nieuwe Zoo toe aan de database
            await _context.SaveChangesAsync();  // Sla de Zoo op in de database

            // Als er AnimalIds zijn, koppel de dieren aan de Zoo
            if (zoo.AnimalIds != null && zoo.AnimalIds.Any())
            {
                foreach (var animalId in zoo.AnimalIds)
                {
                    var animal = await _context.Animals.FindAsync(animalId);  // Haal het dier op
                    if (animal != null)
                    {
                        zoo.Animals.Add(animal);  // Voeg het dier toe aan de Zoo
                    }
                }
                await _context.SaveChangesAsync();  // Sla de wijzigingen op in de database
            }

            return zoo;  // Retourneer de aangemaakte Zoo
        }

        // Methode voor de FeedingTime actie (voederen van de dieren)
        public List<string> FeedingTime(Zoo zoo)
        {
            var feedingMessages = new List<string>();

            // Loop door de dieren en bepaal hun dieet
            foreach (var animal in zoo.Animals)
            {
                if (animal.Diet == DietaryClass.Carnivore)  // Carnivoren eten vlees
                {
                    feedingMessages.Add($"{animal.Name} eet vlees.");
                }
                else if (animal.Diet == DietaryClass.Herbivore)  // Herbivoren eten planten
                {
                    feedingMessages.Add($"{animal.Name} eet planten.");
                }
                else if (animal.Diet == DietaryClass.Omnivore)  // Omnivoren eten zowel vlees als planten
                {
                    feedingMessages.Add($"{animal.Name} eet zowel vlees als planten.");
                }
            }

            return feedingMessages;  // Retourneer de berichten over het dieet van de dieren
        }

        // Methode voor AutoAssign actie (toewijzen van dieren aan verblijven)
        public void AutoAssign(Zoo zoo)
        {
            // Loop door de dieren en wijs ze automatisch toe aan verblijven
            foreach (var animal in zoo.Animals)
            {
                var availableEnclosures = zoo.Enclosures.FindAll(e => e.Animals.Count < e.Size);  // Vind beschikbare verblijven

                if (availableEnclosures.Count > 0)
                {
                    var selectedEnclosure = availableEnclosures[0];  // Selecteer het eerste beschikbare verblijf
                    selectedEnclosure.Animals.Add(animal);  // Voeg het dier toe aan het verblijf
                    animal.Enclosure = selectedEnclosure;  // Koppel het dier aan het verblijf
                    Console.WriteLine($"{animal.Name} is toegewezen aan verblijf {selectedEnclosure.Name}.");
                }
                else
                {
                    // Als er geen beschikbare verblijven zijn, maak een nieuw verblijf aan
                    var newEnclosure = new Enclosure
                    {
                        Name = "Nieuw Verblijf",
                        Size = 100,  // Stel een grootte in voor het nieuwe verblijf
                        Animals = new List<Animal> { animal }  // Voeg het dier toe aan het nieuwe verblijf
                    };
                    zoo.Enclosures.Add(newEnclosure);  // Voeg het nieuwe verblijf toe aan de Zoo
                    animal.Enclosure = newEnclosure;  // Koppel het dier aan het nieuwe verblijf
                    Console.WriteLine($"{animal.Name} is toegewezen aan nieuw verblijf.");
                }
            }
        }

        // Methode voor de CheckConstraints actie (controleer of dieren voldoen aan hun eisen)
        public void CheckConstraints(Zoo zoo)
        {
            // Loop door de dieren en controleer of ze voldoen aan de eisen van hun verblijf
            foreach (var animal in zoo.Animals)
            {
                bool meetsSpaceRequirement = animal.Enclosure.Size >= animal.SpaceRequirement;  // Controleer de ruimte-eis
                bool meetsSecurityRequirement = animal.Enclosure.SecurityLevel >= animal.SecurityRequirement;  // Controleer de beveiligingseis

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
