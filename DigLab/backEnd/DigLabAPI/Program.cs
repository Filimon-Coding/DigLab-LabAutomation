// Program.cs
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using DigLabAPI.Data;

var builder = WebApplication.CreateBuilder(args);

// ------------------------ Configuration ------------------------ //
// You can override these via appsettings.json or environment vars.
var frontendOrigin = builder.Configuration["FrontendOrigin"] ?? "http://localhost:5173";
var pyBaseUrl      = builder.Configuration["PyService:BaseUrl"] ?? "http://localhost:7001";
var connString     = builder.Configuration.GetConnectionString("DigLabDb") ?? "Data Source=diglab.db";

// --------------------------- Services -------------------------- //
builder.Services.AddCors(o =>
    o.AddPolicy("AllowFrontend", p =>
        p.WithOrigins(frontendOrigin)
         .AllowAnyHeader()
         .AllowAnyMethod()
    )
);

// HttpClient for the Python microservice
builder.Services.AddHttpClient("py", c => { c.BaseAddress = new Uri(pyBaseUrl); });

// EF Core + SQLite
builder.Services.AddDbContext<DigLabDb>(opt => opt.UseSqlite(connString));

// Controllers + JSON options
builder.Services.AddControllers()
    .AddJsonOptions(o =>
    {
        // Enums as strings (optional)
        o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        // DateOnly/TimeOnly are supported in .NET 7+; no extra converters needed.
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// --------------------------- Pipeline -------------------------- //
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// If you serve HTTP only in dev, leave HTTPS redirection disabled.
// app.UseHttpsRedirection();

app.UseCors("AllowFrontend");

app.MapControllers();

// Simple roots/health
app.MapGet("/", () => Results.Ok("DigLab API is running"));
app.MapGet("/healthz", () => Results.Ok(new { ok = true, time = DateTime.UtcNow }));

// Ensure database exists (simple bootstrap; switch to migrations later)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<DigLabDb>();
    db.Database.EnsureCreated();
}

app.Run();
