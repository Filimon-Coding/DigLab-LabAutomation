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

    // requested diagnoses at order time (stored as JSON for simplicity)
    public string DiagnosesJson { get; set; } = "[]";

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    // persisted results (normalized as child rows)
    public bool ResultsSaved { get; set; }
    public bool AnyOverridden { get; set; }

    // PDF paths (relative to content root or absolute – we serve via /api/orders/{lab}/pdf)
    public string? RequisitionPdfPath { get; set; }   // created by /api/orders (order step)
    public string? ResultsPdfPath { get; set; }       // optional: if you later generate a results PDF

    public List<OrderResult> Results { get; set; } = new();

    // Helpers
    public IReadOnlyList<string> Diagnoses =>
        System.Text.Json.JsonSerializer.Deserialize<List<string>>(DiagnosesJson) ?? new();

    public void SetDiagnoses(IEnumerable<string> dx) =>
        DiagnosesJson = System.Text.Json.JsonSerializer.Serialize(dx);
}

public class OrderResult
{
    public int Id { get; set; }

    public int OrderId { get; set; }
    public Order Order { get; set; } = default!;

    [Required, MaxLength(100)]
    public string Diagnosis { get; set; } = default!;

    // "POSITIVE" | "NEGATIVE" | "INCONCLUSIVE" | "NONE"
    [Required, MaxLength(20)]
    public string Auto { get; set; } = "NONE";

    [Required, MaxLength(20)]
    public string Final { get; set; } = "NONE";

    public bool Overridden { get; set; }
}
