var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient("ExpenseService", client =>
{
    var baseUrl = builder.Configuration["Services:ExpenseService"] ?? "http://expense-service:8001";
    client.BaseAddress = new Uri(baseUrl);
});

builder.Services.AddHttpClient("CategoryService", client =>
{
    var baseUrl = builder.Configuration["Services:CategoryService"] ?? "http://category-service:8002";
    client.BaseAddress = new Uri(baseUrl);
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

app.Run();

