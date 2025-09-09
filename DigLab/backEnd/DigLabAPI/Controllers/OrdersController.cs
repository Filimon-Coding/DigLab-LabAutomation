using System.Globalization;
using System.Security.Cryptography;
using DigLabAPI.Data;
using DigLabAPI.Models;
using DigLabAPI.Models.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DigLabAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IHttpClientFactory _http;
    private readonly DigLabDb _db;

    public OrdersController(IHttpClientFactory http, DigLabDb db)
    {
        _http = http;
        _db = db;
    }

    private static string NewLabNumber(DateOnly d)
    {
        var ymd = d.ToString("yyyyMMdd", CultureInfo.InvariantCulture);
        var rnd = Convert.ToHexString(RandomNumberGenerator.GetBytes(4)); // 8 hex-chars
        return $"LAB-{ymd}-{rnd}";
    }

    private static object ToView(Order o) => new
    {
        o.LabNumber,
        o.Name,
        o.Personnummer,
        Date = o.Date.ToString("yyyy-MM-dd"),
        Time = o.Time.ToString("HH\\:mm"),
        Diagnoses = o.Diagnoses,
        o.CreatedAtUtc
    };

    // POST /api/orders  (brukes av "Send")
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateOrderDto dto)
    {
        // 1) Valider PNR
        if (string.IsNullOrWhiteSpace(dto.Personnummer) ||
            dto.Personnummer.Length != 11 || !dto.Personnummer.All(char.IsDigit))
            return BadRequest("personnummer must be 11 digits");

        // 2) Finn person for autofyll
        var p = await _db.Persons.AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Personnummer == dto.Personnummer);
        if (p is null) return NotFound("Person not found. Add to Person-register first.");

        // 3) Lag Order
        var order = new Order
        {
            LabNumber = NewLabNumber(dto.Date),
            Name = $"{p.FirstName} {p.LastName}".Trim(),
            Personnummer = p.Personnummer,
            Date = dto.Date,
            Time = dto.Time,
            CreatedAtUtc = DateTime.UtcNow
        };
        order.SetDiagnoses(dto.Diagnoses);

        _db.Orders.Add(order);
        await _db.SaveChangesAsync();

        // 4) Kall Python for å generere PDF (QR = LabNumber)
        var client = _http.CreateClient("py");
        var resp = await client.PostAsJsonAsync("/generate-form", new
        {
            labnummer   = order.LabNumber,
            name        = order.Name,
            date        = order.Date.ToString("yyyy-MM-dd"),
            time        = order.Time.ToString("HH\\:mm"),
            diagnoses   = order.Diagnoses,
            personnummer= order.Personnummer,
            qr_data     = order.LabNumber
        });

        if (!resp.IsSuccessStatusCode)
        {
            var err = await resp.Content.ReadAsStringAsync();
            // (valgfritt) marker statusfelt i DB
            return StatusCode((int)resp.StatusCode, err);
        }

        var pdf = await resp.Content.ReadAsByteArrayAsync();
        var filename = $"DigLab-{order.LabNumber}.pdf";
        return File(pdf, "application/pdf", filename);
    }

    // Hent siste ordrer (til historikkvisning)
    [HttpGet]
    public async Task<ActionResult<IEnumerable<object>>> Recent([FromQuery] int take = 50)
    {
        take = Math.Clamp(take, 1, 200);
        var list = await _db.Orders
            .OrderByDescending(o => o.CreatedAtUtc)
            .Take(take)
            .ToListAsync();
        return list.Select(ToView).ToList();
    }

    // GET /api/orders/{lab}
    [HttpGet("{lab}")]
    public async Task<ActionResult<object>> GetByLab(string lab)
    {
        var o = await _db.Orders.AsNoTracking().FirstOrDefaultAsync(x => x.LabNumber == lab);
        return o is null ? NotFound() : Ok(ToView(o));
    }
}
