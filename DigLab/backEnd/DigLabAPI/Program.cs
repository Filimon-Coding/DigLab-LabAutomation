using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using DigLabAPI.Data;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// ---- Konfig ----
var frontendOrigin = builder.Configuration["FrontendOrigin"] ?? "http://localhost:5173";
var pyBaseUrl      = builder.Configuration["PyService:BaseUrl"] ?? "http://localhost:7001";
var connString     = builder.Configuration.GetConnectionString("DigLabDb")
    ?? throw new InvalidOperationException("Missing connection string 'DigLabDb'");

// ---- Services ----
builder.Services.AddCors(o =>
    o.AddPolicy("AllowFrontend", p => p.WithOrigins(frontendOrigin).AllowAnyHeader().AllowAnyMethod())
);

builder.Services.AddHttpClient("py", c => c.BaseAddress = new Uri(pyBaseUrl));

builder.Services.AddDbContext<DigLabDb>(opt =>
{
    var serverVersion = ServerVersion.AutoDetect(connString);
    opt.UseMySql(connString, serverVersion);
});

builder.Services.AddControllers()
    .AddJsonOptions(o =>
    {
        o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        // DateOnly/TimeOnly støttes i .NET 8
    });

builder.Services.AddEndpointsApiExplorer();


builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "DigLabAPI", Version = "v1" });

    // Swashbuckle trenger manuell mapping av .NET 6+ dato-typer
    c.MapType<DateOnly>(() => new OpenApiSchema { Type = "string", Format = "date" });
    c.MapType<TimeOnly>(() => new OpenApiSchema { Type = "string", Format = "time" });
});

var app = builder.Build();

// ---- Pipeline ----
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowFrontend");
app.MapControllers();
app.MapGet("/", () => Results.Ok("DigLab API is running"));
app.MapGet("/healthz", () => Results.Ok(new { ok = true, time = DateTime.UtcNow }));

// Kjør EF migrasjoner ved oppstart (MySQL best-practice)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<DigLabDb>();
    db.Database.Migrate();
}

app.Run();
