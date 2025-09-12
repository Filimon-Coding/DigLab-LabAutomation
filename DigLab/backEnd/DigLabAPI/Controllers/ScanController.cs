using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;

namespace DigLabAPI.Controllers;

[ApiController]
[Route("scan")]
public class ScanController : ControllerBase
{
    private readonly IHttpClientFactory _http;
    private readonly ILogger<ScanController> _logger;

    public ScanController(IHttpClientFactory http, ILogger<ScanController> logger)
    {
        _http = http;
        _logger = logger;
    }

    /// <summary>
    /// POST /scan/analyze
    /// Accepts: multipart/form-data with field "file" (pdf/image).
    /// Forwards the upload to the Python service and returns its JSON.
    /// </summary>
    [HttpPost("analyze")]
    [RequestSizeLimit(20_000_000)] // 20MB (adjust as needed)
    public async Task<IActionResult> Analyze([FromForm] IFormFile file, CancellationToken ct)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded.");

        // Basic content-type check (allow pdf & images)
        var contentType = string.IsNullOrWhiteSpace(file.ContentType)
            ? "application/octet-stream"
            : file.ContentType;

        if (!(contentType.StartsWith("application/pdf", StringComparison.OrdinalIgnoreCase) ||
              contentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase)))
        {
            return StatusCode(StatusCodes.Status415UnsupportedMediaType,
                $"Unsupported content type: {contentType}");
        }

        // Build multipart payload to forward to Python
        using var form = new MultipartFormDataContent();
        await using var stream = file.OpenReadStream();
        using var streamContent = new StreamContent(stream);
        streamContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);
        form.Add(streamContent, "file", file.FileName); // field name expected by Py service: "file"

        try
        {
            var client = _http.CreateClient("py");
            // Python endpoint you’ll implement (e.g. FastAPI): POST /analyze
            using var resp = await client.PostAsync("/analyze", form, ct);
            var body = await resp.Content.ReadAsStringAsync(ct);

            if (!resp.IsSuccessStatusCode)
            {
                _logger.LogWarning("Py analyze failed: {Status} {Body}", resp.StatusCode, body);
                return StatusCode((int)resp.StatusCode, body);
            }

            // Pass the JSON straight through
            return Content(body, "application/json");
        }
        catch (OperationCanceledException)
        {
            return Problem(statusCode: StatusCodes.Status499ClientClosedRequest, detail: "Client cancelled");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Analyze failed");
            return Problem(statusCode: 500, detail: "Analyze failed");
        }
    }
}