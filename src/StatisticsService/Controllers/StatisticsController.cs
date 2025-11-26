using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using StatisticsService.Models;
using StatisticsService.Hubs;
using System.Text.Json;

namespace StatisticsService.Controllers;

[ApiController]
[Route("[controller]")]
public class StatisticsController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<StatisticsController> _logger;
    private readonly IHubContext<StatisticsHub> _hubContext;

    public StatisticsController(
        IHttpClientFactory httpClientFactory, 
        ILogger<StatisticsController> logger,
        IHubContext<StatisticsHub> hubContext)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _hubContext = hubContext;
    }

    private async Task<List<Expense>> GetAllExpenses()
    {
        var client = _httpClientFactory.CreateClient("ExpenseService");
        var response = await client.GetAsync("/expenses");
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<List<Expense>>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        }) ?? new List<Expense>();
    }

    [HttpGet("daily")]
    public async Task<ActionResult<IEnumerable<DailyStatistic>>> GetDailyStatistics(
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        var expenses = await GetAllExpenses();

        if (startDate.HasValue)
            expenses = expenses.Where(e => e.Date >= startDate.Value).ToList();

        if (endDate.HasValue)
            expenses = expenses.Where(e => e.Date <= endDate.Value).ToList();

        var dailyStats = expenses
            .GroupBy(e => e.Date.Date)
            .Select(g => new DailyStatistic
            {
                Date = g.Key.ToString("yyyy-MM-dd"),
                Total = Math.Round(g.Sum(e => e.Amount), 2),
                Count = g.Count()
            })
            .OrderBy(s => s.Date)
            .ToList();

        // Отправляем обновление всем подключенным клиентам
        await _hubContext.Clients.All.SendAsync("StatisticsUpdated", dailyStats);

        return Ok(dailyStats);
    }

    [HttpGet("monthly")]
    public async Task<ActionResult<IEnumerable<MonthlyStatistic>>> GetMonthlyStatistics(
        [FromQuery] int? year = null)
    {
        var expenses = await GetAllExpenses();

        if (year.HasValue)
            expenses = expenses.Where(e => e.Date.Year == year.Value).ToList();

        var monthlyStats = expenses
            .GroupBy(e => new { e.Date.Year, e.Date.Month })
            .Select(g => new MonthlyStatistic
            {
                Month = $"{g.Key.Year}-{g.Key.Month:D2}",
                Total = Math.Round(g.Sum(e => e.Amount), 2),
                Count = g.Count()
            })
            .OrderBy(s => s.Month)
            .ToList();

        return Ok(monthlyStats);
    }

    [HttpGet("by-category")]
    public async Task<ActionResult<IEnumerable<CategoryStatistic>>> GetCategoryStatistics(
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        var expenses = await GetAllExpenses();

        if (startDate.HasValue)
            expenses = expenses.Where(e => e.Date >= startDate.Value).ToList();

        if (endDate.HasValue)
            expenses = expenses.Where(e => e.Date <= endDate.Value).ToList();

        var totalAmount = expenses.Sum(e => e.Amount);

        var categoryStats = expenses
            .GroupBy(e => e.Category)
            .Select(g => new CategoryStatistic
            {
                Category = g.Key,
                Total = Math.Round(g.Sum(e => e.Amount), 2),
                Count = g.Count(),
                Percentage = totalAmount > 0 ? Math.Round((g.Sum(e => e.Amount) / totalAmount) * 100, 2) : 0
            })
            .OrderByDescending(s => s.Total)
            .ToList();

        return Ok(categoryStats);
    }

    [HttpGet("summary")]
    public async Task<ActionResult<SummaryStatistic>> GetSummaryStatistics()
    {
        var expenses = await GetAllExpenses();

        if (!expenses.Any())
        {
            return Ok(new SummaryStatistic
            {
                TotalExpenses = 0,
                TotalAmount = 0,
                AverageAmount = 0,
                MaxExpense = 0,
                MinExpense = 0
            });
        }

        var totalAmount = expenses.Sum(e => e.Amount);

        return Ok(new SummaryStatistic
        {
            TotalExpenses = expenses.Count,
            TotalAmount = Math.Round(totalAmount, 2),
            AverageAmount = Math.Round(totalAmount / expenses.Count, 2),
            MaxExpense = Math.Round(expenses.Max(e => e.Amount), 2),
            MinExpense = Math.Round(expenses.Min(e => e.Amount), 2)
        });
    }

    [HttpGet("health")]
    public IActionResult Health()
    {
        return Ok(new { status = "healthy", service = "statistics-service" });
    }
}

