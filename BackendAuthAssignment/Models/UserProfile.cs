using System.ComponentModel.DataAnnotations;

namespace BackendAuthAssignment.Models;

public class UserProfile
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid UserId { get; set; }
    public User User { get; set; } = default!;

    [Required, MaxLength(150)]
    public string FullName { get; set; } = default!;

    public DateOnly DateOfBirth { get; set; }

    [Required, MaxLength(150)]
    public string Email { get; set; } = default!;

    // Store free-form JSON string (lat/long etc.)
    [Required]
    public string LocationJson { get; set; } = default!;
}
