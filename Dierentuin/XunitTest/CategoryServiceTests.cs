using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dierentuin.Data;
using Dierentuin.Models;
using Dierentuin.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Dierentuin.Tests
{
    public class CategoryServiceTests
    {
        // Methode om een in-memory database te maken en een CategoryService instantie te verkrijgen
        private CategoryService GetCategoryServiceWithDb()
        {
            var options = new DbContextOptionsBuilder<DBContext>()
                .UseInMemoryDatabase(databaseName: "TestDb_" + System.Guid.NewGuid())
                .Options;
            var context = new DBContext(options);

            // Verwijder bestaande gegevens uit de database
            if (context.Categories != null)
            {
                context.Categories.RemoveRange(context.Categories);
                context.SaveChanges();
            }
            if (context.Animals != null)
            {
                context.Animals.RemoveRange(context.Animals);
                context.SaveChanges();
            }

            return new CategoryService(context);
        }

        [Fact]
        // Test om te controleren of een lege lijst wordt geretourneerd wanneer er geen categorieën zijn
        public async Task GetAllCategories_ReturnsEmptyList_WhenNoCategoriesExist()
        {
            var service = GetCategoryServiceWithDb();
            var result = await service.GetAllCategories();
            Assert.Empty(result);
        }

        [Fact]
        // Test om te controleren of een categorie correct wordt toegevoegd aan de database
        public async Task CreateCategory_AddsCategoryToDatabase()
        {
            var service = GetCategoryServiceWithDb();
            // nieuwe categorie maken met naam mammals
            var newCategory = new Category { Name = "Mammals" };
            var createdCategory = await service.CreateCategory(newCategory);
            //checken of het niet null is
            Assert.NotNull(createdCategory);
            //checken of het gelijk is aan Mammals
            Assert.Equal("Mammals", createdCategory.Name);
            Assert.NotNull(createdCategory.Id);
        }

        [Fact]
        // Test om te controleren of een categorie met dieren correct wordt toegevoegd en geassocieerd
        public async Task CreateCategory_WithAnimals_AddsCategoryAndAssociatesAnimals()
        {
            var options = new DbContextOptionsBuilder<DBContext>()
                .UseInMemoryDatabase(databaseName: "TestDb_" + System.Guid.NewGuid())
                .Options;
            var context = new DBContext(options);
            var service = new CategoryService(context);

            // Voeg testdieren toe
            var animal1 = new Animal { Name = "Lion" };
            var animal2 = new Animal { Name = "Tiger" };
            context.Animals.AddRange(animal1, animal2);
            await context.SaveChangesAsync();
            //niewuwe categorie aanmaken
            var newCategory = new Category
            {
                Name = "Big Cats",
                AnimalIds = new List<int> { animal1.Id, animal2.Id }
            };

            var createdCategory = await service.CreateCategory(newCategory);
            Assert.NotNull(createdCategory);
            Assert.Equal("Big Cats", createdCategory.Name);
            // checken of het gelijk is aan 2 animals die we hierboven hebben gemaakt
            Assert.Equal(2, createdCategory.Animals.Count);
            //checken of het specifiek met de animal namen van boven gelijk aan zijn
            Assert.Contains(createdCategory.Animals, a => a.Name == "Lion");
            Assert.Contains(createdCategory.Animals, a => a.Name == "Tiger");
        }

        [Fact]
        // Test om te controleren of een categorie en de gekoppelde dieren correct worden bijgewerkt
        public async Task UpdateCategory_ModifiesCategoryAndAnimals()
        {
            var options = new DbContextOptionsBuilder<DBContext>()
                .UseInMemoryDatabase(databaseName: "TestDb_" + System.Guid.NewGuid())
                .Options;
            var context = new DBContext(options);
            var service = new CategoryService(context);

            // Maak een categorie en dieren aan
            var animal1 = new Animal { Name = "Lion" };
            var animal2 = new Animal { Name = "Tiger" };
            var animal3 = new Animal { Name = "Leopard" };
            //animals toevoegen van hierboven
            context.Animals.AddRange(animal1, animal2, animal3);
            await context.SaveChangesAsync();
            //nieuwe categorie aanmaken
            var category = new Category
            {
                Name = "Big Cats",
                AnimalIds = new List<int> { animal1.Id, animal2.Id }
            };
            var createdCategory = await service.CreateCategory(category);

            // Wijzig de categorie en dieren
            createdCategory.Name = "Updated Cats";
            createdCategory.AnimalIds = new List<int> { animal2.Id, animal3.Id };
            var updatedCategory = await service.UpdateCategory(createdCategory);
            //checken of de update van de naam goed is gegaan
            Assert.Equal("Updated Cats", updatedCategory.Name);
            Assert.Equal(2, updatedCategory.Animals.Count);
            Assert.DoesNotContain(updatedCategory.Animals, a => a.Name == "Lion");
            Assert.Contains(updatedCategory.Animals, a => a.Name == "Tiger");
            Assert.Contains(updatedCategory.Animals, a => a.Name == "Leopard");
        }

        [Fact]
        // Test om te controleren of een bestaande categorie correct wordt verwijderd
        public async Task DeleteCategory_RemovesCategory_WhenCategoryExists()
        {
            //categorie aanmaken
            var service = GetCategoryServiceWithDb();
            var newCategory = new Category { Name = "Test Category" };
            var createdCategory = await service.CreateCategory(newCategory);
            // de nieuwe categorie verwijderen met de net nieuwe gemaakt id categorie
            var result = await service.DeleteCategory(createdCategory.Id.Value);
            var deletedCategory = await service.GetCategoryById(createdCategory.Id.Value);
            Assert.True(result);
            Assert.Null(deletedCategory);
        }

        [Fact]
        // Test om te controleren of de juiste dieren worden opgehaald op basis van categorie
        public async Task GetAnimalsByCategory_ReturnsCorrectAnimals()
        {
            var options = new DbContextOptionsBuilder<DBContext>()
                .UseInMemoryDatabase(databaseName: "TestDb_" + System.Guid.NewGuid())
                .Options;
            var context = new DBContext(options);
            var service = new CategoryService(context);

            // hier maken we een categorie aan en de dieren ook om te koppelen
            var animal1 = new Animal { Name = "Lion" };
            var animal2 = new Animal { Name = "Tiger" };
            context.Animals.AddRange(animal1, animal2);
            await context.SaveChangesAsync();

            var category = new Category
            {
                Name = "Big Cats",
                AnimalIds = new List<int> { animal1.Id, animal2.Id }
            };
            //service func aanroepen om een categorie aan te maken
            var createdCategory = await service.CreateCategory(category);

            // checken of het serieus gemaakt is met de juiste waardes
            var animals = await service.GetAnimalsByCategory(createdCategory.Id.Value);
            Assert.Equal(2, animals.Count);
            Assert.Contains(animals, a => a.Name == "Lion");
            Assert.Contains(animals, a => a.Name == "Tiger");
        }
    }
}
