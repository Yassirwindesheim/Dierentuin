namespace Dierentuin.Models
{
    public class Category
    {
        public int? Id { get; set; }                 // Unique identifier
        public string? Name { get; set; }            // Name of the category (e.g., Mammals, Birds)

        // Relationships
        public List<Animal>? Animals { get; set; }   // Optional list of animals in this category
    }
}
