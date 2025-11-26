using Microsoft.AspNetCore.Mvc;
using ImportExportService.Models;
using System.Text;
using System.Text.Json;
using System.Globalization;

namespace ImportExportService.Controllers;

[ApiController]
[Route("[controller]")]
public class ImportExportController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<ImportExportController> _logger;

    public ImportExportController(IHttpClientFactory httpClientFactory, ILogger<ImportExportController> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    [HttpPost("import/csv")]
    public async Task<ActionResult<CSVImportResponse>> ImportCSV([FromBody] CSVImportRequest request)
    {
        var response = new CSVImportResponse
        {
            Message = "Import completed",
            ImportedCount = 0,
            Errors = new List<string>()
        };

        try
        {
            var lines = request.CsvContent.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            
            if (lines.Length < 2)
            {
                return BadRequest(new { message = "CSV file is empty or has no data rows" });
            }

            var expenseClient = _httpClientFactory.CreateClient("ExpenseService");
            var categoryClient = _httpClientFactory.CreateClient("CategoryService");

            // Пропускаем заголовок
            for (int i = 1; i < lines.Length; i++)
            {
                try
                {
                    var fields = lines[i].Split(',');
                    
                    if (fields.Length < 3)
                    {
                        response.Errors.Add($"Row {i + 1}: Invalid format");
                        continue;
                    }

                    var categoryName = fields[0].Trim();
                    
                    // Проверяем существование категории
                    var checkResponse = await categoryClient.GetAsync($"/categories/check/{Uri.EscapeDataString(categoryName)}");
                    if (checkResponse.IsSuccessStatusCode)
                    {
                        var checkContent = await checkResponse.Content.ReadAsStringAsync();
                        var checkResult = JsonSerializer.Deserialize<JsonElement>(checkContent);
                        
                        if (!checkResult.GetProperty("exists").GetBoolean())
                        {
                            // Создаем новую категорию
                            var newCategory = new { name = categoryName };
                            var categoryJson = JsonSerializer.Serialize(newCategory);
                            var categoryContent = new StringContent(categoryJson, Encoding.UTF8, "application/json");
                            await categoryClient.PostAsync("/categories", categoryContent);
                        }
                    }

                    var expense = new
                    {
                        category = categoryName,
                        amount = decimal.Parse(fields[1].Trim(), CultureInfo.InvariantCulture),
                        date = DateTime.Parse(fields[2].Trim()),
                        description = fields.Length > 3 ? fields[3].Trim() : ""
                    };

                    var json = JsonSerializer.Serialize(expense);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    
                    var httpResponse = await expenseClient.PostAsync("/expenses", content);

                    if (httpResponse.IsSuccessStatusCode)
                    {
                        response.ImportedCount++;
                    }
                    else
                    {
                        response.Errors.Add($"Row {i + 1}: Failed to create expense");
                    }
                }
                catch (Exception ex)
                {
                    response.Errors.Add($"Row {i + 1}: {ex.Message}");
                }
            }

            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = $"Failed to parse CSV: {ex.Message}" });
        }
    }

    [HttpGet("export/csv")]
    public async Task<ActionResult<CSVExportResponse>> ExportCSV()
    {
        try
        {
            var client = _httpClientFactory.CreateClient("ExpenseService");
            var httpResponse = await client.GetAsync("/expenses");
            httpResponse.EnsureSuccessStatusCode();

            var content = await httpResponse.Content.ReadAsStringAsync();
            var expenses = JsonSerializer.Deserialize<List<Expense>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new List<Expense>();

            var csv = new StringBuilder();
            csv.AppendLine("id,category,amount,date,description");

            foreach (var expense in expenses)
            {
                csv.AppendLine($"{expense.Id},{expense.Category},{expense.Amount:F2},{expense.Date:yyyy-MM-dd},{expense.Description}");
            }

            return Ok(new CSVExportResponse
            {
                CsvContent = csv.ToString(),
                Filename = "expenses_export.csv"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"Failed to export expenses: {ex.Message}" });
        }
    }

    [HttpGet("export/csv/template")]
    public IActionResult GetCSVTemplate()
    {
        var csv = new StringBuilder();
        csv.AppendLine("category,amount,date,description");
        csv.AppendLine("Продукты,1500.50,2024-01-15,Покупка в магазине");

        return Ok(new CSVExportResponse
        {
            CsvContent = csv.ToString(),
            Filename = "expenses_template.csv"
        });
    }

    [HttpGet("health")]
    public IActionResult Health()
    {
        return Ok(new { status = "healthy", service = "import-export-service" });
    }
}

