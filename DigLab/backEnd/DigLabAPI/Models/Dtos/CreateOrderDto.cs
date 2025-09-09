namespace DigLabAPI.Models.Dtos;

public class CreateOrderDto
{
    public string Personnummer { get; set; } = default!;
    public List<string> Diagnoses { get; set; } = new();
    public DateOnly Date { get; set; }   // "YYYY-MM-DD" i JSON blir auto-parset til DateOnly i .NET 8
    public TimeOnly Time { get; set; }   // "HH:MM"
}
