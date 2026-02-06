using System.Security.Claims;
using System.Text.Json;
using BackendAuthAssignment.Data;
using BackendAuthAssignment.Dtos;
using BackendAuthAssignment.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackendAuthAssignment.Controllers;

[ApiController]
[Route("user")]
public class UserController : ControllerBase
{
    private readonly AppDbContext _db;

    public UserController(AppDbContext db) => _db = db;

    [Authorize]
    [HttpPost("register/basic")]
    public async Task<IActionResult> RegisterBasic([FromBody] RegisterBasicDto dto)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        if (string.IsNullOrWhiteSpace(userIdStr)) return Unauthorized();

        if (!DateOnly.TryParse(dto.DateOfBirth, out var dob))
            return BadRequest(new { message = "DateOfBirth must be YYYY-MM-DD." });

        if (string.IsNullOrWhiteSpace(dto.FullName) || string.IsNullOrWhiteSpace(dto.Email))
            return BadRequest(new { message = "FullName and Email are required." });

        var locJson = dto.Location.ValueKind == JsonValueKind.Undefined ? null : dto.Location.GetRawText();
        if (string.IsNullOrWhiteSpace(locJson))
            return BadRequest(new { message = "Location is required (JSON)." });

        var userId = Guid.Parse(userIdStr);

        var user = await _db.Users.Include(u => u.Profile).FirstOrDefaultAsync(u => u.Id == userId);
        if (user is null) return Unauthorized();
        if (!user.IsActive) return Forbid();

        if (user.Profile is null)
        {
            user.Profile = new UserProfile
            {
                UserId = user.Id,
                FullName = dto.FullName.Trim(),
                DateOfBirth = dob,
                Email = dto.Email.Trim(),
                LocationJson = locJson
            };
            _db.UserProfiles.Add(user.Profile);
        }
        else
        {
            user.Profile.FullName = dto.FullName.Trim();
            user.Profile.DateOfBirth = dob;
            user.Profile.Email = dto.Email.Trim();
            user.Profile.LocationJson = locJson;
        }

        user.IsBasicRegistrationComplete = true;
        await _db.SaveChangesAsync();

        return Ok(new { message = "Basic registration completed." });
    }

    // Protected API - requires registration completed
    [Authorize(Policy = "RegistrationComplete")]
    [HttpGet("me")]
    public async Task<IActionResult> Me()
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        if (string.IsNullOrWhiteSpace(userIdStr)) return Unauthorized();

        var userId = Guid.Parse(userIdStr);

        var user = await _db.Users.Include(u => u.Profile).FirstOrDefaultAsync(u => u.Id == userId);
        if (user is null) return Unauthorized();
        if (!user.IsActive) return Forbid();

        return Ok(new
        {
            user.Id,
            user.PhoneNumber,
            user.IsBasicRegistrationComplete,
            profile = user.Profile == null ? null : new
            {
                user.Profile.FullName,
                user.Profile.DateOfBirth,
                user.Profile.Email,
                location = JsonDocument.Parse(user.Profile.LocationJson).RootElement
            }
        });
    }
}
