namespace ImportExportService.Models;

public class Expense
{
    public int Id { get; set; }
    public string Category { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public string Description { get; set; } = string.Empty;
}

public class CSVImportRequest
{
    public string CsvContent { get; set; } = string.Empty;
}

public class CSVImportResponse
{
    public string Message { get; set; } = string.Empty;
    public int ImportedCount { get; set; }
    public List<string> Errors { get; set; } = new();
}

public class CSVExportResponse
{
    public string CsvContent { get; set; } = string.Empty;
    public string Filename { get; set; } = string.Empty;
}

