//using Dierentuin.Models;
//using Microsoft.EntityFrameworkCore;
//using static Dierentuin.Controllers.ZooController;

//public class ZooContext : DbContext
//{
//    public ZooContext(DbContextOptions<ZooContext> options) : base(options) { }

//    public DbSet<Animal> Animals { get; set; }
//    public DbSet<Category> Categories { get; set; }
//    public DbSet<Enclosure> Enclosures { get; set; }

//    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//    {
//        optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=Dierentuin5;Trusted_Connection=True;MultipleActiveResultSets=true");
//    }

//    protected override void OnModelCreating(ModelBuilder modelBuilder)
//    {
//        // Configure your relationships and seeding here if necessary
//    }
//}

