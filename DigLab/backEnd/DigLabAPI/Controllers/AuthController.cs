using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DigLabAPI.Data;
using DigLabAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace DigLabAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly DigLabDb _db;
    private readonly IConfiguration _cfg;
    public AuthController(DigLabDb db, IConfiguration cfg){ _db = db; _cfg = cfg; }

    public record LoginRequest(string Username, string Password);
    public record LoginResponse(string token, string username, string role, DateTime expiresUtc);

    [HttpPost("login")]
    [Produces("application/json")]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest req)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Username == req.Username);
        if (user is null || !BCrypt.Net.BCrypt.Verify(req.Password, user.PasswordHash))
            return Unauthorized("Invalid credentials");

        var expires = DateTime.UtcNow.AddMinutes(int.Parse(_cfg["Jwt:ExpiresMinutes"] ?? "120"));
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Username),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var key   = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_cfg["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: _cfg["Jwt:Issuer"],
            audience: _cfg["Jwt:Audience"],
            claims: claims,
            expires: expires,
            signingCredentials: creds
        );

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);
        return Ok(new LoginResponse(jwt, user.Username, user.Role, expires));
    }
}
