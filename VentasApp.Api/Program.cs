using Microsoft.EntityFrameworkCore;
using VentasApp.Api.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApiDocument(); // Configurar NSwag

// Configuramos Temporalmente SQLite para pruebas rápidas de nube o InMemory
builder.Services.AddDbContext<VentasDbContext>(options =>
    options.UseInMemoryDatabase("VentasDbList"));

var app = builder.Build();

// Seed Database automatically
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<VentasDbContext>();
    context.Database.EnsureCreated();
}

app.UseOpenApi(); // Generar el archivo OpenAPI
app.UseSwaggerUi(); // Interfaz visual de NSwag

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
