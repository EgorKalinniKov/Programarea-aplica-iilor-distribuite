namespace StatisticsService.Models;

public class Expense
{
    public int Id { get; set; }
    public string Category { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public string Description { get; set; } = string.Empty;
}

public class DailyStatistic
{
    public string Date { get; set; } = string.Empty;
    public decimal Total { get; set; }
    public int Count { get; set; }
}

public class MonthlyStatistic
{
    public string Month { get; set; } = string.Empty;
    public decimal Total { get; set; }
    public int Count { get; set; }
}

public class CategoryStatistic
{
    public string Category { get; set; } = string.Empty;
    public decimal Total { get; set; }
    public int Count { get; set; }
    public decimal Percentage { get; set; }
}

public class SummaryStatistic
{
    public int TotalExpenses { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal AverageAmount { get; set; }
    public decimal MaxExpense { get; set; }
    public decimal MinExpense { get; set; }
}

