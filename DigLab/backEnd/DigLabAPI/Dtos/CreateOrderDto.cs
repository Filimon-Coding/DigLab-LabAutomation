namespace DigLabAPI.Models.Dtos
{
    public sealed class CreateOrderDto
    {
        public string Personnummer { get; set; } = default!;
        public List<string> Diagnoses { get; set; } = new();
        public DateOnly Date { get; set; }   // "YYYY-MM-DD"
        public TimeOnly Time { get; set; }   // "HH:MM"
    }

    // ----- Finalize (from Scan page) -----
    public sealed class FinalizeRequest
    {
        public string LabNumber { get; set; } = default!;
        public List<string> Requested { get; set; } = new();
        public List<FinalizeResultRow> Results { get; set; } = new();
        public FinalizeMeta? Meta { get; set; }   // optional extra metadata from analyzer
    }

    public sealed class FinalizeResultRow
    {
        public string Diagnosis { get; set; } = default!;
        public string? Auto { get; set; }        // POSITIVE|NEGATIVE|INCONCLUSIVE|NONE (nullable for safety)
        public string? Final { get; set; }
        public bool Overridden { get; set; }
    }

    public sealed class FinalizeMeta
    {
        public string? Personnummer { get; set; }
        public string? Name { get; set; }
        public string? Date { get; set; }
        public string? Time { get; set; }
        public string? Result { get; set; }
        public double? Confidence { get; set; }
    }

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
}
