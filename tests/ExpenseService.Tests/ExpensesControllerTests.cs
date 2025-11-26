using ExpenseService.Controllers;
using ExpenseService.Data;
using ExpenseService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace ExpenseService.Tests;

public class ExpensesControllerTests
{
    private ExpenseDbContext GetInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<ExpenseDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new ExpenseDbContext(options);
    }

    [Fact]
    public async Task GetExpenses_ReturnsAllExpenses()
    {
        // Arrange
        var context = GetInMemoryContext();
        context.Expenses.Add(new Expense 
        { 
            Category = "Продукты", 
            Amount = 100, 
            Date = DateTime.UtcNow,
            Description = "Test"
        });
        await context.SaveChangesAsync();

        var controller = new ExpensesController(context, NullLogger<ExpensesController>.Instance);

        // Act
        var result = await controller.GetExpenses();

        // Assert
        var okResult = Assert.IsType<ActionResult<IEnumerable<Expense>>>(result);
        var expenses = Assert.IsAssignableFrom<IEnumerable<Expense>>(okResult.Value);
        Assert.Single(expenses);
    }

    [Fact]
    public async Task CreateExpense_AddsNewExpense()
    {
        // Arrange
        var context = GetInMemoryContext();
        var controller = new ExpensesController(context, NullLogger<ExpensesController>.Instance);
        var dto = new ExpenseCreateDto 
        { 
            Category = "Транспорт", 
            Amount = 50.5m, 
            Date = DateTime.UtcNow,
            Description = "Метро"
        };

        // Act
        var result = await controller.CreateExpense(dto);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var expense = Assert.IsType<Expense>(createdResult.Value);
        Assert.Equal("Транспорт", expense.Category);
        Assert.Equal(50.5m, expense.Amount);
    }

    [Fact]
    public async Task GetExpense_ReturnsExpense_WhenExists()
    {
        // Arrange
        var context = GetInMemoryContext();
        var expense = new Expense 
        { 
            Category = "Продукты", 
            Amount = 200, 
            Date = DateTime.UtcNow 
        };
        context.Expenses.Add(expense);
        await context.SaveChangesAsync();

        var controller = new ExpensesController(context, NullLogger<ExpensesController>.Instance);

        // Act
        var result = await controller.GetExpense(expense.Id);

        // Assert
        var okResult = Assert.IsType<ActionResult<Expense>>(result);
        var returnedExpense = Assert.IsType<Expense>(okResult.Value);
        Assert.Equal(expense.Id, returnedExpense.Id);
    }

    [Fact]
    public async Task DeleteExpense_RemovesExpense()
    {
        // Arrange
        var context = GetInMemoryContext();
        var expense = new Expense 
        { 
            Category = "Продукты", 
            Amount = 300, 
            Date = DateTime.UtcNow 
        };
        context.Expenses.Add(expense);
        await context.SaveChangesAsync();

        var controller = new ExpensesController(context, NullLogger<ExpensesController>.Instance);

        // Act
        var result = await controller.DeleteExpense(expense.Id);

        // Assert
        Assert.IsType<OkObjectResult>(result);
        Assert.Empty(context.Expenses);
    }
}
