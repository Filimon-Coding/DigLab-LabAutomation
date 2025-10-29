using DigLabAPI.Data;
using DigLabAPI.Models;
using DigLabAPI.Models.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace DigLabAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly DigLabDb _db;
    public UsersController(DigLabDb db){ _db = db; }

    // --- Admin: create employee ---
    [HttpPost]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<object>> Create([FromBody] CreateUserDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.WorkerId)) return BadRequest("WorkerId required");
        if (await _db.Users.AnyAsync(u => u.WorkerId == dto.WorkerId))
            return Conflict("WorkerId already exists");

        // Parse profession (defaults to Other)
        var prof = Enum.TryParse<Profession>(dto.Profession, true, out var p) ? p : Profession.Other;

        var user = new User {
            FirstName = dto.FirstName.Trim(),
            LastName  = dto.LastName.Trim(),
            WorkerId  = dto.WorkerId.Trim(),
            Username  = dto.WorkerId.Trim(), // login key
            HprNumber = dto.HprNumber.Trim(),
            Profession = prof,
            Role = "user"
        };

        // initial password
        var pwd = string.IsNullOrWhiteSpace(dto.InitialPassword)
            ? GenerateTempPassword()
            : dto.InitialPassword;

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(pwd);

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return Ok(new { ok = true, id = user.Id, username = user.Username, tempPassword = pwd });
    }

    // --- Admin: list employees ---
    [HttpGet]
    [Authorize(Roles = "admin")]
    public async Task<IEnumerable<UserListItemDto>> List()
      => await _db.Users
          .OrderBy(u => u.LastName).ThenBy(u => u.FirstName)
          .Select(u => new UserListItemDto(u.Id, u.FirstName, u.LastName, u.WorkerId, u.HprNumber, u.Profession.ToString(), u.Role))
          .ToListAsync();

    // --- User: change own password ---
    [HttpPut("me/password")]
    [Authorize]
    public async Task<IActionResult> ChangeOwnPassword([FromBody] ChangePasswordDto dto)
    {
        var name = User.Identity?.Name; // we set ClaimTypes.Name = Username on login
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Username == name);
        if (user is null) return Unauthorized();

        if (!BCrypt.Net.BCrypt.Verify(dto.CurrentPassword, user.PasswordHash))
            return BadRequest("Current password incorrect");

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
        await _db.SaveChangesAsync();
        return Ok(new { ok = true });
    }

    // --- Admin: force reset password for a user ---
    [HttpPut("{id:int}/password")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> AdminResetPassword(int id, [FromBody] AdminResetPasswordDto dto)
    {
        var user = await _db.Users.FindAsync(id);
        if (user is null) return NotFound();

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
        await _db.SaveChangesAsync();
        return Ok(new { ok = true });
    }

    private static string GenerateTempPassword()
    {
        // 10 chars: letters+digits
        const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz23456789";
        var rng = RandomNumberGenerator.Create();
        var bytes = new byte[10];
        rng.GetBytes(bytes);
        return new string(bytes.Select(b => chars[b % chars.Length]).ToArray());
    }
}
