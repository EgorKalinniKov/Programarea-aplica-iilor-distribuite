var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddSignalR();

builder.Services.AddHttpClient("ApiGateway", client =>
{
    var baseUrl = builder.Configuration["Services:ApiGateway"] ?? "http://api-gateway:8000";
    client.BaseAddress = new Uri(baseUrl);
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapRazorPages();

app.Run();

