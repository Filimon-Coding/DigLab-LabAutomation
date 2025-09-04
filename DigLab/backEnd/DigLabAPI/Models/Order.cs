namespace DigLabAPI.Models;

public class Order
{
    public int Id { get; set; }

    // Unique lab number (also QR payload)
    public string LabNumber { get; set; } = default!; // e.g. LAB-20250417-8F2A1C3B

    public string Name { get; set; } = default!;
    public string? Personnummer { get; set; } // optional to store
    public DateOnly Date { get; set; }
    public TimeOnly Time { get; set; }

    // Store diagnoses as JSON in one column to keep it simple
    public string DiagnosesJson { get; set; } = "[]";

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    // Convenience property (not mapped) to work with List<string>
    public IReadOnlyList<string> Diagnoses
        => System.Text.Json.JsonSerializer.Deserialize<List<string>>(DiagnosesJson) ?? new();

    public void SetDiagnoses(IEnumerable<string> dx)
        => DiagnosesJson = System.Text.Json.JsonSerializer.Serialize(dx);
}
