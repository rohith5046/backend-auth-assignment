using BackendAuthAssignment.Data;
using BackendAuthAssignment.Models;
using BackendAuthAssignment.Helper;
using Microsoft.EntityFrameworkCore;

namespace BackendAuthAssignment.Services;

public class OtpService
{
    private readonly AppDbContext _db;
    private readonly IConfiguration _config;

    public OtpService(AppDbContext db, IConfiguration config)
    {
        _db = db;
        _config = config;
    }

    private string GenerateOtp()
    {
        var len = int.Parse(_config["Auth:OtpLength"] ?? "6");
        var max = (int)Math.Pow(10, len) - 1;
        var min = (int)Math.Pow(10, len - 1);
        return Random.Shared.Next(min, max + 1).ToString();
    }

    // Returns (allowed, errorMessageIfBlocked)
    private async Task<(bool allowed, string error)> CheckRateLimitsAsync(string phoneNumber)
    {
        var now = DateTimeOffset.UtcNow;

        var hourlyLimit = int.Parse(_config["Auth:OtpHourlyLimit"] ?? "3");
        var dailyLimit = int.Parse(_config["Auth:OtpDailyLimit"] ?? "10");

        var hourlyWindowStart = now.AddHours(-1);
        var dailyWindowStart = now.AddHours(-24);

        var hourlyCount = await _db.OtpRequests.CountAsync(x =>
            x.PhoneNumber == phoneNumber && x.CreatedAt >= hourlyWindowStart);

        if (hourlyCount >= hourlyLimit)
            return (false, "Hourly OTP limit reached. Please try again later.");

        var dailyCount = await _db.OtpRequests.CountAsync(x =>
            x.PhoneNumber == phoneNumber && x.CreatedAt >= dailyWindowStart);

        if (dailyCount >= dailyLimit)
            return (false, "Daily OTP limit exceeded. Please try again tomorrow.");

        return (true, "");
    }

    public async Task<(bool ok, string message)> RequestOtpAsync(string phoneNumber)
    {
        // Rate limiting (DB tracking)
        var (allowed, error) = await CheckRateLimitsAsync(phoneNumber);
        if (!allowed)
        {
            // store a record capturing status (optional but matches requirement)
            _db.OtpRequests.Add(new OtpRequest
            {
                PhoneNumber = phoneNumber,
                OtpHash = Crypto.Sha256($"{phoneNumber}:BLOCKED"),
                ExpiresAt = DateTimeOffset.UtcNow, // irrelevant
                Attempts = 0,
                IsActive = false,
                Status = error.Contains("Hourly") ? "BLOCKED_HOURLY" : "BLOCKED_DAILY",
                CreatedAt = DateTimeOffset.UtcNow
            });
            await _db.SaveChangesAsync();
            return (false, error);
        }

        // Invalidate previous active OTPs (resend invalidates previous)
        var activeOtps = await _db.OtpRequests
            .Where(x => x.PhoneNumber == phoneNumber && x.IsActive)
            .ToListAsync();

        foreach (var o in activeOtps)
            o.IsActive = false;

        var otp = GenerateOtp();
        Console.WriteLine($"OTP (mocked): {otp}");

        var validityMin = int.Parse(_config["Auth:OtpValidityMinutes"] ?? "5");

        var entity = new OtpRequest
        {
            PhoneNumber = phoneNumber,
            OtpHash = Crypto.Sha256($"{phoneNumber}:{otp}"),
            ExpiresAt = DateTimeOffset.UtcNow.AddMinutes(validityMin),
            Attempts = 0,
            IsActive = true,
            Status = "OK",
            CreatedAt = DateTimeOffset.UtcNow
        };

        _db.OtpRequests.Add(entity);
        await _db.SaveChangesAsync();

        // Mock OTP delivery
        Console.WriteLine($"[MOCK OTP] Phone={phoneNumber}, OTP={otp}");

        return (true, "OTP sent (mocked).");
    }

    public async Task<(bool ok, string error)> VerifyOtpAsync(string phoneNumber, string otp)
    {
        var maxAttempts = int.Parse(_config["Auth:OtpMaxAttempts"] ?? "3");

        var active = await _db.OtpRequests
            .OrderByDescending(x => x.CreatedAt)
            .FirstOrDefaultAsync(x => x.PhoneNumber == phoneNumber && x.IsActive);

        if (active is null) return (false, "No active OTP. Please request OTP.");

        if (DateTimeOffset.UtcNow > active.ExpiresAt)
        {
            active.IsActive = false;
            await _db.SaveChangesAsync();
            return (false, "OTP expired. Please request again.");
        }

        if (active.Attempts >= maxAttempts)
        {
            active.IsActive = false;
            await _db.SaveChangesAsync();
            return (false, "Max OTP attempts reached. Please request again.");
        }

        var hash = Crypto.Sha256($"{phoneNumber}:{otp}");
        if (!string.Equals(active.OtpHash, hash, StringComparison.OrdinalIgnoreCase))
        {
            active.Attempts += 1;
            if (active.Attempts >= maxAttempts) active.IsActive = false;
            await _db.SaveChangesAsync();
            return (false, "Invalid OTP.");
        }

        // OTP success: prevent reuse
        active.IsActive = false;
        await _db.SaveChangesAsync();

        return (true, "");
    }
}
