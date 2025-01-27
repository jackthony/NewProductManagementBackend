using Microsoft.EntityFrameworkCore;
using ProductManagement.Domain.Entities;

namespace ProductManagement.Infrastructure.Data;

public class AppDbContext : DbContext
{
    private readonly bool _useSeedData;

    public AppDbContext(DbContextOptions<AppDbContext> options, bool useSeedData = true) : base(options)
    {
        _useSeedData = useSeedData;
    }

    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        if (_useSeedData)
        {
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Electronics" },
                new Category { Id = 2, Name = "Books" }
            );

            modelBuilder.Entity<Product>().HasData(
                new Product { Id = 1, Name = "Laptop", Description = "High-end laptop", Price = 1500, Stock = 10, CategoryId = 1 }
            );
        }
    }
}
