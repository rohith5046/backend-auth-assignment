using BackendAuthAssignment.Data;
using BackendAuthAssignment.Models;
using BackendAuthAssignment.Helper;
using Microsoft.EntityFrameworkCore;

namespace BackendAuthAssignment.Services;

public class AuthService
{
    private readonly AppDbContext _db;
    private readonly IConfiguration _config;
    private readonly JwtTokenService _jwt;

    public AuthService(AppDbContext db, IConfiguration config, JwtTokenService jwt)
    {
        _db = db;
        _config = config;
        _jwt = jwt;
    }

    public async Task<User> GetOrCreateUserAsync(string phoneNumber)
    {
        var user = await _db.Users.FirstOrDefaultAsync(x => x.PhoneNumber == phoneNumber);
        if (user != null) return user;

        user = new User { PhoneNumber = phoneNumber, IsActive = true };
        _db.Users.Add(user);
        await _db.SaveChangesAsync();
        return user;
    }

    public async Task<(string accessToken, string refreshToken, bool regComplete)> CreateSessionAsync(User user)
    {
        if (!user.IsActive) throw new UnauthorizedAccessException("User blocked.");

        var days = int.Parse(_config["Auth:RefreshTokenDays"] ?? "7");
        var refreshToken = Crypto.RandomToken(32);
        var refreshHash = Crypto.Sha256(refreshToken);

        var session = new Session
        {
            UserId = user.Id,
            RefreshTokenHash = refreshHash,
            RefreshTokenExpiresAt = DateTimeOffset.UtcNow.AddDays(days),
            IsRevoked = false
        };

        _db.Sessions.Add(session);
        await _db.SaveChangesAsync();

        var access = _jwt.CreateAccessToken(user, session.Id);
        return (access, refreshToken, user.IsBasicRegistrationComplete);
    }

    public async Task<(string accessToken, bool regComplete)> RefreshAccessTokenAsync(string refreshToken)
    {
        var refreshHash = Crypto.Sha256(refreshToken);

        var session = await _db.Sessions
            .Include(s => s.User)
            .FirstOrDefaultAsync(s => s.RefreshTokenHash == refreshHash);

        if (session is null) throw new UnauthorizedAccessException("Invalid refresh token.");
        if (session.IsRevoked) throw new UnauthorizedAccessException("Session revoked.");
        if (DateTimeOffset.UtcNow > session.RefreshTokenExpiresAt)
            throw new UnauthorizedAccessException("Refresh token expired.");

        if (!session.User.IsActive) throw new UnauthorizedAccessException("User blocked.");

        var access = _jwt.CreateAccessToken(session.User, session.Id);
        return (access, session.User.IsBasicRegistrationComplete);
    }

    public async Task LogoutAsync(string refreshToken)
    {
        var refreshHash = Crypto.Sha256(refreshToken);

        var session = await _db.Sessions.FirstOrDefaultAsync(s => s.RefreshTokenHash == refreshHash);
        if (session is null) return;

        session.IsRevoked = true;
        session.RevokedAt = DateTimeOffset.UtcNow;
        await _db.SaveChangesAsync();
    }
}
