using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DigLabAPI.Controllers
{
    /// <summary>
    /// Håndterer opplasting av PDF/bilde for analyse (videresender til Python-tjenesten).
    /// </summary>
    [ApiController]
    [Route("scan")] // beholdt for kompatibilitet: POST /scan/analyze
    public sealed class ScanController : ControllerBase
    {
        private static readonly string[] AllowedMimePrefixes = { "application/pdf", "image/" };

        private readonly IHttpClientFactory _http;
        private readonly ILogger<ScanController> _logger;

        public ScanController(IHttpClientFactory http, ILogger<ScanController> logger)
        {
            _http = http;
            _logger = logger;
        }

        /// <summary>
        /// Last opp PDF/bilde; sendes videre til Python /analyze og returnerer JSON-resultat.
        /// </summary>
        [HttpPost("analyze")]
        [Consumes("multipart/form-data")]
        [Produces("application/json")]
        [RequestSizeLimit(20_000_000)] // 20 MB
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Analyze([FromForm] AnalyzeRequest req, CancellationToken ct = default)
        {
            // --- 1) Valider input ---
            if (req.File is null || req.File.Length == 0)
                return BadRequest("No file uploaded (form field must be 'file').");

            var contentType = string.IsNullOrWhiteSpace(req.File.ContentType)
                ? "application/octet-stream"
                : req.File.ContentType;

            if (!IsAllowed(contentType))
                return StatusCode(StatusCodes.Status415UnsupportedMediaType,
                    $"Unsupported content type: {contentType}. Allowed: PDF or image/*");

            // --- 2) Bygg multipart for Python ---
            using var form = new MultipartFormDataContent();

            await using var stream = req.File.OpenReadStream();
            using var filePart = new StreamContent(stream);
            filePart.Headers.ContentType = new MediaTypeHeaderValue(contentType);

            // NB: Python-tjenesten forventer feltet "file"
            form.Add(filePart, "file", string.IsNullOrWhiteSpace(req.File.FileName) ? "upload" : req.File.FileName);

            if (!string.IsNullOrWhiteSpace(req.LabNumber))
                form.Add(new StringContent(req.LabNumber), "labNumber");

            // --- 3) Kall Python /analyze ---
            try
            {
                var client = _http.CreateClient("py"); // BaseAddress settes i Program.cs (PyService:BaseUrl)
                using var resp = await client.PostAsync("/analyze", form, ct);
                var body = await resp.Content.ReadAsStringAsync(ct);

                if (!resp.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Python /analyze failed: {Status} {Body}", resp.StatusCode, body);
                    return StatusCode((int)resp.StatusCode, body);
                }

                // --- 4) Returner Python JSON uendret (pass-through) ---
                return Content(body, "application/json");
            }
            catch (OperationCanceledException)
            {
                // 499: Client Closed Request (uoffisiell, men ofte brukt)
                return Problem(statusCode: StatusCodes.Status499ClientClosedRequest, detail: "Client cancelled");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Analyze failed");
                return Problem(statusCode: 500, detail: "Analyze failed");
            }
        }

        private static bool IsAllowed(string contentType)
            => AllowedMimePrefixes.Any(p => contentType.StartsWith(p, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Form-data for /scan/analyze. DTO merkes [FromForm] i action-parameteren.
    /// </summary>
    public sealed class AnalyzeRequest
    {
        /// <summary>Opplastet fil (PDF/bilde). Felt-navn må være "file".</summary>
        public IFormFile File { get; set; } = default!;

        /// <summary>Valgfritt labnummer som følger opplastingen.</summary>
        public string? LabNumber { get; set; }
    }
}
