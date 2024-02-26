using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

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
.WithName("GetWeatherForecast")
.WithOpenApi();

app.MapPost("/formularesult", (string formula,[FromBody] Dictionary<string, double> parameters) =>
{
    string output = "";
    try
    {
        var result = FormulaResult.FormulaEvaluation(formula, parameters);
        output = result.ToString();
    }
    catch(Exception e)
    {
        Console.WriteLine($"Error evaluating the formula: {e.Message}");
        output = e.Message;
    }
    return new { 
        formula, 
        parameters, 
        output, 
        timestamp= DateTime.UtcNow
    };
})
.WithName("GetFormulaResult")
.WithOpenApi();

app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

static class FormulaResult
{
    static internal double FormulaEvaluation(string formula, Dictionary<string, double> parameters)
    {
        // Replace the placeholders in the formula with their corresponding values
        foreach (var param in parameters)
        {
            formula = formula.Replace(param.Key, param.Value.ToString());
        }

        // Evaluate the formula
        var result = new DataTable().Compute(formula, null);
        return Convert.ToDouble(result);
        
    }
}