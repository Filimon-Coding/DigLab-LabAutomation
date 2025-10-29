namespace DigLabAPI.Models;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = default!;
    public string PasswordHash { get; set; } = default!;
    public string Role { get; set; } = "user";
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}
