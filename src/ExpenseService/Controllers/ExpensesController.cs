using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ExpenseService.Data;
using ExpenseService.Models;

namespace ExpenseService.Controllers;

[ApiController]
[Route("[controller]")]
public class ExpensesController : ControllerBase
{
    private readonly ExpenseDbContext _context;
    private readonly ILogger<ExpensesController> _logger;

    public ExpensesController(ExpenseDbContext context, ILogger<ExpensesController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Expense>>> GetExpenses()
    {
        return await _context.Expenses
            .OrderByDescending(e => e.Date)
            .ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Expense>> GetExpense(int id)
    {
        var expense = await _context.Expenses.FindAsync(id);

        if (expense == null)
        {
            return NotFound(new { message = "Expense not found" });
        }

        return expense;
    }

    [HttpPost]
    public async Task<ActionResult<Expense>> CreateExpense(ExpenseCreateDto dto)
    {
        var expense = new Expense
        {
            Category = dto.Category,
            Amount = dto.Amount,
            Date = dto.Date,
            Description = dto.Description,
            CreatedAt = DateTime.UtcNow
        };

        _context.Expenses.Add(expense);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetExpense), new { id = expense.Id }, expense);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Expense>> UpdateExpense(int id, ExpenseUpdateDto dto)
    {
        var expense = await _context.Expenses.FindAsync(id);

        if (expense == null)
        {
            return NotFound(new { message = "Expense not found" });
        }

        if (dto.Category != null)
            expense.Category = dto.Category;

        if (dto.Amount.HasValue)
            expense.Amount = dto.Amount.Value;

        if (dto.Date.HasValue)
            expense.Date = dto.Date.Value;

        if (dto.Description != null)
            expense.Description = dto.Description;

        await _context.SaveChangesAsync();

        return expense;
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteExpense(int id)
    {
        var expense = await _context.Expenses.FindAsync(id);

        if (expense == null)
        {
            return NotFound(new { message = "Expense not found" });
        }

        _context.Expenses.Remove(expense);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Expense deleted successfully", id });
    }

    [HttpGet("health")]
    public IActionResult Health()
    {
        return Ok(new { status = "healthy", service = "expense-service" });
    }
}

