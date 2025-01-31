using Dierentuin.Data;
using Dierentuin.Models;
using Microsoft.EntityFrameworkCore;
namespace Dierentuin.Services

//De CategoryService beheert categorieën van dieren en zorgt voor CRUD-operaties. Het koppelt dieren aan categorieën en houdt de relaties bij.
//Het lastigste is het dynamisch beheren van de dieren die aan een categorie gekoppeld worden, vooral bij updates. Maar dit is goed te doen!



{
    public class CategoryService
    {
        private readonly DBContext _context;

        // Constructor om de DBContext via dependency injection in te voegen
        public CategoryService(DBContext context)
        {
            _context = context;  // Toegang tot de database via de injected context
        }

        // Haalt alle categorieën op uit de database asynchroon
        public async Task<List<Category>> GetAllCategories()
        {
            return await _context.Categories.ToListAsync();  // Haalt alle categorieën uit de database
        }

        // Haalt een specifieke categorie op via het ID
        public async Task<Category> GetCategoryById(int id)
        {
            return await _context.Categories
                .Include(c => c.Animals) // Zorg ervoor dat de bijbehorende dieren ook geladen worden
                .FirstOrDefaultAsync(c => c.Id == id);  // Haalt de categorie op met het opgegeven ID
        }

        // Maakt een nieuwe categorie aan en slaat deze op in de database
        public async Task<Category> CreateCategory(Category category)
        {
            // Als er dier-IDs zijn, probeer dan de bijbehorende dieren op te halen
            if (category.AnimalIds != null && category.AnimalIds.Any())
            {
                var animals = await _context.Animals
                    .Where(a => category.AnimalIds.Contains(a.Id))  // Haal dieren op met de opgegeven IDs
                    .ToListAsync();
                category.Animals = animals;  // Stel de bijbehorende dieren in op de categorie
            }

            _context.Categories.Add(category);  // Voeg de nieuwe categorie toe aan de database
            await _context.SaveChangesAsync();  // Sla de wijzigingen op asynchroon
            return category;  // Retourneer de gemaakte categorie
        }

        // Update een bestaande categorie
        public async Task<Category> UpdateCategory(Category updatedCategory)
        {
            // Haal de bestaande categorie op uit de database
            var existingCategory = await _context.Categories
                .Include(c => c.Animals) // Laad de huidige dieren van de categorie
                .FirstOrDefaultAsync(c => c.Id == updatedCategory.Id);

            if (existingCategory != null)
            {
                // Werk de naam van de categorie bij
                existingCategory.Name = updatedCategory.Name;

                // Als er nieuwe dier-IDs zijn, werk dan de dierenlijst bij
                if (updatedCategory.AnimalIds != null)
                {
                    var animals = await _context.Animals
                        .Where(a => updatedCategory.AnimalIds.Contains(a.Id))  // Haal de nieuwe dieren op
                        .ToListAsync();
                    existingCategory.Animals = animals;  // Update de dierenlijst van de categorie
                }

                await _context.SaveChangesAsync();  // Sla de wijzigingen op asynchroon
            }
            return existingCategory;  // Retourneer de bijgewerkte categorie
        }

        // Verwijdert een categorie uit de database
        public async Task<bool> DeleteCategory(int id)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
            if (category != null)
            {
                _context.Categories.Remove(category);  // Verwijder de categorie uit de database
                await _context.SaveChangesAsync();  // Sla de wijzigingen op asynchroon
                return true;  // Retourneer true als de categorie succesvol is verwijderd
            }
            return false;  // Retourneer false als de categorie niet gevonden werd
        }

        // Wijs een dier toe aan een categorie
        public async Task AssignAnimalToCategory(int animalId, int categoryId)
        {
            // Haal het dier en de categorie op
            var animal = await _context.Animals.FirstOrDefaultAsync(a => a.Id == animalId);
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == categoryId);
            if (animal != null && category != null)
            {
                // Wijzig de categorie van het dier
                animal.CategoryId = categoryId;
                animal.Category = category;
                await _context.SaveChangesAsync();  // Sla de wijzigingen op asynchroon
            }
        }

        // Haalt de IDs van dieren op die behoren tot een bepaalde categorie
        public async Task<List<int>> GetAnimalIdsByCategoryId(int categoryId)
        {
            return await _context.Animals
                .Where(a => a.CategoryId == categoryId)  // Haal dieren op die behoren tot de opgegeven categorie
                .Select(a => a.Id)  // Selecteer alleen de IDs van de dieren
                .ToListAsync();  // Zet de resultaten om naar een lijst
        }

        // Filtert dieren op basis van de categorie-ID
        public async Task<List<Animal>> GetAnimalsByCategory(int categoryId)
        {
            return await _context.Animals
                .Where(a => a.CategoryId == categoryId)  // Haal dieren op die behoren tot de opgegeven categorie
                .ToListAsync();  // Zet de resultaten om naar een lijst
        }
    }
}
