using DigLabAPI.Data;
using DigLabAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;   // <— legg til denne

namespace DigLabAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PersonsController : ControllerBase
{
    private readonly DigLabDb _db;
    public PersonsController(DigLabDb db) => _db = db;

    [HttpGet("by-pnr/{pnr}")]
    public async Task<ActionResult<Person>> GetByPnr(string pnr)
    {
        if (string.IsNullOrWhiteSpace(pnr) || pnr.Length != 11 || !pnr.All(char.IsDigit))
            return BadRequest("personnummer must be 11 digits");

        var p = await _db.Persons.AsNoTracking().FirstOrDefaultAsync(x => x.Personnummer == pnr);
        return p is null ? NotFound() : Ok(p);
    }

    // Enkel create for syntetiske personer
    [HttpPost]
    public async Task<ActionResult<Person>> Create(Person p)
    {
        if (!Regex.IsMatch(p.Personnummer ?? "", @"^\d{11}$"))   // <— FIX
            return BadRequest("personnummer must be 11 digits");

        if (await _db.Persons.AnyAsync(x => x.Personnummer == p.Personnummer))
            return Conflict("Personnummer already exists");

        p.CreatedAtUtc = DateTime.UtcNow;
        _db.Persons.Add(p);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetByPnr), new { pnr = p.Personnummer }, p);
    }
}
