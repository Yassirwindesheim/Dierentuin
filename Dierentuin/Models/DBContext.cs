using Dierentuin.Enum;
using Microsoft.EntityFrameworkCore;

namespace Dierentuin.Models
{
    public class DBContext : DbContext
    {
        public DbSet<Animal> Animals { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Enclosure> Enclosures { get; set; }
        public DbSet<Zoo> Zoos { get; set; }

        // Constructor accepting DbContextOptions
        public DBContext(DbContextOptions<DBContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // This can be left empty or used only when not configuring via dependency injection
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=Dierentuin5;Trusted_Connection=True;MultipleActiveResultSets=true");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Seed Categories
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Mammals" },
                new Category { Id = 2, Name = "Birds" },
                new Category { Id = 3, Name = "Reptiles" }
            );

            // Seed Enclosures
            modelBuilder.Entity<Enclosure>().HasData(
                new Enclosure { Id = 1, Name = "Savanna Exhibit", Climate = Climate.Tropical, HabitatType = HabitatType.Grassland, SecurityLevel = SecurityLevel.Medium, Size = 1000 },
                new Enclosure { Id = 2, Name = "Aviary", Climate = Climate.Temperate, HabitatType = HabitatType.Forest, SecurityLevel = SecurityLevel.Low, Size = 500 },
                new Enclosure { Id = 3, Name = "Reptile House", Climate = Climate.Artic, HabitatType = HabitatType.Aquatic, SecurityLevel = SecurityLevel.High, Size = 300 }
            );

            // Seed Animals
            modelBuilder.Entity<Animal>().HasData(
                new Animal
                {
                    Id = 1,
                    Name = "Lion",
                    Species = "Panthera leo",
                    Size = AnimalSize.Large,
                    Diet = DietaryClass.Carnivore,
                    ActivityPattern = ActivityPattern.Cathemeral,
                    SpaceRequirement = 200,
                    SecurityRequirement = SecurityLevel.High,
                    CategoryId = 1,
                    EnclosureId = 1
                },
                new Animal
                {
                    Id = 2,
                    Name = "Eagle",
                    Species = "Aquila chrysaetos",
                    Size = AnimalSize.Medium,
                    Diet = DietaryClass.Carnivore,
                    ActivityPattern = ActivityPattern.Diurnal,
                    SpaceRequirement = 100,
                    SecurityRequirement = SecurityLevel.Medium,
                    CategoryId = 2,
                    EnclosureId = 2
                },
                new Animal
                {
                    Id = 3,
                    Name = "Python",
                    Species = "Python reticulatus",
                    Size = AnimalSize.Large,
                    Diet = DietaryClass.Carnivore,
                    ActivityPattern = ActivityPattern.Nocturnal,
                    SpaceRequirement = 50,
                    SecurityRequirement = SecurityLevel.High,
                    CategoryId = 3,
                    EnclosureId = 3
                }
            );

            // Cascade delete configuration between Category and Animal
            // Fix foreign key configuration for CategoryId
            modelBuilder.Entity<Category>(builder =>
            {
                builder.ToTable("Categories"); // Make sure this matches your table name in the DB

                // Define the relationship where a Category has many Animals
                builder
                    .HasMany(category => category.Animals)
                    .WithOne(animal => animal.Category)
                    .HasForeignKey(animal => animal.CategoryId)
                    .IsRequired(false)  // Set to true if the relationship is required
                    .OnDelete(DeleteBehavior.Cascade); // Enabling cascade delete if necessary
            });

            modelBuilder.Entity<Animal>(builder =>
            {
                builder.ToTable("Animals"); // Same for the Animals table

                // Define the relationship from the Animal side (optional since we've already defined it from Category)
                builder
                    .HasOne(animal => animal.Category)
                    .WithMany(category => category.Animals)
                    .HasForeignKey(animal => animal.CategoryId)
                    .IsRequired(false)  // If CategoryId can be null, use false
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
        }
}
