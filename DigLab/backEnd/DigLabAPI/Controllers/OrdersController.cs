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

    // >>> Fixed folders (adjust if your path differs) <<<
    private const string FormsDir =
        "/home/neov/Documents/MinCodingLinuxV/Prosjekter/5thSemester/DATA3770HelseteknologiProsjekt/Helseteknologi-prosjekt-Collection/DigLab/backEnd/DigLabAPI/PythonService/forms";
    private const string FormResultsDir =
        "/home/neov/Documents/MinCodingLinuxV/Prosjekter/5thSemester/DATA3770HelseteknologiProsjekt/Helseteknologi-prosjekt-Collection/DigLab/backEnd/DigLabAPI/PythonService/formResults";

    public OrdersController(IHttpClientFactory http, DigLabDb db)
    {
        _http = http;
        _db = db;
    }

    private static string NewLabNumber(DateOnly d)
    {
        var ymd = d.ToString("yyyyMMdd", CultureInfo.InvariantCulture);
        var rnd = Convert.ToHexString(RandomNumberGenerator.GetBytes(4));
        return $"LAB-{ymd}-{rnd}";
    }

    private static OrderListItemDto ToListItem(Order o) => new(
        LabNumber: o.LabNumber,
        Name: o.Name,
        Personnummer: o.Personnummer,
        Date: o.Date.ToString("yyyy-MM-dd"),
        Time: o.Time.ToString("HH\\:mm"),
        Diagnoses: o.Diagnoses.ToList(),
        CreatedAtUtc: o.CreatedAtUtc
    );

    // POST /api/orders — create order, generate requisition PDF to /forms
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateOrderDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Personnummer) || dto.Personnummer.Length != 11 || !dto.Personnummer.All(char.IsDigit))
            return BadRequest("personnummer must be 11 digits");

        var p = await _db.Persons.AsNoTracking().FirstOrDefaultAsync(x => x.Personnummer == dto.Personnummer);
        if (p is null) return NotFound("Person not found. Add to Person-register first.");

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

        // call Python to generate requisition form
        var client = _http.CreateClient("py");
        var resp = await client.PostAsJsonAsync("/generate-form", new
        {
            labnummer = order.LabNumber,
            name = order.Name,
            date = order.Date.ToString("yyyy-MM-dd"),
            time = order.Time.ToString("HH\\:mm"),
            diagnoses = order.Diagnoses,
            personnummer = order.Personnummer,
            qr_data = order.LabNumber
        });

        if (!resp.IsSuccessStatusCode)
        {
            var err = await resp.Content.ReadAsStringAsync();
            return StatusCode((int)resp.StatusCode, err);
        }

        var pdfBytes = await resp.Content.ReadAsByteArrayAsync();
        Directory.CreateDirectory(FormsDir);
        var fileName = $"DigLab-{order.LabNumber}.pdf";
        var absPath = Path.Combine(FormsDir, fileName);
        await System.IO.File.WriteAllBytesAsync(absPath, pdfBytes);

        // return file (unchanged UX)
        return File(pdfBytes, "application/pdf", fileName);
    }

    // GET /api/orders?take=50
    [HttpGet]
    public async Task<ActionResult<IEnumerable<OrderListItemDto>>> Recent([FromQuery] int take = 50)
    {
        take = Math.Clamp(take, 1, 200);
        var list = await _db.Orders.OrderByDescending(o => o.CreatedAtUtc).Take(take).ToListAsync();
        return list.Select(ToListItem).ToList();
    }

    // GET /api/orders/{lab}
    [HttpGet("{lab}")]
    public async Task<ActionResult<OrderDetailsDto>> GetByLab(string lab)
    {
        var o = await _db.Orders.Include(x => x.Results).AsNoTracking().FirstOrDefaultAsync(x => x.LabNumber == lab);
        if (o is null) return NotFound();

        var requested = o.Diagnoses.ToList();
        var results = o.Results.OrderBy(r => r.Diagnosis)
            .Select(r => new FinalizeResultRow(r.Diagnosis, r.Auto, r.Final, r.Overridden)).ToList();

        // We always expose one URL; controller decides which file to serve
        var dto = new OrderDetailsDto(
            LabNumber: o.LabNumber,
            Requested: requested,
            Results: results,
            OverriddenAny: o.AnyOverridden,
            PdfUrl: $"/api/orders/{Uri.EscapeDataString(o.LabNumber)}/pdf"
        );

        return Ok(dto);
    }

    // GET /api/orders/{lab}/pdf — prefer analyzed from /formResults; fallback to /forms
    [HttpGet("{lab}/pdf")]
    public IActionResult GetPdf(string lab, [FromQuery] string? prefer = null)
    {
        // Candidate names we’ll accept
        var analyzedNames = new[]
        {
        $"{lab}.pdf",
        $"DigLab-{lab}-results.pdf",
        $"DigLab-{lab}.pdf"
    };

        var formNames = new[]
        {
        $"DigLab-{lab}.pdf",
        $"{lab}.pdf"
    };

        string? analyzed = analyzedNames
            .Select(n => Path.Combine(FormResultsDir, n))
            .FirstOrDefault(System.IO.File.Exists);

        string? requisition = formNames
            .Select(n => Path.Combine(FormsDir, n))
            .FirstOrDefault(System.IO.File.Exists);

        // If caller explicitly wants analyzed results, enforce it.
        if (string.Equals(prefer, "results", StringComparison.OrdinalIgnoreCase))
        {
            if (analyzed is null)
                return NotFound($"No analyzed file for {lab} in {FormResultsDir}. Tried: {string.Join(", ", analyzedNames)}");
            return ServeFileInline(analyzed);
        }

        // Default behavior: analyzed if present, otherwise requisition form.
        var pick = analyzed ?? requisition;
        if (pick is null)
            return NotFound($"No file for {lab}. Looked in {FormResultsDir} ({string.Join(", ", analyzedNames)}) and {FormsDir} ({string.Join(", ", formNames)}).");

        return ServeFileInline(pick);

        // local function keeps headers consistent
        IActionResult ServeFileInline(string path)
        {
            var ext = Path.GetExtension(path).ToLowerInvariant();
            var mime = ext switch
            {
                ".pdf" => "application/pdf",
                ".png" => "image/png",
                ".jpg" or ".jpeg" => "image/jpeg",
                ".webp" => "image/webp",
                _ => "application/octet-stream"
            };

            Response.Headers["Content-Disposition"] = $"inline; filename=\"{Path.GetFileName(path)}\"";
            Response.Headers["Cache-Control"] = "no-store, no-cache, must-revalidate, max-age=0";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "0";

            return PhysicalFile(path, mime, enableRangeProcessing: true);
        }
    }
}