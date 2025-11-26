using System.ComponentModel.DataAnnotations;

namespace CategoryService.Models;

public class Category
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class CategoryCreateDto
{
    [Required]
    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;
}

