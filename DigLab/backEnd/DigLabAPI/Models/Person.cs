using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace DigLabAPI.Models;

[Index(nameof(Personnummer), IsUnique = true)]
public class Person
{
    public int Id { get; set; }

    [Required, MaxLength(11)]
    public string Personnummer { get; set; } = default!;

    [Required, MaxLength(100)]
    public string FirstName { get; set; } = default!;

    [MaxLength(100)]
    public string? MiddleName { get; set; }

    [Required, MaxLength(100)]
    public string LastName { get; set; } = default!;

    [MaxLength(200)] public string? AddressLine1 { get; set; }
    [MaxLength(200)] public string? AddressLine2 { get; set; }
    [MaxLength(4)]   public string? PostalCode { get; set; }
    [MaxLength(100)] public string? City { get; set; }
    [MaxLength(100)] public string? Email { get; set; }
    [MaxLength(20)]  public string? Phone { get; set; }

    public DateOnly? DateOfBirth { get; set; }

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAtUtc { get; set; }
}
