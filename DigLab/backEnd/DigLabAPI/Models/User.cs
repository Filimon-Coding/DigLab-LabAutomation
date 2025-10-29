namespace DigLabAPI.Models;




public enum Profession
{
    Nurse,
    Doctor,
    Bioengineer,
    Other
}
public class User
{
    public int Id { get; set; }

    // Login = WorkerID for uniqueness
    public string Username { get; set; } = default!; // = WorkerId

    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;

    public string WorkerId { get; set; } = default!; // company internal ID
    public string HprNumber { get; set; } = default!; // HPR-nummer

    public Profession Profession { get; set; } = Profession.Other;

    public string PasswordHash { get; set; } = default!;
    public string Role { get; set; } = "user";
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}
