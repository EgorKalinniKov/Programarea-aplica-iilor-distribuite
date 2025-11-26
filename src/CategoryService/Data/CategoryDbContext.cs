using Microsoft.EntityFrameworkCore;
using CategoryService.Models;

namespace CategoryService.Data;

public class CategoryDbContext : DbContext
{
    public CategoryDbContext(DbContextOptions<CategoryDbContext> options) : base(options)
    {
    }

    public DbSet<Category> Categories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Category>(entity =>
        {
            entity.ToTable("categories");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Name).IsUnique();
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                  .HasColumnName("name")
                  .HasMaxLength(255);
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
        });
    }

    public void SeedDefaultCategories()
    {
        var defaultCategories = new[] 
        { 
            "Продукты", "Транспорт", "Развлечения", 
            "Здоровье", "Одежда", "Другое" 
        };

        foreach (var categoryName in defaultCategories)
        {
            if (!Categories.Any(c => c.Name == categoryName))
            {
                Categories.Add(new Category { Name = categoryName });
            }
        }

        SaveChanges();
    }
}

