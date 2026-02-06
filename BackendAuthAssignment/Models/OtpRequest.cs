using System.ComponentModel.DataAnnotations;

namespace BackendAuthAssignment.Models;

public class OtpRequest
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required, MaxLength(20)]
    public string PhoneNumber { get; set; } = default!;

    // hashed OTP (never store plain OTP)
    [Required]
    public string OtpHash { get; set; } = default!;

    public DateTimeOffset ExpiresAt { get; set; }

    // verification attempts (max 3)
    public int Attempts { get; set; } = 0;

    // one active OTP per phone number
    public bool IsActive { get; set; } = true;

    // tracking & rate limiting
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    // simple status for auditing (OK/BLOCKED_HOURLY/BLOCKED_DAILY)
    [MaxLength(30)]
    public string Status { get; set; } = "OK";
}
