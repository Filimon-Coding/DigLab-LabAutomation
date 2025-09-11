namespace DigLabAPI.Models.Dtos;

public class CreateOrderDto
{
    public string Personnummer { get; set; } = default!;
    public List<string> Diagnoses { get; set; } = new();
    public DateOnly Date { get; set; }   // "YYYY-MM-DD"
    public TimeOnly Time { get; set; }   // "HH:MM"
}

// ----- Finalize (from Scan page) -----
public record FinalizeRequest(
    string LabNumber,
    List<string> Requested,
    List<FinalizeResultRow> Results,
    FinalizeMeta Meta
);

public record FinalizeResultRow(
    string Diagnosis,
    string Auto,
    string Final,
    bool Overridden
);

public record FinalizeMeta(
    string? Personnummer,
    string? Name,
    string? Date,
    string? Time,
    string? Result,
    double? Confidence
);

// ----- Responses used by History -----
public record OrderListItemDto(
    string LabNumber,
    string Name,
    string? Personnummer,
    string Date,
    string Time,
    List<string> Diagnoses,
    DateTime CreatedAtUtc
);

public record OrderDetailsDto(
    string LabNumber,
    List<string> Requested,
    List<FinalizeResultRow> Results,
    bool OverriddenAny,
    string? PdfUrl
);
