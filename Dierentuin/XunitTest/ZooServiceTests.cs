using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dierentuin.Data;
using Dierentuin.Models;
using Dierentuin.Services;
using Dierentuin.Enum;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Microsoft.AspNetCore.Mvc;

namespace Dierentuin.Tests
{
    // deze klasse bevat testen voor de zooservice 
    public class ZooServiceTests
    {
        // hulp methode die een nieuwe  zooservice instantie retourneert  met een Imemorydatabase
        private ZooService GetZooServiceWithDb()
        {
            var options = new DbContextOptionsBuilder<DBContext>()
                .UseInMemoryDatabase(databaseName: "TestDb_" + System.Guid.NewGuid()) // Maak een unieke naam voor elke test
                .Options;
            var context = new DBContext(options);

            // dit verwijdert alle bestaande data uit de db, zodat we kunnen beginnen met een nieuwe schone db
            if (context.Zoos != null)
            {
                context.Zoos.RemoveRange(context.Zoos);
                context.SaveChanges();
            }
            if (context.Animals != null)
            {
                context.Animals.RemoveRange(context.Animals);
                context.SaveChanges();
            }
            // dit geeft een nieuwe zooservice terug met de aangemaakte context
            return new ZooService(context);
        }
        // dit geeft als er geen zoos zijn een lege lijst terug 
        [Fact]
        public async Task GetZoosAsync_ReturnsEmptyList_WhenNoZoosExist()
        {
            // arrange dus dit maakt een nieuwe  zooservice aan in de memory db
            var service = GetZooServiceWithDb();
             
            // Act 
            // dit roept de methode aan om alle zoos terug te krijgen (ophalen)
            var result = await service.GetZoosAsync();

            // Assert 
            //controleert of het leeg is
            Assert.Empty(result);
        }

        [Fact] // als er geen zoos  bestaan geeft dit een lege lijst terug
        public void GetAllZoos_ReturnsEmptyList_WhenNoZoosExist()
        {
            // Arrange maakt een nieuwe zooservice instantie aan
            var service = GetZooServiceWithDb();

            // Act 
            // methode aanroepen om alle zoos te krijgen
            var result = service.GetAllZoos();

            // Assert 
            // controleert of het een  een lege lijst terug geeft
            Assert.Empty(result); 
        }

        // hetzelfde als de func hierboven maar dan voor specifieke zoos en dus als het niet bestaat een lege lijst terug geven
        [Fact]
        public void GetZooById_ReturnsNull_WhenZooDoesNotExist()
        {
            // Arrange
            var service = GetZooServiceWithDb();

            // Act 
            // probeert een zoo op te halen met id (99) die niet bestaat in de db
            var result = service.GetZooById(99);

            // Assert
            Assert.Null(result);
        }

        // controleert of we een nieuwe zoo kunnen toevoegen in de db
        [Fact]
        public async Task CreateZoo_AddsZooToDatabase()
        {
            // Arrange 
            //maakt een nieuwe zoo service aan
            var service = GetZooServiceWithDb();
            var newZoo = new Zoo
            {
                Name = "Test Zoo" //zoo met de naam test zoo
            };

            // Act 
            // hier maken we daadwerlijk via de service
            var createdZoo = await service.CreateZoo(newZoo);

            // Assert 
            // controleren we of de zoo echt gemaakt is met een geldige naam en id
            Assert.NotNull(createdZoo);
            Assert.Equal("Test Zoo", createdZoo.Name);
            Assert.NotEqual(0, createdZoo.Id);
        }

        //controleert of de create zoo met de bijbehorende dieren zoo maakt en de dieren correct koppelt
        [Fact] 
       
        public async Task CreateZoo_WithAnimals_AddsZooAndAssociatesAnimals()
        {
            // Arrange configureert een nieuwe memory in de db voor deze test
            var options = new DbContextOptionsBuilder<DBContext>()
                .UseInMemoryDatabase(databaseName: "TestDb_" + System.Guid.NewGuid())
                .Options;
            var context = new DBContext(options);
            var service = new ZooService(context);

            // Add hier voegen we eerst test animals aan de db toe
            var animal1 = new Animal { Name = "Lion" };
            var animal2 = new Animal { Name = "Tiger" };
            context.Animals.AddRange(animal1, animal2);
            await context.SaveChangesAsync(); // sla de dieren op

            //maakt een nieuwe zoo aan en geven we de animal ids van boven mee zodat we het koppelen
            var newZoo = new Zoo
            {
                Name = "Animal Kingdom",
                AnimalIds = new List<int> { animal1.Id, animal2.Id }
            };

            // Act 
            //hier worden ze gemaakt bij de service  en haalt ze weer op van de service
            var createdZoo = await service.CreateZoo(newZoo);
            var retrievedZoo = service.GetZooById(createdZoo.Id);

            // Assert 
            //controleren of alles goed gegaan is, dus dat de naam klopt en de meegegeven animal ids
            Assert.NotNull(retrievedZoo);
            Assert.Equal("Animal Kingdom", retrievedZoo.Name);
            Assert.Equal(2, retrievedZoo.Animals.Count);
            Assert.Contains(retrievedZoo.Animals, a => a.Name == "Lion");
            Assert.Contains(retrievedZoo.Animals, a => a.Name == "Tiger");
        }


