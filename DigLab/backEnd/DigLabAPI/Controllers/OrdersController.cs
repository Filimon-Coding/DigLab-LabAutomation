using Microsoft.AspNetCore.Mvc;
using DigLabAPI.Data;
using DigLabAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Security.Cryptography;

namespace DigLabAPI.Controllers;

[ApiController]
[Route("orders")]
public class OrdersController : ControllerBase
{
    private readonly IHttpClientFactory _http;
    private readonly DigLabDb _db;

    public OrdersController(IHttpClientFactory http, DigLabDb db)
    {
        _http = http;
        _db = db;
    }

    // Incoming payload from frontend
    public record CreateOrderDto(
        string Name,
        string Date,     // "YYYY-MM-DD"
        string Time,     // "HH:MM"
        string[] Diagnoses,
        string? Personnummer
    );

    // Outgoing shape
    public record OrderView(
        string LabNumber,
        string Name,
        string? Personnummer,
        string Date,
        string Time,
        string[] Diagnoses,
        DateTime CreatedAtUtc
    );

    private static string NewLabNumber()
    {
        // LAB-YYYYMMDD-XXXXXXXX (hex-ish)
        var day = DateTime.UtcNow.ToString("yyyyMMdd");
        var rnd = Convert.ToHexString(RandomNumberGenerator.GetBytes(4)); // 8 hex chars
        return $"LAB-{day}-{rnd}";
    }

    private static DateOnly ParseDate(string s)
        => DateOnly.ParseExact(s, "yyyy-MM-dd", CultureInfo.InvariantCulture);
    private static TimeOnly ParseTime(string s)
        => TimeOnly.ParseExact(s, "HH\\:mm", CultureInfo.InvariantCulture);

    private static OrderView ToView(Order o) =>
        new(
            o.LabNumber,
            o.Name,
            o.Personnummer,
            o.Date.ToString("yyyy-MM-dd"),
            o.Time.ToString("HH\\:mm"),
            o.Diagnoses.ToArray(),
            o.CreatedAtUtc
        );

    [HttpPost("form")]
    public async Task<IActionResult> CreateForm([FromBody] CreateOrderDto dto)
    {
        // 1) Create model
        var order = new Order
        {
            LabNumber = NewLabNumber(),
            Name = dto.Name.Trim(),
            Personnummer = string.IsNullOrWhiteSpace(dto.Personnummer) ? null : dto.Personnummer.Trim(),
            Date = ParseDate(dto.Date),
            Time = ParseTime(dto.Time),
        };
        order.SetDiagnoses(dto.Diagnoses ?? Array.Empty<string>());

        // 2) Save to DB first (so it's persisted even if PDF call fails)
        _db.Orders.Add(order);
        await _db.SaveChangesAsync();

        // 3) Call Python to make PDF (send labnummer + qr_data = labnummer)
        var client = _http.CreateClient("py");
        var resp = await client.PostAsJsonAsync("/generate-form", new
        {
            labnummer = order.LabNumber,
            name = order.Name,
            date = order.Date.ToString("yyyy-MM-dd"),
            time = order.Time.ToString("HH\\:mm"),
            diagnoses = order.Diagnoses,
            personnummer = order.Personnummer,
            qr_data = order.LabNumber // IMPORTANT: QR uses lab number
        });

        if (!resp.IsSuccessStatusCode)
        {
            var err = await resp.Content.ReadAsStringAsync();
            // You might want to mark a status field on the order here.
            return StatusCode((int)resp.StatusCode, err);
        }

        var pdf = await resp.Content.ReadAsByteArrayAsync();
        // Suggested filename with labnummer
        var filename = $"DigLab-{order.LabNumber}.pdf";
        return File(pdf, "application/pdf", filename);
    }

    // GET /orders?take=50  -> recent items
    [HttpGet]
    public async Task<ActionResult<IEnumerable<OrderView>>> GetRecent([FromQuery] int take = 50)
    {
        take = Math.Clamp(take, 1, 200);
        var list = await _db.Orders
            .OrderByDescending(o => o.CreatedAtUtc)
            .Take(take)
            .ToListAsync();

        return list.Select(ToView).ToList();
    }

    // GET /orders/{labnummer}
    [HttpGet("{labnummer}")]
    public async Task<ActionResult<OrderView>> GetByLab(string labnummer)
    {
        var o = await _db.Orders.FirstOrDefaultAsync(x => x.LabNumber == labnummer);
        if (o == null) return NotFound();
        return ToView(o);
    }

    // GET /orders/by-pnr/{personnummer}
    [HttpGet("by-pnr/{personnummer}")]
    public async Task<ActionResult<IEnumerable<OrderView>>> GetByPnr(string personnummer)
    {
        var list = await _db.Orders
            .Where(o => o.Personnummer == personnummer)
            .OrderByDescending(o => o.CreatedAtUtc)
            .ToListAsync();

        return list.Select(ToView).ToList();
    }
}
