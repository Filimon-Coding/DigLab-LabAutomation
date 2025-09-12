using System.Globalization;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Serialization;
using DigLabAPI.Data;
using DigLabAPI.Models;
using DigLabAPI.Models.Dtos;
using DigLabAPI.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;


namespace DigLabAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class OrdersController : ControllerBase
{
    private readonly IHttpClientFactory _http;
    private readonly DigLabDb _db;
    private readonly string _formsDir;
    private readonly string _resultsDir;

    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters = { new JsonStringEnumConverter() }
    };

    public OrdersController(IHttpClientFactory http, DigLabDb db, IOptions<StorageOptions> storage)
    {
        _http = http;
        _db = db;

        var baseDir = AppContext.BaseDirectory;
        _formsDir   = Path.GetFullPath(storage.Value.FormsDir       ?? "storage/forms",       baseDir);
        _resultsDir = Path.GetFullPath(storage.Value.FormResultsDir ?? "storage/formResults", baseDir);

        Directory.CreateDirectory(_formsDir);
        Directory.CreateDirectory(_resultsDir);
    }

    // ---------------- Helpers ----------------

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

    private static string ReqPdfName(string lab)    => $"DigLab-{lab}.pdf";
    private static string ResultPdfName(string lab) => $"DigLab-{lab}-results.pdf";

    // ------------- POST /api/orders (create + requisition PDF) -------------

    [HttpPost]
    [Consumes("application/json")]
    [Produces("application/pdf")]
    public async Task<IActionResult> Create([FromBody] CreateOrderDto dto, CancellationToken ct)
    {
        if (dto is null) return BadRequest("Missing body.");
        if (string.IsNullOrWhiteSpace(dto.Personnummer) || dto.Personnummer.Length != 11 || !dto.Personnummer.All(char.IsDigit))
            return BadRequest("personnummer must be 11 digits");

        var p = await _db.Persons.AsNoTracking().FirstOrDefaultAsync(x => x.Personnummer == dto.Personnummer, ct);
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
        await _db.SaveChangesAsync(ct);

        var client = _http.CreateClient("py");
        using var resp = await client.PostAsJsonAsync("/generate-form", new
        {
            labnummer = order.LabNumber,
            name = order.Name,
            date = order.Date.ToString("yyyy-MM-dd"),
            time = order.Time.ToString("HH\\:mm"),
            diagnoses = order.Diagnoses,
            personnummer = order.Personnummer,
            qr_data = order.LabNumber
        }, JsonOpts, ct);

        var pdfBytes = await resp.Content.ReadAsByteArrayAsync(ct);
        if (!resp.IsSuccessStatusCode || pdfBytes.Length == 0)
        {
            var err = await resp.Content.ReadAsStringAsync(ct);
            return StatusCode((int)resp.StatusCode, $"Failed to generate form PDF: {err}");
        }

        var outPath = Path.Combine(_formsDir, ReqPdfName(order.LabNumber));
        await System.IO.File.WriteAllBytesAsync(outPath, pdfBytes, ct);

        Response.Headers["Content-Disposition"] = $"inline; filename=\"{Path.GetFileName(outPath)}\"";
        Response.Headers["Cache-Control"] = "no-store, no-cache, must-revalidate, max-age=0";
        return File(pdfBytes, "application/pdf");
    }

    // ---------------- GET /api/orders?take=50 ----------------

    [HttpGet]
    [Produces("application/json")]
    public async Task<ActionResult<IEnumerable<OrderListItemDto>>> Recent([FromQuery] int take = 50, CancellationToken ct = default)
    {
        take = Math.Clamp(take, 1, 200);
        var list = await _db.Orders.OrderByDescending(o => o.CreatedAtUtc).Take(take).ToListAsync(ct);
        return list.Select(ToListItem).ToList();
    }

    // ---------------- GET /api/orders/{lab} ----------------

    [HttpGet("{lab}")]
    [Produces("application/json")]
    public async Task<ActionResult<OrderDetailsDto>> GetByLab(string lab, CancellationToken ct)
    {
        var o = await _db.Orders
            .Include(x => x.Results)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.LabNumber == lab, ct);

        if (o is null) return NotFound();

        var requested = o.Diagnoses.ToList();
        var results = o.Results
            .OrderBy(r => r.Diagnosis)
            .Select(r => new FinalizeResultRow
            {
                Diagnosis  = r.Diagnosis,
                Auto       = r.Auto,
                Final      = r.Final,
                Overridden = r.Overridden   // <-- fixed
            })
            .ToList();


        var dto = new OrderDetailsDto(
            LabNumber: o.LabNumber,
            Requested: requested,
            Results: results,
            OverriddenAny: o.AnyOverridden,
            PdfUrl: $"/api/orders/{Uri.EscapeDataString(o.LabNumber)}/pdf"
        );

        return Ok(dto);
    }

    // ------------- GET /api/orders/{lab}/pdf?prefer=results -------------

    [HttpGet("{lab}/pdf")]
    public IActionResult GetPdf(string lab, [FromQuery] string? prefer = null)
    {
        string? analyzed = new[]
        {
            ResultPdfName(lab),
            $"{lab}.pdf",
            $"DigLab-{lab}.pdf"
        }.Select(n => Path.Combine(_resultsDir, n)).FirstOrDefault(System.IO.File.Exists);

        string? requisition = new[]
        {
            ReqPdfName(lab),
            $"{lab}.pdf"
        }.Select(n => Path.Combine(_formsDir, n)).FirstOrDefault(System.IO.File.Exists);

        if (string.Equals(prefer, "results", StringComparison.OrdinalIgnoreCase))
        {
            if (analyzed is null) return NotFound($"No analyzed file for {lab}");
            return ServeFile(analyzed);
        }

        var pick = analyzed ?? requisition;
        if (pick is null) return NotFound($"No PDF found for {lab}");

        return ServeFile(pick);

        IActionResult ServeFile(string path)
        {
            Response.Headers["Content-Disposition"] = $"inline; filename=\"{Path.GetFileName(path)}\"";
            Response.Headers["Cache-Control"] = "no-store, no-cache, must-revalidate, max-age=0";
            return PhysicalFile(path, "application/pdf", enableRangeProcessing: true);
        }
    }

    // ------------- POST /api/orders/{lab}/finalize -------------

    [HttpPost("{lab}/finalize")]
    [Consumes("application/json")]
    [Produces("application/json")]
    public async Task<IActionResult> Finalize(string lab, [FromBody] FinalizeRequest body, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(lab)) return BadRequest("Missing lab");
        if (body is null) return BadRequest("Missing body");

        var order = await _db.Orders
            .Include(o => o.Results)
            .FirstOrDefaultAsync(o => o.LabNumber == lab, ct);

        if (order is null) return NotFound($"Order {lab} not found.");

        // Replace results (simple, predictable)
