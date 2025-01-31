// Uitleg over de testaanpak:
// - Elke test gebruikt een unieke in-memory database voor isolatie. Ook cleanen we data want ik had constant erros met Collection not empty. Was wel pittig deze test.
// - Vier tests: ophalen van dieren (leeg bij geen dieren), opzoeken op ID, toevoegen van een nieuw dier en verwijderen van een dier.
// - Problemen opgelost: data werd gewist tussen tests, de juiste enums werden gebruikt en ID's worden automatisch gegenereerd om conflicten te voorkomen.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dierentuin.Data;
using Dierentuin.Models;
using Dierentuin.Services;
using Dierentuin.Enum;  // Voeg deze using-statement toe voor de juiste enum
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Dierentuin.Tests
{
    public class AnimalServiceTests
    {
        private AnimalService GetAnimalServiceWithDb()
        {
            var options = new DbContextOptionsBuilder<DBContext>()
                .UseInMemoryDatabase(databaseName: "TestDb_" + System.Guid.NewGuid())  // Unieke database naam
                .Options;
            var context = new DBContext(options);

            // Verwijder bestaande data om tests onafhankelijk te maken
            if (context.Animals != null)
            {
                context.Animals.RemoveRange(context.Animals);
                context.SaveChanges();
            }

            return new AnimalService(context);
        }

        [Fact]
        public async Task GetAllAnimals_ReturnsEmptyList_WhenNoAnimalsExist()
        {
            var service = GetAnimalServiceWithDb(); // Nieuwe service met een schone database
            var result = await service.GetAllAnimals();  // Act
            Assert.Empty(result);  // Assert dat de lijst leeg is
        }

        [Fact]
        public void GetAnimalById_ReturnsNull_WhenAnimalDoesNotExist()
        {
            var service = GetAnimalServiceWithDb();
            var result = service.GetAnimalById(99);  // Act
            Assert.Null(result);  // Assert dat er null wordt geretourneerd
        }

        [Fact]
        public void CreateAnimal_AddsAnimalToDatabase()
        {
            var service = GetAnimalServiceWithDb();
            var newAnimal = new Animal
            {
                Name = "Lion",
                ActivityPattern = ActivityPattern.Diurnal,
                Diet = DietaryClass.Carnivore  // Correcte enum gebruikt
            };

            service.CreateAnimal(newAnimal);  // Act
            var retrievedAnimal = service.GetAnimalById(newAnimal.Id);  // Assert
            Assert.NotNull(retrievedAnimal);
            Assert.Equal("Lion", retrievedAnimal.Name);  // Assert dat het dier correct is toegevoegd
        }

        [Fact]
        public void DeleteAnimal_RemovesAnimal_WhenAnimalExists()
        {
            var service = GetAnimalServiceWithDb();
            var newAnimal = new Animal
            {
                Name = "Tiger",
                ActivityPattern = ActivityPattern.Nocturnal,
                Diet = DietaryClass.Carnivore  // Correcte enum gebruikt
            };

            service.CreateAnimal(newAnimal);  // Act
            var deleted = service.DeleteAnimal(newAnimal.Id);  // Act
            var result = service.GetAnimalById(newAnimal.Id);  // Assert
            Assert.True(deleted);
            Assert.Null(result);  // Assert dat het dier is verwijderd
        }
    }
}
