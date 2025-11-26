using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseService.Models;

public class Expense
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(255)]
    public string Category { get; set; } = string.Empty;

    [Required]
    [Column(TypeName = "decimal(10,2)")]
    public decimal Amount { get; set; }

    [Required]
    public DateTime Date { get; set; }

    [MaxLength(1000)]
    public string Description { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class ExpenseCreateDto
{
    [Required]
    public string Category { get; set; } = string.Empty;

    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal Amount { get; set; }

    [Required]
    public DateTime Date { get; set; }

    public string Description { get; set; } = string.Empty;
}

public class ExpenseUpdateDto
{
    public string? Category { get; set; }
    
    [Range(0.01, double.MaxValue)]
    public decimal? Amount { get; set; }
    
    public DateTime? Date { get; set; }
    
    public string? Description { get; set; }
}

