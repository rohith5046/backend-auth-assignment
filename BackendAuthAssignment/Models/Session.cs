
namespace BackendAuthAssignment.Models;

public class Session
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid UserId { get; set; }
    public User User { get; set; } = default!;

    // hashed refresh token stored
    public string RefreshTokenHash { get; set; } = default!;
    public DateTimeOffset RefreshTokenExpiresAt { get; set; }

    public bool IsRevoked { get; set; } = false;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? RevokedAt { get; set; }
}
