using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DigLabAPI.Controllers
{
    // Prefix: /api/scan
    [ApiController]
    [Route("api/[controller]")]
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
        /// Last opp PDF/bilde for analyse. Sendes videre til Python-tjenesten.
        /// </summary>
        [HttpPost("analyze")]
        [Consumes("multipart/form-data")]
        [Produces("application/json")]
        [RequestSizeLimit(20_000_000)] // 20 MB
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Analyze([FromForm] AnalyzeRequest req, CancellationToken ct)
        {
            // 1) Valider input
            if (req.File is null || req.File.Length == 0)
                return BadRequest("No file uploaded (field name must be 'file').");

            var contentType = string.IsNullOrWhiteSpace(req.File.ContentType)
                ? "application/octet-stream"
                : req.File.ContentType;

            // Kun PDF eller bilde
            var isPdf = contentType.StartsWith("application/pdf", StringComparison.OrdinalIgnoreCase);
            var isImg = contentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase);
            if (!isPdf && !isImg)
                return StatusCode(StatusCodes.Status415UnsupportedMediaType,
                    $"Unsupported content type: {contentType}");

            // 2) Bygg multipart for Python
            using var form = new MultipartFormDataContent();

            await using var stream = req.File.OpenReadStream();
            using var filePart = new StreamContent(stream);
            filePart.Headers.ContentType = new MediaTypeHeaderValue(contentType);

            // NB: Python-tjenesten forventer feltet "file"
            form.Add(filePart, "file", req.File.FileName ?? "upload");

            // Ekstra metadata kan sendes som enkle form-felt
            if (!string.IsNullOrWhiteSpace(req.LabNumber))
                form.Add(new StringContent(req.LabNumber), "labNumber");

            // 3) Kall Python
            try
            {
                var client = _http.CreateClient("py"); // BaseAddress settes i Program.cs
                using var resp = await client.PostAsync("/analyze", form, ct);
                var body = await resp.Content.ReadAsStringAsync(ct);

                if (!resp.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Py /analyze failed: {Status} {Body}", resp.StatusCode, body);
                    return StatusCode((int)resp.StatusCode, body);
                }

                // 4) Returner JSON som Python gir (pass-through)
                return Content(body, "application/json");
            }
            catch (OperationCanceledException)
            {
                // 499 = Client Closed Request (uoffisiell, men ofte brukt)
                return Problem(statusCode: StatusCodes.Status499ClientClosedRequest, detail: "Client cancelled");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Analyze failed");
                return Problem(statusCode: 500, detail: "Analyze failed");
            }
        }
    }

    /// <summary>
    /// Form-data for /api/scan/analyze. DTO-en merkes [FromForm] i action-parameteren.
    /// </summary>
    public sealed class AnalyzeRequest
    {
        /// <summary>Opplastet fil (PDF eller bilde). Felt-navn må være "file".</summary>
        public IFormFile File { get; set; } = default!;

        /// <summary>Valgfritt: labnummer knyttet til opplastingen.</summary>
        public string? LabNumber { get; set; }
    }
}
