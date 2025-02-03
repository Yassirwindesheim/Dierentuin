using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dierentuin.Data;
using Dierentuin.Models;
using Dierentuin.Services;
using Dierentuin.Enum;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Dierentuin.Tests
{
    // Deze klasse bevat tests voor de EnclosureService
    public class EnclosureServiceTests
    {
        // Hulp-methode die een nieuwe EnclosureService instantie retourneert
        // met een in-memory database, zodat we altijd met een schone database starten
        private EnclosureService GetEnclosureServiceWithDb()
        {
            var options = new DbContextOptionsBuilder<DBContext>()
                .UseInMemoryDatabase(databaseName: "TestDb_" + System.Guid.NewGuid()) // Unieke db-naam per test
                .Options;
            var context = new DBContext(options);

            // Verwijder alle bestaande data uit de Enclosures tabel
            if (context.Enclosures != null)
            {
                context.Enclosures.RemoveRange(context.Enclosures);
                context.SaveChanges();
            }
            // Verwijder alle bestaande data uit de Animals tabel
            if (context.Animals != null)
            {
                context.Animals.RemoveRange(context.Animals);
                context.SaveChanges();
            }

            // Geef een nieuwe EnclosureService terug met de schone context
            return new EnclosureService(context);
        }

        // Test: Als er geen enclosures bestaan, moet GetAllEnclosures een lege lijst teruggeven
        [Fact]
        public void GetAllEnclosures_ReturnsEmptyList_WhenNoEnclosuresExist()
        {
            // Arrange: Maak een nieuwe EnclosureService met een schone in-memory database
            var service = GetEnclosureServiceWithDb();

            // Act: Roep de methode aan die alle enclosures ophaalt
            var result = service.GetAllEnclosures();

            // Assert: Controleer dat de lijst leeg is
            Assert.Empty(result);
        }

        // Test: Als er geen enclosure met een bepaalde Id bestaat, moet GetEnclosureById null teruggeven
        [Fact]
        public async Task GetEnclosureById_ReturnsNull_WhenEnclosureDoesNotExist()
        {
            // Arrange: Maak een nieuwe EnclosureService aan
            var service = GetEnclosureServiceWithDb();

            // Act: Probeer een enclosure op te halen met een Id (hier 99) die niet bestaat
            var result = await service.GetEnclosureById(99);

            // Assert: Zorg dat het resultaat null is
            Assert.Null(result);
        }

        // Test: Controleer of we een nieuwe enclosure kunnen aanmaken en toevoegen aan de database
        [Fact]
        public async Task CreateEnclosure_AddsEnclosureToDatabase()
        {
            // Arrange: Maak een nieuwe EnclosureService en definieer een nieuwe enclosure
            var service = GetEnclosureServiceWithDb();
            var newEnclosure = new Enclosure
            {
                Name = "Lion Den",               // Naam van de enclosure
                Climate = Climate.Tropical,       // Het klimaat van de enclosure
                HabitatType = HabitatType.Desert, // Het type habitat
                SecurityLevel = SecurityLevel.High, // De beveiligingsgraad
                Size = 500.0                      // De grootte van de enclosure
            };

            // Act: Maak de enclosure aan via de service
            var createdEnclosure = await service.CreateEnclosure(newEnclosure);

            // Assert: Controleer dat de enclosure is aangemaakt met de juiste properties en een geldig Id
            Assert.NotNull(createdEnclosure);
            Assert.Equal("Lion Den", createdEnclosure.Name);
            Assert.Equal(Climate.Tropical, createdEnclosure.Climate);
            Assert.Equal(HabitatType.Desert, createdEnclosure.HabitatType);
            Assert.Equal(SecurityLevel.High, createdEnclosure.SecurityLevel);
            Assert.Equal(500.0, createdEnclosure.Size);
            Assert.NotNull(createdEnclosure.Id);
        }

        // Test: Controleer of we een enclosure kunnen aanmaken met dieren en dat deze correct worden gekoppeld
        [Fact]
        public async Task CreateEnclosure_WithAnimals_AddsEnclosureAndAssociatesAnimals()
        {
            // Arrange: Configureer een nieuwe in-memory database voor deze test
            var options = new DbContextOptionsBuilder<DBContext>()
                .UseInMemoryDatabase(databaseName: "TestDb_" + System.Guid.NewGuid())
                .Options;
            var context = new DBContext(options);
            var service = new EnclosureService(context);

            // Voeg eerst wat test dieren toe aan de database
            var animal1 = new Animal { Name = "Lion" };
            var animal2 = new Animal { Name = "Tiger" };
            context.Animals.AddRange(animal1, animal2);
            await context.SaveChangesAsync(); // Zorg dat de dieren een Id krijgen

            // Maak een nieuwe enclosure aan en geef de AnimalIds mee zodat de dieren gekoppeld worden
            var newEnclosure = new Enclosure
            {
                Name = "Big Cat Enclosure",
                Climate = Climate.Tropical,
                HabitatType = HabitatType.Desert,
                SecurityLevel = SecurityLevel.High,
                Size = 1000.0,
                AnimalIds = new List<int> { animal1.Id, animal2.Id }
            };

            // Act: Maak de enclosure aan via de service
            var createdEnclosure = await service.CreateEnclosure(newEnclosure);

            // Assert: Controleer of de enclosure is aangemaakt met de juiste naam
            // en dat de dieren correct gekoppeld zijn
            Assert.NotNull(createdEnclosure);
            Assert.Equal("Big Cat Enclosure", createdEnclosure.Name);
            var retrievedEnclosure = await service.GetEnclosureById(createdEnclosure.Id.Value);
            Assert.Equal(2, retrievedEnclosure.Animals.Count);
            Assert.Contains(retrievedEnclosure.Animals, a => a.Name == "Lion");
            Assert.Contains(retrievedEnclosure.Animals, a => a.Name == "Tiger");
        }

        // Test: Controleer of een bestaande enclosure kan worden aangepast, inclusief het wijzigen van de gekoppelde dieren
        [Fact]
        public async Task UpdateEnclosure_ModifiesEnclosureAndAnimals()
        {
            // Arrange: Configureer een nieuwe in-memory database en maak een EnclosureService
            var options = new DbContextOptionsBuilder<DBContext>()
                .UseInMemoryDatabase(databaseName: "TestDb_" + System.Guid.NewGuid())
                .Options;
            var context = new DBContext(options);
            var service = new EnclosureService(context);

            // Maak wat initiële dieren en een enclosure aan
            var animal1 = new Animal { Name = "Lion" };
            var animal2 = new Animal { Name = "Tiger" };
            var animal3 = new Animal { Name = "Leopard" };
            context.Animals.AddRange(animal1, animal2, animal3);
            await context.SaveChangesAsync();

            // Maak een enclosure aan met eerst twee dieren (Lion en Tiger)
            var enclosure = new Enclosure
            {
                Name = "Big Cats",
                Climate = Climate.Tropical,
                HabitatType = HabitatType.Desert,
                SecurityLevel = SecurityLevel.High,
                Size = 1000.0,
                AnimalIds = new List<int> { animal1.Id, animal2.Id }
            };
            var createdEnclosure = await service.CreateEnclosure(enclosure);

            // Act: Wijzig de enclosure, pas de naam en klimaat aan en wijzig de gekoppelde dieren
            // Hier verwijderen we Lion en voegen we Leopard toe
            createdEnclosure.Name = "Updated Enclosure";
            createdEnclosure.Climate = Climate.Temperate;
            createdEnclosure.AnimalIds = new List<int> { animal2.Id, animal3.Id };
            var updatedEnclosure = await service.UpdateEnclosure(createdEnclosure);

            // Assert: Controleer of de wijzigingen goed zijn doorgevoerd
            Assert.Equal("Updated Enclosure", updatedEnclosure.Name);
            Assert.Equal(Climate.Temperate, updatedEnclosure.Climate);
            var retrievedEnclosure = await service.GetEnclosureById(updatedEnclosure.Id.Value);
            var animalIds = await service.GetAnimalIdsByEnclosureId(updatedEnclosure.Id.Value);
            Assert.Equal(2, animalIds.Count);
            Assert.Contains(animalIds, id => id == animal2.Id);
            Assert.Contains(animalIds, id => id == animal3.Id);
            Assert.DoesNotContain(animalIds, id => id == animal1.Id);
        }

        // Test: Controleer of een bestaande enclosure wordt verwijderd wanneer DeleteEnclosure wordt aangeroepen
        [Fact]
        public void DeleteEnclosure_RemovesEnclosure_WhenEnclosureExists()
        {
            // Arrange: Maak een nieuwe EnclosureService en definieer een nieuwe enclosure
            var service = GetEnclosureServiceWithDb();
            var newEnclosure = new Enclosure
            {
                Name = "Test Enclosure",
                Climate = Climate.Tropical,
                HabitatType = HabitatType.Desert,
                SecurityLevel = SecurityLevel.Medium,
                Size = 300.0
            };

            // Act: Maak de enclosure aan en verwijder hem vervolgens
            service.CreateEnclosure(newEnclosure).Wait();
            var result = service.DeleteEnclosure(newEnclosure.Id.Value);
            var deletedEnclosure = service.GetEnclosureById(newEnclosure.Id.Value).Result;

            // Assert: Controleer dat de delete operatie succesvol was en dat de enclosure niet meer bestaat
            Assert.True(result);
            Assert.Null(deletedEnclosure);
        }
    }
}
