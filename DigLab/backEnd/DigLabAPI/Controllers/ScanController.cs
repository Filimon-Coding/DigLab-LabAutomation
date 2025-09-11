// backEnd/DigLabAPI/Controllers/ScanController.cs
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.RegularExpressions;
using DigLabAPI.Data;
using DigLabAPI.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace DigLabAPI.Controllers;

[ApiController]
[Route("scan")]
public class ScanController : ControllerBase
{
    private readonly IHttpClientFactory _http;
    private readonly ILogger<ScanController> _logger;
    private readonly DigLabDb _db;

    private readonly string _formResultsDir;

    // LAB-YYYYMMDD-XXXXXXXX (hex)
    private static readonly Regex LabRe =
        new(@"LAB-\d{8}-[A-F0-9]{8}", RegexOptions.IgnoreCase | RegexOptions.Compiled);

    public ScanController(
        IHttpClientFactory http,
        ILogger<ScanController> logger,
        DigLabDb db,
        IOptions<StorageOptions> storage)
    {
        _http   = http;
        _logger = logger;
        _db     = db;

        // Resolve relative paths (like "storage/formsResults") against the app's content root
        _formResultsDir = Path.GetFullPath(storage.Value.FormResultsDir, AppContext.BaseDirectory);
        Directory.CreateDirectory(_formResultsDir);

        _logger.LogInformation("ScanController using formResultsDir={FormResultsDir}", _formResultsDir);
    }

    /// <summary>
    /// Upload a lab result (PDF/image), forward to Python for analysis,
    /// extract labnummer, and persist the uploaded file to formsResults as DigLab-{LAB}-results.pdf.
    /// Returns analyzer JSON + saved file paths.
    /// </summary>
    [HttpPost("analyze")]
    [RequestSizeLimit(20_000_000)]
    public async Task<IActionResult> Analyze([FromForm] IFormFile file, CancellationToken ct)
    {
        if (file is null || file.Length == 0)
            return BadRequest("No file uploaded.");

        var contentType = string.IsNullOrWhiteSpace(file.ContentType)
            ? "application/octet-stream"
            : file.ContentType;

        if (!(contentType.StartsWith("application/pdf", StringComparison.OrdinalIgnoreCase) ||
              contentType.StartsWith("image/",             StringComparison.OrdinalIgnoreCase)))
        {
            return StatusCode(StatusCodes.Status415UnsupportedMediaType,
                $"Unsupported content type: {contentType}");
        }

        // Buffer once (so we can forward + save)
        await using var src = file.OpenReadStream();
        using var ms = new MemoryStream();
        await src.CopyToAsync(ms, ct);
        var bytes = ms.ToArray();

        // ---- Call Python analyzer to obtain JSON (incl. labnummer) ----
        string analyzerJson;
        try
        {
            var client = _http.CreateClient("py"); // BaseAddress comes from Program.cs
            using var form = new MultipartFormDataContent();
            var s = new StreamContent(new MemoryStream(bytes));
            s.Headers.ContentType = new MediaTypeHeaderValue(contentType);
            form.Add(s, "file", file.FileName);

            using var resp = await client.PostAsync("/analyze", form, ct);
            analyzerJson = await resp.Content.ReadAsStringAsync(ct);

            if (!resp.IsSuccessStatusCode)
                return StatusCode((int)resp.StatusCode, analyzerJson);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "Analyze call failed");
            return Problem(statusCode: 500, detail: $"Analyze failed: {ex.Message}");
        }

        // ---- Extract labnummer (strict, with safe fallbacks) ----
        string? lab = null;
        try
        {
            using var doc = JsonDocument.Parse(analyzerJson);
            if (doc.RootElement.TryGetProperty("labnummer", out var labEl))
                lab = labEl.GetString();
        }
        catch { /* fall back below */ }

        if (string.IsNullOrWhiteSpace(lab))
        {
            var m = LabRe.Match(analyzerJson);
            if (m.Success) lab = m.Value.ToUpperInvariant();
        }
        if (string.IsNullOrWhiteSpace(lab))
        {
            var m = LabRe.Match(file.FileName);
            if (m.Success) lab = m.Value.ToUpperInvariant();
        }

        if (string.IsNullOrWhiteSpace(lab))
            return Problem(statusCode: 422, detail: "Analyzer returned no labnummer (and none found via fallback).");

        // ---- Persist the uploaded file to formsResults ----
        var canonical = Path.Combine(_formResultsDir, $"DigLab-{lab}-results.pdf");
        var simple    = Path.Combine(_formResultsDir, $"{lab}.pdf");

        try
        {
            await System.IO.File.WriteAllBytesAsync(canonical, bytes, ct);

            // Also write {LAB}.pdf for compatibility/lookups
            if (!string.Equals(canonical, simple, StringComparison.OrdinalIgnoreCase))
                await System.IO.File.WriteAllBytesAsync(simple, bytes, ct);

            if (!System.IO.File.Exists(canonical))
                return Problem(statusCode: 500,
                    detail: $"Write reported success but file not found at '{canonical}'. Check permissions.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Persist to {Dir} failed", _formResultsDir);
            return Problem(statusCode: 500, detail: $"Persist failed: {ex.Message}");
        }

        _logger.LogInformation("Saved analyzed file for {Lab} -> {Path}", lab, canonical);

        // Optional: warn if order doesn't exist yet (not fatal)
        try
        {
            var exists = await _db.Orders.AnyAsync(o => o.LabNumber == lab, ct);
            if (!exists)
                _logger.LogWarning("Saved analyzed file for {Lab}, but no matching Order found.", lab);
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "Order existence check failed (non-fatal).");
        }

        // ---- Return analyzer JSON + where we saved ----
        return new JsonResult(new
        {
            analyzer = TryParse(analyzerJson) ?? analyzerJson,
            saved = new { dir = _formResultsDir, paths = new[] { canonical, simple } }
        });
    }

    // --- tiny helper to keep response nice ---
    private static object? TryParse(string json)
    {
        try { return JsonSerializer.Deserialize<object>(json); }
        catch { return null; }
    }
}