        // controleert of de Sunrise-methode de juiste activiteitberichten retourneert. het eerste stukje bij arrange is over hetzelfde dus die ga ik niet opnieuw uitleggen
        [Fact]
        public void Sunrise_ReturnsCorrectActivityMessages()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<DBContext>()
                .UseInMemoryDatabase(databaseName: "TestDb_" + System.Guid.NewGuid())
                .Options;
            var context = new DBContext(options);
            var service = new ZooService(context);

            //maak een nieuwe zoo aan met verschillende activiteit patronen
            var zoo = new Zoo { Name = "Test Zoo" };
            var diurnalAnimal = new Animal
            {
                Name = "Lion",
                ActivityPattern = ActivityPattern.Diurnal
            };
            var nocturnalAnimal = new Animal
            {
                Name = "Owl",
                ActivityPattern = ActivityPattern.Nocturnal
            };
            // hier koppelen we de animals aan de zoo zowel in de animal lijst en animalids lijst
            zoo.Animals = new List<Animal> { diurnalAnimal, nocturnalAnimal };
            zoo.AnimalIds = new List<int> { diurnalAnimal.Id, nocturnalAnimal.Id };

            // Act 
            // roep de servie aan en dit moet een message terug geven gebaseerd op de activiteitspatronen.
            var messages = service.Sunrise(zoo);

            // Assert // controleren de berichten, dat ze de juiste berichten terug geven en bevatten
            Assert.Equal(2, messages.Count);
            Assert.Contains(messages, m => m == "Lion is wakker geworden.");
            Assert.Contains(messages, m => m == "Owl slaapt nog.");
        }

        // Controleert of de Sunset-methode de juiste activiteitberichten retourneert. deze werkt hetzelfde als hierboven maar net een andere message
        [Fact]
        public void Sunset_ReturnsCorrectActivityMessages()
        {
            // Arrange 
            var options = new DbContextOptionsBuilder<DBContext>()
                .UseInMemoryDatabase(databaseName: "TestDb_" + System.Guid.NewGuid())
                .Options;
            var context = new DBContext(options);
            var service = new ZooService(context);

            //Maak een Zoo en twee dieren met verschillende activiteitspatronen.
            var zoo = new Zoo { Name = "Test Zoo" };
            var diurnalAnimal = new Animal
            {
                Name = "Lion",
                ActivityPattern = ActivityPattern.Diurnal
            };
            var nocturnalAnimal = new Animal
            {
                Name = "Owl",
                ActivityPattern = ActivityPattern.Nocturnal
            };
            //koppelen aan de zoo
            zoo.Animals = new List<Animal> { diurnalAnimal, nocturnalAnimal };
            zoo.AnimalIds = new List<int> { diurnalAnimal.Id, nocturnalAnimal.Id };

            // Act 
            //roep de sunset aan
            var messages = service.Sunset(zoo);

            // Assert controleert de berichten
            Assert.Equal(2, messages.Count);
            Assert.Contains(messages, m => m == "Lion gaat slapen.");
            Assert.Contains(messages, m => m == "Owl is wakker geworden.");
        }

        //controleert of de methode de juiste message retourneert, dus de voedingsberichten op basis van hun dieet
        [Fact]
        public void FeedingTime_ReturnsCorrectDietMessages()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<DBContext>()
                .UseInMemoryDatabase(databaseName: "TestDb_" + System.Guid.NewGuid())
                .Options;
            var context = new DBContext(options);
            var service = new ZooService(context);

            // een nieuwe zoo maken en animals eronder
            var zoo = new Zoo { Name = "Test Zoo" };
            var carnivore = new Animal
            {
                Name = "Lion",
                Diet = DietaryClass.Carnivore //eet vlees
            };
            var herbivore = new Animal
            {
                Name = "Giraffe",
                Diet = DietaryClass.Herbivore // eet planten
            };
            var omnivore = new Animal
            {
                Name = "Bear",
                Diet = DietaryClass.Omnivore // eet allebei
            };

            //koppelt de dieren aan de zo
            zoo.Animals = new List<Animal> { carnivore, herbivore, omnivore };

            // Act
            // roepen we de service func aan. deze geeft een lijst met boodschappen gebaseerd op het dieet van elk dier.
            var messages = service.FeedingTime(zoo);

            // Assert
            // Controleer dat er drie berichten zijn en dat elk bericht klopt met het dieet van het dier.
            Assert.Equal(3, messages.Count);
            Assert.Contains(messages, m => m == "Lion eet vlees.");
            Assert.Contains(messages, m => m == "Giraffe eet planten.");
            Assert.Contains(messages, m => m == "Bear eet zowel vlees als planten.");
        }
    }
}