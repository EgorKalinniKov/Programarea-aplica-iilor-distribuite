using CategoryService.Controllers;
using CategoryService.Data;
using CategoryService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

namespace CategoryService.Tests;

public class CategoriesControllerTests
{
    private CategoryDbContext GetInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<CategoryDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new CategoryDbContext(options);
    }

    [Fact]
    public async Task GetCategories_ReturnsAllCategories()
    {
        // Arrange
        var context = GetInMemoryContext();
        context.Categories.Add(new Category { Name = "Продукты" });
        context.Categories.Add(new Category { Name = "Транспорт" });
        await context.SaveChangesAsync();

        var controller = new CategoriesController(context, NullLogger<CategoriesController>.Instance);

        // Act
        var result = await controller.GetCategories();

        // Assert
        var okResult = Assert.IsType<ActionResult<IEnumerable<Category>>>(result);
        var categories = Assert.IsAssignableFrom<IEnumerable<Category>>(okResult.Value);
        Assert.Equal(2, categories.Count());
    }

    [Fact]
    public async Task CreateCategory_AddsNewCategory()
    {
        // Arrange
        var context = GetInMemoryContext();
        var controller = new CategoriesController(context, NullLogger<CategoriesController>.Instance);
        var dto = new CategoryCreateDto { Name = "Развлечения" };

        // Act
        var result = await controller.CreateCategory(dto);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var category = Assert.IsType<Category>(createdResult.Value);
        Assert.Equal("Развлечения", category.Name);
        Assert.Single(context.Categories);
    }

    [Fact]
    public async Task CreateCategory_ReturnsError_WhenDuplicate()
    {
        // Arrange
        var context = GetInMemoryContext();
        context.Categories.Add(new Category { Name = "Продукты" });
        await context.SaveChangesAsync();

        var controller = new CategoriesController(context, NullLogger<CategoriesController>.Instance);
        var dto = new CategoryCreateDto { Name = "Продукты" };

        // Act
        var result = await controller.CreateCategory(dto);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task DeleteCategory_RemovesCategory()
    {
        // Arrange
        var context = GetInMemoryContext();
        context.Categories.Add(new Category { Name = "Продукты" });
        await context.SaveChangesAsync();

        var controller = new CategoriesController(context, NullLogger<CategoriesController>.Instance);

        // Act
        var result = await controller.DeleteCategory("Продукты");

        // Assert
        Assert.IsType<OkObjectResult>(result);
        Assert.Empty(context.Categories);
    }
}
