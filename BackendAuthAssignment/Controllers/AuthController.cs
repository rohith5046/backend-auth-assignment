using BackendAuthAssignment.Dtos;
using BackendAuthAssignment.Services;
using Microsoft.AspNetCore.Mvc;

namespace BackendAuthAssignment.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly OtpService _otpService;
    private readonly AuthService _authService;

    public AuthController(OtpService otpService, AuthService authService)
    {
        _otpService = otpService;
        _authService = authService;
    }

    [HttpPost("request-otp")]
    public async Task<IActionResult> RequestOtp([FromBody] RequestOtpDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.PhoneNumber))
            return BadRequest(new { message = "PhoneNumber required." });

        var (ok, message) = await _otpService.RequestOtpAsync(dto.PhoneNumber.Trim());
        if (!ok) return StatusCode(429, new { message }); // Too Many Requests

        return Ok(new { message });
    }

    [HttpPost("verify-otp")]
    public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.PhoneNumber) || string.IsNullOrWhiteSpace(dto.Otp))
            return BadRequest(new { message = "PhoneNumber and Otp are required." });

        var (ok, error) = await _otpService.VerifyOtpAsync(dto.PhoneNumber.Trim(), dto.Otp.Trim());
        if (!ok) return Unauthorized(new { message = error });

        var user = await _authService.GetOrCreateUserAsync(dto.PhoneNumber.Trim());
        if (!user.IsActive) return Forbid();

        var (access, refresh, regComplete) = await _authService.CreateSessionAsync(user);

        return Ok(new AuthSuccessResponse(access, refresh, regComplete));
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.RefreshToken))
            return BadRequest(new { message = "RefreshToken required." });

        try
        {
            var (access, regComplete) = await _authService.RefreshAccessTokenAsync(dto.RefreshToken);
            return Ok(new { accessToken = access, isBasicRegistrationComplete = regComplete });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] LogoutDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.RefreshToken))
            return BadRequest(new { message = "RefreshToken required." });

        await _authService.LogoutAsync(dto.RefreshToken);
        return Ok(new { message = "Logged out." });
    }
}
