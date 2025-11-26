using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CategoryService.Data;
using CategoryService.Models;

namespace CategoryService.Controllers;

[ApiController]
[Route("[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly CategoryDbContext _context;
    private readonly ILogger<CategoriesController> _logger;

    public CategoriesController(CategoryDbContext context, ILogger<CategoriesController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Category>>> GetCategories()
    {
        return await _context.Categories
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Category>> GetCategory(int id)
    {
        var category = await _context.Categories.FindAsync(id);

        if (category == null)
        {
            return NotFound(new { message = "Category not found" });
        }

        return category;
    }

    [HttpPost]
    public async Task<ActionResult<Category>> CreateCategory(CategoryCreateDto dto)
    {
        if (await _context.Categories.AnyAsync(c => c.Name == dto.Name))
        {
            return BadRequest(new { message = "Category already exists" });
        }

        var category = new Category
        {
            Name = dto.Name,
            CreatedAt = DateTime.UtcNow
        };

        _context.Categories.Add(category);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetCategory), new { id = category.Id }, category);
    }

    [HttpDelete("{name}")]
    public async Task<IActionResult> DeleteCategory(string name)
    {
        var category = await _context.Categories.FirstOrDefaultAsync(c => c.Name == name);

        if (category == null)
        {
            return NotFound(new { message = "Category not found" });
        }

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Category deleted successfully", name });
    }

    [HttpGet("check/{name}")]
    public async Task<IActionResult> CheckCategoryExists(string name)
    {
        var exists = await _context.Categories.AnyAsync(c => c.Name == name);
        return Ok(new { exists });
    }

    [HttpGet("health")]
    public IActionResult Health()
    {
        return Ok(new { status = "healthy", service = "category-service" });
    }
}

