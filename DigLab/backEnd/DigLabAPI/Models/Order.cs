using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace DigLabAPI.Models;

[Index(nameof(LabNumber), IsUnique = true)]
public class Order
{
    public int Id { get; set; }

    [Required, MaxLength(32)]
    public string LabNumber { get; set; } = default!; // LAB-YYYYMMDD-XXXXXXXX

    [Required, MaxLength(200)]
    public string Name { get; set; } = default!;

    [MaxLength(11)]
    public string? Personnummer { get; set; }

    public DateOnly Date { get; set; }
    public TimeOnly Time { get; set; }

    // lagres som JSON-string (enkelt)
    public string DiagnosesJson { get; set; } = "[]";

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    // Hjelpefelt for kode
    public IReadOnlyList<string> Diagnoses =>
        System.Text.Json.JsonSerializer.Deserialize<List<string>>(DiagnosesJson) ?? new();

    public void SetDiagnoses(IEnumerable<string> dx) =>
        DiagnosesJson = System.Text.Json.JsonSerializer.Serialize(dx);
}
