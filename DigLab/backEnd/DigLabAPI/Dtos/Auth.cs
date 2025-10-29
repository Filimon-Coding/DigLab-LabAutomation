namespace DigLabAPI.Models.Dtos;

public record CreateUserDto(
    string FirstName,
    string LastName,
    string WorkerId,
    string HprNumber,
    string Profession,   // "Nurse" | "Doctor" | "Bioengineer" | "Other"
    string? InitialPassword // optional: if null, server generates
);

public record UserListItemDto(
    int Id, string FirstName, string LastName,
    string WorkerId, string HprNumber, string Profession, string Role
);

public record ChangePasswordDto(string CurrentPassword, string NewPassword);
public record AdminResetPasswordDto(string NewPassword);
