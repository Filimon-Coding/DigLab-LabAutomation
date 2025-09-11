using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using DigLabAPI.Data;
using DigLabAPI.Options;             // <-- make sure this using is present
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);   // <-- create builder first

// Bind StorageOptions from appsettings
builder.Services.Configure<StorageOptions>(builder.Configuration.GetSection("Storage"));  // <-- now it's valid

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
    .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// (optional) ensure storage dirs exist at boot
{
    var storage = app.Services.GetRequiredService<IOptions<StorageOptions>>().Value;
    Directory.CreateDirectory(Path.GetFullPath(storage.FormsDir,        AppContext.BaseDirectory));
    Directory.CreateDirectory(Path.GetFullPath(storage.FormResultsDir,  AppContext.BaseDirectory));
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowFrontend");
app.MapControllers();
app.MapGet("/", () => Results.Ok("DigLab API is running"));
app.MapGet("/healthz", () => Results.Ok(new { ok = true, time = DateTime.UtcNow }));

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<DigLabDb>();
    db.Database.Migrate();
}

app.Run();
