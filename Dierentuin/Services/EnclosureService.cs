using Dierentuin.Data;
using Dierentuin.Models;
using Microsoft.EntityFrameworkCore;


//De EnclosureService beheert verblijven voor dieren en zorgt voor CRUD-operaties. Het biedt ook functionaliteit om dieren aan verblijven toe te wijzen en te controleren of ruimte- en beveiligingseisen voldaan worden.
//De uitdaging zit vooral in het dynamisch toewijzen van dieren aan verblijven en het controleren van de eisen, maar het is haalbaar.



namespace Dierentuin.Services
{
    public class EnclosureService
    {
        private readonly DBContext _context;

        // Constructor om de DBContext via dependency injection in te voegen
        public EnclosureService(DBContext context)
        {
            _context = context;  // Toegang tot de database via de injected context
        }

        // CRUD Operations

        // Haalt alle omheiningen op uit de database
        public List<Enclosure> GetAllEnclosures()
        {
            return _context.Enclosures.Include(e => e.Animals).ToList();  // Haal omheiningen op, inclusief dieren die erin zitten
        }

        // Haalt een specifieke omheining op via het ID
        public async Task<Enclosure> GetEnclosureById(int id)
        {
            return await _context.Enclosures.Include(e => e.Animals)
                                             .FirstOrDefaultAsync(e => e.Id == id);  // Haalt omheining op met bijbehorende dieren
        }

        // Maakt een nieuwe omheining aan en slaat deze op in de database
        public async Task<Enclosure> CreateEnclosure(Enclosure enclosure)
        {
            _context.Enclosures.Add(enclosure);  // Voeg de nieuwe omheining toe aan de database
            await _context.SaveChangesAsync();  // Sla de wijzigingen op

            // Als er dier-IDs zijn, wijs dan de dieren toe aan de omheining
            if (enclosure.AnimalIds != null && enclosure.AnimalIds.Any())
            {
                foreach (var animalId in enclosure.AnimalIds)
                {
                    await AssignAnimalToEnclosure(animalId, enclosure.Id.Value);  // Wijs elk dier toe aan de omheining
                }
            }

            return enclosure;  // Retourneer de aangemaakte omheining
        }

        // Wijst een dier toe aan een omheining
        public async Task AssignAnimalToEnclosure(int animalId, int enclosureId)
        {
            var animal = await _context.Animals.FirstOrDefaultAsync(a => a.Id == animalId);  // Haal het dier op
            var enclosure = await _context.Enclosures.FirstOrDefaultAsync(c => c.Id == enclosureId);  // Haal de omheining op
            if (animal != null && enclosure != null)
            {
                animal.EnclosureId = enclosureId;  // Koppel het dier aan de omheining
                animal.Enclosure = enclosure;  // Koppel het dier aan de omheving in het geheugen
                await _context.SaveChangesAsync();  // Sla de wijzigingen op
            }
        }

        // Werkt een bestaande omheining bij
        public async Task<Enclosure> UpdateEnclosure(Enclosure updatedEnclosure)
        {
            var existingEnclosure = await _context.Enclosures.FirstOrDefaultAsync(c => c.Id == updatedEnclosure.Id);  // Haal de bestaande omheining op
            if (existingEnclosure != null)
            {
                // Werk de eigenschappen van de omheining bij
                existingEnclosure.Name = updatedEnclosure.Name;
                existingEnclosure.Climate = updatedEnclosure.Climate;
                existingEnclosure.HabitatType = updatedEnclosure.HabitatType;
                existingEnclosure.SecurityLevel = updatedEnclosure.SecurityLevel;
                existingEnclosure.Size = updatedEnclosure.Size;

                // Als er nieuwe dier-IDs zijn, werk dan de toegewezen dieren bij
                if (updatedEnclosure.AnimalIds != null)
                {
                    var existingAnimals = await _context.Animals.Where(a => a.EnclosureId == existingEnclosure.Id).ToListAsync();  // Haal de bestaande dieren op
                    foreach (var animal in existingAnimals)
                    {
                        animal.EnclosureId = null;  // Verwijder het dier uit de omheining
                    }

                    foreach (var animalId in updatedEnclosure.AnimalIds)
                    {
                        await AssignAnimalToEnclosure(animalId, existingEnclosure.Id.Value);  // Wijs de dieren opnieuw toe aan de omheining
                    }
                }

                await _context.SaveChangesAsync();  // Sla de wijzigingen op
            }

            return existingEnclosure;  // Retourneer de bijgewerkte omheining
        }

        // Haalt de dier-IDs op die bij een specifieke omheining horen
        public async Task<List<int>> GetAnimalIdsByEnclosureId(int enclosureId)
        {
            return await _context.Animals
                .Where(a => a.EnclosureId == enclosureId)  // Filter op de opgegeven omheining-ID
                .Select(a => a.Id)  // Selecteer alleen de dier-IDs
                .ToListAsync();  // Zet de resultaten om naar een lijst
        }

        // Verwijdert een omheining uit de database
        public bool DeleteEnclosure(int id)
        {
            var enclosureToDelete = _context.Enclosures
                .Include(e => e.Animals)  // Laad de dieren die in de omheining zitten
                .FirstOrDefault(e => e.Id == id);  // Zoek de omheining op

            if (enclosureToDelete == null)
            {
                return false;  // Omheining bestaat niet
            }

            // Controleer of er dieren in de omheining zitten
            if (enclosureToDelete.Animals != null && enclosureToDelete.Animals.Any())
            {
                // Verplaats de dieren naar een andere omheining voordat de omheining wordt verwijderd
                var defaultEnclosure = _context.Enclosures.FirstOrDefault(e => e.Name == "DefaultEnclosure");  // Kies een standaard omheining voor herplaatsing

                foreach (var animal in enclosureToDelete.Animals)
                {
                    animal.EnclosureId = defaultEnclosure?.Id;  // Wijs het dier toe aan de standaard omheining
                }

                _context.SaveChanges();  // Sla de herplaatsing van de dieren op
            }

            _context.Enclosures.Remove(enclosureToDelete);  // Verwijder de omheining uit de database
            _context.SaveChanges();  // Commit de wijziging naar de database
            return true;  // Omheining succesvol verwijderd
        }

        // Extra acties voor het beheren van dieren in omheiningen

        // Voegt een dier toe aan een omheining
        public void AddAnimalToEnclosure(int enclosureId, Animal animal)
        {
            var enclosure = _context.Enclosures.FirstOrDefault(e => e.Id == enclosureId);  // Zoek de omheining op
            if (enclosure != null)
            {
                enclosure.Animals.Add(animal);  // Voeg het dier toe aan de omheining
                _context.SaveChanges();  // Sla de wijziging op in de database
                Console.WriteLine($"Animal {animal.Name} added to {enclosure.Name} enclosure.");
            }
        }

        // Verwijdert een dier uit een omheining
        public void RemoveAnimalFromEnclosure(int enclosureId, int animalId)
        {
            var enclosure = _context.Enclosures.Include(e => e.Animals).FirstOrDefault(e => e.Id == enclosureId);  // Zoek de omheining op, inclusief dieren
            var animal = enclosure?.Animals.FirstOrDefault(a => a.Id == animalId);  // Zoek het dier in de omheining
            if (animal != null)
            {
                enclosure.Animals.Remove(animal);  // Verwijder het dier uit de omheining
                _context.SaveChanges();  // Sla de wijziging op
                Console.WriteLine($"Animal {animal.Name} removed from {enclosure.Name} enclosure.");
            }
        }

        // Optioneel: Extra methoden kunnen hier worden toegevoegd voor verdere beheerfunctionaliteiten
    }
}
