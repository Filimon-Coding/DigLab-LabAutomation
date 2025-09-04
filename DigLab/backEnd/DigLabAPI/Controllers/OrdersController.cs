using Microsoft.AspNetCore.Mvc;

namespace DigLabAPI.Controllers;

[ApiController]
[Route("orders")]
public class OrdersController : ControllerBase
{
    private readonly IHttpClientFactory _http;
    public OrdersController(IHttpClientFactory http) => _http = http;

    public record OrderDto(string Name, string Date, string Time, string[] Diagnoses, string? Personnummer);

    [HttpPost("form")]
    public async Task<IActionResult> CreateForm([FromBody] OrderDto dto)
    {
        var client = _http.CreateClient("py");
        var resp = await client.PostAsJsonAsync("/generate-form", new {
            name = dto.Name,
            date = dto.Date,
            time = dto.Time,
            diagnoses = dto.Diagnoses,
            personnummer = dto.Personnummer,   // show on PDF
            qr_data = dto.Personnummer         // QR encodes personnummer (testing)
        });

        if (!resp.IsSuccessStatusCode)
            return StatusCode((int)resp.StatusCode, await resp.Content.ReadAsStringAsync());

        var pdf = await resp.Content.ReadAsByteArrayAsync();
        return File(pdf, "application/pdf", $"DigLab-{DateTime.UtcNow:yyyyMMddHHmmss}.pdf");
    }

}