// Replace results (simple, predictable)
        order.Results.Clear();
        foreach (var row in body.Results)
        {
            var auto  = string.IsNullOrWhiteSpace(row.Auto)  ? "NONE" : row.Auto!;
            var final = string.IsNullOrWhiteSpace(row.Final) ? auto    : row.Final!;

            order.Results.Add(new OrderResult
            {
                // EITHER use the FK:
                OrderId    = order.Id,
                // OR: Order = order,

                Diagnosis  = row.Diagnosis,
                Auto       = auto,
                Final      = final,
                Overridden = row.Overridden
                            || (!string.Equals(auto, final, StringComparison.OrdinalIgnoreCase))
            });
        }
        order.AnyOverridden = order.Results.Any(r => r.Overridden);
        await _db.SaveChangesAsync(ct);



        // Ask Python to create a results PDF (best effort)
        var client = _http.CreateClient("py");
        using var resp = await client.PostAsJsonAsync("/finalize-form", new
        {
            labnummer = order.LabNumber,
            name = order.Name,
            date = order.Date.ToString("yyyy-MM-dd"),
            time = order.Time.ToString("HH\\:mm"),
            results = order.Results.Select(r => new { diagnosis = r.Diagnosis, final = r.Final, auto = r.Auto }).ToArray()
        }, JsonOpts, ct);

        bool savedPdf = false;
        if (resp.IsSuccessStatusCode && string.Equals(resp.Content.Headers.ContentType?.MediaType, "application/pdf", StringComparison.OrdinalIgnoreCase))
        {
            var pdf = await resp.Content.ReadAsByteArrayAsync(ct);
            if (pdf.Length > 0)
            {
                var outPath = Path.Combine(_resultsDir, ResultPdfName(order.LabNumber));
                await System.IO.File.WriteAllBytesAsync(outPath, pdf, ct);
                savedPdf = true;
            }
        }

        return Ok(new { ok = true, savedPdf });
    }
}
