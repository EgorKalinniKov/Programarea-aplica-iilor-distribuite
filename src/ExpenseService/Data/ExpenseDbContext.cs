using Microsoft.EntityFrameworkCore;
using ExpenseService.Models;

namespace ExpenseService.Data;

public class ExpenseDbContext : DbContext
{
    public ExpenseDbContext(DbContextOptions<ExpenseDbContext> options) : base(options)
    {
    }

    public DbSet<Expense> Expenses { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Expense>(entity =>
        {
            entity.ToTable("expenses");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Category)
                  .HasColumnName("category")
                  .HasMaxLength(255);
            entity.Property(e => e.Amount)
                  .HasColumnName("amount")
                  .HasColumnType("decimal(10,2)");
            entity.Property(e => e.Date).HasColumnName("date");
            entity.Property(e => e.Description)
                  .HasColumnName("description")
                  .HasMaxLength(1000);
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
        });
    }
}

